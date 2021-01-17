using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Mmc.Mspace.Common.ShellService;
using Mmc.Mspace.Main.ViewModels;
using Mmc.Mspace.Services.MapHostService;
using MMC.MSpace.Views;
using Mmc.Windows.Services;
using MMC.MSpace.ViewModels;
using Mmc.Mspace.PoiManagerModule.Views;
using Mmc.Mspace.PoiManagerModule.ViewModels;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Mmc.Mspace.Common;
using Mmc.Mspace.RegularInspectionModule.ViewModels;
using Mmc.Mspace.RegularInspectionModule.Views;
using Mmc.Mspace.Common.Messenger;
using Mmc.MathUtil;
using Mmc.Mspace.Const.ConstPath;
using Mmc.Framework.Services;
using Gvitech.CityMaker.FdeGeometry;
using Mmc.Wpf.Toolkit.Helpers;
using Mmc.Mspace.RegularInspectionModule;
using Mmc.Mspace.NavigationModule.Navigation;
using Mmc.Mspace.Theme.Pop;
using Mmc.Mspace.PoiManagerModule.Dto;
using Mmc.Mspace.Common.Cache;
using Mmc.Mspace.Services.HttpService;
using Mmc.Windows.Utils;
using System.Threading;
using System.Threading.Tasks;
using QQ2564874169.Miniblink;
using Mmc.Mspace.RegularInspectionModule.model;
using Mmc.Mspace.Const.ConstDataInterface;
using System.Collections.Generic;
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.Math;
using Mmc.Mspace.Common.Models.pipelines;

namespace MMC.MSpace
{

