using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Math;
using Mmc.Framework.Services;
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

namespace Mmc.Mspace.IntelligentAnalysisModule.AreaWidth
{
    /// <summary>
    /// AreaWidthView.xaml 的交互逻辑
    /// </summary>
    public partial class AreaWidthView
    {
        public AreaWidthView()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void Poi_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            dynamic dataContext = btn.DataContext;
            IPoint poi = dataContext as IPoint;
            //GviMap.Camera.FlyToEnvelope(poi.Envelope);

            GviMap.Camera.GetCamera2(out IPoint pointCamera, out IEulerAngle eulerAngle);
            ////GviMap.Camera.FlyToEnvelope(point.Envelope);
            eulerAngle.Tilt = -90;
            eulerAngle.Heading = 110;
            pointCamera.X = poi.X;
            pointCamera.Y = poi.Y;
            pointCamera.Z = 1000;
            GviMap.Camera.SetCamera2(pointCamera, eulerAngle, 0);
        }

        private void keydown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;

            //屏蔽非法按键
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal || e.Key.ToString() == "Tab")
            {
                if (txt.Text.Contains(".") && e.Key == Key.Decimal)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                if (txt.Text.Contains(".") && e.Key == Key.OemPeriod)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                if (e.Key.ToString() != "RightCtrl")
                {
                    MessageBox.Show(this.Resources["Txt_InnerPage_ConnPointManage_TabMyConnPoint_AddMyCloudSeeNum_Prompt"].ToString(), this.Resources["Txt_InnerPage_ConnPointManage_TabMyConnPoint_AddMyCloudSeeNum_PromptTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void textChange(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);

            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }
    }
}
