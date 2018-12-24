using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickaxeCore
{
    class Option
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }
        public object Argument { get; set; }
    }
}
