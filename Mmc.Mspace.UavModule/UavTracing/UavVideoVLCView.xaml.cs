using Mmc.Mspace.Const.ConstPath;
using Mmc.Windows.Utils;
using Mmc.Mspace.UavModule.UavTracing;
using System;
using System.Linq;
using System.Windows;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using Mmc.Mspace.Common.Cache;
using Mmc.Mspace.Services.HttpService;
using Mmc.Windows.Services;
using Mmc.Mspace.Theme.Pop;

namespace Mmc.Mspace.UavModule
{
    /// <summary>
    /// StatisticsWebView.xaml 的交互逻辑
    /// </summary>
    public partial class UavVideoVLCView
    {
        private string OriginUrl;
        private string AIUrl;
        private SingleVideoViewModel _singleVideoViewModel = new SingleVideoViewModel();
        //private VlcDotNetCoreModel _vlcDotNetCoreModel = new VlcDotNetCoreModel();

        //private static Assembly currentAssembly = Assembly.GetEntryAssembly();
        //private static string currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
        //private static string destinationFolder = Path.Combine(currentDirectory, "thumbnails");

        public UavVideoVLCView()
        {
            InitializeComponent();
            VideoView.DataContext = _singleVideoViewModel;//禁止弹框
            Loaded += UavVideoVLCView_Loaded;

            //var libDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            //var mediaPlayer = this.VlcControl.SourceProvider.MediaPlayer;
            //this.VlcControl.SourceProvider.CreatePlayer(libDirectory, mediaOptions());
        }

        private void UavVideoVLCView_Loaded(object sender, RoutedEventArgs e)
        {
            var permission = CacheData.UserInfo.mspace_config.configs.FirstOrDefault(t => t.config_key == "AIButton");
            if (permission != null)
            {
                AIBtnPanel.Visibility = Visibility.Visible;
            }
        }

        private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                base.DragMove();
            }
            catch (Exception)
            {
            }
        }

        public void SetUrl(string url)
        {
            OriginUrl = url;

            if (string.IsNullOrEmpty(url))
            {
                Messages.ShowMessage("暂无视频在线，请稍后再试");
                return;
            }

            try
            {
                _singleVideoViewModel.SetStreamUrl(url);
                _singleVideoViewModel.Play();

                //this.VlcControl.SourceProvider.MediaPlayer.Play(url);
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
            }

        }

        private void getAiUrl()
        {
            try
            {
                var json = JsonUtil.DeserializeFromFile<dynamic>(
                    AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);

                string getAIUrl = $"{json.aiUrl}stream/?streamUrl={OriginUrl}";
                HttpService httpService = new HttpService();
                var res = httpService.HttpRequest(getAIUrl);
                var resDynamic = JsonUtil.DeserializeFromString<dynamic>(res);

                AIUrl = resDynamic?.outputStreamUrl;
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message);
                SystemLog.Log(ex);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

            try
            {
                _singleVideoViewModel.Stop();
                //this.VlcControl.SourceProvider.MediaPlayer.Pause();
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
            }

            #region 关闭AI流
            DateTime startTime = DateTime.Now;
            //var json = JsonUtil.DeserializeFromFile<dynamic>(
            //    AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
            //string closeStreamUrl = $"{json.aiUrl}stream-off";
            //HttpService httpService = new HttpService();
            //var res = httpService.HttpRequest(closeStreamUrl);

            DateTime stopTime = DateTime.Now;
            TimeSpan elapsedTime = stopTime - startTime;
            Console.WriteLine("关闭AI流 uavVideoVlcView Spendtime: {0}-{1}", elapsedTime, elapsedTime.TotalMilliseconds);
            #endregion

            ((UavWebVideoViewModel)this.DataContext).IsChecked = false;
        }

        private void CheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            //this.VlcControl.SourceProvider.MediaPlayer.Pause();
            getAiUrl();
            SystemLog.Log($"UavVideoVLCView AIUrl is [{AIUrl}]");
            if (string.IsNullOrWhiteSpace(AIUrl))
            {
                Messages.ShowMessage("AI视频流为空!");
                CheckBox.IsChecked = false;
                return;
            }

            try
            {
                //this.VlcControl.SourceProvider.MediaPlayer.Play(AIUrl);
                _singleVideoViewModel.SetStreamUrl(AIUrl);
                _singleVideoViewModel.Play();
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
            }
        }

        private void CheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            SystemLog.Log($"UavVideoVLCView OriginUrl is [{OriginUrl}]");
            if (string.IsNullOrWhiteSpace(OriginUrl))
            {
                Messages.ShowMessage("视频流为空!");
                return;
            }
            try
            {
                _singleVideoViewModel.SetStreamUrl(OriginUrl);
                _singleVideoViewModel.Play();
                //this.VlcControl.SourceProvider.MediaPlayer.Play(OriginUrl);
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
            }

        }

        // vlc options
        private string[] mediaOptions()
        {
            return new[]
            {
                //"--intf", "dummy", /* no interface                   */
                //"--vout", "dummy", /* we don't want video output     */
                "--no-audio", /* we don't want audio decoding   */
                //"--no-video-title-show", /* nor the filename displayed     */
                //"--no-stats", /* no stats */
                //"--no-sub-autodetect-file", /* we don't want subtitles        */
                "--no-snapshot-preview", /* 不显示缩略图 */
                "--no-osd",/*不显示路径*/
                //"--no-snapshot-sequential",
            };
        }

        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}