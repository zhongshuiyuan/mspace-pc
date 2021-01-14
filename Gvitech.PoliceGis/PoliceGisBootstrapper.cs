using Mmc.Mspace.Common.ShellService;
using Mmc.Mspace.Services;
using Mmc.Mspace.Services.DataSourceServices;
using Mmc.Mspace.Services.FieldsFilterService;
using Mmc.Mspace.Services.HttpService;
using Mmc.Mspace.Services.LayerGroupService;
using Mmc.Mspace.Services.MapHostService;
using Mmc.Mspace.Services.MovePoiService;
using Mmc.Mspace.Services.NetRouteAnalysisService;
using Mmc.Mspace.Services.ShowCaptureObjectService;
using Mmc.Mspace.Services.StatisticService;
using MMC.MSpace.Views;
using Mmc.Windows.Services;
using Mmc.Wpf.Toolkit.Utils;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using Mmc.Mspace.Common.Cache;
using Mmc.Mspace.Common.Dto;
using Mmc.Mspace.Languagepack.LanguageManager;
using Mmc.Mspace.Services.LocalConfigService;
using Mmc.Mspace.Theme.Pop;
using Mmc.Windows.Utils;
using System.Threading.Tasks;
using System.Threading;
using Mmc.Mspace.Const.ConstPath;
using SuperDog;

