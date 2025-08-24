using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Windows.Forms.VisualStyles;

namespace IoT_Notifications {
    internal class HttpIntegration : IIntegrationProvider {
        public string Name { get; private set; }
        public bool Silent { get; set; }

        public event EventHandler<bool>? StateChanged;

        WebApplication? App;
        //DirectoryInfo ImageDir;

        NotificationForm? LastNotification = null;

        public HttpIntegration(string name) : this(name, Directory.CreateTempSubdirectory("IoT Notifications")) { }

        public HttpIntegration(string name, string imageDir) : this(name, new DirectoryInfo(imageDir)) { }

        public HttpIntegration(string name, DirectoryInfo imageDir) {
            this.Name = name;
            //this.ImageDir = imageDir;
        }

        private WebApplication BuildWebApp() {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions() {
                ApplicationName = "IoT Notifications",
            });

            builder.Services
                .AddSingleton(this)
                .AddHostedService<LifetimeEventsService>()
                .AddCors()
                .AddAuthorization()
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = "BasicAuth";
                    options.DefaultChallengeScheme = "BasicAuth";
                }).AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuth", null);

            var app = builder.Build();

            app
                .UseCors()
                .UseAuthentication()
                .UseRouting()
                .UseAuthorization();

            app.MapGet("/camera-event", (HttpContext ctx) => {
                ctx.Response.StatusCode = 200;
            });

            app.MapPost("/camera-event", async (HttpContext ctx, [FromHeader(Name = "Content-Type")] string contentType) => {
                //var now = DateTime.Now;

                Trace.WriteLine($"[{DateTimeOffset.Now.ToUnixTimeMilliseconds()}] {ctx.Request.Method} {ctx.Request.Path}");

                try {
                    // Resize and save image (Toasts will need files if reimplemented)
                    Image? image = null;
                    if (contentType.StartsWith("image/")) {
                        //FileStream? imageFile = File.Create(Path.Combine(this.ImageDir.FullName, $"{Guid.NewGuid().ToString()}.jpeg"));
                        //using (Stream inStream = new MemoryStream(), outStream = imageFile) {
                        using (Stream inStream = new MemoryStream()) {
                        await ctx.Request.Body.CopyToAsync(inStream);
                            var resizedImage = Helpers.ResizeImage(Image.FromStream(inStream), 512, 288);
                            //resizedImage.Save(outStream, ImageFormat.Jpeg);
                            image = resizedImage;
                        }
                    }

                    if (Program.UISynchronizationContext != null) {
                        Program.UISynchronizationContext.Post(_ => {
                            if (LastNotification != null && !LastNotification.Expired) {
                                // Update existing
                                if (image != null) {
                                    LastNotification.QueueImage(image, TimeSpan.FromMilliseconds(500));
                                } else {
                                    LastNotification.Touch();
                                }
                            } else {
                                // Create new
                                LastNotification = new NotificationForm() {
                                    Silent = this.Silent
                                };
                                LastNotification.Show(image, TimeSpan.FromMilliseconds(500));
                            }
                        }, null);
                    }

                    /*var toastBuilder = new ToastContentBuilder()
                        .AddAudio(null, null, true)
                        .AddText("Motion detected")
                        .SetProtocolActivation(new Uri("http://front-camera.local/"));

                    if (heroImageUri != null) {
                        toastBuilder.AddHeroImage(heroImageUri);
                    }

                    string tag;
                    if (LastNotification != null && now.Subtract(LastNotification.Value.timestamp).TotalMilliseconds < 3000) {
                        tag = LastNotification.Value.tag;
                    } else {
                        tag = Guid.NewGuid().ToString();
                    }

                    var toast = new ToastNotification(toastBuilder.GetXml()) {
                        ExpirationTime = now.AddDays(1),
                        Tag = tag,
                    };

                    LastNotification = (now, tag);

                    if (Program.ToastNotifier != null) {
                        Program.ToastNotifier.Show(toast);
                    }*/
                } catch (Exception e) {
                    Trace.WriteLine($"Error handling {ctx.Request.Method} {ctx.Request.Path}: {e.Message}");
                }
            });

            return app;
        }

        public Task Start(CancellationToken cancellationToken) {
            this.App = this.BuildWebApp();
            if (this.App != null) {
                this.App.Urls.Clear();
                this.App.Urls.Add("http://*:9000");
                this.App.RunAsync(cancellationToken);
            }
            return Task.CompletedTask;
        }

        public async Task Stop(TimeSpan timeout) {
            //return Task.WhenAll(
            //    //Task.Run(() => this.ImageDir.Delete(true)),
            //    this.App != null ? this.App.StopAsync(timeout) : Task.CompletedTask
            //);
            if (this.App != null) {
                await this.App.StopAsync(timeout);
                this.App = null;
            }
        }

        public void AttachMenuItems(ToolStripItemCollection collection, CancellationToken shutdownToken) {
            // Integration toggle
            var toggle = new ToolStripMenuItem($"{this.Name} enabled", null, (sender, _) => {
                var self = sender as ToolStripMenuItem;
                if (self == null) return;
                self.Enabled = false;
                self.Checked = !self.Checked;
            });

            toggle.CheckedChanged += async (sender, _) => {
                var self = sender as ToolStripMenuItem;
                if (self == null || self.Enabled) return;

                var newChecked = self.Checked;
                try {
                    if (self.Checked) {
                        await this.Start(shutdownToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
                    } else {
                        await this.Stop(TimeSpan.FromMicroseconds(5000));
                    }
                } catch (Exception exc) {
                    newChecked = !newChecked;
                    Trace.TraceError($"Failed to change {this.Name} state: {exc.Message}");
                } finally {
                    self.Enabled = true;
                    self.Checked = newChecked;
                }
            };

            this.StateChanged += (_, state) => {
                if (toggle != null) {
                    toggle.Checked = state;
                }
            };

            // Silence toggle
            var silence = new ToolStripMenuItem("Silent", null, (sender, _) => {
                var self = sender as ToolStripMenuItem;
                if (self == null) return;
                this.Silent = self.Checked = !self.Checked;
            });

            collection.AddRange([toggle, silence]);
        }

        private class LifetimeEventsService : IHostedService {
            public LifetimeEventsService(HttpIntegration integration, IHostApplicationLifetime lifetime) {
                lifetime.ApplicationStopped.Register(() => {
                    Trace.WriteLine("ApplicationStopped");
                    if (integration.StateChanged != null) {
                        integration.StateChanged.Invoke(integration, false);
                    }
                });

                lifetime.ApplicationStarted.Register(() => {
                    Trace.WriteLine("ApplicationStarted");
                    if (integration.StateChanged != null) {
                        integration.StateChanged.Invoke(integration, true);
                    }
                });
            }

            public Task StartAsync(CancellationToken cancellationToken) { return Task.CompletedTask; }

            public Task StopAsync(CancellationToken cancellationToken) { return Task.CompletedTask;  }
        }

        private class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
            [Obsolete]
            public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }

            protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
                if (!Request.Headers.ContainsKey("Authorization")) {
                    return AuthenticateResult.Fail("Missing Authorization Header");
                }

                try {
                    var authHeader = Request.Headers["Authorization"].ToString();
                    if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase)) {
                        return AuthenticateResult.Fail("Invalid Authorization Scheme");
                    }

                    var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Substring("Basic ".Length))).Split(':');
                    var username = credentials[0];
                    var password = credentials[1];

                    // **Replace this with your actual user validation logic**
                    if (username == "user" && password == "password") {
                        var claims = new[] { new Claim(ClaimTypes.Name, username) };
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);

                        return AuthenticateResult.Success(ticket);
                    } else {
                        return AuthenticateResult.Fail("Invalid Username or Password");
                    }
                } catch {
                    return AuthenticateResult.Fail("Invalid Authorization Header");
                }
            }
        }
    }
}
