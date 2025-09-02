using IoT_Notifications.Integrations;
using System.Diagnostics;

namespace IoT_Notifications {
    internal static class Program {
        static IntegrationCollection? Integrations = null;

        public static SynchronizationContext? UISynchronizationContext = null;
        public static CancellationToken ShutdownToken = CancellationToken.None;

        //public static ToastNotifierCompat? ToastNotifier = null; // TODO: Abstract away
        static NotifyIcon? NotifyIcon = null;

        [STAThread]
        static int Main() {
            ApplicationConfiguration.Initialize();
            UISynchronizationContext = new WindowsFormsSynchronizationContext();

            Console.CancelKeyPress += (sender, eventArgs) => Application.Exit();
            CancellationTokenSource cts = new CancellationTokenSource();
            ShutdownToken = cts.Token;

            // Gather integrations (TODO: Configurable from settings form, this is currently test data)
            if (Properties.Settings.Default.Integrations == null) {
                Integrations = Properties.Settings.Default.Integrations = new IntegrationCollection();

                var testIntegration = new HttpIntegration() {
                    Name = "HTTP Example"
                };

                Integrations.Add(testIntegration.Guid, testIntegration);
                Properties.Settings.Default.Save();
            } else {
                Integrations = Properties.Settings.Default.Integrations;
            }

            // Build UI
            //ToastNotifier = ToastNotificationManagerCompat.CreateToastNotifier();
            //if (ToastNotifier.Setting != NotificationSetting.Enabled) {
            //    Trace.WriteLine("Unable to manage toasts");
            //    return -1;
            //}

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

            if (Integrations != null && Integrations.Count > 0) {
                foreach (IIntegration integration in Integrations.Values) {
                    integration.AttachMenuItems(items, shutdownToken);
                }
                items.Add(new ToolStripSeparator());
            }

            if (Debugger.IsAttached) {
                items.Add(new ToolStripMenuItem("Show test popup...", null, (s, e) => {
                    var notification = new NotificationForm();
                    notification.Show(null);
                }));
            }

            // TODO: Global silent option
            items.Add(new ToolStripMenuItem("Settings", null, (s, e) => new SettingsForm().Show()));

            items.Add(new ToolStripSeparator());

            items.Add(new ToolStripMenuItem("Quit", null, (s, e) => Application.Exit()));

            return menu;
        }
    }
}