    public partial class Shell : Window
    {
        private DoubleAnimation c_daListAnimation;
        TranslateTransform tt = new TranslateTransform();
        Thread t = null;
        public bool c_bState = true;//记录菜单栏状态 false隐藏 true显示
        System.Windows.Visibility visibility = Visibility.Hidden;
        public Shell()
        {
            this.InitializeComponent();
            bool flag = !StringExtension.ParseTo<bool>(ConfigurationManager.AppSettings["IsFullScreen"], false);
            if (flag)
            {
                base.MaxHeight = (base.Height = SystemParameters.WorkArea.Height);
                base.MaxWidth = (base.Width = SystemParameters.WorkArea.Width);
            }
            else
            {
                base.MaxHeight = (base.Height = SystemParameters.PrimaryScreenHeight);
                base.MaxWidth = (base.Width = SystemParameters.PrimaryScreenWidth);
            }
            base.Loaded += this.Shell_Loaded;
            base.LocationChanged += this.Shell_LocationChanged;
            StartHeartthread();
            c_daListAnimation = new DoubleAnimation();
            c_daListAnimation.BeginTime = TimeSpan.FromSeconds(1);//获取或设置此 Timeline 将要开始的时间。
            c_daListAnimation.FillBehavior = FillBehavior.HoldEnd;//获取或设置一个值，该值指定 Timeline 在活动周期结束后的行为方式。
            c_daListAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));//获取或设置此时间线播放的时间长度，而不是计数重复。
            getStackList();
        }




        public void ShowHiddenMenu()
        {
            double panelWidth = leftViewPanel.ActualWidth - leftStatus.ActualWidth;
            if ((bool)this.leftStatus.IsChecked)
            {
                c_daListAnimation.From = 0;
                c_daListAnimation.To = -panelWidth;
            }
            else
            {
                c_daListAnimation.From = -panelWidth;
                c_daListAnimation.To = 0;
            }
            c_daListAnimation.BeginTime = TimeSpan.FromSeconds(0.01);//设置动画将要开始的时间
            tt.X = 0;

            leftViewPanel.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.XProperty, c_daListAnimation);
        }

        private void Shell_LocationChanged(object sender, EventArgs e)
        {
            var systemMenuView = ServiceManager.GetService<IShellService>(null).ShellMenu;
            var mapWin = ServiceManager.GetService<IMaphostService>(null).MapWindow;
            mapWin.Top = base.Top;
            bool IsCompareView = ServiceManager.GetService<IShellService>(null).IsCompareView;
            if (!IsCompareView)
                mapWin.Left = (bool)this.leftStatus.IsChecked ? base.Left : (base.Left);
            else
                mapWin.Left = (bool)this.leftStatus.IsChecked ? base.Left : (base.Left);
            if (ServiceManager.GetService<IShellService>(null).OnShellLocationChanged != null)
            {
                double[] locationArr = new double[] { this.Top, this.Left, mapWin.Width };
                ServiceManager.GetService<IShellService>(null).OnShellLocationChanged(locationArr);
            }

            userMessageVModel?.GetWindowPosition();
        }
        Window1 window1 = null;
        private void Shell_Loaded(object sender, RoutedEventArgs e)
       {
            this.InitializeShellService();
            this.Owner = System.Windows.Application.Current.MainWindow;
            this.DataContext = Mmc.Mspace.Main.ViewModels.ShellViewModel.Instance;

            NavLeftWin(CommonContract.LeftMenuEnum.LeftManagementView);

            ServiceManager.GetService<IShellService>(null).LeftPanel = leftViewPanel;


            Messenger.Messengers.Register<CommonContract.LeftMenuEnum>("LeftMenuEnum", (t) => { NavLeftWin(t); });
            Messenger.Messengers.Register<bool>("BottomMenuEnum", (t) => { ShowBottomMenu(t); });
            Messenger.Messengers.Register<bool>("BottomMenuEnumNavigation", (t) => { ShowBottomNavigationMenu(t); });
            Messenger.Messengers.Register<bool>("openComparison", (t) => { this.CancelComparison.Visibility = Visibility.Visible; });
            Messenger.Messengers.Register<bool>("zhibeiCommand", (t) => { zhibei(); });
            
            Messenger.Messengers.Register<bool>("IntelligentAnalysisShow", (t) => {
                if (t)
                {
                    try
                    {
                        window1 = new Window1();
                        window1.Height = Application.Current.MainWindow.Height * 0.8;
                        window1.Width = Application.Current.MainWindow.Width - 600;
                        window1.Left = 420;
                        window1.Top = Application.Current.MainWindow.Height * 0.1;
                        window1.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                }
                else
                {
                    if (window1 == null) return;
                    window1.Close();
                }
               
            });
          
            //描线管理
            Messenger.Messengers.Register<bool>("DrawLineManage", (t) => {
                this.leftStatus.IsChecked = t;
                this.ShowHiddenMenu();
                this.comparison.Visibility = (bool)this.leftStatus.IsChecked? Visibility.Collapsed : Visibility.Visible;
                this.CancelComparison.Visibility = Visibility.Collapsed;
            });
            Messenger.Messengers.Register<bool>("ShowHiddenMenu", (t) =>
            {
                if (!(bool)this.leftStatus.IsChecked && t)
                {
                    this.leftStatus.IsChecked = true;
                    ShowHiddenMenu();
                }
                else if ((bool)this.leftStatus.IsChecked && !t)
                {
                    this.leftStatus.IsChecked = false;
                    ShowHiddenMenu();
                }
            });

            #region Logo配置
            string logoConfigFile = AppDomain.CurrentDomain.BaseDirectory + ConfigPath.LogoConfig;

            if (File.Exists(logoConfigFile))
            {
                try
                {
                    var logoConfig = JsonUtil.DeserializeFromFile<dynamic>(logoConfigFile);

                    if (logoConfig.TopTitleIcon != null)
                    {
                        if (!string.IsNullOrWhiteSpace(logoConfig.TopTitleIcon.ToString()))
                        {
                            //img1.Source = logoConfig.TopTitleIcon;
                        }
                        else
                        {
                            //img1.Source = null;
                        }

                    }

                    if (logoConfig.BottomTitle != null)
                    {
                        if (!string.IsNullOrWhiteSpace(logoConfig.BottomTitle.ToString()))
                        {
                            topTitle.Content = logoConfig.BottomTitle;
                        }
                        else
                        {
                            topTitle.Content = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Messages.ShowMessage($"加载Logo配置异常:{ex.Message}");
                }
            }
            #endregion
        }

        private void ShowBottomNavigationMenu(bool status)
        {
            if (status)
            {
                if (navigationView == null)
                {
                    navigationView = new NavigationView();
                    navigationViewModel = new NavigationViewModel();
                    navigationView.DataContext = navigationViewModel;
                    navigationViewModel.Initialize();
                }
                //navigationViewModel.LoadRegionsData();
                this.BottomMenu.Content = navigationView;
            }
            else
            {
                this.BottomMenu.Content = null;
            }
        }

        private LeftManagementView leftManagementView;
        private LeftManagementVModel leftManagementVModel;
        private RegularInspectionView regularInspectionView;
        private RegularInspectionVModel regularInspectionVModel;
        private HistoryDomView historyDomView;
        private HistoryDomVModel historyDomVModel;

        private ComparisonView comparisonView;
        private ComparisonVModel comparisonVModel;

        private NavigationView navigationView;
        private NavigationViewModel navigationViewModel;

        public void NavLeftWin(CommonContract.LeftMenuEnum menu)
        {
            switch (menu)
            {
                case CommonContract.LeftMenuEnum.LeftManagementView:
                    if (leftManagementView == null)
                    {
                        leftManagementView = new LeftManagementView();
                        leftManagementVModel = new LeftManagementVModel();
                        leftManagementView.DataContext = leftManagementVModel;
                    }
                    this.comparison.Visibility = Visibility.Collapsed;
                    regularInspectionVModel?.MapControlEventManagement(false); 
                    regularInspectionVModel?.CloseAddWin(); 
                    this.leftView.Content = leftManagementView;
                    this.leftManagementVModel.ReLoaded();
                    break;
                case CommonContract.LeftMenuEnum.RegularInspectionView:
                    if (regularInspectionView == null)
                    {
                        regularInspectionView = new RegularInspectionView();
                        regularInspectionVModel = new RegularInspectionVModel();
                        regularInspectionView.DataContext = regularInspectionVModel;

                    }
                    comparisonView = new ComparisonView();
                    comparisonVModel = new ComparisonVModel();
                    comparisonView.DataContext = comparisonVModel;
                    regularInspectionVModel.updateRenderLayer = comparisonVModel.GetMapSource;
                    regularInspectionVModel?.MapControlEventManagement(true);
                    this.comparison.Visibility = Visibility.Visible;
                    comparisonVModel.UpdateSource();
                    this.comparison.Content = comparisonView;
                    this.leftView.Content = regularInspectionView;
                    //RegInsDataRenderManager.Instance.RecoverRenderLayer();
                    break;
            }
        }

        public void ShowBottomMenu(bool status)
        {
            if (status)
            {
                if (historyDomView == null)
                {
                    historyDomView = new HistoryDomView();
                    historyDomVModel = new HistoryDomVModel();
                    historyDomView.DataContext = historyDomVModel;
                    historyDomVModel.Initialize();
                }
                historyDomVModel.LoadRegionsData();
                this.BottomMenu.Content = historyDomView;
            }
            else
            {
                historyDomVModel.ClearData();
                this.BottomMenu.Content = null;
            }
        }
        private void InitializeShellService()
        {
            ServiceManager.GetService<IShellService>(null).ShellWindow = this;
            ServiceManager.GetService<IShellService>(null).ShellMenu = this._Menu;
            ServiceManager.GetService<IShellService>(null).ToolMenu = this.tool;
            ServiceManager.GetService<IShellService>(null).BottomToolMenu = this.bottomTool;
            ServiceManager.GetService<IShellService>(null).PopView = this.popView;
            //ServiceManager.GetService<IShellService>(null).BottomView = this.bottomView;
            ServiceManager.GetService<IShellService>(null).RightToolMenu = this.rightToolView;
            //ServiceManager.GetService<IShellService>(null).RightView = this.rightView;
            ServiceManager.GetService<IShellService>(null).ContentView = this.contentView;
            ServiceManager.GetService<IShellService>(null).ProgressView = this.progressView;
            ServiceManager.GetService<IShellService>(null).ToolMenu.Visibility = Visibility.Collapsed;
            ServiceManager.GetService<IShellService>(null).ToolRouteplanningMenu = EditwaypointMenu;
            ServiceManager.GetService<IShellService>(null).ToolRouteplanningMenu.Visibility = Visibility.Collapsed;

            ServiceManager.GetService<IShellService>(null).IsCompareView = false;

            //设置WGS84投影集合
            var jsonPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + ConfigPath.WGS84_UTM_Path;
            Wgs84UtmUtil.Load(jsonPath);
            var leftview = new SysMenuViewModel()
            {
                UserName = Mmc.Mspace.Common.Cache.CacheData.UserInfo.username,
            };
            // this.Tool.DataContext = leftview;
            this.TopMenu.DataContext = leftview;

        }

        private void Menu_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Button)//判断是按钮
            {
                // e.Handled = true;
            }
            else
            {
                try { base.DragMove(); } catch (Exception) { }
            }
        }
        private List<StakeModel> _stakeModels = new List<StakeModel>();
        public List<StakeModel> StakeModels
        {
            get { return _stakeModels; }
            set
            {
                _stakeModels = value;
            }
        }
        /// <summary>
        /// 获取中
        /// </summary>
        private void getStackList()
        {
            try
            {
                Task.Run(() =>
                {
                    Thread.Sleep(5000);
                    string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.stakeindex);
                    this.StakeModels = (JsonUtil.DeserializeFromString<List<StakeModel>>(resStr));
                    DrawAutoLine(true);
                    GviMap.AxMapControl.RcMouseWheel -= AxMapControl_RcMouseWheel;
                    GviMap.AxMapControl.RcMouseWheel += AxMapControl_RcMouseWheel;

                    GviMap.Camera.GetCamera2(out IPoint pointCamera, out IEulerAngle eulerAngle);
                    ////GviMap.Camera.FlyToEnvelope(point.Envelope);
                    eulerAngle.Tilt = -90;
                    eulerAngle.Heading = 0;
                    pointCamera.X = 115.42919769497675;
                    pointCamera.Y = 38.30008324358834;
                    pointCamera.Z = 2300;
                    GviMap.Camera.SetCamera2(pointCamera, eulerAngle, 0);
                });
            }
            catch (Exception e)
            {
                SystemLog.Log(e);
            }
        }
        public double currentHeight = 10000;
        private bool AxMapControl_RcMouseWheel(uint Flags, short Delta, int X, int Y)
        {
            if (!isVisible) return false;
            IPoint state;
            IEulerAngle eulerAngle;
            GviMap.Camera.GetCamera2(out state, out eulerAngle);
            if ((state.Z > 0 && state.Z < 100) && (currentHeight != 100))
            {
                currentHeight = 100;
                if (rLine != null)
                {
                    GviMap.ObjectManager.DeleteObject(rLine.Guid);
                    DrawAutoLine();
                }
            }
            else if ((state.Z > 100 && state.Z < 500) && (currentHeight != 500))
            {
                currentHeight = 500;
                if (rLine != null)
                {
                    GviMap.ObjectManager.DeleteObject(rLine.Guid);
                    DrawAutoLine();
                }
            }
            else if ((state.Z > 500 && state.Z < 1000) && (currentHeight != 1000))
            {
                currentHeight = 1000;
                if (rLine != null)
                {
                    GviMap.ObjectManager.DeleteObject(rLine.Guid);
                    DrawAutoLine();
                }
            }
            else if ((state.Z > 1000 && state.Z < 2000) && (currentHeight != 2000))
            {
                currentHeight = 2000;
                if (rLine != null)
                {
                    GviMap.ObjectManager.DeleteObject(rLine.Guid);
                    DrawAutoLine();
                }
            }
            else if ((state.Z > 2000 && state.Z < 5000) && (currentHeight != 5000))
            {
                currentHeight =5000;
                if (rLine != null)
                {
                    GviMap.ObjectManager.DeleteObject(rLine.Guid);
                    DrawAutoLine();
                }
            }
            else if ((state.Z > 5000&& state.Z < 10000) && (currentHeight !=10000))
            {
                currentHeight = 10000;
                if (rLine != null)
                {
                    GviMap.ObjectManager.DeleteObject(rLine.Guid);
                    DrawAutoLine();
           
                }
            }
           else if ((state.Z >=10000 && state.Z < 40000) && (currentHeight !=30000))
            {
                currentHeight =30000;
                if (rLine != null)
                {
                    GviMap.ObjectManager.DeleteObject(rLine.Guid);
                    DrawAutoLine();
                }
            }
            else if ((state.Z > 40000 ) && (currentHeight!=50000))
            {
                ClearPatrolList();
                currentHeight = 50000;
                if (rLine != null)
                {
                    GviMap.ObjectManager.DeleteObject(rLine.Guid);
                    DrawAutoLine();
                }
            }
            return false;
        }

        List<Guid> guids = new List<Guid>();
        IRenderPolyline rLine;
        public Dictionary<string, Guid> poiList = new Dictionary<string, Guid>();
        private void SetVideo()
        {
            if (StakeModels.Count > 0)
            {
                for (int i = 0; i < StakeModels.Count; i++)
                {
                    var point = StakeModels[i];

                    var poi = GviMap.GeoFactory.CreateGeometry(gviGeometryType.gviGeometryPOI, gviVertexAttribute.gviVertexAttributeZ) as IPOI;
                    poi.Name = point.Sn;
                    poi.SetPostion(Convert.ToDouble(point.Lng), Convert.ToDouble(point.Lat),string.IsNullOrEmpty(StakeModels[i].Height) ? 0: Convert.ToDouble(StakeModels[i].Height));
                    poi.Size = 50;
                    poi.ShowName = true;
                    poi.MaxVisibleDistance = 10000.0;
                    poi.MinVisibleDistance = 0;
                    poi.ImageName = string.Format(AppDomain.CurrentDomain.BaseDirectory + "项目数据\\shp\\IMG_POI\\{0}.png", "stake");
                    poi.SpatialCRS = GviMap.SpatialCrs;
                    var rPoi = GviMap.ObjectManager.CreateRenderPOI(poi);
                    rPoi.DepthTestMode = gviDepthTestMode.gviDepthTestAlways;
                    this.poiList.Add(rPoi.Guid.ToString(), rPoi.Guid);
                }
            }
        }

        public void ClearPatrolList()
        {
            Dictionary<string, Guid> expr_07 = this.poiList;
            bool flag = expr_07 == null || expr_07.Count > 0;
            if (flag)
            {
                foreach (KeyValuePair<string, Guid> current in this.poiList)
                {
                    GviMap.ObjectManager.DeleteObject(current.Value);
                }
            }
            this.poiList = new Dictionary<string, Guid>();
        }
        private void DrawAutoLine(bool first=false)
        {
            try
            {
                string header = "linestring z (";
                string end = ")";
                string line = "";
                for (int i = 0; i < StakeModels.Count; i++)
                {
                    if (i > this.StakeModels.Count - 1) return;
                    if (i == StakeModels.Count - 1)
                    {
                        line += (StakeModels[i].Lng + " " + StakeModels[i].Lat + " " + (string.IsNullOrEmpty(StakeModels[i].Height) ? "0" : StakeModels[i].Height));
                    }
                    else
                    {
                        line += (StakeModels[i].Lng + " " + StakeModels[i].Lat + " " + (string.IsNullOrEmpty(StakeModels[i].Height) ? "0" : StakeModels[i].Height) + ",");

                    }
                }
                string Geom = header + line + end;

                var polyLine = GviMap.GeoFactory.CreatePolyline(Geom, GviMap.SpatialCrs);
                CurveSymbol curveSymbol = new CurveSymbol();
                curveSymbol.Color = ColorConvert.Argb(100, 255, 0, 0);//GviMap.LinePolyManager.CurveSym
                curveSymbol.Width = first?4f: getWidth();

                rLine = GviMap.ObjectManager.CreateRenderPolyline(polyLine, curveSymbol, GviMap.ProjectTree.RootID);
                rLine.MaxVisibleDistance = 10000.0;
                rLine.MinVisibleDistance = 1.0;
                rLine.VisibleMask = gviViewportMask.gviViewAllNormalView;
                //GviMap.Camera.FlyToEnvelope(polyLine.Envelope);
             
                guids.Add(rLine.Guid);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.guandao.Content = "隐藏管道";
                });
                ClearPatrolList();
                SetVideo();
            }
            catch (Exception e)
            {
                SystemLog.Log(e);
            }
        }

        private float getWidth()
        {
            IPoint state;
            IEulerAngle eulerAngle;
            GviMap.Camera.GetCamera2(out state, out eulerAngle);
            if (state.Z <= 100)
            {
                return -10;
            }
            if (state.Z <= 500)
            {
                return -10;
            }
            if (state.Z <= 1000)
            {
                return 3f;
            }
            if (state.Z > 1000& state.Z < 2000)
            {
                return 4f;
            }
            if (state.Z > 2000&& state.Z<5000)
            {
                return 8f;
            }
            if (state.Z > 5000 && state.Z < 10000)
            {
                return 20f;
            }
            if (state.Z > 10000)
            {
                return 200f;
            }
            return 10f;

        }

        private void zhibei()
        {
            IPoint state;
            IEulerAngle eulerAngle;
            GviMap.Camera.GetCamera2(out state, out eulerAngle);
            eulerAngle.Heading = 0.0;
            GviMap.Camera.SetCamera2(state, eulerAngle, gviSetCameraFlags.gviSetCameraNoFlags);
        }
        private void Menu_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                base.DragMove();
                return;
            }

            var shell = this;
            if (shell.WindowState == WindowState.Normal)
            {
                //获取鼠标坐标
                Point position = e.GetPosition(this);
                var sreenPt = PointToScreen(position);
                var allSreens = System.Windows.Forms.Screen.AllScreens;

                //支持多屏幕拖放
                if (allSreens.Length > 0)
                {
                    for (int i = 0; i < allSreens.Length; i++)
                    {
                        var screen = allSreens[i];
                        if (screen.WorkingArea.Contains((int)sreenPt.X, (int)sreenPt.Y))
                        {
                            if (base.Left == screen.WorkingArea.Left && base.Top == screen.WorkingArea.Top)
                            {
                                Application.Current.MainWindow.WindowState = WindowState.Minimized;
                            }
                            else
                            {
                                base.Left = screen.WorkingArea.Left;
                                base.Top = screen.WorkingArea.Top;
                            }
                        }
                    }
                }
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                shell.WindowState = WindowState.Normal;
            }
        }



        public void Logout()
        {

        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2) return;
            var shell = this;
            if (shell.WindowState == WindowState.Normal)
            {
                //获取鼠标坐标
                Point position = e.GetPosition(this);
                var sreenPt = PointToScreen(position);
                var allSreens = System.Windows.Forms.Screen.AllScreens;

                //支持多屏幕拖放
                if (allSreens.Length > 0)
                {
                    for (int i = 0; i < allSreens.Length; i++)
                    {
                        var screen = allSreens[i];
                        if (screen.WorkingArea.Contains((int)sreenPt.X, (int)sreenPt.Y))
                        {
                            if (base.Left == screen.WorkingArea.Left && base.Top == screen.WorkingArea.Top)
                            {
                                Application.Current.MainWindow.WindowState = WindowState.Minimized;
                            }
                            else
                            {
                                base.Left = screen.WorkingArea.Left;
                                base.Top = screen.WorkingArea.Top;
                            }
                        }
                    }
                }
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                shell.WindowState = WindowState.Normal;
            }
        }

        private void _Menu_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                base.DragMove();

            }
            else
            {
                var shell = this;
                if (shell.WindowState == WindowState.Normal)
                {
                    //获取鼠标坐标
                    Point position = e.GetPosition(this);
                    var sreenPt = PointToScreen(position);
                    var allSreens = System.Windows.Forms.Screen.AllScreens;

                    //支持多屏幕拖放
                    if (allSreens.Length > 0)
                    {
                        for (int i = 0; i < allSreens.Length; i++)
                        {
                            var screen = allSreens[i];
                            if (screen.WorkingArea.Contains((int)sreenPt.X, (int)sreenPt.Y))
                            {
                                if (base.Left == screen.WorkingArea.Left && base.Top == screen.WorkingArea.Top)
                                {
                                    Application.Current.MainWindow.WindowState = WindowState.Minimized;
                                }
                                else
                                {
                                    base.Left = screen.WorkingArea.Left;
                                    base.Top = screen.WorkingArea.Top;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                    shell.WindowState = WindowState.Normal;
                }
            }


        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowHiddenMenu();
        }

        private void Tool_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            //UserCenterView view = new UserCenterView();
            //view.Show();
        }

        private void Tool_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Tool_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {


        }



        UserMessagesVModel userMessageVModel = null;
        bool userMessageVisible = true;
        private void Tool_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //deadLine.Visibility = Visibility.Visible;
            if (userMessageVModel == null)
            {
                userMessageVModel = new UserMessagesVModel();
            }
            if (userMessageVisible)
            {
                userMessageVModel.UserMessageShow();
                useImg.Source = (ImageSource)Helpers.ResourceHelper.FindResourceByKey("userImg_C");
                directImg.Source = (ImageSource)Helpers.ResourceHelper.FindResourceByKey("buttom");
                // userName.Foreground = //System.Windows.Media.Color.FromScRgb();
            }
            else
            {
                userMessageVModel.UserMessageHide();
                useImg.Source = (ImageSource)Helpers.ResourceHelper.FindResourceByKey("userImg");
                directImg.Source = (ImageSource)Helpers.ResourceHelper.FindResourceByKey("top");
            }
            userMessageVisible = !userMessageVisible;
        }

        private void Shell_OnClosed(object sender, EventArgs e)
        {
            try
            {
                Messenger.Messengers.Notify("PatrolmanListVModelUnChecked");
                t?.Abort();
                #region 关闭无人机AI流
                DateTime startTime = DateTime.Now;

                //var json = JsonUtil.DeserializeFromFile<dynamic>(
                //    AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
                //string closeStreamUrl = $"{json.aiUrl}stream-off";
                //HttpService httpService = new HttpService();
                //var res = httpService.HttpRequest(closeStreamUrl);

                DateTime stopTime = DateTime.Now;
                TimeSpan elapsedTime = stopTime - startTime;
                Console.WriteLine("shell.xaml.cs 关闭AI流 uavVideoVlcView Spendtime: {0}-{1}", elapsedTime, elapsedTime.TotalMilliseconds);
                #endregion

                #region 请求退出登录
                HttpServiceHelper.Instance.GetRequest("/api/login/logout");
                #endregion
                Application.Current.Shutdown(0);
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
            }
        }
        private void StartHeartthread()
        {
            t = new Thread(HeartBit);
            t.Start();

            // HeartBit();           
        }
        private void HeartBit()
        {
            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(180000); // 3分钟刷新一次
                HeartBitOneTime();
            }
        }

        private async void HeartBitOneTime()
        {
            await Task.Run(() =>
            {
                // string api = HttpServiceHelper.Instance.PostRequestAsync("/api/login/keepping");
                string resStr = string.Empty;
                try
                {
                    resStr = HttpServiceHelper.Instance.PostRequestAsync("/api/login/keepping");
                }
                catch (Exception ex)
                {
                    SystemLog.Log(ex);
                }

            });
        }

        private void CancelComparison_Click(object sender, RoutedEventArgs e)
        {
            this.CancelComparison.Visibility = Visibility.Collapsed;
            Messenger.Messengers.Notify("closeComparison", true);
        }
        private bool isVisible = true;
        private void guandao_Click(object sender, RoutedEventArgs e)
        {

            if (isVisible)
            {
                ClearPatrolList();
                this.guandao.Content = "显示管道";
                rLine?.SetVisibleMask(GviMap.Viewport.ViewportMode, 0, false);
                isVisible = false;

            }
            else
            {
                SetVideo();
                isVisible = true;
                this.guandao.Content = "隐藏管道";
                rLine?.SetVisibleMask(GviMap.Viewport.ViewportMode, 0, true);
            }
        }
    }
}
