using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonEnvironment.Elements
{
    public class DbReport : DbElement
    {
        public DbReport() { }
        public string Issue { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Comment { get; set; }
        public bool IsClosed { get; set; } = false;
        public string User { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}
