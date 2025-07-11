using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win_app.Label
{
    public class LabelFormats
    {
        public List<LabelItemFormat> Fixed { get; set; } = new();
        public List<LabelItemFormat> Variable { get; set; } = new();
        public List<LabelItemFormat> Shapes { get; set; } = new();
    }
}
