using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using Windows.UI.Notifications;

namespace IoT_Notifications {
    internal static class Program {
        static Dictionary<string, IIntegrationProvider> Integrations = new Dictionary<string, IIntegrationProvider>();

        public static SynchronizationContext? UISynchronizationContext = null;
        public static CancellationToken ShutdownToken = CancellationToken.None;

        public static ToastNotifierCompat? ToastNotifier = null; // TODO: Abstract away
        static NotifyIcon? NotifyIcon = null;

        [STAThread]
        static int Main() {
            ApplicationConfiguration.Initialize();
            UISynchronizationContext = new WindowsFormsSynchronizationContext();

            Console.CancelKeyPress += (sender, eventArgs) => Application.Exit();
            CancellationTokenSource cts = new CancellationTokenSource();
            ShutdownToken = cts.Token;

            // Gather integrations (TODO: Configurable)
            Integrations.Add("HTTP", new HttpIntegration("HTTP", new string[] {
                "F9fcBmQRXZxSZVA6Wg3G5mSjPNtB0s7X"
            }));

            // Build ui
            ToastNotifier = ToastNotificationManagerCompat.CreateToastNotifier();
            if (ToastNotifier.Setting != NotificationSetting.Enabled) {
                Trace.WriteLine("Unable to manage toasts");
                return -1;
            }

            NotifyIcon = new NotifyIcon() {
                Text = "IoT Notifications",
                Icon = Properties.Resources.AppIcon,
                ContextMenuStrip = BuildContextMenu(cts.Token),
                Visible = false
            };

            // Start integrations
            var startupTasks = Integrations.Values.Select(integration => integration.Start(cts.Token)).ToArray();
            if (Task.WaitAll(startupTasks, 5000)) {
                // Add quit entry with separator to end of tray menu and make visible
                NotifyIcon.Visible = true;

                // Run
                Application.Run();
            } else {
                Trace.WriteLine("Integrations startup ran away or failed");
            }

            // Shutdown everything
            if (NotifyIcon != null) {
                NotifyIcon.Visible = false;
            }

            cts.Cancel();
            if (!Task.WaitAll(Integrations.Values.Select(integ => integ.Stop(TimeSpan.FromMilliseconds(3000))).ToArray(), 5000)) {
                Trace.WriteLine("Not all integrations shutdown gracefully");
            }

            Trace.WriteLine("Goodbye");
            Trace.Flush();

            return 0;
        }

        private static ContextMenuStrip BuildContextMenu(CancellationToken shutdownToken) {
            var menu = new ContextMenuStrip();
            var items = menu.Items;

            foreach (IIntegrationProvider integration in Integrations.Values) {
                integration.AttachMenuItems(items, shutdownToken);
            }

            if (Debugger.IsAttached) {
                items.Add(new ToolStripSeparator());
                items.Add(new ToolStripMenuItem("Show test popup...", null, (s, e) => {
                    var notification = new NotificationForm();
                    notification.Show(null);
                }));
            }

            items.Add(new ToolStripSeparator());
            items.Add(new ToolStripMenuItem("Quit", null, (s, e) => Application.Exit()));

            return menu;
        }

        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e) {
            throw new NotImplementedException();
        }
    }
}