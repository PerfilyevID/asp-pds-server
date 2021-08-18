using MongoDB.Bson;
using PDS_Server.Elements.Plugins;

namespace CommonEnvironment.Elements.Revit
{
    public class DbCheckResult : DbElement
    {
        public DbCheckResult() { }
        public ObjectId? Project { get; set; }
        public ObjectId? Document { get; set; }
        public DbCheckRow[] Data { get; set; }
    }
}
