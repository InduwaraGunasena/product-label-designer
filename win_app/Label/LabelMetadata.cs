using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win_app.Label
{
    // this class is used for define the properties of the label.
    public class LabelMetadata
    {
        public float Width { get; set; }  // width of the label
        public float Height { get; set; }  // height of the label
        public string Unit { get; set; }  // which unit(mm, inch) is used to measure everything in the application
        public int Dpi { get; set; } // dpi of the printer
    }

}
