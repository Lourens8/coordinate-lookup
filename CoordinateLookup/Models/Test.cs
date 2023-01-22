using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateLookup.Models
{
    [DebuggerDisplay("Name = {Name}")]
    public class Test
    {
        public string Name { get; set; }
        public decimal Position { get; set; }
        public decimal P2 { get; set; }
    }
}
