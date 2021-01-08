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
            InitializeComponent(); string url = "http://wrj.mmcuav.cn/web?token=" + HttpServiceUtil.Token;
            Browser.LoadUri(url);
            windowsFormsHost.Child = Browser; //把浏览器与前后台进行对接  
        }
    }
}
