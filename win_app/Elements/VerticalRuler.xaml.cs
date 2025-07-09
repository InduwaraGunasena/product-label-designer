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
    /// Interaction logic for VerticalRuler.xaml
    /// </summary>
    public partial class VerticalRuler : UserControl
    {
        public VerticalRuler()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelTopProperty =
    DependencyProperty.Register("LabelTop", typeof(double), typeof(VerticalRuler),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty LabelBottomProperty =
            DependencyProperty.Register("LabelBottom", typeof(double), typeof(VerticalRuler),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double LabelTop
        {
            get => (double)GetValue(LabelTopProperty);
            set => SetValue(LabelTopProperty, value);
        }

        public double LabelBottom
        {
            get => (double)GetValue(LabelBottomProperty);
            set => SetValue(LabelBottomProperty, value);
        }

        public enum MeasurementUnit
        {
            Millimeter,
            Inch
        }

        public static readonly DependencyProperty ZoomLevelProperty =
    DependencyProperty.Register("ZoomLevel", typeof(double), typeof(VerticalRuler),
        new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty UnitSizeProperty =
            DependencyProperty.Register("UnitSize", typeof(double), typeof(VerticalRuler),
                new FrameworkPropertyMetadata(3.78, FrameworkPropertyMetadataOptions.AffectsRender)); // Default: mm in WPF (96dpi / 25.4)

        public static readonly DependencyProperty UnitTypeProperty =
            DependencyProperty.Register("UnitType", typeof(MeasurementUnit), typeof(VerticalRuler),
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

            // Draw white background
            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

            double unitPixelSize = UnitSize * ZoomLevel;

            double scaledTop = LabelTop * ZoomLevel;
            double scaledBottom = LabelBottom * ZoomLevel;

            double logicalUnit = 0;

            for (double y = scaledTop; y <= scaledBottom; y += unitPixelSize, logicalUnit += 1)
            {
                if (y >= 0 && y <= height)
                {
                    double tickLength = (logicalUnit % 10 == 0) ? 10 : 4;

                    // Draw vertical tick line from left
                    dc.DrawLine(new Pen(Brushes.Gray, 1), new Point(0, y), new Point(tickLength, y));

                    // Draw text every 10 units
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

                        // Draw the text right to the tick
                        dc.DrawText(text, new Point(tickLength + 2, y - text.Height / 2));
                    }
                }
            }
        }



    }
}
