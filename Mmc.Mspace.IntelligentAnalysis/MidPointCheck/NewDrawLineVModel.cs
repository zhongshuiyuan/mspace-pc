using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.RenderControl;
using Mmc.Framework.Draw;
using Mmc.Framework.Services;
using Mmc.MathUtil;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.IntelligentAnalysisModule.Models;
using Mmc.Mspace.PoiManagerModule.Models;
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
        public Action HideParentsWin;
        public Action ShowParentsWin;
        public Action<LineItem> AddPipe;
        List<Guid> guids = new List<Guid>();
        ICurveSymbol curveSymbol;        
        private DrawCustomerUC drawCustomer;
        // public Action<InspectModel, string> AddTIF;
        private NewDrawLineView newDrawLineView = null;

        private TraceListView traceListView = null;
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
        private string _sn;
        public string Sn
        {
            get { return _sn; }
            set
            {
                _sn = value; OnPropertyChanged("Sn");
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
        private StakeModel _startPoi;
        public StakeModel StartPoi
        {
            get { return _startPoi; }
            set
            {
                _startPoi = value; OnPropertyChanged("StartPoi");
            }
        }

        private string _SelectedItem;

        public string SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; OnPropertyChanged("SelectedItem"); }
        }
        private PipeModel _selectPipeModel;
        /// <summary>
        /// 管线选中
        /// </summary>
        public PipeModel SelectPipeModel
        {
            get { return _selectPipeModel; }
            set
            {
                _selectPipeModel = value;
                OnPropertyChanged("SelectPipeModel");
                getStackList();
            }
        }
        private List<PipeModel> _pipeModels = new List<PipeModel>();
        public List<PipeModel> PipeModels
        {
            get { return _pipeModels; }
            set
            {
                _pipeModels = value;
                OnPropertyChanged("PipeModels");
            }
        }
        private List<StakeModel> _stakeModels = new List<StakeModel>();
        public List<StakeModel> StakeModels
        {
            get { return _stakeModels; }
            set
            {
                _stakeModels = value;
                OnPropertyChanged("StakeModels");
            }
        }

        private List<TracingLineModel> _tracingLineModels = new List<TracingLineModel>();
        public List<TracingLineModel> TracingLineModels
        {
            get { return _tracingLineModels; }
            set
            {
                _tracingLineModels = value;
                OnPropertyChanged("TracingLineModels");
            }
        }
        
        private StakeModel _endPoi;
        public StakeModel EndPoi
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

        private RelayCommand _deletePointCommand;
        public RelayCommand DeletePointCommand
        {
            get { return _deletePointCommand ?? (_deletePointCommand = new RelayCommand(OnDeletePointCommand)); }
            set { _deletePointCommand = value; }
        }

        private RelayCommand _savePointCommand;
        public RelayCommand SavePointCommand
        {
            get { return _savePointCommand ?? (_savePointCommand = new RelayCommand(OnSavePointCommand)); }
            set { _savePointCommand = value; }
        }

        
        public NewDrawLineVModel()
        {
            this.NewLineCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
               AddLineData();               
            });
            this.GoDrawLine = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                if (string.IsNullOrEmpty(Sn))
                {
                    Messages.ShowMessage("请选输入描线编号！");
                    return;
                }
                if (string.IsNullOrEmpty(PipeName))
                {
                    Messages.ShowMessage("请选输入描线名称！");
                    return;
                }
                if (SelectPipeModel == null)
                {
                    Messages.ShowMessage("请选择对应管线！");
                    return;
                }
                if (SelectedItem == null)
                {
                    Messages.ShowMessage("请选择描线方式！");
                    return;
                }
                if (StartPoi ==null|| EndPoi==null)
                {
                    Messages.ShowMessage("请输入起始桩号！");
                    return;
                }
                this.HideParentsWin();
                //隐藏列表界面 
                RegisterDrawLine();

                if(traceListView==null)
                {
                    traceListView = new TraceListView();
                    traceListView.Owner = Application.Current.MainWindow;
                    traceListView.DataContext = this;
                    this.gettracingline();
                    traceListView.Show();
                }

                traceListView.Left = 50;
                traceListView.Top = Application.Current.MainWindow.Height * 0.55;
                this.gettracingline();
                traceListView.Show();
            });

            this.getPipeList();
        }

        private void OnSavePointCommand() {
            this.AddLineData();
        }

        private void OnDeletePointCommand()
        {
            this.DelObjs();
        }
        //关闭新增窗口
        private void OnCancelCommand()
        {
           if (Messages.ShowMessageDialog("提示", "未保存手动描点信息，是否保存并退出？"))
            {
                //保存数据
                AddLineData();
                //清楚数据
                this.HideWin();
             
                this.ShowParentsWin();
            }
            else
            {
                RCDrawManager.Instance.PolylineDraw.OnDrawFinished -= PlanPolylineDraw_OnDrawFinished;
                RCDrawManager.Instance.PolylineDraw.UnRegister(GviMap.AxMapControl);
                traceListView.Hide();
                this.DelObjs();
                Geom = "";
                this.ShowParentsWin();
            }
            ChangedItem = null;
        }
        public void ShowDrawWin()
        {
            newDrawLineView = new NewDrawLineView();
            newDrawLineView.DataContext = this;
            newDrawLineView.Owner = Application.Current.MainWindow;
            HideWin = newDrawLineView.CloseWindow;
            newDrawLineView.Left = 50;
            newDrawLineView.Top = Application.Current.MainWindow.Height * 0.05;
            newDrawLineView.Show();
        }
        /// <summary>
        /// 获取描点列表
        /// </summary>
        private void gettracingline()
        {
            Task.Run(() =>
            {
                this.TracingLineModels = new List<TracingLineModel>();
                string param = "?page=1&page_size=20&start=" + StartPoi.Id + "&edn=" + EndPoi.Id;
                string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.tracinglineList + param);
                this.TracingLineModels = (JsonUtil.DeserializeFromString<List<TracingLineModel>>(resStr));
            });
        }
        private LineItem ChangedItem = null;
        public void ChangedData(LineItem lineItem)
        {
            ChangedItem = lineItem;
            this.Sn = lineItem.sn;
            this.PipeName = lineItem.name;
            
        }
        public void ClearData()
        {
            ChangedItem = null;
            this.Sn ="";
            this.PipeName = "";
            this.StartPoi = null;
            this.EndPoi = null;
            this.EndPoi = null;
            this.SelectedItem = null;
        }

        /// <summary>
        /// 获取中
        /// </summary>
        private void getStackList()
        {
            Task.Run(() =>
            {
                string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.stakeindex+ "?pipe_id="+SelectPipeModel.Id);
                this.StakeModels = (JsonUtil.DeserializeFromString<List<StakeModel>>(resStr));
            });
        }
        public void getPipeList()
        {
            Task.Run(() => {
                string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.PipeList);
                this.PipeModels = (JsonUtil.DeserializeFromString<List<PipeModel>>(resStr));
            });
        }
        private void  AddLineData()
        {
            if(string.IsNullOrEmpty(Geom))
            {
                Messages.ShowMessage("未描线，请描线后再次保存！");
                return;
            }


     
        
            //校验数据
            string api = string.Empty;
            api = MarkInterface.AddLine;
            LineItem lineItem = new LineItem();
            lineItem.name = PipeName;
            lineItem.pipe_id = SelectPipeModel.Id;
            lineItem.sn = Sn;
            lineItem.type_id = TypenameToNum(SelectedItem);
            lineItem.start = StartPoi.Id;
            lineItem.end = EndPoi.Id;
            lineItem.geom = Geom;
            if(guids.Count!=0)
            {
                lineItem.guid = guids[0];
            }
            string url = MarkInterface.AddLine;          
            var jsonData = JsonUtil.SerializeToString(lineItem);
            string resStr = HttpServiceHelper.Instance.PostRequestForData(url, jsonData);

            var list = JsonUtil.DeserializeFromString<dynamic>(resStr);
            string id = list["id"];

            if(string.IsNullOrEmpty(id))
            {
                Messages.ShowMessage("新增失败，请检查数据");
                return;
            }
            for (int i = 0; i < TracingLineModels.Count; i++)
            {
                TracingLineModel tracingLineModel = new TracingLineModel();
                tracingLineModel.Lat = TracingLineModels[0].Lat;
                tracingLineModel.Lng = TracingLineModels[0].Lng;
                tracingLineModel.Stake = TracingLineModels[0].Sn;
                tracingLineModel.Height = TracingLineModels[0].Height;
                tracingLineModel.Traces = id;
                string resStr1 = HttpServiceHelper.Instance.PostRequestForData(PipelineInterface.tracinglineCreate, JsonUtil.SerializeToString(tracingLineModel));
            }
            Messages.ShowMessage("新增成功");
            AddPipe(lineItem);
            newDrawLineView.Hide();
            traceListView.Hide();
            ChangedItem = null;
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
                ShowParentsWin();
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

            if(!string.IsNullOrEmpty(Geom))
            {
                string one = "(";
                string two= ")";
                int IndexofA = Geom.IndexOf(one);
                int IndexofB = Geom.IndexOf(two);
                string Ru = Geom.Substring(IndexofA + 1, IndexofB - IndexofA - 1);
                string[] points = Ru.Split(',');
                List<TracingLineModel> list = new List<TracingLineModel>();
                for (int i = 0; i < points.Count(); i++)
                {
                    string[] xyz =  points[0].Split(' ');
                    //int index = (Convert.ToInt32(StartPoi.Sn.Substring(2, StartPoi.Sn.Length - 2) ) + i);
                    TracingLineModel tracingLineModel = new TracingLineModel()
                    {
                        Sn = "AA" + i+100,
                        Index =i+1,
                        Lng = xyz[0],
                        Lat = xyz[1],
                        Height = xyz[2],
                    };
                    list.Add(tracingLineModel);
                }
                TracingLineModels = list;
            }
        
            guids.Add(renderPolyline.Guid);
        }
        private void DelObjs()
        {
            foreach (var item in guids)
            {
                GviMap.ObjectManager.DeleteObject(item);
            }
            guids.Clear();
            TracingLineModels = new List<TracingLineModel>();
        }


    }
}
