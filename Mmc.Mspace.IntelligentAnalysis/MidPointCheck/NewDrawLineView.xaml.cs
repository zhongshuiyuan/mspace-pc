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

namespace Mmc.Mspace.IntelligentAnalysisModule.MidPointCheck
{
    /// <summary>
    /// NewDrawLineView.xaml 的交互逻辑
    /// </summary>
    public partial class NewDrawLineView
    {
        public NewDrawLineView()
        {
            InitializeComponent();
            //DrawLineWay.Items.Add("手动");
            //DrawLineWay.Items.Add("自动");
            VisibleOrNot.Items.Add("是");
            VisibleOrNot.Items.Add("否");
        }
        public void CloseWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Close();
            });

        }
    }
}
