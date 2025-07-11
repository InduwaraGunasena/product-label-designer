using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win_app.Label
{
    public class LabelItems
    {
        public List<LabelItem> Fixed { get; set; } = new();
        public List<LabelItem> Variable { get; set; } = new();
        public List<LabelItem> Shapes { get; set; } = new();
    }

}
