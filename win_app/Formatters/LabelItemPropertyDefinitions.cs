using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Text.Json.Serialization; 

namespace win_app.Formatters
{
    public enum PropertyType
    {
        TextInput,
        NumericInput,
        Dropdown,
        InputDropdown,
        FilePath,
        Checkbox,
        IconSelection
    }

    public enum IconSelectionMode
    {
        Single,
        Multiple
    }

    public class IconOption : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Key { get; set; }
        public string Icon { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                    OnSelectedChanged?.Invoke(this);
                }
            }
        }
        
        [JsonIgnore]
        public Action<IconOption>? OnSelectedChanged { get; set; }

        public ICommand OnClickCommand => new RelayCommand(() =>
        {
            OnSelectedChanged?.Invoke(this);
        });

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class LabelItemProperty : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public PropertyType Type { get; set; }

        public List<string>? Options { get; set; }
        public List<IconOption>? IconOptions { get; set; }

        public IconSelectionMode? SelectionMode { get; set; }

        public string? SelectedValue { get; set; }
        public string? DefaultValue { get; set; }

        public LabelItemProperty(string name, PropertyType type, List<string> options, string? defaultValue = null)
        {
            Name = name;
            Type = type;
            Options = options;
            DefaultValue = defaultValue;
            SelectedValue = defaultValue;
        }

        public LabelItemProperty(string name, PropertyType type)
        {
            Name = name;
            Type = type;
        }

        public LabelItemProperty(string name, List<IconOption> icons, IconSelectionMode selectionMode, string? defaultValue = null)
        {
            Name = name;
            Type = PropertyType.IconSelection;
            IconOptions = icons;
            SelectionMode = selectionMode;
            DefaultValue = defaultValue;

            foreach (var icon in icons)
            {
                icon.OnSelectedChanged = HandleIconSelected;
                if (defaultValue != null && icon.Key == defaultValue)
                    icon.IsSelected = true;
            }
        }

        private void HandleIconSelected(IconOption selectedIcon)
        {
            if (SelectionMode == IconSelectionMode.Single)
            {
                foreach (var icon in IconOptions!)
                {
                    // Temporarily disable callback to avoid recursion
                    var originalCallback = icon.OnSelectedChanged;
                    icon.OnSelectedChanged = null;
                    icon.IsSelected = (icon == selectedIcon);
                    icon.OnSelectedChanged = originalCallback;
                }
            }
            else if (SelectionMode == IconSelectionMode.Multiple)
            {
                var originalCallback = selectedIcon.OnSelectedChanged;
                selectedIcon.OnSelectedChanged = null;
                selectedIcon.IsSelected = !selectedIcon.IsSelected;
                selectedIcon.OnSelectedChanged = originalCallback;
            }
        }



        public event PropertyChangedEventHandler? PropertyChanged;

        public LabelItemProperty Clone()
        {
            var clone = new LabelItemProperty(Name, Type)
            {
                Options = Options?.ToList(),
                IconOptions = IconOptions?.Select(icon => new IconOption
                {
                    Key = icon.Key,
                    Icon = icon.Icon,
                    IsSelected = icon.IsSelected
                }).ToList(),
                SelectionMode = SelectionMode,
                DefaultValue = DefaultValue,
                SelectedValue = DefaultValue
            };

            if (clone.IconOptions != null && clone.SelectionMode != null)
            {
                foreach (var icon in clone.IconOptions)
                    icon.OnSelectedChanged = clone.HandleIconSelected;
            }

            return clone;
        }
    }
}
