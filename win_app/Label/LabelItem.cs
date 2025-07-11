using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using win_app.Formatters;

namespace win_app.Label
{
    // every label item has these properties.
    public class LabelItem
    {
        public string Name { get; set; } // a unique name assign to that item
        private string _type;
        public string Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    Formats = LabelItemFormatterRegistry.GetPropertyDefinitions(_type);
                }
            }
        }

        public string? Value { get; set; }  // the value of that item (except for shapes)
        public string Category { get; set; } // "Fixed" or "Variable"

        public List<LabelItemProperty> Formats { get; set; } = new();


    }

}
