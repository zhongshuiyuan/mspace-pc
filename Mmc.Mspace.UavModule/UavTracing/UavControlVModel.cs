using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.Windows.Input;
using System.Xml.Serialization;
using Mmc.Mspace.Common.ShellService;
using Mmc.Mspace.Common.Models;
using Mmc.Windows.Services;
using Mmc.Windows.Utils;
using Mmc.Wpf.Commands;
using Mmc.Mspace.UavModule.Dto;
using Mmc.Mspace.Theme.Pop;
using Mmc.Framework.Services;
using Mmc.Mspace.Const.ConstDataBase;
using Mmc.Mspace.Const.ConstPath;
using Mmc.Mspace.Services.HttpService;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.Utils;

namespace Mmc.Mspace.UavModule.UavTracing
{
    public class UavControlVModel : CheckedToolItemModel
    {
        private UavControlView _controlView;
        private string _sendMessage = "";
        private readonly TimeSpan delay = TimeSpan.FromMilliseconds(5);
        private string _takeoffHeight = "2";
        private int _sendVehicleRouteId = 0;
        private bool IsVideoing = false;
        ObservableCollection<CombRouteListModel> routeInfos;
        public DeviceInfo deviceInfo { get; set; }

        #region socket real time value
        //radio
        private bool _isRadioLockChecked;
        private bool _isRadioControlChecked;
        private bool _isRadioResetChecked;
        #endregion 

        //无人机信息
        private UavTraceVModel _uavTraceVM;
        [XmlIgnore]
        public UavTraceVModel UavTraceVM
        {
            get { return this._uavTraceVM; }
            set { base.SetAndNotifyPropertyChanged<UavTraceVModel>(ref this._uavTraceVM, value, "UavTraceVM"); }
        }

        public Action<string, string> OnTimeUpdated;

        private string _vehicleTile;
        [XmlIgnore]
        public string VehicleTile
        {
            get { return this._vehicleTile + " 无人机 "; }
            set { base.SetAndNotifyPropertyChanged<string>(ref this._vehicleTile, value, "VehicleTile"); }
        }

        private string _tipJoytypeTile;
        [XmlIgnore]
        public string TipJoytypeTile
        {
            get { return this._tipJoytypeTile; }
            set { base.SetAndNotifyPropertyChanged<string>(ref this._tipJoytypeTile, value, "TipJoytypeTile"); }
        }

        /// <summary>
        /// 摇杆类型名称
        /// </summary>
        private string _joyTypeText;
        [XmlIgnore]
        public string JoyTypeText
        {
            get { return this._joyTypeText; }
            set { _joyTypeText = value; NotifyPropertyChanged("JoyTypeText");
                TipJoytypeTile = Helpers.StrHelper.StrPadRight(_joyTypeText, 10);
            }
        }

        #region radio        
        [XmlIgnore]
        public bool isRadioLockChecked
        {
            get { return this._isRadioLockChecked; }
            set { base.SetAndNotifyPropertyChanged<bool>(ref this._isRadioLockChecked, value, "isRadioLockChecked"); }
        }

        [XmlIgnore]
        public bool isRadioControlChecked
        {
            get { return this._isRadioControlChecked; }
            set { base.SetAndNotifyPropertyChanged<bool>(ref this._isRadioControlChecked, value, "isRadioControlChecked"); }
        }

        [XmlIgnore]
        public bool isRadioResetChecked
        {
            get { return this._isRadioResetChecked; }
            set { base.SetAndNotifyPropertyChanged<bool>(ref this._isRadioResetChecked, value, "isRadioResetChecked"); }
        }
        #endregion              

        #region 仪表盘
        private ChartValues<double> _lastTenSecSeries;
        [XmlIgnore]
        public ChartValues<double> LastTenSecSeries
        {
            get { return this._lastTenSecSeries; }
            set { base.SetAndNotifyPropertyChanged<ChartValues<double>>(ref this._lastTenSecSeries, value, "SeriesCollection"); }
        }

        private void initLastHourSeries()
        {
            LastTenSecSeries = new ChartValues<double> { -10, -5, 0, 5, 10, 15 };
        }

        #endregion