namespace MMC.MSpace
{
    public class PoliceGisBootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return UnityContainerExtensions.Resolve<MapHost>(base.Container, new ResolverOverride[0]);
        }

        public static string loginUserName { get; set; }

        public static string loginPwd { get; set; }
        protected override void InitializeShell()
        {
            
            //WPFLoginView view = new WPFLoginView();
            //LoginView view = new LoginView();
            //DogCheck();
            //if (!(bool)view.ShowDialog())
            //{
            //    Environment.Exit(0);
            //    return;
            //}
            login();

            WebBrowserVersionEmulation();
            DXSplashScreen.Show<SplashWindow>();
            DXSplashScreen.Progress(0.0);
            string text = System.Windows.Forms.Application.LocalUserAppDataPath + "\\logs";
            SystemLog.InitSysLog(text);
            RegisterServices();
            SystemLog.Log("启动加载界面");
            double screenInchSize = ScreenHelper.GetScreenInchSize();
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory+"Config\\Screen\\", "*inch.xaml");
            Dictionary<double, string> dictionary = new Dictionary<double, string>();
            foreach (string text2 in files)
            {
                FileInfo fileInfo = new FileInfo(text2);
                double num = double.Parse(fileInfo.Name.Replace("inch.xaml", ""));
                double num2 = Math.Abs(screenInchSize - num);
                bool flag = dictionary.Count != 0;
                if (flag)
                {
                    bool flag2 = num2 < dictionary.FirstOrDefault<KeyValuePair<double, string>>().Key;
                    if (flag2)
                    {
                        dictionary.Clear();
                        dictionary.Add(num2, text2);
                    }
                }
                else
                {
                    dictionary.Add(num2, text2);
                }
            }
            SystemLog.Log("初始化wpf样式");
            FileStream stream = new FileStream(dictionary.FirstOrDefault<KeyValuePair<double, string>>().Value, FileMode.Open, FileAccess.Read);
            ResourceDictionary item = XamlReader.Load(stream) as ResourceDictionary;
            ResourceDictionary resourceDictionary = (ResourceDictionary)System.Windows.Application.LoadComponent(new Uri("/Mmc.Mspace.Theme;component/Styles.xaml", UriKind.Relative));
            resourceDictionary.MergedDictionaries.Insert(0, item);
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            SystemLog.Log("弹出shell窗口");
            System.Windows.Application.Current.MainWindow = (Window)base.Shell;
            System.Windows.Application.Current.MainWindow.Show();

            UserdeadLineMessage();
        
           
        }
        string vendorCodeString =
           "reLILQjZBM4SgnYgB9LPCRstGPGiWyvJJbjCh6uzsjkJvhaGjK4xDLvhqWUq69DH4uiuBHnJAeBYktPP64xcjkbm9Ujvz59jVskIpF402RsCx2Qi1LTjuafY8GJJUeW0C6Acj2yf5aXHkz1qK / HmtRzCbmMmqF + 5Us8ON9smX6QmZ3z9NRbg0CiC7ZdUa+HKE4x2BDuUJKJbeSRJyYtEOxmr7OYgX+qdkjbtK4Jevd++LuRboHINxipJzq2/tvSDhTLJsGqSS1xfNViN0+9AWUwTR8vIHlzHLCkJXFP14xYryAXJcZ3rApnVO3FzhsyTfGIq3AjBA2TIiuRa/VKyMZhY4NltrQAlStr3cMQngQnnCZjSGtR+AqAha2Sb3KLdrUkkMm4QE8FQHBcBU+UBz/TfQFaq1NRki56zfkRX3jWGAxpKDHKx70nOjNClv4+ScDP2+h7EPUBkUJhT8fiex1Ys/20oKTD7O/1CrAMxotelT7AfrTgCQXh1YnkVEGbUV0beZifzccRf3ODSAJ5mjWo/DH1kRxEn6DtBZEgaCMjE2Zn06JrXWJ2/If41Ufmk9XUAUz3hQN560pTqeqGWr67tJWdIGYhSSSNjAq/I8sEXp9lv5zeLsD0JjtJeil38dpNPeFOIU5nBjtYDqyWfEwz/2kL/1X/9OwhI+O5mGbLGbQCKBdr7YiywxNg2KIorISyQoX3iE8SuG8Ly+Rg4JOKeHwtLOH2W7mORSa+Nu5cnpGnRxPwtFDzvWGmqPT84tmar3vi6xWneKBgWbb8Uba6XhgYRCqYhhBOQ1v4F8JokU0Zam1DQHp+ZojcpR+iqgNbS2ojn13GOkag64c2f6BcycNce0PGKHx+Ci1trVRH3fqIyvn+EA/a28McfIkMwCaX5gdhb4JAYfeuHAlxWUX8V4W3Th33qXCUIm4M4lY90gjhGOSN3fxp/kZLEI2tjhfBYpHWwCxLqJ8nmDV4sew==";

        private string scope;
        public void DogCheck()
        {
            DogFeature feature = DogFeature.Default;

            Dog dog = new Dog(feature);

            scope = "<dogscope />";

            DogStatus status = dog.Login(vendorCodeString, scope);

            if (DogStatus.StatusOk == status)
            {
                status = dog.Logout();
            }
            else
            {
                //Messages.ShowMessageDialog("提示","授权狗未连接");
                MessageBox.Show("授权狗未连接");
                Environment.Exit(0);
                //return;
            }

            dog.Dispose();

        }
        private void UserdeadLineMessage()
        {
            string deadLineDay = CacheData.UserInfo.life_day;
            if (Convert.ToInt32(deadLineDay) != -1&& Convert.ToInt32(deadLineDay)<=10)
            {
               Messages.ShowMessage( "  有效期还剩" + CacheData.UserInfo.life_day + "天");
            }           
        }
        private void ShowVersionUpdate(string version)
        {
            MessageBoxView msg = new MessageBoxView
            {
                Title = "版本更新提醒",
                Message = $"检测到MSpace更新版本,当前最新版本为:{version},可前往配置页面进行更新!",
                BtnCancelContent = "忽略此版本",
                Owner = Application.Current.MainWindow
            };
            msg.CancelAction = () =>
            {
                CacheData.SaveConfig("IgnoreVersion", version);
                CacheData.SaveConfig("IsIgnoreVersion", "True");
            };
            msg.Show();
        }

        private static void RegisterServices()
        {
            SystemLog.Log("开始注册服务...", 0);
            SystemLog.Log("注册服务<MaphostService>...", 0);
            ServiceManager.RegisterService<IMaphostService>(new ProvideService(MaphostService.GetDefault));
            SystemLog.Log("注册服务<LocalWsConfigService>...", 0);
            ServiceManager.RegisterService<ILocalWsConfigService>(new ProvideService(LocalWsConfigService.GetDefault));
            SystemLog.Log("注册服务<InspectionService>...", 0);
            ServiceManager.RegisterService<IInspectionService>(new ProvideService(InspectionService.GetDefault));
            //SystemLog.Log("注册服务<RouteAnalysisService>...", 0);
            //ServiceManager.RegisterService<IRouteAnalysisService>(new ProvideService(RouteAnalysisService.GetDefault));
            SystemLog.Log("注册服务<RouteBDAnalysisService>...", 0);
            ServiceManager.RegisterService<IRouteBDAnalysisService>(new ProvideService(RouteBDAnalysisService.GetDefault));
            SystemLog.Log("注册服务<DataBaseService>...", 0);
            ServiceManager.RegisterService<IDataBaseService>(new ProvideService(DataBaseService.GetDefault));
            SystemLog.Log("注册服务<FieldsFilterService>...", 0);
            ServiceManager.RegisterService<IFieldsFilterService>(new ProvideService(FieldsFilterService.GetDefault));
            SystemLog.Log("注册服务<LayerGroupService>...", 0);
            ServiceManager.RegisterService<ILayerGroupService>(new ProvideService(LayerGroupService.GetDefault));
            SystemLog.Log("注册服务<CameraInfoService>...", 0);
            ServiceManager.RegisterService<ICameraInfoService>(new ProvideService(CameraInfoService.GetDefault));
            SystemLog.Log("注册服务<QueryService>...", 0);
            ServiceManager.RegisterService<IQueryService>(new ProvideService(QueryService.GetDefault));
            SystemLog.Log("注册服务<MaphostService>...", 0);
            ServiceManager.RegisterService<IMaphostService>(new ProvideService(MaphostService.GetDefault));
            SystemLog.Log("注册服务<ShellService>...", 0);
            ServiceManager.RegisterService<IShellService>(new ProvideService(ShellService.GetDefault));
            SystemLog.Log("注册服务<ShowCaptureObjectService>...", 0);
            ServiceManager.RegisterService<IShowCaptureObjectService>(new ProvideService(ShowCaptureObjectService.GetDefault));
            SystemLog.Log("注册服务<HttpServiceConfigService>...", 0);
            ServiceManager.RegisterService<IHttpServiceConfigService>(new ProvideService(HttpServiceConfigService.GetDefault));
            SystemLog.Log("注册服务<NetWorkCheckService>...", 0);
            ServiceManager.RegisterService<INetWorkCheckService>(new ProvideService(NetWorkCheckService.GetDefault));
            SystemLog.Log("注册服务<MovePoiService>...", 0);
            ServiceManager.RegisterService<IMovePoiService>(new ProvideService(MovePoiService.GetDefault));
            SystemLog.Log("注册服务<StatisticLayerService>...", 0);
            ServiceManager.RegisterService<IStatisticLayerService>(new ProvideService(StatisticLayerService.GetDefault));
            SystemLog.Log("注册服务<PoliceHttpService>...", 0);
            ServiceManager.RegisterService<IPoliceHttpService>(new ProvideService(PoliceHttpService.GetDefault));
            SystemLog.Log("注册服务<HttpVideoService>...", 0);
            ServiceManager.RegisterService<IVideoHttpService>(new ProvideService(VideoHttpService.GetDefault));
            SystemLog.Log("注册服务<HttpVideoService>...", 0);
            ServiceManager.RegisterService<ISubjectCaseHttpService>(new ProvideService(SubjectCaseHttpService.GetDefault));
            //SystemLog.Log("注册服务<ObjectForScriptService>...", 0);
            //ServiceManager.RegisterService<IObjectForScriptService>(new ProvideService(ObjectForScriptService.GetDefault));
            SystemLog.Log("结束注册服务...", 0);
        }

        private static void WebBrowserVersionEmulation()
        {
            const string BROWSER_EMULATION_KEY =
            @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
            //
            // app.exe and app.vshost.exe
            String appname = Process.GetCurrentProcess().ProcessName + ".exe";
            //
            // Webpages are displayed in IE9 Standards mode, regardless of the !DOCTYPE directive.
            const int browserEmulationMode = 11001;

            RegistryKey browserEmulationKey =
                Registry.CurrentUser.OpenSubKey(BROWSER_EMULATION_KEY, RegistryKeyPermissionCheck.ReadWriteSubTree) ??
                Registry.CurrentUser.CreateSubKey(BROWSER_EMULATION_KEY);

            if (browserEmulationKey != null)
            {
                browserEmulationKey.SetValue(appname, browserEmulationMode, RegistryValueKind.DWord);
                browserEmulationKey.Close();
            }
        }

        public void SetLanguage()
        {
            if (string.IsNullOrEmpty(CacheData.CurrentLanguage)) return;
            var language = LanguageManager.Languages.SingleOrDefault(t => t.Name == CacheData.CurrentLanguage);
            LanguageManager.ChangeLanguages(Application.Current.Resources, language);
        }
        private void login()
        {
            LanguageSwitching("zh-CN");
            string configFile = AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig;

            if (!File.Exists(configFile))
            {
                MessageBox.Show(Helpers.ResourceHelper.FindKey("ConfigFileMiss"));
                SystemLog.Log(string.Format("找不到配置文件:{0}", configFile));
                return;
            }

            var config = JsonUtil.DeserializeFromFile<dynamic>(configFile);

            if (config.poiUrl == null || config.mspaceVersion == null)
            {
                MessageBox.Show(Helpers.ResourceHelper.FindKey("ConfigItemError"));
                SystemLog.Log(string.Format("配置项不能为空:poiUrl or mspaceVersion"));
                return;
            }

            HttpServiceUtil.MspaceHostUrl = config.poiUrl;
            HttpServiceUtil.MspaceVersion = config.mspaceVersion;

            WebConfig.MspaceHostUrl = config.poiUrl;
            WebConfig.MspaceVersion = config.mspaceVersion;
            WebConfig.UavSocketUri = config.uavSocketUri;
            double TracePoiDistance = 5000;
            double LabelMaxDistance = 1000;
            if (config.tracePoiMaxDistance != null && double.TryParse(config.tracePoiMaxDistance.ToString(), out TracePoiDistance))
                WebConfig.TracePoiMaxDistance = TracePoiDistance;
            if (config.labelMaxDistance != null && double.TryParse(config.labelMaxDistance.ToString(), out LabelMaxDistance))
                WebConfig.LabelMaxDistance = LabelMaxDistance;

            string version = "&mspace_version=" + WebConfig.MspaceVersion;
            //string url = WebConfig.MspaceHostUrl + @"/api/login/gislogin" + version;
            string url = "/api/login/gislogin";
            var userString = string.Format("username=madmin&password=madmin123");
            //var userString = string.Format("username={0}&password={1}", loginUserName, loginPwd);
            var result = HttpServiceHelper.Instance.PostUrlByFormUrlencoded(url, userString);
            var resultObj = JsonUtil.DeserializeFromString<dynamic>(result);
            var token = resultObj.data.token;
            HttpServiceUtil.Token = token;
            HttpServiceHelper.Instance.HttpService.Token = token;
            GetUserInfo();

        }

        public void GetUserInfo()
        {
            try
            {
                string userApi = Mmc.Mspace.Const.ConstDataInterface.UserCfgInterface.UserCng;
                var resutl = HttpServiceHelper.Instance.GetRequestAsync(userApi);
                CacheData.UserInfo = JsonUtil.DeserializeFromString<UserInfo>(resutl);
                CacheData.UserInfo.loginPwd = loginPwd;
            }
            catch (Exception ex)
            {
                SystemLog.WriteLog("LoginView.GetUserInfo", ex);
            }
        }
        public void LanguageSwitching(string lang)
        {
            try
            {
                if (string.IsNullOrEmpty(lang)) return;
                var language = LanguageManager.Languages.SingleOrDefault(t => t.Name == lang);
                LanguageManager.ChangeLanguages(Application.Current.Resources, language);
                CacheData.SaveLanguage(lang);
            }
            catch (Exception ex)
            {
                SystemLog.WriteLog("LoginView.LanguageSwitching", ex);
            }
        }

    }
}