using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimplyView
{
    public class MainWindowViewModel : ObservableObject, IDisposable
    {
        private CancellationManager CancelationManager { get; } = new();
        private ICamera Camera { get; } = new ShieldCamera();

        public Uri VideoUri => Camera.GetVideoUri();

        private bool _IsLeftPressed;
        public bool IsLeftPressed
        {
            get => _IsLeftPressed;
            set
            {
                if (SetProperty(ref _IsLeftPressed, value))
                {
                    if (value)
                    {
                        Camera.StartPan(CameraDirection.Left);
                    }
                    else
                    {
                        Camera.StopPan(CameraDirection.Left);
                    }
                }
            }
        }

        private bool _IsUpPressed;
        public bool IsUpPressed
        {
            get => _IsUpPressed;
            set
            {
                if (SetProperty(ref _IsUpPressed, value))
                {
                    if (value)
                    {
                        Camera.StartPan(CameraDirection.Up);
                    }
                    else
                    {
                        Camera.StopPan(CameraDirection.Up);
                    }
                }
            }
        }

        private bool _IsRightPressed;
        public bool IsRightPressed
        {
            get => _IsRightPressed;
            set
            {
                if (SetProperty(ref _IsRightPressed, value))
                {
                    if (value)
                    {
                        Camera.StartPan(CameraDirection.Right);
                    }
                    else
                    {
                        Camera.StopPan(CameraDirection.Right);
                    }
                }
            }
        }

        private bool _IsDownPressed;
        public bool IsDownPressed
        {
            get => _IsDownPressed;
            set
            {
                if (SetProperty(ref _IsDownPressed, value))
                {
                    if (value)
                    {
                        Camera.StartPan(CameraDirection.Down);
                    }
                    else
                    {
                        Camera.StopPan(CameraDirection.Down);
                    }
                }
            }
        }

        private bool _IsIREnabled;
        public bool IsIREnabled
        {
            get => _IsIREnabled;
            set
            {
                if (SetProperty(ref _IsIREnabled, value))
                {
                    Camera.ApplySetting(new BoolSetting("ircut", value));
                }
            }
        }

        private bool _ShowMovement;
        public bool ShowMovement
        {
            get => _ShowMovement;
            set
            {
                if (SetProperty(ref _ShowMovement, value))
                {
                    Camera.ApplySetting(new BoolSetting(CameraOptions.ShowMovement, value));
                }
            }
        }

        private ImageSource? _CurrentImage;
        public ImageSource? CurrentImage
        {
            get => _CurrentImage;
            set => SetProperty(ref _CurrentImage, value);
        }

        public MainWindowViewModel()
        {
            CancellationToken token = CancelationManager.GetNextToken();
            Task.Factory.StartNew(() => LoadImages(), token);
            
            async void LoadImages()
            {
                while(!token.IsCancellationRequested)
                {
                    CurrentImage = await Camera.GetNextFrame(token);
                }
            }
        }

        internal async Task OnLoad()
        {
            IReadOnlyList<Setting> settings = await Camera.GetSettings();

            _IsIREnabled = settings
                .OfType<BoolSetting>()
                .FirstOrDefault(x => x.Name == "ircut")?.Value == false;
            OnPropertyChanged(nameof(IsIREnabled));

            _ShowMovement = settings
                .OfType<BoolSetting>()
                .FirstOrDefault(x => x.Name == CameraOptions.ShowMovement)?.Value == true;
            OnPropertyChanged(nameof(ShowMovement));
        }

        public void Dispose()
        {
            CancelationManager.Cancel();
        }
    }
}