        #region Command
        [XmlIgnore]
        public ICommand CloseCmd { get; set; }
        [XmlIgnore]
        public ICommand CamPitchUp { get; set; }
        [XmlIgnore]
        public ICommand CamPitchDown { get; set; }
        [XmlIgnore]
        public ICommand CamHeadLeft { get; set; }
        [XmlIgnore]
        public ICommand CamHeadRight { get; set; }
        [XmlIgnore]
        public ICommand CamZoomOut { get; set; }
        [XmlIgnore]
        public ICommand CamZoomIn { get; set; }
        [XmlIgnore]
        public ICommand CamCamCapture { get; set; }
        [XmlIgnore]
        public ICommand CamCamVideo { get; set; }
        [XmlIgnore]
        public ICommand cmdJoyArm { get; set; }
        [XmlIgnore]
        public ICommand cmdJoyLocked { get; set; }
        [XmlIgnore]
        public ICommand cmdJoyLand { get; set; }
        [XmlIgnore]
        public ICommand cmdJoyReturnModel { get; set; }
        [XmlIgnore]
        public ICommand cmdJoyLoadRoute { get; set; }
        [XmlIgnore]
        public ICommand cmdJoyTakeoff { get; set; }
        [XmlIgnore]
        public ICommand cmdJoyAutoModel { get; set; }
        [XmlIgnore]
        public ICommand cmdJoySendRoute { get; set; }
        [XmlIgnore]// 网抢
        public ICommand cmdNetGunShoot { get; set; }
        [XmlIgnore]// 抛投
        public ICommand cmdJettison { get; set; }
        [XmlIgnore]// 抛投
        public ICommand cmdJettisonUp { get; set; }
        [XmlIgnore]// 抛投
        public ICommand cmdJettisonDown { get; set; }
        [XmlIgnore]// 抛投
        public ICommand cmdJettisonMode { get; set; }
        [XmlIgnore]// 催泪弹
        public ICommand cmdTearBomb { get; set; }
        #endregion

        //释放窗体
        public void releaseWindow()
        {
            _controlView?.Close();
            _controlView = null;
        }
        private Window _shellView;

        public string WindowTitle { get; set; }
        public override void OnChecked()
        {
            JoyTypeText = Helpers.ResourceHelper.FindKey("NoneJoystick");
            initLastHourSeries();
            ShowWindow();
            base.OnChecked();
            ToDo();
        }

        private void ShowWindow()
        {
            _shellView = ServiceManager.GetService<IShellService>(null).ShellWindow;
            WindowTitle = Helpers.ResourceHelper.FindKey("RemoteControl");
            if (_controlView == null)
                _controlView = new UavControlView();
            this._controlView.Left = _shellView.Left;
            this._controlView.Top = _shellView.Height - 256;
            this._controlView.Width = _shellView.Width;
            this._controlView.Height = 256;
            this._controlView.Owner = Application.Current.MainWindow;

            _controlView.DataContext = this;
            _controlView.Show();
        }

        private void ToDo()
        {
            //Radio
            _controlView.RadioCamLock.Checked += new RoutedEventHandler(OnRadioCamLockChecked);
            _controlView.RadioCamControl.Checked += new RoutedEventHandler(OnRadioCamControlChecked);
            _controlView.RadioCamReset.Checked += new RoutedEventHandler(OnRadioCamResetChecked);
            selectRouteComboBox();
        }

        public override void OnUnchecked()
        {
            try
            {
                base.OnUnchecked();
                this.releaseWindow();
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
            }
        }
        /// <summary>
        /// 获取航线列表
        /// </summary>
        private void selectRouteComboBox(string searchName="")
        {
            var json = JsonUtil.DeserializeFromFile<dynamic>(AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
            string url = string.Format("{0}/api/flight-course/getlist?name=", json.poiUrl) + searchName;
            var httpservice = new HttpService();

            var listResult = RequestRouteList(httpservice, url);
            var listResultJson = JsonUtil.DeserializeFromString<dynamic>(listResult);

            if ((bool)listResultJson.status)
            {
                if (routeInfos == null)
                    routeInfos = new ObservableCollection<CombRouteListModel>();
                else
                    routeInfos.Clear();
                foreach (var itemInfo in listResultJson.data)
                {
                    CombRouteListModel routeInfo = new CombRouteListModel { id = itemInfo.id, name = itemInfo.name };
                    routeInfos.Add(routeInfo);
                }
                this.CombRouteListModels = routeInfos;
            }
        }

        private string RequestRouteList(HttpService httpservice, string url)
        {
            try
            {
                httpservice.Token = HttpServiceUtil.Token;
                var result = httpservice.HttpRequestAsync(url);
                return result;
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
                return null;
            }
        }

        public void OnUavtracking(DeviceInfo info)
        {
            //if (info.httpStatus != "1")
            //{
            //    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("DeviceNotOnline"));
            //    return;
            //}

            if (this.deviceInfo.deviceHardId == info.deviceHardId)
            {
                UavTraceVModel.ToUavInfoVM(info, this.UavTraceVM);
                LastTenSecSeries.Add(UavTraceVM.Altitude);
                LastTenSecSeries.RemoveAt(0);
            }
        }             

        private string RequestWarterService(HttpService httpservice, string url)
        {
            try
            {
                httpservice.Token = HttpServiceUtil.Token;
                var result = httpservice.HttpRequestAsync(url);
                return result;
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
                return null;
            }
        }

        public void OnRadioCamLockChecked(object sender, RoutedEventArgs e)
        {
            var cmdData = cmdCamLock();
            var jsonData = JsonUtil.SerializeToString(cmdData);
            _sendMessage = jsonData;
            SystemLog.WriteLog("CamCamLock:" + _sendMessage);
            if (OnTimeUpdated != null)
            {
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId,  _sendMessage);                
            }
        }

