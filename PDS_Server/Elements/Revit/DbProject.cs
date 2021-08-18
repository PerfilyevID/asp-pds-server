using MongoDB.Bson;

namespace CommonEnvironment.Elements
{
    public class DbProject : DbElement
    {
        public DbProject() { }
        public string Name { get; set; }
        public string Color { get; set; }
        public ObjectId Team { get; set; }
        public ObjectId[] Documents { get; set; }
    }
}
