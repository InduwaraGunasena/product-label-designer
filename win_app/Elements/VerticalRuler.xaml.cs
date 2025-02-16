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


        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            double height = ActualHeight;
            // Example: Draw tick marks every 10 pixels
            for (double y = 0; y < height; y += 10)
            {
                double tickWidth = (y % 50 == 0) ? 15 : 7;
                dc.DrawLine(new Pen(Brushes.Gray, 1), new Point(30, y), new Point(30 - tickWidth, y));
                if (y % 50 == 0)
                {
                    // Optionally draw text (scale value)
                    FormattedText text = new FormattedText(
                        y.ToString(),
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"),
                        10,
                        Brushes.Black,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    dc.DrawText(text, new Point(5, y + 4));
                }
            }
        }


    }
}