        private bool isFloat(string val)
        {
            if (val == null) return false;
            Regex reg = new Regex(@"^(-?\d+)(\.\d+)?$");
            return reg.Match(val).Success;
        }

        /// <summary>
        /// set vehicle takeoff height
        /// </summary>
        public string SelectTakeOffHeight
        {
            get { return Convert.ToInt16(_takeoffHeight) > 20 ? "20" : _takeoffHeight; }
            set { base.SetAndNotifyPropertyChanged<string>(ref _takeoffHeight, value, "SelectTakeOffHeight"); }
        }

        /// <summary>
        /// 选择航线名称
        /// </summary>
        private string _selectRouteName;
        public string SelectRouteName
        {
            get { return _selectRouteName; }
            set { _selectRouteName = value; NotifyPropertyChanged("SelectRouteName"); }
        }

        private ObservableCollection<CombRouteListModel> _routeListModel;

        [XmlIgnore]
        public ObservableCollection<CombRouteListModel> CombRouteListModels
        {
            get { return this._routeListModel; }
            set { base.SetAndNotifyPropertyChanged<ObservableCollection<CombRouteListModel>>(ref this._routeListModel, value, "CombRouteListModels"); }
        }

        private CombRouteListModel _selectedRouteListModel;
        [XmlIgnore]
        public CombRouteListModel SelectedRouteListModel
        {
            get { return this._selectedRouteListModel; }
            set
            {
                base.SetAndNotifyPropertyChanged<CombRouteListModel>(ref this._selectedRouteListModel, value, "SelectedRouteListModel");
                if (_selectedRouteListModel != null)
                    _sendVehicleRouteId = (int)_selectedRouteListModel.id;
            }
        }

