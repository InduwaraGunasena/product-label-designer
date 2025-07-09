using Fluent; // Import the Fluent Ribbon namespace
using System;
using System.Collections.Generic;
using System.Diagnostics; // Required for Debug.WriteLine
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using win_app.Elements;
using win_app.Models;
using Xceed.Wpf.Toolkit;
using static win_app.Elements.HorizontalRuler;
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

        bool _suppressAutoCenter = false;

        private readonly DispatcherTimer _rulerUpdateTimer = new DispatcherTimer();


        public main_editing_window(List<LabelData> labelDataList)
        // public main_editing_window()
        {
            InitializeComponent();
            _labelDataList = labelDataList;

           
            // Setup timer interval and event
            _rulerUpdateTimer.Interval = TimeSpan.FromMilliseconds(150);
            _rulerUpdateTimer.Tick += (s, args) =>
            {
                _rulerUpdateTimer.Stop();
                PrintLabelAndViewportCoordinates();
            };

            // Open the left pane
            LeftPane.DataContext = new LeftPaneViewModel();

            DataGridLabelData.ItemsSource = _labelDataList; // Bind data to UI

            // Attach to layout update of Zoombox to detect pan
            ZoomboxControl.LayoutUpdated += ZoomboxControl_LayoutUpdated;
        }

        private void ScheduleRulerUpdate()
        {
            _rulerUpdateTimer.Stop();
            _rulerUpdateTimer.Start(); // reset the timer
        }


        private void ZoomboxControl_LayoutUpdated(object? sender, EventArgs e)
        {
            ScheduleRulerUpdate();
        }


        private void DocumentPropertiesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0.5;
            var documentPropertiesWindow = new document_properties();
            documentPropertiesWindow.Owner = this;

            documentPropertiesWindow.LabelAccepted += def =>
            {
                Debug.WriteLine("LabelAccepted triggered!");
                ZoomSlider.Value = 100; // Set zoom to 100%

                // Wait until layout is rendered to get correct viewport sizes
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Debug.WriteLine("Calling AddLabelToCanvas AFTER layout render");
                    AddLabelToCanvas(def); // draw label
                    UpdateCanvasSizeAndRecenter();
                    ZoomboxControl.ZoomTo(1.0);

                    if (!_suppressAutoCenter)
                    {
                        ZoomboxControl.CenterContent();
                    }

                }), DispatcherPriority.Render); // <--- use DispatcherPriority.Render instead of Loaded
            };


            documentPropertiesWindow.ShowDialog();
            this.Opacity = 1.0;
        }

        private void AddLabelToCanvas(LabelDefinition def)
        {
            double zoom = ZoomSlider.Value / 100.0;

            // Compute available area from ScrollViewer
            double visibleWidth = ZoomboxControl.ActualWidth;
            double visibleHeight = ZoomboxControl.ActualHeight;

            Debug.WriteLine($"Viewport: {visibleWidth} x {visibleHeight}");

            // Scale so label takes 70% of visible area at 100% zoom
            double baseScaleX = (visibleWidth * 0.7) / def.Width;
            double baseScaleY = (visibleHeight * 0.7) / def.Height;
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

            // Set canvas to tightly wrap the label + small padding
            double padding = 50; // enough for rulers, outline, selection handles

            double canvasWidth = scaledWidth + padding * 2;
            double canvasHeight = scaledHeight + padding * 2;

            LabelHostCanvas.Width = canvasWidth;
            LabelHostCanvas.Height = canvasHeight;

            double labelLeft = padding;
            double labelTop = padding;


            // Clear old elements except ZoomCenterIndicator
            for (int i = LabelHostCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (LabelHostCanvas.Children[i] is Canvas c && c.Name == "ZoomCenterIndicator")
                    continue;
                LabelHostCanvas.Children.RemoveAt(i);
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
            LabelHostCanvas.Children.Add(outerCanvas);

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
            LabelHostCanvas.Children.Add(outerBorder);

            // INNER CANVAS (content area - white background, where editing happens)
            var innerCanvas = new Canvas
            {
                Width = contentWidth,
                Height = contentHeight,
                Background = Brushes.White
            };
            Canvas.SetLeft(innerCanvas, labelLeft + marginLeft);
            Canvas.SetTop(innerCanvas, labelTop + marginTop);
            LabelHostCanvas.Children.Add(innerCanvas);

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
            LabelHostCanvas.Children.Add(innerBorder);

            // You can keep a reference to innerCanvas for future editing
            innerCanvas.Name = "LabelContentArea";

            // Store the inner canvas for include items(for future)
            _innerLabelCanvas = innerCanvas;
            _outerLabelCanvas = outerCanvas;
            _innerLabelBorder = innerBorder;
            _outerLabelBorder = outerBorder;

            ScheduleRulerUpdate();

        }


        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ZoomPercentageText != null)
                ZoomPercentageText.Text = $"{(int)ZoomSlider.Value}%";

            // Corrected the assignment to use the Zoom method instead of treating it as a property
            ZoomboxControl.ZoomTo(ZoomSlider.Value / 100.0);

            ScheduleRulerUpdate();
        }


        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomSlider.Value = Math.Max(ZoomSlider.Minimum, ZoomSlider.Value - 10);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomSlider.Value = Math.Min(ZoomSlider.Maximum, ZoomSlider.Value + 10);
        }

        private void UpdateCanvasSizeAndRecenter()
        {
            if (!_suppressAutoCenter)
            {
                ZoomboxControl.CenterContent();
            }

        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            UpdateCanvasSizeAndRecenter(); // initial layout complete
        }

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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _suppressAutoCenter = true;
            Dispatcher.BeginInvoke(() =>
            {
                _suppressAutoCenter = false;
            }, DispatcherPriority.Render);
        }

        // Event handler for recenter button click
        private void RecenterButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomboxControl.CenterContent();

            ScheduleRulerUpdate();
        }


        // Event handler for zoom fit button click
        private void ZoomFitButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomboxControl.FitToBounds();

            ScheduleRulerUpdate();
        }

        private void PrintLabelAndViewportCoordinates()
        {
            if (_outerLabelCanvas == null || ZoomboxControl == null)
                return;

            // Get the label's left & right (relative to LabelHostCanvas)
            double labelLeft = Canvas.GetLeft(_outerLabelCanvas);
            double labelRight = labelLeft + _outerLabelCanvas.Width;

            // Transform viewport (0,0) and (ActualWidth, ActualHeight) from Zoombox to LabelHostCanvas
            GeneralTransform transform = ZoomboxControl.TransformToVisual(LabelHostCanvas);

            Point viewportTopLeftInCanvas = transform.Transform(new Point(0, 0));
            Point viewportBottomRightInCanvas = transform.Transform(new Point(ZoomboxControl.ActualWidth, ZoomboxControl.ActualHeight));

            //Debug.WriteLine("====== Coordinate Info ======");
            //Debug.WriteLine($"Label Left (Canvas): {labelLeft}");
            //Debug.WriteLine($"Label Right (Canvas): {labelRight}");
            //Debug.WriteLine($"Viewport Top Left (Canvas): {viewportTopLeftInCanvas.X}");
            //Debug.WriteLine($"Viewport Bottom Right (Canvas): {viewportBottomRightInCanvas.X}");

            // Relative to viewport
            double labelLeftRelativeToViewport = labelLeft - viewportTopLeftInCanvas.X;
            double labelRightRelativeToViewport = labelRight - viewportTopLeftInCanvas.X;

            Debug.WriteLine($"Label Left Relative to Viewport: {labelLeftRelativeToViewport}");
            Debug.WriteLine($"Label Right Relative to Viewport: {labelRightRelativeToViewport}");

            HorizontalRuler.LabelLeft = labelLeftRelativeToViewport;
            HorizontalRuler.LabelRight = labelRightRelativeToViewport;
            HorizontalRuler.ZoomLevel = ZoomSlider.Value / 100.0;
            HorizontalRuler.UnitSize = 96.0 / 25.4; // 1 mm = 96 DPI / 25.4
            HorizontalRuler.UnitType = MeasurementUnit.Millimeter;


        }


    }
}