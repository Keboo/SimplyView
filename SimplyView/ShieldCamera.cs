using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SimplyView
{
    public class ShieldCamera : ICamera
    {
        //http://192.168.0.86/get_camera_params.cgi?loginuse=admin&loginpas=123&1610693957440&_=1610693957441
        private const string CameraAddress = "192.168.0.86";
        private const string UserName = "admin";
        private const string Password = "123";

        private static Regex SettingsParser { get; }
            = new(@"var\s+(?<Name>\w+)=(?<Value>[^;]+);");

        private static HttpClient HttpClient { get; } = new HttpClient();

        private static string BuildPanUrl(int command) 
            => BuildUrl("decoder_control.cgi",
                $"command={command}",
                $"onestep=0");

        private static string BuildCameraControlUrl(int parameter, int value) 
            => BuildUrl("camera_control.cgi",
                $"param={parameter}",
                $"value={value}");

        private static string BuildCameraVideoUrl() => BuildUrl("videostream.cgi");

        private static string BuildSettingsUrl() => BuildUrl("get_camera_params.cgi");

        private static string BuildUrl(string path, params string[] queryStringParts)
        {
            UriBuilder builder = new("http", CameraAddress, 80, path)
            {
                Query = string.Join('&', QueryStringParts().Concat(queryStringParts))
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

        private static async Task SetIRMode(bool isOn)
            => await HttpClient.GetAsync(BuildCameraControlUrl(14, isOn ? 0 : 1));

        public Uri GetVideoUri() => new(BuildCameraVideoUrl());

        private Lazy<VideoCapture> VideoCapture { get; } = new Lazy<VideoCapture>(() =>
          {
              VideoCapture vc = new();
              vc.Open(BuildCameraVideoUrl());
              return vc;
          });

        public async Task<BitmapSource?> GetNextFrame(CancellationToken token)
        {
            using Mat mat = new();
            if (VideoCapture.Value.Read(mat) && !token.IsCancellationRequested)
            {
                return await Application.Current.Dispatcher.InvokeAsync(() => mat.ToBitmapSource());
            }

            return null;
        }

        public void Dispose()
        {
            if (VideoCapture.IsValueCreated)
            {
                VideoCapture.Value.Dispose();
            }
        }

        public async Task<IReadOnlyList<Setting>> GetSettings()
        {
            string settingsString = await HttpClient.GetStringAsync(BuildSettingsUrl());

            var rv = new List<Setting>();
            foreach(Match match in SettingsParser.Matches(settingsString))
            {
                string name = match.Groups["Name"].Value;
                string value = match.Groups["Value"].Value;

                switch(name)
                {
                    case "ircut":
                        rv.Add(new BoolSetting(name, value == "1"));
                        break;
                    default:
                        rv.Add(new StringSetting(name, value));
                        break;
                }
            }

            return rv;
        }

        public async Task ApplySetting(Setting setting)
        {
            if (setting is BoolSetting boolSetting 
                && setting.Name == "ircut")
            {
                await SetIRMode(boolSetting.Value);
            }
            throw new NotImplementedException($"No settings control defined for {setting}");
        }
    }
}
