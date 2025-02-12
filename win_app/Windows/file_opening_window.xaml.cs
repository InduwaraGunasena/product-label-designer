using System;
using System.Windows;


namespace win_app.Windows
{
    /// <summary>
    /// Interaction logic for file_opening_window.xaml
    /// </summary>
    public partial class file_opening_window : Window
    {
        public file_opening_window()
        {
            InitializeComponent();
            BackButton.Click += BackButton_Click; // Attach event handler
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Open starting_window.xaml
            startingWindow startingWin = new startingWindow();
            startingWin.Show();

            // Close current window
            this.Close();
        }
    }
}
