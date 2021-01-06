using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.RenderControl;
using Mmc.Framework.Draw;
using Mmc.Framework.Services;
using Mmc.MathUtil;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.Services.HttpService;
using Mmc.Mspace.Theme.Pop;
using Mmc.Windows.Services;
using Mmc.Windows.Utils;
using Mmc.Wpf.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Mmc.Mspace.IntelligentAnalysisModule.MidPointCheck
{
    public class NewDrawLineVModel : BaseViewModel
    {
        public Action HideWin;
        public Action<LineItem> AddPipe;
        List<Guid> guids = new List<Guid>();
        ICurveSymbol curveSymbol;        
        private DrawCustomerUC drawCustomer;
        // public Action<InspectModel, string> AddTIF;
        private NewDrawLineView newDrawLineView = null;
        private string Geom = "";
        private string _pipeName;
        public string PipeName
        {
            get { return _pipeName; }
            set
            {
                _pipeName = value; OnPropertyChanged("PipeName");
            }
        }
        private string _pipe;
        public string Pipe
        {
            get { return _pipe; }
            set
            {
                _pipe = value; OnPropertyChanged("Pipe");
            }
        }
        private string _startPoi;
        public string StartPoi
        {
            get { return _startPoi; }
            set
            {
                _startPoi = value; OnPropertyChanged("StartPoi");
            }
        }
        private string _endPoi;
        public string EndPoi
        {
            get { return _endPoi; }
            set
            {
                _endPoi = value; OnPropertyChanged("EndPoi");
            }
        }        
        public ICommand NewLineCmd { get; set; }
        public ICommand GoDrawLine { get; set; }

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(OnCancelCommand)); }
            set { _cancelCommand = value; }
        }
        public NewDrawLineVModel()
        {
            newDrawLineView = new NewDrawLineView();
            newDrawLineView.DataContext = this;
             HideWin = newDrawLineView.CloseWindow;
            this.NewLineCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
               AddLineData();               
            });
            this.GoDrawLine = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                if (string.IsNullOrEmpty(StartPoi)|| string.IsNullOrEmpty(EndPoi))
                {
                    Messages.ShowMessage("请输入起始桩号！");
                    return;
                }
                RegisterDrawLine();
            });
        }
        //关闭新增窗口
        private void OnCancelCommand()
        {
            //清楚数据
     
            this.HideWin();
        }
        public void ShowDrawWin()
        {
            newDrawLineView.Owner = Application.Current.MainWindow;
            newDrawLineView.Left = 380;
            newDrawLineView.Top = Application.Current.MainWindow.Height * 0.2;
            newDrawLineView?.Show();
        }
        private void  AddLineData()
        {
            string api = string.Empty;
            api = MarkInterface.AddLine;
            LineItem lineItem = new LineItem();
            lineItem.name = PipeName;
            lineItem.pipe_id = Pipe;
            lineItem.type_id = TypenameToNum(newDrawLineView.DrawLineWay.SelectedItem.ToString());
            lineItem.start = StartPoi??"0";
            lineItem.end = EndPoi ?? "0";
            lineItem.geom = Geom;
            if(guids.Count!=0)
            {
                lineItem.guid = guids[0];
            }
            string url = MarkInterface.AddLine;          
            var jsonData = JsonUtil.SerializeToString(lineItem);
            string resStr = HttpServiceHelper.Instance.PostRequestForData(url, jsonData);           
            using (JsonTextReader reader = new JsonTextReader(new StringReader(resStr)))
            {
                JValue astatues = (JValue)JToken.ReadFrom(reader);
                //string astatues = o["status"].ToString();
                if (astatues.ToString() == "添加成功")
                {
                    Messages.ShowMessage("新增成功");
                    AddPipe(lineItem);
                    newDrawLineView.Hide();
                }
                else
                {
                    Messages.ShowMessage("新增失败，请检查数据");
                }
            }
        }
        private string TypenameToNum(string typename)
        {
            string num = "1";
            if (typename == "自动")
            {
                num = "1";
            }
            else if (typename == "手动")
            {
                num = "0";
            }
            return num;
        }
        private void RegisterDrawLine()
        {            
                try
                {
                    if (drawCustomer == null)
                    {
                        drawCustomer = new DrawCustomerUC(Helpers.ResourceHelper.FindKey("pipeLineMarkerKey"), DrawCustomerType.MenuCommand);
                        //注册绘制多边形事件
                    }
                    RCDrawManager.Instance.PolylineDraw.Register(GviMap.AxMapControl, drawCustomer, RCMouseOperType.PickPoint);
                    RCDrawManager.Instance.PolylineDraw.OnDrawFinished -= PlanPolylineDraw_OnDrawFinished;
                    RCDrawManager.Instance.PolylineDraw.OnDrawFinished += PlanPolylineDraw_OnDrawFinished;


                }
                catch (Exception e)
                {
                    SystemLog.Log(e);
                }         
        }
        private void PlanPolylineDraw_OnDrawFinished(object sender, object result)
        {
            var rPolyline = result as IRenderPolyline;
            var polyLine = rPolyline.GetFdeGeometry() as IPolyline;
            if (polyLine == null || polyLine.PointCount < 2)
            {
                RCDrawManager.Instance.PolylineDraw.OnDrawFinished -= PlanPolylineDraw_OnDrawFinished;
                RCDrawManager.Instance.PolylineDraw.UnRegister(GviMap.AxMapControl);
                return;
            }
            curveSymbol = GviMap.TraceLinePolyManager.CreateCurveSymbol(0.4f, System.Drawing.Color.Yellow, gviDashStyle.gviDashSmall);
            IRenderPolyline renderPolyline = GviMap.ObjectManager.CreateRenderPolyline(polyLine, curveSymbol);         
            RCDrawManager.Instance.PolylineDraw.OnDrawFinished -= PlanPolylineDraw_OnDrawFinished;
            RCDrawManager.Instance.PolylineDraw.UnRegister(GviMap.AxMapControl);          
            var pt = polyLine.GetPoint(0);
            var prjWkt = Wgs84UtmUtil.GetWkt(pt.X);
            if (!string.IsNullOrEmpty(prjWkt))
                polyLine.ProjectEx(prjWkt);
            // this.Len = polyLine.Length;
            polyLine.Project(GviMap.SpatialCrs);
            //  label.VisibleMask = gviViewportMask.gviViewAllNormalView;
            renderPolyline.VisibleMask = gviViewportMask.gviViewAllNormalView;
            Geom = polyLine.AsWKT();
            guids.Add(renderPolyline.Guid);
        }
        private void DelObjs()
        {
            foreach (var item in guids)
            {
                GviMap.ObjectManager.DeleteObject(item);
            }
            guids.Clear();
        }


    }
}
