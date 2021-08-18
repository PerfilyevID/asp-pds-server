using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS_Server.Elements.Plugins
{
    public class DbCheckRow
    {
        public DbCheckRow() { }
        public string Name { get; set; }
        public string Description { get; set; }
        public Tuple<float, DateTime>[] Data { get; set; }
    }
}
