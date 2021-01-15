using System;
using System.Windows;

namespace SimplyView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await MediaElement.Open(new Uri(@"http://192.168.0.85/videostream.cgi?loginuse=admin&loginpas=123"));
        }
    }
}
