using System;
using System.Collections.Generic;

namespace CommonEnvironment.Elements
{
    public class DbChat : DbElement
    {
        public DbChat() { }
        public List<DbMessage> Messages { get; set; }
        public DateTime LastChange { get; set; } = DateTime.UtcNow;
    }
}
