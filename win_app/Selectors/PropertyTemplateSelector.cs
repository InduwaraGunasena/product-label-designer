using System.Windows;
using System.Windows.Controls;
using win_app.Models;

namespace win_app.Selectors
{
    public class PropertyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate InputDropdownTemplate { get; set; }
        public DataTemplate IconSelectionTemplate { get; set; }
        public DataTemplate TextInputTemplate { get; set; }
        public DataTemplate FilePathTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is LabelPropertyViewModel prop)
            {
                return prop.Type switch
                {
                    PropertyType.InputDropdown => InputDropdownTemplate,
                    PropertyType.IconSelection => IconSelectionTemplate,
                    PropertyType.TextInput => TextInputTemplate,
                    PropertyType.FilePath => FilePathTemplate,
                    _ => TextInputTemplate,
                };
            }

            return base.SelectTemplate(item, container);
        }
    }
}
