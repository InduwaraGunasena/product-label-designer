using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace win_app.Elements
{
    /// <summary>
    /// Interaction logic for startingWindowMenuButton.xaml
    /// </summary>
    public partial class startingWindowMenuButton : UserControl
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(startingWindowMenuButton), new PropertyMetadata(false));


        public startingWindowMenuButton()
        {
            InitializeComponent();
        }

        // Title Property
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(startingWindowMenuButton), new PropertyMetadata("Title Text"));

        // Description Property
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(startingWindowMenuButton), new PropertyMetadata("Description Text"));

        // ImageSource Property
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(startingWindowMenuButton), new PropertyMetadata(null));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnButtonSelected?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnButtonSelected;
    }
}
