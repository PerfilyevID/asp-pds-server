using MongoDB.Bson;

namespace CommonEnvironment.Elements
{
    public class DbFamilyCategory : DbElement
    {
        public DbFamilyCategory() { }
        public string Name { get; set; }
        public string ImageData { get; set; }
        public DbFamilyCategory[] Children { get; set; }
    }
}
