using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace win_app.Elements
{
    /// <summary>
    /// Interaction logic for HorizontalRuler.xaml
    /// </summary>
    public partial class HorizontalRuler : UserControl
    {
        public HorizontalRuler()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelLeftProperty =
    DependencyProperty.Register("LabelLeft", typeof(double), typeof(HorizontalRuler),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty LabelRightProperty =
            DependencyProperty.Register("LabelRight", typeof(double), typeof(HorizontalRuler),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double LabelLeft
        {
            get => (double)GetValue(LabelLeftProperty);
            set => SetValue(LabelLeftProperty, value);
        }

        public double LabelRight
        {
            get => (double)GetValue(LabelRightProperty);
            set => SetValue(LabelRightProperty, value);
        }

        public enum MeasurementUnit
        {
            Millimeter,
            Inch
        }

        public static readonly DependencyProperty ZoomLevelProperty =
    DependencyProperty.Register("ZoomLevel", typeof(double), typeof(HorizontalRuler),
        new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty UnitSizeProperty =
            DependencyProperty.Register("UnitSize", typeof(double), typeof(HorizontalRuler),
                new FrameworkPropertyMetadata(3.78, FrameworkPropertyMetadataOptions.AffectsRender)); // Default: mm in WPF (96dpi / 25.4)

        public static readonly DependencyProperty UnitTypeProperty =
            DependencyProperty.Register("UnitType", typeof(MeasurementUnit), typeof(HorizontalRuler),
                new FrameworkPropertyMetadata(MeasurementUnit.Millimeter, FrameworkPropertyMetadataOptions.AffectsRender));

        public double ZoomLevel
        {
            get => (double)GetValue(ZoomLevelProperty);
            set => SetValue(ZoomLevelProperty, value);
        }

        public double UnitSize
        {
            get => (double)GetValue(UnitSizeProperty);
            set => SetValue(UnitSizeProperty, value);
        }

        public MeasurementUnit UnitType
        {
            get => (MeasurementUnit)GetValue(UnitTypeProperty);
            set => SetValue(UnitTypeProperty, value);
        }


        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double width = ActualWidth;
            double height = ActualHeight;

            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

            double unitPixelSize = UnitSize * ZoomLevel;

            double scaledLeft = LabelLeft * ZoomLevel;
            double scaledRight = LabelRight * ZoomLevel;

            double labelWidthPixels = scaledRight - scaledLeft;

            // Tick drawing loop from left to right inside the label area
            double logicalUnit = 0; // start from 0 at label's left
            for (double x = scaledLeft; x <= scaledRight; x += unitPixelSize, logicalUnit += 1)
            {
                if (x >= 0 && x <= width) // Only draw ticks that fall within the ruler area
                {
                    double tickHeight = (logicalUnit % 10 == 0) ? 15 : 7;

                    dc.DrawLine(new Pen(Brushes.Gray, 1), new Point(x, height), new Point(x, height - tickHeight));

                    if (logicalUnit % 10 == 0)
                    {
                        string label = UnitType switch
                        {
                            MeasurementUnit.Millimeter => $"{logicalUnit:0}",
                            MeasurementUnit.Inch => $"{(logicalUnit / 25.4):0.00}",
                            _ => $"{logicalUnit}"
                        };

                        FormattedText text = new FormattedText(
                            label,
                            System.Globalization.CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Segoe UI"),
                            10,
                            Brushes.Black,
                            VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        dc.DrawText(text, new Point(x + 2, 2));
                    }
                }
            }
        }


    }
}
