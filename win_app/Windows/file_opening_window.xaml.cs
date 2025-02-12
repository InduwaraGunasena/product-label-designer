using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;


namespace win_app.Windows
{
    /// <summary>
    /// Interaction logic for file_opening_window.xaml
    /// </summary>
    public partial class file_opening_window : Window
    {
        private string _selection;
        public file_opening_window(string selection)
        {
            InitializeComponent();
            
            _selection = selection;

            // Update UI based on selection
            UpdateUI();

            BackButton.Click += BackButton_Click; // Attach event handler
            BrowseButton.Click += BrowseButton_Click;
        }

        private void UpdateUI()
        {
            if (_selection == "Open a label design")
            {
                // Update title, description, textbox, and image
                Title = "Open Label Design";
                TitleText.Text = "Open a Label Design";
                DescriptionText.Text = "Select a label design file and then click 'Open' button to edit your design file.";
                FileSelectDescription.Text = "Choose your label design file to edit.";
                FilePath.Text = " Select a label design file";
                NextButton.Content = "Open";

                // Update image source
                HeaderImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/images/open-label.png"));

            }
            else if (_selection == "Auto generate a label")
            {
                Title = "Auto Generate Label";
                TitleText.Text = "Auto Generate a Label Design";
                DescriptionText.Text = "Import your data as an excel sheet and then click 'Generate' button to create a label automatically.";
                FileSelectDescription.Text = "Choose your excel sheet which having all the label data";
                FilePath.Text = " Select a excel sheet";
                NextButton.Content = "Generate";

                // Update image source
                HeaderImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/images/auto-generate.png"));
            }
        }


        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set file filter based on selection
            if (_selection == "Open a label design")
            {
                openFileDialog.Filter = "Label Design Files (*.lbl;*.xml)|*.lbl;*.xml|All Files (*.*)|*.*";
            }
            else if (_selection == "Auto generate a label")
            {
                openFileDialog.Filter = "Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
            }

            openFileDialog.Title = "Select a File";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath.Text = openFileDialog.FileName;
                NextButton.IsEnabled = true; // Enable the "Next" button once a file is selected
            }
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
