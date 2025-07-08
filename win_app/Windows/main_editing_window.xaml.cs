using Fluent; // Import the Fluent Ribbon namespace
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using win_app.Elements;
using win_app.Models.Shortcuts;
using Xceed.Wpf.Toolkit;
using static win_app.Windows.document_properties;
using static win_app.Windows.file_opening_window;

namespace win_app.Windows
{
    /// <summary>
    /// Interaction logic for main_editing_window.xaml
    /// </summary>
    public partial class main_editing_window : RibbonWindow
    {

        private List<LabelData> _labelDataList;

        private Canvas _innerLabelCanvas;
        private Canvas _outerLabelCanvas;
        private Border _innerLabelBorder;
        private Border _outerLabelBorder;
        private double _marginLeft, _marginTop;


        public main_editing_window(List<LabelData> labelDataList)
        // public main_editing_window()
        {
            InitializeComponent();
            _labelDataList = labelDataList;

            // Open the left pane
            LeftPane.DataContext = new LeftPaneViewModel();

            DataGridLabelData.ItemsSource = _labelDataList; // Bind data to UI
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 700 || e.NewSize.Height < 300) // Adjust threshold as needed
            {
                LeftPane.Visibility = Visibility.Collapsed;
                LeftPaneColumn.Width = new GridLength(0); // Collapse the entire column
            }
            else
            {
                LeftPane.Visibility = Visibility.Visible;
                LeftPaneColumn.Width = new GridLength(1, GridUnitType.Star); // Restore the column
            }
        }


        private void DocumentPropertiesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0.5;
            var documentPropertiesWindow = new document_properties();
            documentPropertiesWindow.Owner = this;

            documentPropertiesWindow.LabelAccepted += def =>
            {
                ZoomSlider.Value = 100; // Set zoom to 100%
                AddLabelToCanvas(def);

                // Center the content after the layout is updated
                Dispatcher.InvokeAsync(() =>
                {
                    ZoomboxControl.ZoomTo(1.0);   // Ensure zoom is consistent with slider
                    ZoomboxControl.CenterContent(); // <--- THIS centers it!
                });
            };


            documentPropertiesWindow.ShowDialog();
            this.Opacity = 1.0;
        }

        private void AddLabelToCanvas(LabelDefinition def)
        {
            double zoom = ZoomSlider.Value / 100.0;

            // Compute available area from ScrollViewer
            double visibleWidth = CanvasScrollViewer.ViewportWidth;
            double visibleHeight = CanvasScrollViewer.ViewportHeight;

            Debug.WriteLine($"Viewport: {visibleWidth} x {visibleHeight}");

            // Scale so label takes 80% of visible area at 100% zoom
            double baseScaleX = (visibleWidth * 0.8) / def.Width;
            double baseScaleY = (visibleHeight * 0.8) / def.Height;
            double baseScale = Math.Min(baseScaleX, baseScaleY);

            double scaledWidth = def.Width * baseScale * zoom;
            double scaledHeight = def.Height * baseScale * zoom;

            double marginLeft = def.MarginLeft * baseScale * zoom;
            double marginTop = def.MarginTop * baseScale * zoom;
            double marginRight = def.MarginRight * baseScale * zoom;
            double marginBottom = def.MarginBottom * baseScale * zoom;

            _marginLeft = marginLeft;
            _marginTop = marginTop;


            // Inner content area
            double contentWidth = scaledWidth - marginLeft - marginRight;
            double contentHeight = scaledHeight - marginTop - marginBottom;

            // Set canvas size to match the label size (plus a small buffer if desired)
            double canvasWidth = Math.Max(visibleWidth, scaledWidth);
            double canvasHeight = Math.Max(visibleHeight, scaledHeight);

            DesignCanvas.Width = canvasWidth;
            DesignCanvas.Height = canvasHeight;

            double labelLeft = (canvasWidth - scaledWidth) / 2;
            double labelTop = (canvasHeight - scaledHeight) / 2;

            // Clear old elements except ZoomCenterIndicator
            for (int i = DesignCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (DesignCanvas.Children[i] is Canvas c && c.Name == "ZoomCenterIndicator")
                    continue;
                DesignCanvas.Children.RemoveAt(i);
            }

            // OUTER CANVAS (label area with yellow background)
            var outerCanvas = new Canvas
            {
                Width = scaledWidth,
                Height = scaledHeight,
                Background = Brushes.Yellow,
            };
            Canvas.SetLeft(outerCanvas, labelLeft);
            Canvas.SetTop(outerCanvas, labelTop);
            DesignCanvas.Children.Add(outerCanvas);

            // BORDER for label outline
            var outerBorder = new Border
            {
                Width = scaledWidth,
                Height = scaledHeight,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };
            Canvas.SetLeft(outerBorder, labelLeft);
            Canvas.SetTop(outerBorder, labelTop);
            DesignCanvas.Children.Add(outerBorder);

            // INNER CANVAS (content area - white background, where editing happens)
            var innerCanvas = new Canvas
            {
                Width = contentWidth,
                Height = contentHeight,
                Background = Brushes.White
            };
            Canvas.SetLeft(innerCanvas, labelLeft + marginLeft);
            Canvas.SetTop(innerCanvas, labelTop + marginTop);
            DesignCanvas.Children.Add(innerCanvas);

            // BORDER for label inner area
            var innerBorder = new Border
            {
                Width = contentWidth,
                Height = contentHeight,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };
            Canvas.SetLeft(innerBorder, labelLeft + marginLeft);
            Canvas.SetTop(innerBorder, labelTop + marginTop);
            DesignCanvas.Children.Add(innerBorder);

            // You can keep a reference to innerCanvas for future editing
            innerCanvas.Name = "LabelContentArea";

            // Store the inner canvas for include items(for future)
            _innerLabelCanvas = innerCanvas;
            _outerLabelCanvas = outerCanvas;
            _innerLabelBorder = innerBorder;
            _outerLabelBorder = outerBorder;
        }


        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ZoomPercentageText != null)
                ZoomPercentageText.Text = $"{(int)ZoomSlider.Value}%";

            // Corrected the assignment to use the Zoom method instead of treating it as a property
            ZoomboxControl.ZoomTo(ZoomSlider.Value / 100.0);
        }


        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomSlider.Value = Math.Max(ZoomSlider.Minimum, ZoomSlider.Value - 10);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomSlider.Value = Math.Min(ZoomSlider.Maximum, ZoomSlider.Value + 10);
        }

        // This method is called when the user clicks the ctrl with " + " buttons
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            var zoomIn = ShortcutManager.GetShortcut("ZoomIn");
            var zoomOut = ShortcutManager.GetShortcut("ZoomOut");

            if ((Keyboard.Modifiers & zoomIn.Modifiers) == zoomIn.Modifiers && e.Key == zoomIn.Key)
            {
                ZoomIn_Click(this, null);
                e.Handled = true;
            }
            else if ((Keyboard.Modifiers & zoomOut.Modifiers) == zoomOut.Modifiers && e.Key == zoomOut.Key)
            {
                ZoomOut_Click(this, null);
                e.Handled = true;
            }
        }

        // This method is called when the user scrolls the mouse wheel while holding ctrl
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta > 0)
                    ZoomIn_Click(this, null);
                else if (e.Delta < 0)
                    ZoomOut_Click(this, null);

                e.Handled = true;
            }
        }

    }
}