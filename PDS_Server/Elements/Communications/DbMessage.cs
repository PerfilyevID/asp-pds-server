using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonEnvironment.Elements
{
    public class DbMessage
    {
        public DbMessage() { }
        public ObjectId? From { get; set; }
        public ObjectId? To { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}
