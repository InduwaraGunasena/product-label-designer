using System;
using System.Collections.Generic;
using System.Windows;
using Fluent; // Import the Fluent Ribbon namespace
using win_app.Elements;
using static win_app.Windows.file_opening_window;

namespace win_app.Windows
{
    /// <summary>
    /// Interaction logic for main_editing_window.xaml
    /// </summary>
    public partial class main_editing_window : RibbonWindow
    {

        private List<LabelData> _labelDataList;

        public main_editing_window(List<LabelData> labelDataList)
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

    }
}
