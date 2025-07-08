using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace win_app.Models
{
    public static class ShortcutManager
    {
        public static Dictionary<string, Shortcut> Shortcuts { get; private set; }

        static ShortcutManager()
        {
            Shortcuts = new Dictionary<string, Shortcut>
        {
            { "ZoomIn", new Shortcut(Key.OemPlus, ModifierKeys.Control) },
            { "ZoomOut", new Shortcut(Key.OemMinus, ModifierKeys.Control) }
        };
        }

        public static void UpdateShortcut(string action, Shortcut shortcut)
        {
            if (Shortcuts.ContainsKey(action))
                Shortcuts[action] = shortcut;
        }

        public static Shortcut GetShortcut(string action)
        {
            return Shortcuts.TryGetValue(action, out var shortcut) ? shortcut : null;
        }
    }
}

