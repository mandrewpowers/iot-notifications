using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;

namespace IoT_Notifications {
    internal class HttpIntegration : IIntegrationProvider {
        public string Name { get; private set; }
        public bool Silent { get; private set; }

        public event EventHandler<bool>? StateChanged;

        WebApplication? App;
        HashSet<string> AuthenticationTokens;

        NotificationForm? LastNotification = null;

        public HttpIntegration(string name, IEnumerable<string> authenticationTokens) {
            this.Name = name;
            this.AuthenticationTokens = new HashSet<string>(authenticationTokens);
        }

        private WebApplication BuildWebApp() {
            var appName = $"IoT Notifications ({this.Name})";

            var builder = WebApplication.CreateBuilder(); // Apparently setting the app name has sideaffects

            builder.WebHost.ConfigureKestrel(options => {
                options.ListenAnyIP(9000, listenOptions => {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                    listenOptions.UseHttps(httpsOptions => {
                        httpsOptions.ServerCertificate = GenerateCertificate(appName, "192.168.1.200", DateTimeOffset.Now.AddHours(72));
                    });
                });
            });

            builder.Services
                .AddSingleton(this)
                .AddHostedService<LifetimeEventsService>()
                .AddCors();
                //.AddAuthorization()
                //.AddAuthentication(options => {
                //    options.DefaultAuthenticateScheme = "BasicAuth";
                //    options.DefaultChallengeScheme = "BasicAuth";
                //}).AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuth", null);

            var app = builder.Build();

            app
                .UseCors()
                //.UseAuthentication()
                .UseRouting();
                //.UseAuthorization();

            app.MapGet("/camera-event", (HttpContext ctx) => {
                if (ctx.Request.Query.TryGetValue("token", out var tokens)) {
                    if (tokens.Any(token => this.AuthenticationTokens.Contains(token))) {
                        ctx.Response.StatusCode = 200;
                    } else {
                        ctx.Response.StatusCode = 400;
                    }
                    return;
                }
                ctx.Response.StatusCode = 200;
            });

            app.MapPost("/camera-event", async (HttpContext ctx, [FromHeader(Name = "Content-Type")] string contentType) => {
                StringValues tokens;
                if (!ctx.Request.Query.TryGetValue("token", out tokens)) {
                    ctx.Response.StatusCode = 401;
                    return;
                } else if (!tokens.Any(token => token != null && this.AuthenticationTokens.Contains(token))) {
                    ctx.Response.StatusCode = 403;
                    return;
                }

                try {
                    // Resize and save image (Toasts will need files if reimplemented)
                    Image? image = null;
                    if (contentType.StartsWith("image/")) {
                        using (Stream inStream = new MemoryStream()) {
                        await ctx.Request.Body.CopyToAsync(inStream);
                            var resizedImage = Helpers.ResizeImage(Image.FromStream(inStream), 512, 288);
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
                } catch (Exception e) {
                    Trace.WriteLine($"Error handling {ctx.Request.Method} {ctx.Request.Path}: {e.Message}");
                }
            });

            return app;
        }

        public Task Start(CancellationToken cancellationToken) {
            this.App = this.BuildWebApp();
            if (this.App != null) {
                this.App.RunAsync(cancellationToken);
            }
            return Task.CompletedTask;
        }

        public async Task Stop(TimeSpan timeout) {
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

        public static X509Certificate2 GenerateCertificate(string subjectName, string dnsName, DateTimeOffset expiration) {
            using (var rsa = RSA.Create(2048)) {
                var certReq = new CertificateRequest($"cn={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                var sanBuilder = new SubjectAlternativeNameBuilder();
                sanBuilder.AddDnsName(dnsName);
                certReq.CertificateExtensions.Add(sanBuilder.Build());

                var cert = certReq.CreateSelfSigned(DateTimeOffset.Now, expiration);
                return new X509Certificate2(cert.Export(X509ContentType.Pfx));
            }
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

        // Basic auth is broken on AXIS Firmware 9.x.x so this will be worked on later

        //private class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
        //    [Obsolete]
        //    public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }

        //    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        //        if (!Request.Headers.ContainsKey("Authorization")) {
        //            return AuthenticateResult.Fail("Missing Authorization Header");
        //        }

        //        try {
        //            var authHeader = Request.Headers["Authorization"].ToString();
        //            if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase)) {
        //                return AuthenticateResult.Fail("Invalid Authorization Scheme");
        //            }

        //            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Substring("Basic ".Length))).Split(':');
        //            var username = credentials[0];
        //            var password = credentials[1];

        //            // **Replace this with your actual user validation logic**
        //            if (username == "user" && password == "password") {
        //                var claims = new[] { new Claim(ClaimTypes.Name, username) };
        //                var identity = new ClaimsIdentity(claims, Scheme.Name);
        //                var principal = new ClaimsPrincipal(identity);
        //                var ticket = new AuthenticationTicket(principal, Scheme.Name);

        //                return AuthenticateResult.Success(ticket);
        //            } else {
        //                return AuthenticateResult.Fail("Invalid Username or Password");
        //            }
        //        } catch {
        //            return AuthenticateResult.Fail("Invalid Authorization Header");
        //        }
        //    }
        //}
    }
}
