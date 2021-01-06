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
            this.Owner = Application.Current.MainWindow;
            DrawLineWay.Items.Add("手动");
            DrawLineWay.Items.Add("自动");
            VisibleOrNot.Items.Add("是");
            VisibleOrNot.Items.Add("否");
        }
        public void CloseWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Hide();
            });

        }
        private void DrawLineWay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DrawLineWay.SelectedItem != null && DrawLineWay.SelectedItem.ToString() == "手动")
            {
                DrawlineBtn.Visibility = Visibility.Visible;
                midPoiNum.Visibility = Visibility.Collapsed;
            }
            if (DrawLineWay.SelectedItem != null && DrawLineWay.SelectedItem.ToString() == "自动")
            {
                DrawlineBtn.Visibility = Visibility.Hidden;
                midPoiNum.Visibility = Visibility.Visible;
            }
            //DrawlineBtn
        }
        private void GetLineList()
        {

        }
    }
}
