namespace IoT_Notifications.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class IntegrationSettingsAttribute : Attribute {
        public Type ControlType { get; }

        public IntegrationSettingsAttribute(Type controlType) {
            if (!typeof(UserControl).IsAssignableFrom(controlType)) {
                throw new ArgumentException("Type must be a UserControl", nameof(controlType));
            }
            this.ControlType = controlType;
        }
    }
}
