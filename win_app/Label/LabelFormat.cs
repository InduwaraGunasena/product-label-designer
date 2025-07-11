using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win_app.Label
{
    // This is used to store how the label items look like. That is all formats and appearance will be save here
    public class LabelFormat
    {
        public string Name { get; set; }  // the unique name identifier for that item
        public Dictionary<string, object> Props { get; set; } = new();  // A dictionary of formats applied to that item.
    }

}
