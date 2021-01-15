using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimplyView
{
    public class ShieldCamera : ICamera
    {
        //http://192.168.0.85/get_camera_params.cgi?loginuse=admin&loginpas=123&1610693957440&_=1610693957441
        private const string CameraAddress = "192.168.0.85";
        private const string UserName = "admin";
        private const string Password = "123";

        private static HttpClient HttpClient { get; } = new HttpClient();

        private static string BuildPanUrl(int command)
        {
            UriBuilder builder = new("http", CameraAddress, 80, "decoder_control.cgi")
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
                yield return $"onestep=0";
            }
        }

        private static string BuildCameraControlUrl(int parameter, int value)
        {
            UriBuilder builder = new("http", CameraAddress, 80, "camera_control.cgi")
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
                yield return $"param={parameter}";
                yield return $"value={value}";
            }
        }

        private static string BuildCameraVideoUrl()
        {
            UriBuilder builder = new("http", CameraAddress, 80, "videostream.cgi")
            {
                Query = string.Join('&', QueryStringParts())
            };

            return builder.ToString();

            IEnumerable<string> QueryStringParts()
            {
                yield return $"loginuse={UserName}";
                yield return $"loginpas={Password}";
            }
        }


        private static int GetStartPanCommand(CameraDirection direction)
            => direction switch
            {
                CameraDirection.Up => 0,
                CameraDirection.Down => 2,
                CameraDirection.Left => 4,
                CameraDirection.Right => 6,
                _ => throw new InvalidOperationException()
            };

        private static int GetStopPanCommand(CameraDirection direction)
            => GetStartPanCommand(direction) + 1;

        public async Task StartPan(CameraDirection direction) 
            => await HttpClient.GetAsync(BuildPanUrl(GetStartPanCommand(direction)));

        public async Task StopPan(CameraDirection direction) 
            => await HttpClient.GetAsync(BuildPanUrl(GetStopPanCommand(direction)));

        public async Task SetIRMode(bool isOn)
            => await HttpClient.GetAsync(BuildCameraControlUrl(14, isOn ? 1 : 0));

        public Uri GetVideoUri() => new(BuildCameraVideoUrl());
    }
}
