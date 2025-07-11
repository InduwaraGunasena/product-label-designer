using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using win_app.Formatters;

namespace win_app.Formatters
{
    public class LabelPropertyViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }           // e.g., "Font"
        public PropertyType Type { get; set; }     // e.g., InputDropdown
        public ObservableCollection<string> Options { get; set; }
        public ObservableCollection<IconOption> IconOptions { get; set; }

        public IconSelectionMode? SelectionMode { get; set; }

        private string _selectedValue;
        public string SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (_selectedValue != value)
                {
                    _selectedValue = value;
                    PropertyModel.SelectedValue = value; // 🟡 persist to model
                    OnPropertyChanged(nameof(SelectedValue));
                }
            }
        }

        public LabelItemProperty PropertyModel { get; set; }

        public LabelPropertyViewModel(LabelItemProperty property)
        {
            PropertyModel = property;
            Name = property.Name;
            Type = property.Type;
            Options = new ObservableCollection<string>(property.Options ?? new());
            IconOptions = property.IconOptions != null ? new ObservableCollection<IconOption>(property.IconOptions) : new();
            SelectionMode = property.SelectionMode;
            SelectedValue = property.SelectedValue ?? property.DefaultValue ?? "";
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}