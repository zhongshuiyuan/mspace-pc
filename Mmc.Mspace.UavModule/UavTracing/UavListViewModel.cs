using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Common.ShellService;
using Mmc.Mspace.Const.ConstPath;
using Mmc.Mspace.Services.HttpService;
using Mmc.Windows.Services;
using Mmc.Windows.Utils;
using Mmc.Wpf.Commands;
using Mmc.Mspace.UavModule.Dto;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Mmc.Mspace.Theme.Pop;
using Mmc.Mspace.UavModule.WebSocket;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Mmc.Mspace.Common.Messenger;
using Mmc.Mspace.Common.Cache;
using Mmc.Mspace.UavModule.Service;
using Mmc.Mspace.ToolModule.ViewControl;
using Application = System.Windows.Application;


namespace Mmc.Mspace.UavModule.UavTracing
{
    public class GCHelper
    {
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
    }

    public class UavListViewModel : CheckedToolItemModel
    {
        private UavItemViewModel selectedUavItemModel;
        private UavControlVModel _uavControlVModel;
        private MultiVideoVModel _multiVideoVM;
        private JoyService _joyService;
        private UavTracingView uavTracingView;
        private Window shell;
        private readonly double _socketConnectTimerOut = 10000; //ms
        private string _groundstationList;

        private bool _isSocketLogin = false;   //用户是否登录  true:在线  false:离线
        private bool _isSocketOffline = true;  //socket是否掉线
        private System.Timers.Timer timerConnect;
        private DateTime _timeout;

        //base on WebSocketSharp
        private WebSocketBase wb;
        private static readonly object lockObj = new object();

        //new 
        private readonly int _strShowMaxLen = 12;
        private readonly int _strShowMinLen = 8;


        public UavListViewModel()
        {
            this.WindowTitle = Helpers.ResourceHelper.FindKey("Dronetrack");
        }

        [XmlIgnore]
        public ICommand CloseCmd { get; set; }

        [XmlIgnore]
        public ICommand ButtonCmd { get; set; }

        [XmlIgnore]
        public ICommand connectSocketCMD { get; set; }

       
        [XmlIgnore]
        public ICommand AreaListCmd { get; set; }
        [XmlIgnore]
        public ICommand MissionListCmd { get; set; }
        [XmlIgnore]
        public UavItemViewModel SelectedUavItemModel
        {
            get { return this.selectedUavItemModel; }
            set
            {
                base.SetAndNotifyPropertyChanged<UavItemViewModel>(ref this.selectedUavItemModel, value, "SelectedUavItemModel");
                var item = value;
                if (this._uavControlVModel != null && item != null)
                {
                    this._uavControlVModel.deviceInfo = item.deviceInfo;
                }
            }
        }

        private ObservableCollection<UavItemAreaViewModel> uavItemAreaModels = new ObservableCollection<UavItemAreaViewModel>();

        [XmlIgnore]
        public ObservableCollection<UavItemAreaViewModel> UavItemAreaModels
        {
            get { return this.uavItemAreaModels; }
            set { SetAndNotifyPropertyChanged<ObservableCollection<UavItemAreaViewModel>>(ref uavItemAreaModels, value, "UavItemAreaModels"); }
        }

        private string _socketState;//socket 在线状态
        [XmlIgnore]
        public string socketState
        {
            get { return this._socketState; }
            set { base.SetAndNotifyPropertyChanged<string>(ref this._socketState, value, "socketState"); }
        }


        private bool _ModelIsChecked=true;

        public bool ModelIsChecked
        {
            get { return _ModelIsChecked; }
            set { base.SetAndNotifyPropertyChanged<bool>(ref this._ModelIsChecked, value, "ModelIsChecked"); }
        }

        private bool _DrawIsChecked;

        public bool DrawIsChecked
        {
            get { return _DrawIsChecked; }
            set { base.SetAndNotifyPropertyChanged<bool>(ref this._DrawIsChecked, value, "DrawIsChecked"); }
        }
        public string WindowTitle { get; set; }

