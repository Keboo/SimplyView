using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace SimplyView
{

    public class MainWindowViewModel : ObservableObject
    {
        public const string CameraAddress = "192.168.0.85";


        public ICommand MoveCameraCommand { get; }
        private ICamera Camera { get; }

        public MainWindowViewModel()
        {
            MoveCameraCommand = new RelayCommand<CameraDirection>(OnMoveCamera);
        }

        private void OnMoveCamera(CameraDirection direction)
        {
            switch(direction)
            {
                case CameraDirection.Left:
                    Camera.PanLeft();
                    break;
                case CameraDirection.Up:
                    Camera.PanUp();
                    break;
                case CameraDirection.Right:
                    Camera.PanRight();
                    break;
                case CameraDirection.Down:
                    Camera.PanDown();
                    break;
            }
        }
    }
}
