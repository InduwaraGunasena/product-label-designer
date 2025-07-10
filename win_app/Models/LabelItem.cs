using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win_app.Models
{
    public class LabelItem : INotifyPropertyChanged
    {
        public string Name { get; set; }   

        private string _type;
        public string Type  
        {
            get => _type;
            set
            {
                _type = value;  
                OnPropertyChanged(nameof(Type));
                LoadDefaultPropertiesForType(value);
            }
        }

        public Dictionary<string, object> Properties { get; set; } = new();

        private void LoadDefaultPropertiesForType(string type)
        {
            Properties.Clear();
            if (LabelItemTypes.TypeProperties.TryGetValue(type, out var props))
            {
                foreach (var prop in props)
                {
                    Properties[prop.Name] = null;
                }
            }
            OnPropertyChanged(nameof(Properties));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