        public bool IsMultiScreen { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            CheckConnectState();
            timerConnect = new System.Timers.Timer(_socketConnectTimerOut);
            timerConnect.Elapsed += TimerConnectOnElapsed;

            base.ViewType = ViewType.CheckedIcon;
            this.CloseCmd = new RelayCommand(() =>
            {
                base.IsChecked = false;
            });
            this.connectSocketCMD = new RelayCommand(() =>
            {
                var json = JsonUtil.DeserializeFromFile<dynamic>(AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
                string uri = json.uavSocketUri;
                WebSocketService wss = new BuissnesServiceImpl();
                if (wb == null)
                {
                    wb = new WebSocketBase(uri, wss);
                    wb.start();
                }
                var addUser = new SocketLogin
                {
                    type = 100,
                    systemCode = "MMC",
                    state = 1,
                    username = CacheData.UserInfo.username,
                };
                wb.send(JsonUtil.SerializeToString(addUser));

                timerConnect.Start();
            });
            this.AreaListCmd = new RelayCommand(() =>
            {
                this.UavItemAreaModels = this.GetUavList();
            });
            this.MissionListCmd = new RelayCommand(() =>
            {
                this.UavItemAreaModels = this.GetUavListNew();
            });

            //websocket data push transfer
            Messenger.Messengers.Register<string>("websocketReceivedMessage", (p) =>
            {
                onReceiveMessage(p);
            });
        }

        public void CheckConnectState()
        {
            socketState = !_isSocketOffline ? Helpers.ResourceHelper.FindKey("Logged") : Helpers.ResourceHelper.FindKey("Notlogged");

            try
            {
                shell?.Dispatcher?.Invoke(() =>
                {
                    if (_isSocketOffline)
                    {
                        this.uavTracingView.connectSocketBtn.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.uavTracingView.connectSocketBtn.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
                return;
            }
        }

        private void TimerConnectOnElapsed(object sender, ElapsedEventArgs e)
        {
            DateTime lastConnecTime = _timeout;

            TimeSpan span = DateTime.Now - lastConnecTime;
            Console.WriteLine("Websocket lost time: ", span.Seconds);
            if (span.Seconds > 10)
            {
                _isSocketOffline = true;
                CheckConnectState();
            }
        }

        /// <summary>
        /// 接收websocket数据
        /// </summary>
        /// <param name="message"></param>
        private void onReceiveMessage(string message)
        {
            Console.WriteLine("-----onReceiveMessage", message);
            if (!string.IsNullOrEmpty(message))
            {
                var messageJson = JsonUtil.DeserializeFromString<dynamic>(message);
                int type = messageJson.type;

                switch (type)
                {
                    case (int)TypeEnum.N_WEB_CONTROLLER.CMD_TYPE_SERVER_HEARTBEAT:
                        _isSocketOffline = false;

                        lock (lockObj)
                        {
                            _timeout = DateTime.Now;
                        }

                        CheckConnectState();

                        Console.WriteLine("-----CMD_TYPE: " + (int)TypeEnum.N_WEB_CONTROLLER.CMD_TYPE_SERVER_HEARTBEAT + " timestamp: " + messageJson.time);
                        break;

                    case (int)TypeEnum.N_WEB_CONTROLLER.CMD_TYPE_CONNECT_FAILED:
                        Messages.ShowMessage($"连接失败:{messageJson.msgContent}");
                        break;
                    case (int)TypeEnum.N_WEB_CONTROLLER.CMD_TYPE_SENDDATA_FAILED:
                        Messages.ShowMessage($"数据发送失败:{messageJson.msgContent}");
                        break;
                    default:
                        Console.WriteLine("-----CMD_TYPE: MSpace WebControl $ Have NO This Type!");
                        break;

                }
            }

        }

        private void TimeUpdated(string serial, string sendMessage)
        {
            if (this._uavControlVModel.deviceInfo.deviceHardId != serial)
                return;

            if (!_isSocketOffline)
            {
                wb?.send(sendMessage);
                Console.WriteLine("-----UavListViewModel::TimeUpdated \n" + sendMessage);
            }
            //else
            //Messages.ShowMessage(Helpers.ResourceHelper.FindKey("DeviceUserNotLogin"));
        }

        public void connectLogout(object source, ElapsedEventArgs e)
        {
            _isSocketOffline = true;
            Console.WriteLine("UavListViewModel::connectLogout  Socket connect is logout!" + _isSocketOffline);
        }

        private FullScreenViewModel fullScreenViewModel { get; set; }

        public override void OnChecked()
        {
            base.OnChecked();

            #region 多屏适配

            var json = JsonUtil.DeserializeFromFile<dynamic>(
                AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);

            if (json.openMultiScreen != null && (bool)json.openMultiScreen)
            {
                //获取屏幕信息,判断是否单屏
                Screen[] screens = Screen.AllScreens;
                this.IsMultiScreen = screens?.Length > 1;
            }

            if (IsMultiScreen && _multiVideoVM == null)
            {
                _multiVideoVM = new MultiVideoVModel();
            }

            #endregion

            var shellView = ServiceManager.GetService<IShellService>(null).ShellWindow;
            shell = ServiceManager.GetService<IShellService>(null).ShellWindow;         
            ServiceManager.GetService<IShellService>(null).LeftPanel.Visibility = System.Windows.Visibility.Collapsed;
            ServiceManager.GetService<IShellService>(null).ShellMenu.Visibility = System.Windows.Visibility.Hidden;
            ServiceManager.GetService<IShellService>(null).BottomToolMenu.Visibility = System.Windows.Visibility.Hidden;
            FrameworkElement bottomToolMenu = ServiceManager.GetService<IShellService>(null).BottomToolMenu;
            ItemsControl itemsControl = (ItemsControl)bottomToolMenu.FindName("tools");
            bool flag = itemsControl == null;
            if (!flag)
            {
                ObservableCollection<ToolItemModel> observableCollection = (ObservableCollection<ToolItemModel>)itemsControl.ItemsSource;
                bool flag2 = observableCollection != null;
                if (flag2)
                {
                    foreach (ToolItemModel toolItemModel in observableCollection)
                    {
                        bool flag3 = toolItemModel.Content == "Fullscreen";
                        if (flag3)
                        {
                            ((CheckedToolItemModel)toolItemModel).IsChecked = true;
                        }
                    }
                }
            }

            if (this._uavControlVModel == null)
            {
                this._uavControlVModel = new UavControlVModel();
                _uavControlVModel.OnTimeUpdated += new Action<string, string>(this.TimeUpdated);
            }

            if (this._joyService == null)
            {
                this._joyService = new JoyService();
                this._joyService.uavControlVModel = this._uavControlVModel;
            }


            this.UavItemAreaModels = this.GetUavList();

            if (this.uavTracingView == null)
            {
                this.uavTracingView = new UavTracingView();
                this.uavTracingView.Owner = Application.Current.MainWindow;
            }

            this.uavTracingView.Left = shellView.Left + 10;
            this.uavTracingView.Top = shellView.Top + 10;
            this.uavTracingView.Width = 390;
            this.uavTracingView.Height = 680;
            this.uavTracingView.DataContext = this;
            this.uavTracingView.Show();

            //this.uavTracingView.MountTypeChangedAction = (s) =>
            //{
            //    uavMountType = s;
            //    this.UavItemAreaModels = this.GetUavList();

            //    Messenger.Messengers.Notify("MountViewChange", s);
            //};

            //Messenger.Messengers.Notify("MountViewChange", 283);


            if (UavItemAreaModels != null)
            {
                foreach (var uavItemAreaViewModel in UavItemAreaModels)
                {
                    foreach (var item in uavItemAreaViewModel.UavItemModels)
                    {
                        item.VideoControllerVM.OffsetHeight = this.uavTracingView.ActualHeight;
                        item.VideoControllerVM.Width = this.uavTracingView.Width;
                        item.VideoControllerVM.WindowTitle =
                            string.Format(Helpers.ResourceHelper.FindKey("Realtimevideo") + "--{0}", item.deviceInfo.deviceName);
                    }
                }

                if (UavItemAreaModels.Count > 0)
                {
                    foreach (var uavItemAreaViewModel in UavItemAreaModels)
                    {
                        if (uavItemAreaViewModel.UavItemModels.Count > 0)
                        {
                            this.SelectedUavItemModel = uavItemAreaViewModel.UavItemModels.HasValues<UavItemViewModel>()
                                ? uavItemAreaViewModel.UavItemModels[0]
                                : null;
                            this.selectedUavItemModel.IsExpanded = true;
                            break;
                        }
                    }
                }
            }

            this._uavControlVModel.OnChecked();

            if (IsMultiScreen)
            {
                _multiVideoVM?.OnChecked();

                foreach (var uavItemAreaViewModel in uavItemAreaModels)
                {
                    foreach (var uavItemModel in uavItemAreaViewModel.UavItemModels)
                    {
                        uavItemModel.SetVideoScreenModels(_multiVideoVM.VideoScreenVModels);
                    }
                }
            }
        }

        public override void OnUnchecked()
        {
            base.OnUnchecked();

            if (this.uavTracingView != null)
                this.uavTracingView.Hide();

            this._uavControlVModel?.OnUnchecked();
            this._multiVideoVM?.OnUnchecked();
            this.uavItemAreaModels?.ForEach(t => t.UavItemModels?.ForEach(p => p.VideoControllerVM.IsChecked = false));
            RestoreEnv();
            this.UavItemAreaModels?.Clear();
            ServiceManager.GetService<IShellService>(null).LeftPanel.Visibility = System.Windows.Visibility.Visible;
            ServiceManager.GetService<IShellService>(null).BottomToolMenu.Visibility = System.Windows.Visibility.Visible;
            ServiceManager.GetService<IShellService>(null).ShellMenu.Visibility = System.Windows.Visibility.Visible;
            GCHelper.ClearMemory();
        }

        public void RestoreEnv()
        {
            if (UavItemAreaModels != null)
            {
                foreach (var uavItemAreaViewModel in UavItemAreaModels)
                {
                    foreach (var item in uavItemAreaViewModel.UavItemModels)
                    {
                        item.RestoreEnv();
                    }
                }
            }
        }

        private ObservableCollection<UavItemAreaViewModel> GetUavList()
        {
            try
            {
                var json = JsonUtil.DeserializeFromFile<dynamic>(AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
                string rootUrl = json.poiUrl;
                string url = string.Format("{0}/api/task/uav", rootUrl);
                var httpSrv = new HttpService();
                httpSrv.Token = HttpServiceUtil.Token;
                _groundstationList = httpSrv.HttpRequestAsync(url);

                var Uavlist = JsonUtil.DeserializeFromString<dynamic>(_groundstationList);
                var list = Uavlist.data;

                ObservableCollection<UavItemAreaViewModel> models = new ObservableCollection<UavItemAreaViewModel>();

                foreach (var outItem in list)
                {
                    var model = new UavItemAreaViewModel();
                    var uavList = new ObservableCollection<UavItemViewModel>();

                    int onLineCount = 0;

                    foreach (var item in outItem.list)
                    {
                        DeviceInfo deviceInfo = new DeviceInfo();
                        deviceInfo.goods_id = item.goods_id;
                        deviceInfo.goods_name = item.goods_name;
                        deviceInfo.deviceType = item.deviceType;
                        deviceInfo.deviceName = item.deviceName;
                        deviceInfo.deviceSerial = item.deviceSerial;
                        deviceInfo.deviceHardId = item.deviceHardId;
                        deviceInfo.httpStatus = item.httpStatus;
                        deviceInfo.socketStatus = item.socketStatus;
                        deviceInfo.max_voltage = item.max_voltage;
                        deviceInfo.return_voltage = item.return_voltage;
                        deviceInfo.addtime = item.addtime;

                        var innerModel = new UavItemViewModel()
                        {
                            UavTileTips = int.Parse(deviceInfo.httpStatus) == 1
                                    ? string.Format("{0}({1})", string.IsNullOrEmpty(deviceInfo.deviceName) ? deviceInfo.deviceHardId : deviceInfo.deviceName, Helpers.ResourceHelper.FindKey("InService"))
                                    : string.IsNullOrEmpty(deviceInfo.deviceName) ? deviceInfo.deviceHardId : deviceInfo.deviceName,
                            UavTitle = int.Parse(deviceInfo.httpStatus) == 1
                                    ? string.Format("{0}({1})", string.IsNullOrEmpty(deviceInfo.deviceName) ? Helpers.StrHelper.StrPadRight(deviceInfo.deviceHardId, _strShowMinLen) : Helpers.StrHelper.StrPadRight(deviceInfo.deviceName, _strShowMinLen), Helpers.ResourceHelper.FindKey("InService"))
                                    : string.IsNullOrEmpty(deviceInfo.deviceName) ? Helpers.StrHelper.StrPadRight(deviceInfo.deviceHardId, _strShowMaxLen) : Helpers.StrHelper.StrPadRight(deviceInfo.deviceName, _strShowMaxLen),                           
                            UavControllableState = Convert.ToInt16(deviceInfo.socketStatus) == 1
                                ? Helpers.ResourceHelper.FindKey("Controllable")
                                : Helpers.ResourceHelper.FindKey("Uncontrollable"),
                            deviceInfo = deviceInfo,
                            IsExpanded = false,
                            IsComboxEnabled = true,
                            IsMultiScreen = this.IsMultiScreen,
                            UavTracingModel = new UavTracingModel
                            {
                                deviceInfo = deviceInfo,
                            },
                            VideoControllerVM = new VideoControllerVModel
                            {
                                deviceInfo = deviceInfo,
                                MultiVideoVM = this._multiVideoVM,
                                IsMultiScreen = this.IsMultiScreen
                            },
                        };
                        innerModel.UavTracingModel.OnTracking += new Action<DeviceInfo>(_uavControlVModel.OnUavtracking);

                        if (item.httpStatus == 1)
                            onLineCount++;

                        uavList.Add(innerModel);
                    }

                    //model.AreaID = outItem.area_id;
                    model.AreaName = outItem.area_name;
                    model.UavItemModels = uavList;
                    model.OnLine = onLineCount;
                    model.OffLine = uavList.Count - onLineCount;
                    models.Add(model);
                }

                return models;
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
                return null;
            }
        }

        private ObservableCollection<UavItemAreaViewModel> GetUavListNew()
        {
            try
            {
                var json = JsonUtil.DeserializeFromFile<dynamic>(AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
                string rootUrl = json.poiUrl;
                string url = string.Format("{0}/api/uav/get-uav-list", rootUrl);
                var httpSrv = new HttpService();
                httpSrv.Token = HttpServiceUtil.Token;
                _groundstationList = httpSrv.HttpRequestAsync(url);

                var Uavlist = JsonUtil.DeserializeFromString<dynamic>(_groundstationList);
                var list = Uavlist.data;

                ObservableCollection<UavItemAreaViewModel> models = new ObservableCollection<UavItemAreaViewModel>();

                foreach (var outItem in list)
                {
                    var model = new UavItemAreaViewModel();
                    var uavList = new ObservableCollection<UavItemViewModel>();

                    int onLineCount = 0;

                    foreach (var item in outItem.list)
                    {
                        DeviceInfo deviceInfo = new DeviceInfo();
                        deviceInfo.goods_id = item.goods_id;
                        deviceInfo.goods_name = item.goods_name;
                        deviceInfo.deviceType = item.deviceType;
                        deviceInfo.deviceName = item.deviceName;
                        deviceInfo.deviceSerial = item.deviceSerial;
                        deviceInfo.deviceHardId = item.deviceHardId;
                        deviceInfo.httpStatus = item.httpStatus;
                        deviceInfo.socketStatus = item.socketStatus;
                        deviceInfo.max_voltage = item.max_voltage;
                        deviceInfo.return_voltage = item.return_voltage;
                        deviceInfo.addtime = item.addtime;

                        var innerModel = new UavItemViewModel()
                        {
                            UavTileTips = int.Parse(deviceInfo.httpStatus) == 1
                                    ? string.Format("{0}({1})", string.IsNullOrEmpty(deviceInfo.deviceName) ? deviceInfo.deviceHardId : deviceInfo.deviceName, Helpers.ResourceHelper.FindKey("InService"))
                                    : string.IsNullOrEmpty(deviceInfo.deviceName) ? deviceInfo.deviceHardId : deviceInfo.deviceName,
                            UavTitle = int.Parse(deviceInfo.httpStatus) == 1
                                    ? string.Format("{0}({1})", string.IsNullOrEmpty(deviceInfo.deviceName) ? Helpers.StrHelper.StrPadRight(deviceInfo.deviceHardId, _strShowMinLen) : Helpers.StrHelper.StrPadRight(deviceInfo.deviceName, _strShowMinLen), Helpers.ResourceHelper.FindKey("InService"))
                                    : string.IsNullOrEmpty(deviceInfo.deviceName) ? Helpers.StrHelper.StrPadRight(deviceInfo.deviceHardId, _strShowMaxLen) : Helpers.StrHelper.StrPadRight(deviceInfo.deviceName, _strShowMaxLen),
                            UavControllableState = Convert.ToInt16(deviceInfo.socketStatus) == 1
                                ? Helpers.ResourceHelper.FindKey("Controllable")
                                : Helpers.ResourceHelper.FindKey("Uncontrollable"),
                            deviceInfo = deviceInfo,
                            IsExpanded = false,
                            IsComboxEnabled = true,
                            IsMultiScreen = this.IsMultiScreen,
                            UavTracingModel = new UavTracingModel
                            {
                                deviceInfo = deviceInfo,
                            },
                            VideoControllerVM = new VideoControllerVModel
                            {
                                deviceInfo = deviceInfo,
                                MultiVideoVM = this._multiVideoVM,
                                IsMultiScreen = this.IsMultiScreen
                            },
                        };
                        innerModel.UavTracingModel.OnTracking += new Action<DeviceInfo>(_uavControlVModel.OnUavtracking);

                        if (item.httpStatus == 1)
                            onLineCount++;

                        uavList.Add(innerModel);
                    }

                    //model.AreaID = outItem.area_id;
                    model.AreaName = outItem.area_name;
                    model.UavItemModels = uavList;
                    model.OnLine = onLineCount;
                    model.OffLine = uavList.Count - onLineCount;
                    models.Add(model);
                }

                return models;
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
                return null;
            }
        }

    }



}