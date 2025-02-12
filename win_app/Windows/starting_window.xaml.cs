using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using win_app.Elements;
using win_app.Windows; // Import Windows namespace to access file_opening_window

namespace win_app.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class startingWindow : Window
    {
        private startingWindowMenuButton? selectedButton;

        public startingWindow()
        {
            InitializeComponent();
            RegisterButtonEvents();
        }

        private void RegisterButtonEvents()
        {
            foreach (var child in SelectionPanel.Children)
            {
                if (child is startingWindowMenuButton button)
                {
                    button.OnButtonSelected += Button_OnSelected;
                }
            }
        }

        private void Button_OnSelected(object? sender, EventArgs e)
        {
            if (sender is startingWindowMenuButton clickedButton)
            {
                // Deselect previous button
                if (selectedButton != null)
                {
                    selectedButton.IsSelected = false;
                }

                // Select new button
                clickedButton.IsSelected = true;
                selectedButton = clickedButton;

                // Show Next button
                NextButton.IsEnabled = true;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedButton == null)
            {
                MessageBox.Show("Please select an option first.");
                return;
            }

            // Check if "Open a label design" or "Auto generate a label" is selected
            if (selectedButton.Title == "Open a label design" || selectedButton.Title == "Auto generate a label")
            {
                file_opening_window fileWindow = new file_opening_window();
                fileWindow.Show(); // Open the new window
                this.Close(); // Close the current window
            }
            else
            {
                MessageBox.Show($"You selected: {selectedButton.Title}");
            }
        }
    }
}
