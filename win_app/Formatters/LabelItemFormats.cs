using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using win_app.Formatters;

namespace win_app.Formatters
{
    public static class LabelItemFormats
    {
        public static List<ItemFormatPropertyDefinition> TextProperties => new()
        {
            new("font", PropertyType.InputDropdown, defaultValue: "Arial", options: SystemFonts),
            new("fontSize", PropertyType.InputDropdown, defaultValue: "12", options: new() { "8", "10", "12", "14", "16", "18", "24" }),
            new("isBold", PropertyType.Checkbox, "false"),
            new("isItalic", PropertyType.Checkbox, "false"),
            new("isUnderline", PropertyType.Checkbox, "false"),
            new("horizontalAlign", PropertyType.Dropdown, "left", new() { "left", "center", "right", "justify" }),
            new("verticalAlign", PropertyType.Dropdown, "top", new() { "top", "middle", "bottom" }),
            new("width", PropertyType.NumericInput, "100"),
            new("height", PropertyType.NumericInput, "30"),
            new("posX", PropertyType.NumericInput),
            new("posY", PropertyType.NumericInput),
        };

        public static List<ItemFormatPropertyDefinition> ImageProperties => new()
        {
            new("imagePath", PropertyType.FilePath),
            new("width", PropertyType.NumericInput),
            new("height", PropertyType.NumericInput),
            new("posX", PropertyType.NumericInput),
            new("posY", PropertyType.NumericInput),
        };

        public static List<ItemFormatPropertyDefinition> BarcodeProperties => new()
        {
            new("type", PropertyType.Dropdown, "qr", new() { "qr", "code128", "ean13" }),
            new("width", PropertyType.NumericInput),
            new("height", PropertyType.NumericInput),
            new("posX", PropertyType.NumericInput),
            new("posY", PropertyType.NumericInput),
        };

        public static List<ItemFormatPropertyDefinition> LineProperties => new()
        {
            new("length", PropertyType.NumericInput),
            new("thickness", PropertyType.NumericInput),
            new("startPosX", PropertyType.NumericInput),
            new("startPosY", PropertyType.NumericInput),
            new("endPosX", PropertyType.NumericInput),
            new("endPosY", PropertyType.NumericInput),
            new("angle", PropertyType.NumericInput)
        };

        private static List<string> SystemFonts => Fonts.SystemFontFamilies.Select(f => f.Source).ToList();
    }
}
