namespace mini_ITS.Core.Options
{
    public class DatabaseOptions
    {
        public string DataSource { get; set; }
        public string InitialCatalog { get; set; }
        public bool PersistSecurityInfo { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public int ConnectTimeout { get; set; }
    }
}
