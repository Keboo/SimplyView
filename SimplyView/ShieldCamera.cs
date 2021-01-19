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
    public sealed class ShieldCamera : ICamera
    {
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

        private BackgroundSubtractor BackgroundSubtractor { get; } 
            = BackgroundSubtractorMOG.Create(history: 200, backgroundRatio:0.001);
        private Mat ForegroundMask { get; } = new();
        private bool ShowMovement { get; set; }

        private static Mat Element { get; }
            = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(5, 5));

        public async Task<BitmapSource?> GetNextFrame(CancellationToken token)
        {
            using Mat mat = new();
            if (VideoCapture.Value.Read(mat) && !token.IsCancellationRequested)
            {
                if (ShowMovement)
                {
                    BackgroundSubtractor.Apply(mat, ForegroundMask);
                    using var erode = ForegroundMask.Erode(Element);
                    using var dialate = erode.Dilate(Element, iterations: 2);
                    using var alpha = new Mat(ForegroundMask.Size(), MatType.CV_8UC1, new Scalar(200));
                    Cv2.BitwiseOr(alpha, dialate, alpha);
                    var channels = Enumerable.Range(0, mat.Channels())
                        .Select(x => mat.ExtractChannel(x))
                        .Concat(new[] { alpha })
                        .ToArray();
                    Cv2.Merge(channels, mat);
                }
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
            BackgroundSubtractor.Dispose();
            ForegroundMask.Dispose();
        }

        public async Task<IReadOnlyList<Setting>> GetSettings()
        {
            string settingsString = await HttpClient.GetStringAsync(BuildSettingsUrl());

            var rv = new List<Setting>();
            rv.Add(new BoolSetting(CameraOptions.ShowMovement, ShowMovement));
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
            if (setting is BoolSetting boolSetting)
            {
                switch(setting.Name)
                {
                    case "ircut":
                        await SetIRMode(boolSetting.Value);
                        break;
                    case CameraOptions.ShowMovement:
                        ShowMovement = boolSetting.Value;
                        break;
                }
            }
            throw new NotImplementedException($"No settings control defined for {setting}");
        }
    }
}
