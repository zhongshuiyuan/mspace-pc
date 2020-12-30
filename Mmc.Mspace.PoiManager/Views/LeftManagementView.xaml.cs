using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mmc.Mspace.Common.Messenger;
using UserControl = System.Windows.Controls.UserControl;

namespace Mmc.Mspace.PoiManagerModule.Views
{
    /// <summary>
    /// LeftManagementView.xaml 的交互逻辑
    /// </summary>
    public partial class LeftManagementView : UserControl
    {
        public LeftManagementView()
        {
            InitializeComponent();
        }

        private void btnLeftCollapsed_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Messengers.Notify("ShowHiddenMenu", true);
        }
    }
}
