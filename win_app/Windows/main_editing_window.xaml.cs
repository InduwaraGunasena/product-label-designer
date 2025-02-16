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
using System.Windows.Shapes;
using Fluent; // Import the Fluent Ribbon namespace
using win_app.Elements;

namespace win_app.Windows
{
    /// <summary>
    /// Interaction logic for main_editing_window.xaml
    /// </summary>
    public partial class main_editing_window : RibbonWindow
    {
        public main_editing_window()
        {
            InitializeComponent();
            // Assuming your LeftPane is named "LeftPane" in XAML
            LeftPane.DataContext = new LeftPaneViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
