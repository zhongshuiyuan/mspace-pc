using LiveCharts;
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
using Mmc.Mspace.Common.Messenger;

namespace Mmc.Mspace.UavModule.UavTracing
{
    /// <summary>
    /// UavControlView.xaml 的交互逻辑
    /// </summary>
    public partial class UavControlView 
    {
        public UavControlView()
        {
            InitializeComponent();
            Loaded += UavControlView_Loaded;

        }

        private void UavControlView_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger.Messengers.Register<string>("MountViewChange", (mountType) =>
            {
                //switch (mountType)
                //{
                //    case "353":
                //        rbtnTearBomb.IsChecked = true;
                //        break;
                //    case "369":
                //        rbtnNetGun.IsChecked = true;
                //        break;
                //    case "298":
                //        rbtnJettison.IsChecked = true;
                //        break;
                //    default:
                //        rbtnDefault.IsChecked = true;
                //        break;
                //}
            });
        }

    }
}
