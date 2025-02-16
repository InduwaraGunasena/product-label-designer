using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

namespace win_app.Elements
{
    public class LeftPaneViewModel
    {
        // Collection of tool items
        public ObservableCollection<LeftPaneToolItem> Tools { get; set; }

        public LeftPaneViewModel()
        {
            Tools = new ObservableCollection<LeftPaneToolItem>
        {
            new LeftPaneToolItem { Icon = "/Assets/icons/icons8-text-100.png", Label = "Text" },
            new LeftPaneToolItem { Icon = "/Assets/icons/icons8-barcode-100.png", Label = "Barcode" },
            new LeftPaneToolItem { Icon = "/Assets/icons/icons8-image-100.png", Label = "Picture" },           
            new LeftPaneToolItem { Icon = "/Assets/icons/icons8-line-100.png", Label = "Line" },
            new LeftPaneToolItem { Icon = "/Assets/icons/icons8-rectangular-100.png", Label = "Rectangle" },
            new LeftPaneToolItem { Icon = "/Assets/icons/icons8-oval-100.png", Label = "Ellipse" },
            new LeftPaneToolItem { Icon = "/Assets/icons/icons8-invert-selection-100.png", Label = "Invert" }
        };
        }
    }
}
