using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

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

        // Defines which properties are available for each type
        public static Dictionary<string, List<LabelItemProperty>> TypeProperties { get; } = new()
        {
            {
                "Text", new List<LabelItemProperty>
                {
                    new LabelItemProperty("Font", PropertyType.InputDropdown,
                        Fonts.SystemFontFamilies.Select(f => f.Source).ToList()),

                    new LabelItemProperty("FontSize", PropertyType.InputDropdown,
                        new List<string> { "8", "9", "10", "11", "12", "14", "16", "18", "20", "24", "28", "32", "36", "48", "72" }),

                    new LabelItemProperty("Alignment", new List<IconOption>
                    {
                        new IconOption { Key = "Left", Icon = "/Assets/icons/icons8-align-left-50.png" },
                        new IconOption { Key = "Center", Icon = "/Assets/icons/icons8-align-center-50.png" },
                        new IconOption { Key = "Right", Icon = "/Assets/icons/icons8-align-right-50.png" },
                        new IconOption { Key = "Justify", Icon = "/Assets/icons/icons8-align-justify-50.png" }
                    }, IconSelectionMode.Single),

                    new LabelItemProperty("Text Format", new List<IconOption>
                    {
                        new IconOption { Key = "Bold", Icon = "/Assets/icons/icons8-bold-50.png" },
                        new IconOption { Key = "Italic", Icon = "/Assets/icons/icons8-italic-50.png" },
                        new IconOption { Key = "Underline", Icon = "/Assets/icons/icons8-underline-50.png" }
                    }, IconSelectionMode.Multiple)
                }

            },
            
            {
                "Image", new List<LabelItemProperty>
                {
                    new LabelItemProperty("Image Path", PropertyType.FilePath)
                }
            },


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





    public enum PropertyType
    {
        TextInput,      // For simple text input only
        NumericInput,   // For numeric input only
        Dropdown,       // For dropdown selection only. No text input allowed
        InputDropdown,  // For dropdown with input option. When user type something, it shows related items in the dropdown.
        FilePath,       // For file path selection
        Checkbox,       // For boolean values
        IconSelection   // For icon selection 
    }

    public enum IconSelectionMode
    {
        Single,
        Multiple
    }

    public class IconOption : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Key { get; set; }       // Logical value (e.g., "Left", "Bold")
        public string Icon { get; set; }      // Could be Unicode, image path, or glyph
        public bool IsSelected                // Optional binding for toggle state
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                    OnSelectedChanged?.Invoke(this); // notify external handler
                }
            }
        }

        public Action<IconOption>? OnSelectedChanged { get; set; } // set by parent LabelItemProperty

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class LabelItemProperty
    {
        public string Name { get; set; }
        public PropertyType Type { get; set; }

        public List<string>? Options { get; set; }               // For dropdowns/input dropdowns
        public List<IconOption>? IconOptions { get; set; }       // For icon selections
        public IconSelectionMode? SelectionMode { get; set; }    // Only used for IconSelection

        // For icon selection
        public LabelItemProperty(string name, List<IconOption> icons, IconSelectionMode selectionMode)
        {
            Name = name;
            Type = PropertyType.IconSelection;
            IconOptions = icons;
            SelectionMode = selectionMode;

            // ✅ Hook up selection callback for each icon
            foreach (var icon in icons)
            {
                icon.OnSelectedChanged = HandleIconSelected;
            }
        }


        // For dropdown/input dropdown
        public LabelItemProperty(string name, PropertyType type, List<string> options)
        {
            Name = name;
            Type = type;
            Options = options;
        }

        // Basic constructor (needed for FilePath, TextInput, etc.)

        public LabelItemProperty(string name, PropertyType type)
        {
            Name = name;
            Type = type;
        }

        private void HandleIconSelected(IconOption selectedIcon)
        {
            if (SelectionMode == IconSelectionMode.Single)
            {
                foreach (var icon in IconOptions!)
                {
                    if (icon != selectedIcon && icon.IsSelected)
                        icon.IsSelected = false;
                }
            }
        }

    }

}
