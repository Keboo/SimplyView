﻿using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace SimplyView
{
    public class MainWindowViewModel : ObservableObject
    {
        private ICamera Camera { get; } = new ShieldCamera();

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
    }
}
