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
        private MiniblinkBrowser Browser = new MiniblinkBrowser();
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
       
            Messenger.Messengers.Register<bool>("IntelligentAnalysisShow", (t) => {
                if (t)
                {
                    window1 = new Window1();
                    window1.Height = Application.Current.MainWindow.Height * 0.8;
                    window1.Width = Application.Current.MainWindow.Width -600;
                    window1.Left = 420;
                    window1.Top = Application.Current.MainWindow.Height * 0.1;
                    window1.Show();
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
                    //RegInsDataRenderManager.Instance.SaveRenderLayersStatus();
                    //RegInsDataRenderManager.Instance.HideRenderLayer();
                    //RegInsDataRenderManager.Instance.CloseHintView();
                    break;
                case CommonContract.LeftMenuEnum.RegularInspectionView:
                    if (regularInspectionView == null)
                    {
                        regularInspectionView = new RegularInspectionView();
                        regularInspectionVModel = new RegularInspectionVModel();
                        regularInspectionView.DataContext = regularInspectionVModel;

                    }

                    if (comparisonView == null)
                    {
                        comparisonView = new ComparisonView();
                        comparisonVModel = new ComparisonVModel();
                        comparisonView.DataContext = comparisonVModel;
                       regularInspectionVModel.updateRenderLayer = comparisonVModel.GetMapSource;
                    }
                    regularInspectionVModel?.MapControlEventManagement(true);
                    this.comparison.Visibility = Visibility.Visible;
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
    }
}
