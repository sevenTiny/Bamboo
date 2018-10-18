namespace SevenTiny.Bantina.Spring
{
    public class SpringContext
    {
        private SpringContext() { Current = this; }
        public SpringContext(int tenantId, int userId, object instance, string method, object[] parameters)
        {
            TenantId = tenantId;
            UserId = userId;
            InstanceObject = instance;
            Method = method;
            Parameters = parameters;
            Current = this;
        }

        public int TenantId { get; }
        public int UserId { get; }
        public object InstanceObject { get; }
        public object[] Parameters { get; }
        public string Method { get; }
        public object Result { get; set; }

        public static IServiceProvider RequestServices { get; }
        public static SpringContext Current { get; private set; }
    }
}