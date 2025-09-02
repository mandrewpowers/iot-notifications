namespace IoT_Notifications.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class SettingsSectionAttribute : Attribute {
        public string Name { get; }

        public SettingsSectionAttribute(string name) {
            this.Name = name;
        }
    }
}
