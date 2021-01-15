using System.Net.Http;
using System.Threading.Tasks;

namespace SimplyView
{
    public class ShieldCamera : ICamera
    {
        private const string CameraAddress = "192.168.0.85";


        private static HttpClient HttpClient { get; } = new HttpClient();

        public ShieldCamera()
        {
            //http://192.168.0.85/decoder_control.cgi?loginuse=admin&loginpas=123&command=0&onestep=0&16106884157890.3094793391192612&_=1610688415790
            //http://192.168.0.85/decoder_control.cgi?loginuse=admin&loginpas=123&command=1&onestep=0&16106883137510.007062689618213547&_=1610688313751
        }

        public Task PanDown()
        {
            throw new System.NotImplementedException();
        }

        public Task PanLeft()
        {
            throw new System.NotImplementedException();
        }

        public Task PanRight()
        {
            throw new System.NotImplementedException();
        }

        public Task PanUp()
        {
            throw new System.NotImplementedException();
        }
    }
}
