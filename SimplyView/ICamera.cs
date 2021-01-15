using System.Threading.Tasks;

namespace SimplyView
{
    public interface ICamera
    {
        Task StartPan(CameraDirection direction);
        Task StopPan(CameraDirection direction);
    }
}
