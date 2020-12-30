using Mmc.Mspace.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.UavModule.UavTracing
{
    public class DataCenterVModel: CheckedToolItemModel
    {        
        string Url = "";
        WebView webView = null;
        public override void Initialize()
        {
            base.Initialize();
            base.ViewType = ViewType.CheckedIcon;
            Url = @"https://x.mmcuav.cn/dispatch/#/login?a=mmczylh&b=zylh123456";
        }

        public override void OnChecked()
        {
            base.OnChecked();
            webView = new WebView(Url);
            webView.CloseWin -= OnUnchecked;
            webView.CloseWin += OnUnchecked;
            webView.Width = 1680;
            webView.Height = 1050;
            webView.Show();
        }

        public override void OnUnchecked()
        {
            base.OnUnchecked();
            webView.Hide();
        }
    }
}