        public void OnRadioCamControlChecked(object sender, RoutedEventArgs e)
        {
            var cmdData = cmdCamControl();
            var jsonData = JsonUtil.SerializeToString(cmdData);
            _sendMessage = jsonData;
            SystemLog.WriteLog("CamCamControl:" + _sendMessage);
            if (OnTimeUpdated != null)
            {
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId,  _sendMessage);
            }
        }
        public void OnRadioCamResetChecked(object sender, RoutedEventArgs e)
        {
            var cmdData = cmdCamReset();
            var jsonData = JsonUtil.SerializeToString(cmdData);
            _sendMessage = jsonData;
            SystemLog.WriteLog("CamCamReset:" + _sendMessage);
            if (OnTimeUpdated != null)
            {
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId,  _sendMessage);
            }
        }

        private void gviCam()
        {
            IVector3 v3 = null; IEulerAngle angle = null;
            GviMap.Camera.GetCamera(out v3, out angle);
            GviMap.Camera.SetCamera(v3, angle, gviSetCameraFlags.gviSetCameraNoFlags);
        }

        public override void Initialize()
        {
            base.Initialize();

            this.UavTraceVM = new UavTraceVModel();
            this.CloseCmd = new RelayCommand(() =>
            {
                base.IsChecked = false;
                this.releaseWindow();
            });            
            this.CamPitchUp = new RelayCommand(() =>
            {
                var cmdData = cmdPitchUp();
                var jsonData = JsonUtil.SerializeToString(cmdData);

                _sendMessage = jsonData;
                SystemLog.WriteLog("CamPitchUp:" + _sendMessage);
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);

            });
            this.CamPitchDown = new RelayCommand(() =>
            {
                var cmdData = cmdPitchDown();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                SystemLog.WriteLog("CamPitchDown:" + _sendMessage);
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);

            });
            this.CamHeadLeft = new RelayCommand(() =>
            {
                var cmdData = cmdHeadLeft();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                SystemLog.WriteLog("CamHeadLeft:" + _sendMessage);
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);

            });
            this.CamHeadRight = new RelayCommand(() =>
            {
                var cmdData = cmdHeadRight();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                SystemLog.WriteLog("CamHeadRight:" + _sendMessage);
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);

            });
            this.CamZoomOut = new RelayCommand(() =>
            {
                var cmdData = cmdZoomOut();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                SystemLog.WriteLog("CamZoomOut:" + _sendMessage);
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);

            });

            this.CamZoomIn = new RelayCommand(() =>
            {
                var cmdData = cmdZoomIn();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                SystemLog.WriteLog("CamZoomIn:" + _sendMessage);
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);

            });

            this.CamCamCapture = new RelayCommand(() =>
            {
                var cmdData = cmdCamCapture();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                SystemLog.WriteLog("CamCamCapture:" + _sendMessage);
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);

            });

            this.CamCamVideo = new RelayCommand(() =>
            {
                var cmdData = cmdCamVideo();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                SystemLog.WriteLog("CamCamVideo:" + _sendMessage);
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);

            });            

            this.cmdJoyTakeoff = new RelayCommand(() =>
            {
                var cmdData = cmdVehicleTakeoff();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJoyTakeoff:" + _sendMessage);

            });

            this.cmdJoyAutoModel = new RelayCommand(() =>
            {
                var cmdData = cmdVehicleAutoModel();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJoyAutoModel:" + _sendMessage);

            });
            this.cmdJoyArm = new RelayCommand(() =>
            {
                var cmdData = cmdVehicleArm();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJoyArm:" + _sendMessage);

            });
            this.cmdJoyLocked = new RelayCommand(() =>
            {
                var cmdData = cmdVehicleLocked();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJoyLocked:" + _sendMessage);

            });
            this.cmdJoyLand = new RelayCommand(() =>
            {
                var cmdData = cmdVehicleLand();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJoyLand:" + _sendMessage);

            });
            this.cmdJoyReturnModel = new RelayCommand(() =>
            {
                var cmdData = cmdVehicleReturn();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJoyReturnModel:" + _sendMessage);

            });

            this.cmdJoyLoadRoute = new RelayCommand(() =>
            {
                if (string.IsNullOrEmpty(SelectRouteName))
                {
                    Messages.ShowMessage("航线名称不存在，请选择后重试！");
                    return;
                }

                var cmdData = cmdVehicleLoadRoute();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJoyLoadRoute:" + _sendMessage);

            });
            this.cmdJoySendRoute = new RelayCommand(() =>
            {
                var cmdData = cmdVehicleSendRoute();
                var jsonData = JsonUtil.SerializeToString(cmdData);
                _sendMessage = jsonData;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJoySendRoute:" + _sendMessage);

            });
            this.cmdNetGunShoot = new RelayCommand(() =>
            {
                var cmdData = socketItemNetGunShoot();
                var json = JsonUtil.SerializeToString(cmdData);
                _sendMessage = json;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdNetGunShoot:" + _sendMessage);

            });

            this.cmdJettison = new RelayCommand(() =>
            {
                var cmdData = socketItemJettison();
                var json = JsonUtil.SerializeToString(cmdData);
                _sendMessage = json;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJettison:" + _sendMessage);

            });

            this.cmdJettisonUp = new RelayCommand(() =>
            {
                var cmdData = socketItemJettisonUp();
                var json = JsonUtil.SerializeToString(cmdData);
                _sendMessage = json;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJettisonUp:" + _sendMessage);

            });

            this.cmdJettisonDown = new RelayCommand(() =>
            {
                var cmdData = socketItemJettisonDown();
                var json = JsonUtil.SerializeToString(cmdData);
                _sendMessage = json;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJettisonDown:" + _sendMessage);

            });

            this.cmdJettisonMode = new RelayCommand<bool>((b) =>
            {
                var cmdData = socketItemJettisonMode(b);
                var json = JsonUtil.SerializeToString(cmdData);
                _sendMessage = json;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdJettisonMode:" + _sendMessage);

            });

            this.cmdTearBomb = new RelayCommand<bool>(b =>
            {
                var cmdData = socketItemTearBomb(b);
                var json = JsonUtil.SerializeToString(cmdData);
                _sendMessage = json;
                OnTimeUpdated?.Invoke(deviceInfo.deviceHardId, _sendMessage);
                SystemLog.WriteLog("cmdTearBomb:" + _sendMessage);

            });
        }

        #region create SocketItem
        /// <summary>
        /// 解锁 cmdFunction:2110 cmdValue:量值,<100解锁,大于100,上锁
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdVehicleArm()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2110,
                cmdValue = 0
            };

            return cmd;
        }

        /// <summary>
        /// 解锁 cmdFunction:2110 cmdValue:量值,<100解锁,大于100,上锁
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdVehicleLocked()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2110,
                cmdValue = 200
            };

            return cmd;
        }

        /// <summary>
        ///一键起飞 cmdFunction:2111 cmdValue:起飞高度(0,20) 默认2m
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdVehicleTakeoff()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2111,
                cmdValue = Convert.ToInt16(_takeoffHeight) > 20 ? 20 : Convert.ToInt16(_takeoffHeight),// 起飞高度(0,20) 默认2m
            };

            return cmd;
        }

        /// <summary>
        /// 航线模式 cmdFunction:2115
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdVehicleAutoModel()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2115,
            };

            return cmd;
        }

        /// <summary>
        /// 安全降落 cmdFunction:2116
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdVehicleLand()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2116,
            };

            return cmd;
        }

        /// <summary>
        /// 一键返航 cmdFunction:2112
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdVehicleReturn()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2112,
            };

            return cmd;
        }

        /// <summary>
        /// 地面站下载航线 cmdFunction:2113 cmdValue:航线id
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdVehicleLoadRoute()//地面站下载航线
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2113,
                cmdValue = SelectedRouteListModel.id
            };

            return cmd;
        }

        /// <summary>
        /// 发送航线到无人机 cmdFunction:2114
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdVehicleSendRoute()//发送航线至无人机
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2114,
            };

            return cmd;
        }


        /// <summary>
        /// 俯仰向上 cmdFunction:2210 cmdState:[1:向上],[2:向下] cmdValue:偏移量
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdPitchUp()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2210,
                cmdState = 1,
                cmdValue = 5
            };

            return cmd;
        }

        /// <summary>
        /// 俯仰向下 cmdFunction:2210 cmdState:[1:向上],[2:向下] cmdValue:偏移量
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdPitchDown()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2210,
                cmdState = 2,
                cmdValue = 5
            };

            return cmd;
        }

        /// <summary>
        /// 偏航向左 cmdFunction:2211 cmdState:[1:向左],[2:向右] cmdValue:偏移量
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdHeadLeft()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2211,
                cmdState = 1,
                cmdValue = -1
            };

            return cmd;
        }

        /// <summary>
        /// 偏航向右 cmdFunction:2211 cmdState:[1:向左],[2:向右] cmdValue:偏移量
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdHeadRight()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2211,
                cmdState = 2,
                cmdValue = -1
            };

            return cmd;
        }

        /// <summary>
        /// Zoom变焦(缩小) cmdFunction:2212 cmdState:[1:放大],[2:缩小]
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdZoomOut()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2212,
                cmdState = 2,
            };

            return cmd;
        }

        /// <summary>
        /// Zoom变焦(放大) cmdFunction:2212 cmdState:[1:放大],[2:缩小]
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdZoomIn()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2212,
                cmdState = 1,
            };

            return cmd;
        }

        /// <summary>
        /// Lock锁定模式 cmdFunction:2213
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdCamLock()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2213,
            };

            return cmd;
        }

        /// <summary>
        /// Free控制模式 cmdFunction:2214
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdCamControl()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2214,
            };

            return cmd;
        }

        /// <summary>
        /// Center回中模式 cmdFunction:2215
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdCamReset()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2215,
            };

            return cmd;
        }

        /// <summary>
        /// TakePic拍照 cmdFunction:2216
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdCamCapture()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2216,
            };

            return cmd;
        }

        /// <summary>
        /// 录像  开始录制:[cmdFunction:2217]   停止录制:[cmdFunction:2218]
        /// </summary>
        /// <returns></returns>
        private SocketItem cmdCamVideo()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;

            if (IsVideoing)
            {
                cmd.data = new SocketData
                {
                    cmdFunction = 2218,
                };
            }
            else
            {
                cmd.data = new SocketData
                {
                    cmdFunction = 2217
                };
            }

            IsVideoing = !IsVideoing;

            return cmd;
        }

        private SocketItem socketItemNetGunShoot()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2280,
            };

            return cmd;
        }

        private SocketItem socketItemJettison()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2242,
            };

            return cmd;
        }

        private SocketItem socketItemJettisonUp()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2240,
                cmdValue = 1
            };

            return cmd;
        }

        private SocketItem socketItemJettisonDown()
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2240,
                cmdValue = 0
            };

            return cmd;
        }

        private SocketItem socketItemJettisonMode(bool mode)
        {
            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = 2241
            };

            return cmd;
        }

        private SocketItem socketItemTearBomb(bool mode)
        {

            var cmd = new SocketControl();
            cmd.deviceHardId = deviceInfo.deviceHardId;
            cmd.data = new SocketData
            {
                cmdFunction = mode ? 2266 : 2265
            };

            return cmd;
        }
        #endregion
    }
}
