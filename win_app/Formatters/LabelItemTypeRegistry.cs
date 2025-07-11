using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace win_app.Formatters
{
    public static class LabelItemTypeRegistry
    {
        private static readonly Dictionary<string, List<ItemFormatPropertyDefinition>> _typeFormats = new()
        {
            ["text"] = LabelItemFormats.TextProperties,
            ["image"] = LabelItemFormats.ImageProperties,
            ["barcode"] = LabelItemFormats.BarcodeProperties,
            ["line"] = LabelItemFormats.LineProperties
        };

        public static IEnumerable<string> GetSupportedTypes() => _typeFormats.Keys;

        public static List<ItemFormatPropertyDefinition> GetFormatProperties(string type)
        {
            return _typeFormats.TryGetValue(type.ToLower(), out var props) ? props : new();
        }

        public static void RegisterType(string type, List<ItemFormatPropertyDefinition> properties)
        {
            _typeFormats[type.ToLower()] = properties;
        }

        public static void UnregisterType(string type)
        {
            _typeFormats.Remove(type.ToLower());
        }
    }
}
