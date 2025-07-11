using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win_app.Models
{
    public class LabelDesign
    {
        // Any label design can have fixed and variable items only.
        // Fixed items are those that are always present in the label design. For example, logo, title, etc.
        public List<LabelItem> FixedItems { get; set; } = new();

        // Variable items are those that can be change by lebel to label. For example, price, receiver's address, etc.
        public List<LabelItem> VariableItems { get; set; } = new();

        // FUTURE: all the label design items like borders, background, lines, shapes, etc. can be added here.
    }
}
