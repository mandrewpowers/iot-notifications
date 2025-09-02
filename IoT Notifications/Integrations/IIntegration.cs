using System.Runtime.Serialization;

namespace IoT_Notifications.Integrations {
    public interface IIntegration {
        // Properties
        Guid Guid { get; set; }
        string Name { get; set; }
        bool Silent { get; set; }

        // Events
        public event EventHandler<bool>? StateChanged;

        // Startup and shutdown
        Task Start(CancellationToken cancellationToken);
        Task Stop(TimeSpan timeout);

        // UI
        void AttachMenuItems(ToolStripItemCollection collection, CancellationToken shutdownToken);
    }
}
