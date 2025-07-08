using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace win_app.Models
{
    public class Shortcut
    {
        public Key Key { get; set; }
        public ModifierKeys Modifiers { get; set; }

        public Shortcut(Key key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public override string ToString() => $"{Modifiers} + {Key}";
    }
}

