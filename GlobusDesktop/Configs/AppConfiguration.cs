namespace GlobusDesktop.Configs
{
    public class AppConfiguration
    {
        public GrafanaConfiguration GrafanaConfiguration { get; set; }
        public ZabbixConfiguration ZabbixConfiguration { get; set; }

        public static AppConfiguration Default
            => new ()
            {
                ZabbixConfiguration = new ZabbixConfiguration()
                {
                    Host = "localhost",
                    Password = "Admin",
                    User = "Admin"
                },
                GrafanaConfiguration = new GrafanaConfiguration()
                {
                    Host = "localhost",
                    Token = "none"
                }
            };
    }
}