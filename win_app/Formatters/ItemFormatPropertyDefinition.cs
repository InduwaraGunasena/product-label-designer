using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using win_app.Formatters;

namespace win_app.Formatters
{
    public class ItemFormatPropertyDefinition
    {
        public string Name { get; set; }
        public PropertyType Type { get; set; }
        public string? DefaultValue { get; set; }
        public List<string>? Options { get; set; }

        public ItemFormatPropertyDefinition(string name, PropertyType type, string? defaultValue = null, List<string>? options = null)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
            Options = options;
        }
    }
}
