using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimplyView
{
    public interface ICamera
    {
        Task StartPan(CameraDirection direction);
        Task StopPan(CameraDirection direction);
        Task SetIRMode(bool isOn);
        Uri GetVideoUri();
        Task<BitmapSource?> GetSnapshot();
    }
}
