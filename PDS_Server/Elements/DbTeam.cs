using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonEnvironment.Elements
{
    public class DbTeam : DbElement
    {
        public DbTeam() { }
        public string Name { get; set; }
        public ObjectId? Owner { get; set; }
        public ObjectId[] Users { get; set; }
        public int MaxCount { get; set; }
    }
}
