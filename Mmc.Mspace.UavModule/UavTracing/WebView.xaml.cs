using CefSharp;

using Mmc.Mspace.Services.HttpService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Mmc.Mspace.UavModule.UavTracing
{
    /// <summary>
    /// WebView.xaml 的交互逻辑
    /// </summary>
    public partial class WebView
    {
        public Action CloseWin;
        public WebView(string url)
        {
            InitializeComponent();
            this.Browser.Address = url;
            this.Owner = Application.Current.MainWindow;           
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            CloseWin();
        }     
        //Address="https://testx.mmcuav.cn/pad/"
    }
}
