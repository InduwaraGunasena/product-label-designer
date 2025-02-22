using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics; // Required for Debug.WriteLine


namespace win_app.Windows
{

    /// <summary>
    /// Interaction logic for document_properties.xaml
    /// </summary>
    public partial class document_properties : Window
    {

        private double canvasWidth;
        private double canvasHeight;

        public document_properties()
        {
            InitializeComponent();
            this.Deactivated += OnWindowDeactivated; // Detect when user clicks outside

            // Attach event handlers
            LabelWidthTextBox.TextChanged += (s, e) => UpdateLabelPreview();
            LabelHeightTextBox.TextChanged += (s, e) => UpdateLabelPreview();
            MarginLeftTextBox.TextChanged += (s, e) => UpdateLabelPreview();
            MarginTopTextBox.TextChanged += (s, e) => UpdateLabelPreview();
            MarginRightTextBox.TextChanged += (s, e) => UpdateLabelPreview();
            MarginBottomTextBox.TextChanged += (s, e) => UpdateLabelPreview();

            // Initialize preview
            UpdateLabelPreview();

        }


        private void LabelPreviewCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            // Get actual dimensions
            canvasWidth = LabelPreviewCanvas.ActualWidth;
            canvasHeight = LabelPreviewCanvas.ActualHeight;

            Debug.WriteLine($"Canvas Width: {canvasWidth}, Canvas Height: {canvasHeight}");

            // Now, you can use these variables in your UpdateLabelPreview method
            UpdateLabelPreview();
        }


        private void UpdateLabelPreview()
        {
            if (!double.TryParse(LabelWidthTextBox.Text, out double labelWidth) ||
                !double.TryParse(LabelHeightTextBox.Text, out double labelHeight)
               )
            {
                return; // Stop if input is invalid
            }
            
            double previewWidth;
            double previewHeight;

            if (labelHeight > labelWidth)
            {
                previewHeight = canvasHeight * 0.9; // take 80% of height in available height of the canvas area for the label
                previewWidth = previewHeight * (labelWidth / labelHeight);
            }
            else
            {
                previewWidth = canvasWidth * 0.9; // take 80% of width in available width of the canvas area for the label
                previewHeight = previewWidth * (labelHeight / labelWidth);
            }

            // Set Label Rectangle Size
            LabelRectangle.Width = previewWidth;
            LabelRectangle.Height = previewHeight;

            // Set Label Rectangle Position
            double left = (LabelPreviewCanvas.Width - previewWidth) / 2;
            double top = (LabelPreviewCanvas.Height - previewHeight) / 2;
            Canvas.SetLeft(LabelRectangle, left);
            Canvas.SetTop(LabelRectangle, top);

            // Clear existing margin lines
            LabelPreviewCanvas.Children.OfType<Line>().ToList().ForEach(line => LabelPreviewCanvas.Children.Remove(line));
            // Reset all textbox borders before validation
            ResetTextBoxBorders();

            // Left margin
            if (!double.TryParse(MarginLeftTextBox.Text, out double marginLeft) || marginLeft < 0 || marginLeft > labelWidth)
            {
                HighlightInvalidInput(MarginLeftTextBox);
                return;
            }
            // In preview, if we input left margin=10, we need to get the real proportion for the left margin by considering the preview length of the label
            double previewtLeftMargin = (marginLeft / labelWidth) * previewWidth;  

            double leftMargin_x1 = ((canvasWidth - previewWidth) / 2) + previewtLeftMargin;
            double leftMargin_x2 = leftMargin_x1; // same for x2 too.
            double leftMargin_y1 = ((canvasHeight - previewHeight) / 2);
            double leftMargin_y2 = leftMargin_y1 + previewHeight;
            AddMarginLine(leftMargin_x1, leftMargin_y1, leftMargin_x2, leftMargin_y2); // Left margin

            // Right margin
            if (!double.TryParse(MarginRightTextBox.Text, out double marginRight) || marginRight < 0 || marginRight > labelWidth)
            {
                HighlightInvalidInput(MarginRightTextBox);
                return;
            }
            double previewtRightMargin = (marginRight / labelWidth) * previewWidth;

            double rightMargin_x1 = ((canvasWidth - previewWidth) / 2) + previewWidth - previewtRightMargin;
            double rightMargin_x2 = rightMargin_x1;
            double rightMargin_y1 = (canvasHeight - previewHeight) / 2;
            double rightMargin_y2 = rightMargin_y1 + previewHeight;
            AddMarginLine(rightMargin_x1, rightMargin_y1, rightMargin_x2, rightMargin_y2); // Right margin

            // Bottom margin
            if (!double.TryParse(MarginBottomTextBox.Text, out double marginBottom) || marginBottom < 0 || marginBottom > labelHeight)
            {
                HighlightInvalidInput(MarginBottomTextBox);
                return;
            }
            double previewtBottomMargin = (marginBottom / labelHeight) * previewHeight;

            double bottomMargin_x1 = (canvasWidth - previewWidth) / 2;
            double bootomMargin_x2 = bottomMargin_x1 + previewWidth;
            double bootomMargin_y1 = ((canvasHeight - previewHeight) / 2) + previewHeight - previewtBottomMargin; 
            double bootomMargin_y2 = bootomMargin_y1;
            AddMarginLine(bottomMargin_x1, bootomMargin_y1, bootomMargin_x2, bootomMargin_y2);

            // Top margin
            if (!double.TryParse(MarginTopTextBox.Text, out double marginTop) || marginTop < 0 || marginTop > labelHeight)
            {
                HighlightInvalidInput(MarginTopTextBox);
                return;
            }
            double previewtTopMargin = (marginTop / labelHeight) * previewHeight;

            double topMargin_x1 = (canvasWidth - previewWidth) / 2;
            double topMargin_x2 = topMargin_x1 + previewWidth;
            double topMargin_y1 = ((canvasHeight - previewHeight) / 2) + previewtTopMargin;
            double topMargin_y2 = topMargin_y1;
            AddMarginLine(topMargin_x1, topMargin_y1, topMargin_x2, topMargin_y2);
        }


        private void AddMarginLine(double x1, double y1, double x2, double y2)
        {
            Line line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 2, 2 }
            };
            LabelPreviewCanvas.Children.Add(line);
        }


        private void HighlightInvalidInput(TextBox textBox)
        {
            textBox.BorderBrush = Brushes.Red;
            textBox.BorderThickness = new Thickness(2);
        }

        private void ResetTextBoxBorders()
        {
            MarginLeftTextBox.BorderBrush = Brushes.Gray;
            MarginRightTextBox.BorderBrush = Brushes.Gray;
            MarginTopTextBox.BorderBrush = Brushes.Gray;
            MarginBottomTextBox.BorderBrush = Brushes.Gray;

            MarginLeftTextBox.BorderThickness = new Thickness(1);
            MarginRightTextBox.BorderThickness = new Thickness(1);
            MarginTopTextBox.BorderThickness = new Thickness(1);
            MarginBottomTextBox.BorderThickness = new Thickness(1);
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
                uCount = 5,  // Number of flashes
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
