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
using win_app.Elements;


namespace win_app.Windows
{

    /// <summary>
    /// Interaction logic for document_properties.xaml
    /// </summary>
    public partial class document_properties : Window
    {

        private double canvasWidth;
        private double canvasHeight;

        public LabelDefinition LabelDefinition { get; private set; }
        public event Action<LabelDefinition>? LabelAccepted;


        public document_properties()
        {
            InitializeComponent();
            this.Deactivated += OnWindowDeactivated; // Detect when user clicks outside

            // Attach event handlers
            LabelWidthTextBox.ValueChanged += (s, e) => UpdateLabelPreview();
            LabelHeightTextBox.ValueChanged += (s, e) => UpdateLabelPreview();
            MarginLeftTextBox.ValueChanged += (s, e) => UpdateLabelPreview();
            MarginTopTextBox.ValueChanged += (s, e) => UpdateLabelPreview();
            MarginRightTextBox.ValueChanged += (s, e) => UpdateLabelPreview();
            MarginBottomTextBox.ValueChanged += (s, e) => UpdateLabelPreview();

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
            // Guard: If canvas is not rendered yet, skip update
            if (double.IsNaN(canvasWidth) || double.IsNaN(canvasHeight) || canvasWidth == 0 || canvasHeight == 0)
            {
                Debug.WriteLine("Canvas size not ready. Skipping preview update.");
                return;
            }


            if (!double.TryParse(LabelWidthTextBox.Text, out double labelWidth))
            {
                return; // Stop if input is invalid
            }
            
            double labelHeight = LabelHeightTextBox.Value ?? 0;

            // Prevent divide by zero or invalid preview calculations
            if (labelWidth <= 0 || labelHeight <= 0)
            {
                AcceptButton.IsEnabled = false;
                return;
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


            // Reset borders
            LabelHeightTextBox.BorderBrush = Brushes.Gray;
            LabelWidthTextBox.BorderBrush = Brushes.Gray;
            LabelHeightTextBox.BorderThickness = new Thickness(1);
            LabelWidthTextBox.BorderThickness = new Thickness(1);


            // Each margin validated and drawn separately
            if (double.TryParse(MarginLeftTextBox.Text, out double marginLeft) && marginLeft >= 0 && marginLeft <= labelWidth)
            {
                double pxLeft = left + (marginLeft / labelWidth) * previewWidth;
                AddMarginLine(pxLeft, top, pxLeft, top + previewHeight);
            }
            else HighlightInvalidInput(MarginLeftTextBox);

            if (double.TryParse(MarginRightTextBox.Text, out double marginRight) && marginRight >= 0 && marginRight <= labelWidth)
            {
                double pxRight = left + previewWidth - (marginRight / labelWidth) * previewWidth;
                AddMarginLine(pxRight, top, pxRight, top + previewHeight);
            }
            else HighlightInvalidInput(MarginRightTextBox);

            if (double.TryParse(MarginTopTextBox.Text, out double marginTop) && marginTop >= 0 && marginTop <= labelHeight)
            {
                double pxTop = top + (marginTop / labelHeight) * previewHeight;
                AddMarginLine(left, pxTop, left + previewWidth, pxTop);
            }
            else HighlightInvalidInput(MarginTopTextBox);

            if (double.TryParse(MarginBottomTextBox.Text, out double marginBottom) && marginBottom >= 0 && marginBottom <= labelHeight)
            {
                double pxBottom = top + previewHeight - (marginBottom / labelHeight) * previewHeight;
                AddMarginLine(left, pxBottom, left + previewWidth, pxBottom);
            }
            else HighlightInvalidInput(MarginBottomTextBox);

            // enable the accept button
            AcceptButton.IsEnabled = labelWidth > 0 && labelHeight > 0;
        }


        private void AddMarginLine(double x1, double y1, double x2, double y2)
        {
            if (double.IsNaN(x1) || double.IsNaN(x2) || double.IsNaN(y1) || double.IsNaN(y2))
            {
                Debug.WriteLine("Skipping AddMarginLine due to NaN values.");
                return;
            }

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


        private void HighlightInvalidInput(Control control)
        {
            control.BorderBrush = Brushes.Red;
            control.BorderThickness = new Thickness(2);
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

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate and collect label properties
            if (!double.TryParse(LabelWidthTextBox.Text, out double width) || width <= 0) return;
            if (!double.TryParse(LabelHeightTextBox.Text, out double height) || height <= 0) return;
            double.TryParse(MarginLeftTextBox.Text, out double marginLeft);
            double.TryParse(MarginTopTextBox.Text, out double marginTop);
            double.TryParse(MarginRightTextBox.Text, out double marginRight);
            double.TryParse(MarginBottomTextBox.Text, out double marginBottom);

            LabelDefinition = new LabelDefinition
            {
                Width = width,
                Height = height,
                MarginLeft = marginLeft,
                MarginTop = marginTop,
                MarginRight = marginRight,
                MarginBottom = marginBottom
            };

            LabelAccepted?.Invoke(LabelDefinition);
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
