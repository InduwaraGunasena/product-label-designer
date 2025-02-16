using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace win_app.Windows
{
    /// <summary>
    /// Interaction logic for document_properties.xaml
    /// </summary>
    public partial class document_properties : Window
    {
        public document_properties()
        {
            InitializeComponent();
            this.Deactivated += OnWindowDeactivated; // Detect when user clicks outside

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            FlashWindow(); // Flash the window
        }

        private void FlashWindow()
        {
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            FLASHWINFO fi = new FLASHWINFO
            {
                cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(FLASHWINFO))),
                hwnd = hWnd,
                dwFlags = FLASHW_TRAY | FLASHW_TIMERNOFG, // Flash only when not in focus
                uCount = 2,  // Number of flashes
                dwTimeout = 0
            };
            FlashWindowEx(ref fi);
        }

        #region FlashWindow API
        private const uint FLASHW_STOP = 0;
        private const uint FLASHW_CAPTION = 1;
        private const uint FLASHW_TRAY = 2;
        private const uint FLASHW_ALL = 3;
        private const uint FLASHW_TIMER = 4;
        private const uint FLASHW_TIMERNOFG = 12;

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
        #endregion

    }
}
