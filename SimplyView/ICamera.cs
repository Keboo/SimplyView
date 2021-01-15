using System.Threading.Tasks;

namespace SimplyView
{
    public interface ICamera
    {
        Task PanLeft();
        Task PanUp();
        Task PanRight();
        Task PanDown();
    }
}
