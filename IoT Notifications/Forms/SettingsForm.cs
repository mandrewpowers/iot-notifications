using IoT_Notifications.Attributes;
using System.Data;
using System.Reflection;

namespace IoT_Notifications {
    public partial class SettingsForm : Form {
        public SettingsForm() {
            InitializeComponent();

            var settingsSectionTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsDefined(typeof(SettingsSectionAttribute))).ToArray();

            if (settingsSectionTypes.Length > 0) {
                foreach (var type in settingsSectionTypes) {
                    var attr = type.GetCustomAttribute<SettingsSectionAttribute>();
                    if (attr == null) continue;

                    var node = new TreeNode(attr.Name) {
                        Tag = type,
                    };

                    if (this.settingsNavigation.Nodes.Count == 0) {
                        this.PopulateSettingsContainer(type, null);
                    }

                    this.settingsNavigation.Nodes.Add(node);
                }
            } else {
                // TODO: this.PopulateSettingsContainer<GeneralSettings>(null);
            }
        }

        private T PopulateSettingsContainer<T>(object? tag) where T : UserControl, new() {
            return (T)this.PopulateSettingsContainer(typeof(T), tag);
        }

        private UserControl PopulateSettingsContainer(Type controlType, object? tag) {
            if (!typeof(UserControl).IsAssignableFrom(controlType)) {
                throw new ArgumentException("Type must be a UserControl", nameof(controlType));
            }

            UserControl? panel = null;
            try {
                panel = Activator.CreateInstance(controlType) as UserControl;
                if (panel == null) {
                    throw new ArgumentException($"Failed to create settings panel: {controlType.FullName}");
                }
            } catch (Exception ex) {
                throw new ArgumentException($"Failed to create settings panel: {controlType.FullName}", ex);
            }

            panel.Tag = tag;
            panel.Dock = DockStyle.Top;

            this.settingsContainer.Controls.Clear();
            this.settingsContainer.Controls.Add(panel);

            panel.Focus();

            return panel;
        }

        private void settingsNavigation_AfterSelect(object sender, TreeViewEventArgs e) {
            var tag = e.Node?.Tag as Type;

            if (tag != null && typeof(UserControl).IsAssignableFrom(tag)) {
                this.PopulateSettingsContainer(tag, null);
            }
        }
    }
}
