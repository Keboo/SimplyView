using System.ComponentModel;
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
            Closing += MainWindow_Closing;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.OnLoad();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.Dispose();
        }
    }
}
