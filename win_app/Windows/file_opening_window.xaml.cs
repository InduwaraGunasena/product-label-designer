using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;


namespace win_app.Windows
{
    /// <summary>
    /// Interaction logic for file_opening_window.xaml
    /// </summary>
    public partial class file_opening_window : Window
    {
        private string _selection;
        private string _selectedFilePath;

        public class LabelData
        {
            public Dictionary<string, string> Fields { get; set; }
        }

        public IEnumerable<LabelData> ReadExcelData(string filePath)
        {

            // Ensure EPPlus can read the license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Read column headers
                var headers = new string[colCount];
                for (int col = 1; col <= colCount; col++)
                {
                    headers[col - 1] = worksheet.Cells[1, col].Text;
                }

                // Stream rows instead of loading everything into memory
                for (int row = 2; row <= rowCount; row++)
                {
                    var labelData = new LabelData { Fields = new Dictionary<string, string>() };

                    for (int col = 1; col <= colCount; col++)
                    {
                        string header = headers[col - 1];
                        string value = worksheet.Cells[row, col].Text;
                        labelData.Fields[header] = value;
                    }

                    yield return labelData; // Yield each row instead of storing all at once
                }
            }
        }


        public file_opening_window(string selection)
        {
            InitializeComponent();
            
            _selection = selection;

            // Update UI based on selection
            UpdateUI();

            BackButton.Click += BackButton_Click; // Attach event handler
            BrowseButton.Click += BrowseButton_Click;
            NextButton.Click += NextButton_Click;
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
                _selectedFilePath = openFileDialog.FileName;
                FilePath.Text = _selectedFilePath;
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

        // Add NextButton_Click event handler here. open the MainWindow.xaml window with the selected file path
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            
            // Read Excel data
            List<LabelData> labelDataList = new List<LabelData>(ReadExcelData(_selectedFilePath));

            // Open the main_editing_window with the selected file path if has
            main_editing_window mainEditingWindow = new main_editing_window(labelDataList);
            //main_editing_window mainEditingWindow = new main_editing_window();
            mainEditingWindow.Show();
           
            // Close current window
            this.Close();
        }

        

    }
}
