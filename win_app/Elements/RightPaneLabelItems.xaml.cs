using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using win_app.Models;

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

        // List of types (Text, Number, etc.)
        public List<string> ItemTypes => LabelItemTypes.AllTypes;

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
            var itemTypes = win_app.Models.LabelItemTypes.AllTypes;

            ((DataGridComboBoxColumn)FixedItemsGrid.Columns[1]).ItemsSource = itemTypes;
            ((DataGridComboBoxColumn)VariableItemsGrid.Columns[1]).ItemsSource = itemTypes;

        }

        // Selection change handlers
        private void FixedItemsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsFixedStyleVisible = FixedItemsGrid.SelectedItem != null;
        }

        private void VariableItemsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsVariableStyleVisible = VariableItemsGrid.SelectedItem != null;
        }

        // Add/Remove buttons
        private void AddFixedItem_Click(object sender, RoutedEventArgs e)
        {
            FixedItems.Add(new LabelItem { Name = "NewFixedItem", Type = "Text" });
        }

        private void RemoveFixedItem_Click(object sender, RoutedEventArgs e)
        {
            if (FixedItemsGrid.SelectedItem is LabelItem selected)
                FixedItems.Remove(selected);
        }

        private void AddVariableItem_Click(object sender, RoutedEventArgs e)
        {
            VariableItems.Add(new LabelItem { Name = "NewVariableItem", Type = "Text" });
        }

        private void RemoveVariableItem_Click(object sender, RoutedEventArgs e)
        {
            if (VariableItemsGrid.SelectedItem is LabelItem selected)
                VariableItems.Remove(selected);
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
