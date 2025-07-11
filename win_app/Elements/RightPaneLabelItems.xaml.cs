using Microsoft.Win32; // Needed for OpenFileDialog
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using win_app.Label;
using win_app.Formatters;
using win_app.Services;

namespace win_app.Elements
{
    /// <summary>
    /// Interaction logic for RightPaneLabelItems.xaml
    /// </summary>
    public partial class RightPaneLabelItems : UserControl, INotifyPropertyChanged
    {
        // Properties for style panel visibility
        private bool _isFixedStyleVisible;
        public bool IsFixedStyleVisible
        {
            get => _isFixedStyleVisible;
            set
            {
                _isFixedStyleVisible = value;
                OnPropertyChanged(nameof(IsFixedStyleVisible));
            }
        }

        private bool _isVariableStyleVisible;
        public bool IsVariableStyleVisible
        {
            get => _isVariableStyleVisible;
            set
            {
                _isVariableStyleVisible = value;
                OnPropertyChanged(nameof(IsVariableStyleVisible));
            }
        }

        // Observable collections for items
        public ObservableCollection<LabelItem> FixedItems { get; set; } = new();
        public ObservableCollection<LabelItem> VariableItems { get; set; } = new();

        public ObservableCollection<LabelPropertyViewModel> SelectedItemProperties { get; set; } = new();

        // List of types (Text, Number, etc.)
        public List<string> ItemTypes => LabelItemFormatterRegistry.GetAllTypes();

        // Constructor
        public RightPaneLabelItems()
        {
            InitializeComponent();
            DataContext = this;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                FixedItemsGrid.UpdateLayout();
                VariableItemsGrid.UpdateLayout();
            }));

            // Set ItemsSource in code
            var itemTypes = LabelItemFormatterRegistry.GetAllTypes();

            ((DataGridComboBoxColumn)FixedItemsGrid.Columns[1]).ItemsSource = itemTypes;
            ((DataGridComboBoxColumn)VariableItemsGrid.Columns[1]).ItemsSource = itemTypes;

        }

        // Selection change handlers
        private void FixedItemsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFixedItem = FixedItemsGrid.SelectedItem as LabelItem;
            IsFixedStyleVisible = SelectedFixedItem != null;

            LoadPropertiesForSelectedItem(SelectedFixedItem);
        }

        private void VariableItemsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedVariableItem = VariableItemsGrid.SelectedItem as LabelItem;
            IsVariableStyleVisible = SelectedVariableItem != null;
            LoadPropertiesForSelectedItem(SelectedVariableItem);
        }

        private void LoadPropertiesForSelectedItem(LabelItem? item)
        {
            SelectedItemProperties.Clear();

            if (item == null) return;

            // First time: populate with defaults
            if (item.Formats == null || item.Formats.Count == 0)
                item.Formats = LabelItemFormatterRegistry.GetPropertyDefinitions(item.Type);

            foreach (var prop in item.Formats)
                SelectedItemProperties.Add(new LabelPropertyViewModel(prop));

        }
        // create the number for getting a unique name.
        private string GetUniqueName(string baseName, ObservableCollection<LabelItem> items)
        {
            int counter = 0;
            string uniqueName;
            do
            {
                uniqueName = counter == 0 ? baseName : $"{baseName} ({counter})";
                counter++;
            } while (items.Any(item => item.Name == uniqueName));
            return uniqueName;
        }


        // Add/Remove buttons
        private void AddFixedItem_Click(object sender, RoutedEventArgs e)
        {
            string name = GetUniqueName("NewFixedItem", FixedItems);
            FixedItems.Add(new LabelItem { Name = name, Type = "Text" });
        }

        private void RemoveFixedItem_Click(object sender, RoutedEventArgs e)
        {
            if (FixedItemsGrid.SelectedItem is LabelItem selected)
                FixedItems.Remove(selected);
        }

        private void AddVariableItem_Click(object sender, RoutedEventArgs e)
        {
            string name = GetUniqueName("NewVariableItem", VariableItems);
            VariableItems.Add(new LabelItem { Name = name, Type = "Text" });
        }

        private void RemoveVariableItem_Click(object sender, RoutedEventArgs e)
        {
            if (VariableItemsGrid.SelectedItem is LabelItem selected)
                VariableItems.Remove(selected);
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            // Find the parent DataContext (LabelPropertyViewModel)
            if (button.DataContext is LabelPropertyViewModel propertyViewModel)
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                    Title = "Select an Image File"
                };

                // Use Window.GetWindow to set the owner for dialog
                var owner = Window.GetWindow(this);
                bool? result = (owner != null) ? dialog.ShowDialog(owner) : dialog.ShowDialog();

                if (result == true)
                {
                    propertyViewModel.SelectedValue = dialog.FileName;
                }
            }
        }

        private LabelItem? _selectedFixedItem;
        public LabelItem? SelectedFixedItem
        {
            get => _selectedFixedItem;
            set
            {
                _selectedFixedItem = value;
                OnPropertyChanged(nameof(SelectedFixedItem));
                OnPropertyChanged(nameof(SelectedItemProperties));
            }
        }

        private LabelItem? _selectedVariableItem;
        public LabelItem? SelectedVariableItem
        {
            get => _selectedVariableItem;
            set
            {
                _selectedVariableItem = value;
                OnPropertyChanged(nameof(SelectedVariableItem));
                OnPropertyChanged(nameof(SelectedItemProperties));
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        // Save the current tabke items in both fixed and variable items.
        private void SaveLabelDesign_Click(object sender, RoutedEventArgs e)
        {
            var design = new LabelDesign
            {
                Items = new LabelItems
                {
                    Fixed = FixedItems.Select(item => { item.Category = "Fixed"; return item; }).ToList(),
                    Variable = VariableItems.Select(item => { item.Category = "Variable"; return item; }).ToList()
                }
            };

            var dialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                FileName = "LabelDesign.json"
            };

            if (dialog.ShowDialog() == true)
            {
                LabelDesignManager.SaveToFile(design, dialog.FileName);
                MessageBox.Show("Design saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Load the fixed and variable items to the tables.
        private void LoadLabelDesign_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json"
            };

            if (dialog.ShowDialog() == true)
            {
                var loaded = LabelDesignManager.LoadFromFile(dialog.FileName);
                if (loaded != null)
                {
                    FixedItems.Clear();
                    VariableItems.Clear();

                    foreach (var item in loaded.Items.Fixed)
                        FixedItems.Add(item);

                    foreach (var item in loaded.Items.Variable)
                        VariableItems.Add(item);

                    MessageBox.Show("Design loaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

    }
}
