namespace CommonEnvironment.Elements
{
    public class DbApplication : DbElement
    {
        public DbApplication() { }
        public string Version { get; set; }
        public string Changelog { get; set; }
        public string Link { get; set; }
    }
}
