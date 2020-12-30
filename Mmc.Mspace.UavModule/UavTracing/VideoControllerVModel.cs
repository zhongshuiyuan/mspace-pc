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
using Mmc.Mspace.Theme.Pop;
using Mmc.Mspace.UavModule.Dto;

namespace Mmc.Mspace.UavModule.UavTracing
{
    public class VideoControllerVModel : CheckedToolItemModel
    {
        HttpService _httpService;
        [XmlIgnore]
        public ICommand CloseCmd { get; set; }

        public string WindowTitle { get; set; }

        //多屏有视频墙VM
        public MultiVideoVModel MultiVideoVM { get; set; }

        //屏幕状态
        public VideoScreenVModel ScreenVModel { get; set; }

        //单屏VM
        private UavWebVideoViewModel _oneScreenVm;

        public double OffsetHeight { get; set; }

        public double Width { get; set; }

        public DeviceInfo deviceInfo { get; set; }

        public bool IsMultiScreen { get; set; }

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
 
        public override void OnChecked()
        {
            if (IsMultiScreen)
            {
                if (ScreenVModel == null)
                {
                    MessageBox.Show("请指定屏幕");
                    this.IsChecked = false;
                    return;
                }

                if (ScreenVModel.IsUsed && ScreenVModel.UavId != deviceInfo.deviceHardId)
                {
                    MessageBox.Show(string.Format("{0}屏已工作,请选择其他屏", ScreenVModel.ScreenIndex));
                    this.IsChecked = false;
                    return;
                }

                MultiVideoVM?.OpenVideo(ScreenVModel?.ScreenIndex, deviceInfo.deviceHardId, deviceInfo.httpStatus, WindowTitle);
                ScreenVModel.IsUsed = true;
                ScreenVModel.UavId = deviceInfo.deviceHardId;
                ScreenVModel.ScreenItem = ScreenVModel.ToString();
            }
            else
            {
                if (_oneScreenVm == null)
                {
                    _oneScreenVm = new UavWebVideoViewModel
                    {
                        deviceHardId = this.deviceInfo.deviceHardId,
                        httpStatus = this.deviceInfo.httpStatus,
                    };
                    _oneScreenVm.OnClosed += new Action<string>(OnClosedOneVm);
                }
                _oneScreenVm.Width = this.Width;
                _oneScreenVm.WindowTitle = this.WindowTitle;
                _oneScreenVm.IsChecked = true;//打开视频
            }
        }

        //单屏模式下退出视频界面
        private void OnClosedOneVm(string ground_series)
        {
            if (this.deviceInfo.deviceHardId == deviceInfo.deviceHardId)
                this.IsChecked = false;
        }

        public override void OnUnchecked()
        {
            base.OnUnchecked();
            if (IsMultiScreen)
            {
                if (this.deviceInfo.deviceHardId != ScreenVModel?.UavId)//非所属无人机打开的视频不处理
                    return;
                MultiVideoVM?.CloseVideo(ScreenVModel?.ScreenIndex, deviceInfo.deviceHardId);
                ScreenVModel.IsUsed = false;
                ScreenVModel.ScreenItem = ScreenVModel.ToString();
            }
            else
            {
                if (_oneScreenVm != null && _oneScreenVm.IsChecked)
                    _oneScreenVm?.OnUnchecked();
            }

        }
    }
}
