using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimplyView
{
    public class ShieldCamera : ICamera
    {
        private const string CameraAddress = "192.168.0.85";
        private const string UserName = "admin";
        private const string Password = "123";

        private static HttpClient HttpClient { get; } = new HttpClient();

        public ShieldCamera()
        {
            //Up
            //http://192.168.0.85/decoder_control.cgi?loginuse=admin&loginpas=123&command=0&onestep=0&16106884157890.3094793391192612&_=1610688415790
            //http://192.168.0.85/decoder_control.cgi?loginuse=admin&loginpas=123&command=1&onestep=0&16106883137510.007062689618213547&_=1610688313751

            //Down
            //http://192.168.0.85/decoder_control.cgi?loginuse=admin&loginpas=123&command=2&onestep=0&16106902284270.6151412474032529&_=1610690228427
            //http://192.168.0.85/decoder_control.cgi?loginuse=admin&loginpas=123&command=3&onestep=0&16106902285610.6712627220233651&_=1610690228561
        }

        private static string BuildPanUrl(int command)
        {
            var builder = new UriBuilder("http", CameraAddress, 80, "decoder_control.cgi")
            {
                Query = string.Join('&', QueryStringParts())
            };

            var rv = builder.ToString();
            Debug.WriteLine(rv);
            return rv;

            IEnumerable<string> QueryStringParts()
            {
                yield return $"loginuse={UserName}";
                yield return $"loginpas={Password}";
                yield return $"command={command}";
                yield return $"onestep=1";
            }
        }

        public async Task PanUp()
        {
            await HttpClient.GetAsync(BuildPanUrl(0));
            await Task.Delay(TimeSpan.FromSeconds(0.3));
            await HttpClient.GetAsync(BuildPanUrl(1));
        }

        public async Task PanDown()
        {
            await HttpClient.GetAsync(BuildPanUrl(2));
            await Task.Delay(TimeSpan.FromSeconds(0.3));
            await HttpClient.GetAsync(BuildPanUrl(3));
        }

        public async Task PanLeft()
        {
            await HttpClient.GetAsync(BuildPanUrl(4));
            await Task.Delay(TimeSpan.FromSeconds(0.3));
            await HttpClient.GetAsync(BuildPanUrl(5));
        }

        public async Task PanRight()
        {
            await HttpClient.GetAsync(BuildPanUrl(6));
            await Task.Delay(TimeSpan.FromSeconds(0.3));
            await HttpClient.GetAsync(BuildPanUrl(7));
        }
    }
}
