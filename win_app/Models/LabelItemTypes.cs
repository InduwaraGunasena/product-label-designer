using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win_app.Models
{
    public static class LabelItemTypes
    {
        public static List<string> AllTypes { get; } = new()
        {
            "Text",
            "Number",
            "Date",
            "Barcode",
            "Image"
        };

        // Optionally allow dynamic modifications in the future
        public static void AddType(string newType)
        {
            if (!AllTypes.Contains(newType))
                AllTypes.Add(newType);
        }

        public static void RemoveType(string type)
        {
            AllTypes.Remove(type);
        }
    }
}
