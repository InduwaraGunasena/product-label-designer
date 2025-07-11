using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win_app.Label
{
    public class LabelItemFormat
    {
        public string Name { get; set; }   // Links to LabelItem.Name
        public Dictionary<string, object> Props { get; set; } = new();  // Flexible set of props per type
    }
}
