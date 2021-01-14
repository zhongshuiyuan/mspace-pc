using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// TraceListView.xaml 的交互逻辑
    /// </summary>
    public partial class TraceListView 
    {
        public TraceListView()
        {
            InitializeComponent();
            this.MidLinedg.LoadingRow += new EventHandler<DataGridRowEventArgs>(this.DgSceneRecord_LoadingRow);

        }

        private void DgSceneRecord_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

     

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");

            e.Handled = re.IsMatch(e.Text);
        }
    }
}
