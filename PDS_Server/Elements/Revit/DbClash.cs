using MongoDB.Bson;

namespace PDS_Server.Elements.Revit
{
    public class DbClash
    {
        public DbClash() { }
        public int Id { get; set; }
        public bool IsClosed { get; set; }
        public string Name { get; set; }
        public string ImageData { get; set; }
        public string Element_A { get; set; }
        public string Element_B { get; set; }
        public DbPoint Point { get; set; }
        public int Status { get; set; } = -1;
        public int GroupId { get; set; } = -1;
        public ObjectId Chat { get; set; }
    }
}
