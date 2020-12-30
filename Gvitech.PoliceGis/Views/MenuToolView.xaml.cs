using System;
using System.Collections.Generic;
using System.IO;
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
using Mmc.Mspace.Const.ConstPath;
using Mmc.Mspace.Theme.Pop;
using Mmc.Windows.Utils;

namespace MMC.MSpace.Views
{
    /// <summary>
    /// MenuToolView.xaml 的交互逻辑
    /// </summary>
    public partial class MenuToolView : UserControl
    {
        public MenuToolView()
        {
            InitializeComponent();

            Loaded += MenuToolView_Loaded;
        }

        private void MenuToolView_Loaded(object sender, RoutedEventArgs e)
        {
            string logoConfigFile = AppDomain.CurrentDomain.BaseDirectory + ConfigPath.LogoConfig;

            if (File.Exists(logoConfigFile))
            {
                try
                {
                    var logoConfig = JsonUtil.DeserializeFromFile<dynamic>(logoConfigFile);

                    if (logoConfig.CenterLogoIcon != null)
                    {
                        if (!string.IsNullOrWhiteSpace(logoConfig.CenterLogoIcon.ToString()))
                        {
                            imgCenterLogo.Source = logoConfig.CenterLogoIcon;
                        }
                        else
                        {
                            imgCenterLogo.Source = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Messages.ShowMessage($"加载Logo配置异常:{ex.Message}");
                }
            }
        }
    }
}
