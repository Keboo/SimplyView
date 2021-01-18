using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimplyView
{
    public interface ICamera : IDisposable
    {
        Task StartPan(CameraDirection direction);
        Task StopPan(CameraDirection direction);
        Uri GetVideoUri();
        Task<BitmapSource?> GetNextFrame(CancellationToken token);
        Task<IReadOnlyList<Setting>> GetSettings();
        Task ApplySetting(Setting setting);
    }
}
