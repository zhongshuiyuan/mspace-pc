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
using System.Windows.Shapes;

namespace Mmc.Mspace.RegularInspectionModule.Views
{
    /// <summary>
    /// NewInspectionView.xaml 的交互逻辑
    /// </summary>
    public partial class NewInspectionView 
    {
        //private MiniblinkBrowser Browser = new MiniblinkBrowser();
        public NewInspectionView()
        {
            InitializeComponent();
            //string url = "http://baidu.com" ;
            //Browser.LoadUri(url);
            //windowsFormsHost.Child = Browser; //把浏览器与前后台进行对接  
        }

        public void CloseWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Hide();
            });
         
        }
    }
}
