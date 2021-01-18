using Mmc.Mspace.Common.Cache;
using Mmc.Mspace.Services.HttpService;
using QQ2564874169.Miniblink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskManagement
{
    /// <summary>
    /// TaskView.xaml 的交互逻辑
    /// </summary>
    public partial class TaskView : UserControl
    {
        private MiniblinkBrowser Browser = new MiniblinkBrowser();
        public TaskView()
        {
            InitializeComponent();

            //username=madmin&password=madmin123
            
            //string url = "http://wrj.mmcuav.cn/web/#/?username=" + CacheData.UserInfo.username + "&password="+ CacheData.UserInfo.loginPwd;
            string url = "http://112.245.48.207:8088/web/#/?token=" + HttpServiceUtil.Token;
            Browser.LoadUri(url);
            windowsFormsHost.Child = Browser; //把浏览器与前后台进行对接  
        }
    }
}
