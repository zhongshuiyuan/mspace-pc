using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Common.ShellService;
using Mmc.Mspace.Const.ConstPath;
using Mmc.Mspace.Services.HttpService;
using Mmc.Windows.Services;
using Mmc.Windows.Utils;
using Mmc.Wpf.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Mmc.Mspace.UavModule.UavTracing
{
    public class UavWebVideoViewModel : CheckedToolItemModel
    {
        private UavVideoVLCView videoView;
        HttpService _httpService;
        [XmlIgnore]
        public ICommand CloseCmd { get; set; }

        public string WindowTitle { get; set; }

        public string VideoUrl { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            base.ViewType = ViewType.CheckedIcon;
            _httpService = new HttpService();
            this.CloseCmd = new RelayCommand(() =>
            {
                base.IsChecked = false;
            });
        }
        public Action<string> OnClosed { get; set; }
        public override void OnChecked()
        {
            base.OnChecked();
            _httpService.Token = HttpServiceUtil.Token;
            var json1 = JsonUtil.DeserializeFromFile<dynamic>(AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
            VideoUrl = "";
            var uavVideo = "rtmp://pull.mmcuav.cn/live/mmcfactory";

            if (string.IsNullOrEmpty(deviceHardId))
                VideoUrl = VideoUrl + uavVideo;
            else
            {
                try
                {
                    var json = JsonUtil.DeserializeFromFile<dynamic>(AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
                    string getVideoUrl = string.Format("{0}/api/aircraft/video-url?videoType=rtmp&deviceHardId=", json.poiUrl) + deviceHardId;
                    var res = _httpService.HttpRequest(getVideoUrl);
                    var resDynamic = JsonUtil.DeserializeFromString<dynamic>(res);
                    if ((bool)resDynamic.status)
                    {
                        string rtmp_url = resDynamic.data.vUrl;
                        VideoUrl = VideoUrl + rtmp_url;
                    }
                }
                catch (Exception ex)
                {
                    SystemLog.Log(ex);
                    VideoUrl = VideoUrl + uavVideo;
                }

            }

            var shell = ServiceManager.GetService<IShellService>(null).ShellWindow;
            shell.Dispatcher.Invoke(() =>
            {
                if (videoView == null)
                    videoView = new UavVideoVLCView();
                videoView.Owner = Application.Current.MainWindow;
                videoView.Width = 540;
                videoView.Height = 385;
                videoView.Left = shell.Width - 750 - 10;
                videoView.Top = 10;
                videoView.DataContext = this;
                videoView.Show();
                videoView.SetUrl(VideoUrl);
            });
        }
        public double OffsetHeight { get; set; }
        public double Width { get; set; }

        public string deviceHardId { get; set; }
        public string httpStatus { get; set; }

        public override void OnUnchecked()
        {
            DateTime startTime = DateTime.Now;

            base.OnUnchecked();
            var shell = ServiceManager.GetService<IShellService>(null).ShellWindow;
            shell.Dispatcher.Invoke(() =>
            {
                videoView?.Close();
                videoView = null;
                if (OnClosed != null)
                    this.OnClosed(this.deviceHardId);
            });

            Helpers.TimeHelper.ElapsedTime(startTime, "UAV web video close");
        }
    }
}
