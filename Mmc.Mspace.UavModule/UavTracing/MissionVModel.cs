using Gvitech.CityMaker.FdeGeometry;
using Mmc.Framework.Services;
using Mmc.Mspace.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.UavModule.UavTracing
{
    public class MissionVModel : CheckedToolItemModel
    {
        string Url = "";
        WebView webView = null;
        public override void Initialize()
        {          
            base.Initialize();
            base.ViewType = ViewType.CheckedIcon;
            Url = @"https://x.mmcuav.cn/pad/#/pages/index/index?a=mmczylh&b=zylh123456";         
        }

        public override void OnChecked()
        {           
            base.OnChecked();            
            webView = new WebView(Url);
            webView.CloseWin -= OnUnchecked;
            webView.CloseWin += OnUnchecked;
            webView.Show();
        }

        public override void OnUnchecked()
        {          
            base.OnUnchecked();
            webView.Hide();
        }

    }
}
