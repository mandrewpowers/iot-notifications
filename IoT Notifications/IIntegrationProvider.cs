using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT_Notifications {
    internal interface IIntegrationProvider {
        Task Start(CancellationToken cancellationToken);
        Task Stop(TimeSpan timeout);

        void AttachMenuItems(ToolStripItemCollection collection, CancellationToken shutdownToken);
    }
}
