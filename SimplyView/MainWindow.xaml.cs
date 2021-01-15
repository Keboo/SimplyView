using System;
using System.Windows;

namespace SimplyView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel { get; }
        public MainWindow()
        {
            DataContext = ViewModel = new MainWindowViewModel();
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await MediaElement.Open(ViewModel.VideoUri);
        }
    }
}
