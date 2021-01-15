using System.Windows;

namespace SimplyView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Unosquare.FFME.Library.FFmpegDirectory = @"D:\Dev\SimplyView\ffmpeg";
        }
    }
}
