using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace win_app.Formatters
{
    public static class LabelItemFormatterRegistry
    {
        private static readonly Dictionary<string, List<LabelItemProperty>> TypePropertyDefinitions = new()
        {
            {
                "Text", new List<LabelItemProperty>
                {
                    new("Font", PropertyType.InputDropdown, Fonts.SystemFontFamilies.Select(f => f.Source).ToList(), "Arial"),
                    new("FontSize", PropertyType.InputDropdown, new() { "8", "9", "10", "11", "12", "14", "16", "18", "20", "24", "28", "32", "36", "48", "72" }, "12"),
                    new("Alignment", new List<IconOption>
                    {
                        new() { Key = "Left", Icon = "/Assets/icons/icons8-align-left-50.png" },
                        new() { Key = "Center", Icon = "/Assets/icons/icons8-align-center-50.png" },
                        new() { Key = "Right", Icon = "/Assets/icons/icons8-align-right-50.png" },
                        new() { Key = "Justify", Icon = "/Assets/icons/icons8-align-justify-50.png" }

                    }, IconSelectionMode.Single, "Left"),
                    new("Text Format", new List<IconOption>
                    {
                        new() { Key = "Bold", Icon = "/Assets/icons/icons8-bold-50.png" },
                        new() { Key = "Italic", Icon = "/Assets/icons/icons8-italic-50.png" },
                        new() { Key = "Underline", Icon = "/Assets/icons/icons8-underline-50.png" }
                    }, IconSelectionMode.Multiple),

                }
            },
            {
                "Image", new List<LabelItemProperty>
                {
                    new("Image Path", PropertyType.FilePath)
                }
            },
            {
                "Barcode", new List<LabelItemProperty>
                {
                    new("Type", PropertyType.Dropdown, new() { "Barcode", "QR"}, "Barcode"),
                }
            }
        };

        public static List<string> GetAllTypes() => TypePropertyDefinitions.Keys.ToList();

        public static List<LabelItemProperty> GetPropertyDefinitions(string type)
        {
            return TypePropertyDefinitions.ContainsKey(type)
                ? TypePropertyDefinitions[type].Select(p => p.Clone()).ToList()
                : new();
        }
    }
}
