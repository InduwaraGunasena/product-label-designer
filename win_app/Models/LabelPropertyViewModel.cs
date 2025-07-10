using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using win_app.Models;

namespace win_app.Models
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
                _selectedValue = value;
                OnPropertyChanged(nameof(SelectedValue));
            }
        }

        public LabelPropertyViewModel(LabelItemProperty property)
        {
            Name = property.Name;
            Type = property.Type;

            Options = property.Options != null ? new ObservableCollection<string>(property.Options) : new ObservableCollection<string>();
            IconOptions = property.IconOptions != null ? new ObservableCollection<IconOption>(property.IconOptions) : new ObservableCollection<IconOption>();
            SelectionMode = property.SelectionMode;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}