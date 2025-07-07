using Fluent; // Import the Fluent Ribbon namespace
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using win_app.Elements;
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
        DispatcherTimer resizeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };


        public main_editing_window(List<LabelData> labelDataList)
        // public main_editing_window()
        {
            InitializeComponent();
            _labelDataList = labelDataList;

            // Open the left pane
            LeftPane.DataContext = new LeftPaneViewModel();

            DataGridLabelData.ItemsSource = _labelDataList; // Bind data to UI

            resizeTimer.Tick += (s, e) =>
            {
                resizeTimer.Stop();
                UpdateCanvasSizeAndRecenter();
            };

            ZoomboxControl.LayoutUpdated += (s, e) =>
            {
                if (!resizeTimer.IsEnabled)
                    resizeTimer.Start();
            };
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

            UpdateCanvasSizeAndRecenter();

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
            var scrollViewer = CanvasScrollViewer;

            // Get visible size
            double visibleWidth = scrollViewer.ViewportWidth;
            double visibleHeight = scrollViewer.ViewportHeight;

            // Set canvas size slightly larger than viewport to allow panning/zooming
            double canvasWidth = Math.Max(visibleWidth * 1.2, def.Width);
            double canvasHeight = Math.Max(visibleHeight * 1.2, def.Height);
            DesignCanvas.Width = canvasWidth;
            DesignCanvas.Height = canvasHeight;

            // Calculate label scale factor to fit 80% of visible area
            double scaleX = (visibleWidth * 0.8) / def.Width;
            double scaleY = (visibleHeight * 0.8) / def.Height;
            double scale = Math.Min(scaleX, scaleY); // Keep aspect ratio

            double labelWidth = def.Width * scale;
            double labelHeight = def.Height * scale;

            // Center label in canvas
            double left = (canvasWidth - labelWidth) / 2;
            double top = (canvasHeight - labelHeight) / 2;

            // Remove all elements except ZoomCenterIndicator
            for (int i = DesignCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (DesignCanvas.Children[i] is Canvas c && c.Name == "ZoomCenterIndicator")
                    continue;
                DesignCanvas.Children.RemoveAt(i);
            }

            // Create scaled label
            var rect = new Rectangle
            {
                Width = labelWidth,
                Height = labelHeight,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);
            DesignCanvas.Children.Add(rect);


            // Update canvas size and center
            UpdateCanvasSizeAndRecenter();
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

        private void UpdateCanvasSizeAndRecenter()
        {
            // Measure visible area for canvas
            var tabContentArea = ZoomboxControl;
            double visibleWidth = tabContentArea.ActualWidth;
            double visibleHeight = tabContentArea.ActualHeight;

            // Set canvas size to be a bit larger (to allow scroll space)
            double canvasWidth = visibleWidth * 1.2;
            double canvasHeight = visibleHeight * 1.2;

            DesignCanvas.Width = canvasWidth;
            DesignCanvas.Height = canvasHeight;

            // Recenter the label (if it exists)
            foreach (UIElement child in DesignCanvas.Children)
            {
                if (child is Rectangle labelRect)
                {
                    double labelWidth = labelRect.Width;
                    double labelHeight = labelRect.Height;

                    Canvas.SetLeft(labelRect, (canvasWidth - labelWidth) / 2);
                    Canvas.SetTop(labelRect, (canvasHeight - labelHeight) / 2);
                }
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            UpdateCanvasSizeAndRecenter(); // initial layout complete
        }


    }
}