using MongoDB.Bson;
using PDS_Server.Elements.Revit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonEnvironment.Elements
{
    public class DbClashResult : DbElement
    {
        public DbClashResult() { }
        public ObjectId? Project { get; set; }
        public DbClash[] Clashes { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public DateTime LastChange { get; set; } = DateTime.UtcNow;
        public int ItemsCount { get; set; }
        public int ItemsDone { get; set; }
    }
}
