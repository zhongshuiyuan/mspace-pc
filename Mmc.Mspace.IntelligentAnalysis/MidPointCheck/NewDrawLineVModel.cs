using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.RenderControl;
using Mmc.Framework.Draw;
using Mmc.Framework.Services;
using Mmc.MathUtil;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.IntelligentAnalysisModule.Models;
using Mmc.Mspace.PoiManagerModule.Dto;
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
using System.Collections.ObjectModel;
using System.Drawing;
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
        private updatePointView _updatePointView = null;


        

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

        private bool _startIsDropDownOpen;

        public bool StartIsDropDownOpen
        {
            get { return _startIsDropDownOpen; }
            set { _startIsDropDownOpen = value; OnPropertyChanged("StartIsDropDownOpen"); }
        }
        private bool _endIsDropDownOpen;

        public bool EndIsDropDownOpen
        {
            get { return _endIsDropDownOpen; }
            set { _endIsDropDownOpen = value; OnPropertyChanged("EndIsDropDownOpen"); }
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

        private string _isShowItem="是";

        public string IsShowItem
        {
            get { return _isShowItem; }
            set { _isShowItem = value; OnPropertyChanged("IsShowItem"); }
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
        private List<StakeModel> _stakeModels2 = new List<StakeModel>();
        public List<StakeModel> StakeModels2
        {
            get { return _stakeModels2; }
            set
            {
                _stakeModels2 = value;
                OnPropertyChanged("StakeModels2");
            }
        }

        private List<StakeModel> _selectStakeModels = new List<StakeModel>();
        /// <summary>
        /// 手动中需选择的中线桩
        /// </summary>
        public List<StakeModel> SelectStakeModels
        {
            get { return _selectStakeModels; }
            set
            {
                _selectStakeModels = value;
                OnPropertyChanged("SelectStakeModels");
            }
        }

        private ObservableCollection<TracingLineModel> _tracingLineModels = new ObservableCollection<TracingLineModel>();
        public ObservableCollection<TracingLineModel> TracingLineModels
        {
            get { return _tracingLineModels; }
            set
            {
                _tracingLineModels = value;
                OnPropertyChanged("TracingLineModels");
            }
        }


        private TracingLineModel changeTracingLineModel;

        public TracingLineModel ChangeTracingLineModel
        {
            get { return changeTracingLineModel; }
            set { changeTracingLineModel = value; OnPropertyChanged("ChangeTracingLineModel"); }
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


        private RelayCommand<object> _startSearchCommand;

        public RelayCommand<object> StartSearchCommand
        {

            get { return _startSearchCommand ?? (_startSearchCommand = new RelayCommand<object>(OnSearchCommand)); }
            set { _startSearchCommand = value; }
        }
        private RelayCommand<object> _endSearchCommand;

        public RelayCommand<object> EndSearchCommand
        {

            get { return _endSearchCommand ?? (_endSearchCommand = new RelayCommand<object>(OnEndSearchCommand)); }
            set { _endSearchCommand = value; }
        }

        private RelayCommand<object> _deleteItemCommang;

        public RelayCommand<object> DeleteItemCommang
        {
            get { return _deleteItemCommang??(_deleteItemCommang=new RelayCommand<object>(OnDeleteItemCommang)); }
            set { _deleteItemCommang = value; }
        }


        private RelayCommand _updateCancelCommand;
        public RelayCommand UpdateCancelCommand
        {
            get { return _updateCancelCommand ?? (_updateCancelCommand = new RelayCommand(OnUpdateCancelCommand)); }
            set { _updateCancelCommand = value; }
        }
        private RelayCommand _updateSaveCommand;
        public RelayCommand UpdateSaveCommand
        {
            get { return _updateSaveCommand ?? (_updateCancelCommand = new RelayCommand(OnUpdateSaveCommand)); }
            set { _updateSaveCommand = value; }
        }

        

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

        private RelayCommand<TracingLineModel> _editCommand;

        public RelayCommand<TracingLineModel> EditCommand
        {
            get { return _editCommand ?? (_editCommand = new RelayCommand<TracingLineModel>(OnEditCommand)); }
            set { _editCommand = value; }
        }

        public NewDrawLineVModel()
        {
            this.NewLineCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
               AddLineData();               
            });
            this.GoDrawLine = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                TracingLineModels = new ObservableCollection<TracingLineModel>();
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
                getSelectStakeList();
                //隐藏列表界面 
                if (SelectedItem == "手动")
                {
           

                    if (ChangedItem!=null)
                    {
                    this.gettracinglineList();

                    }
                    else
                    {
                    RegisterDrawLine();

                    }
                }
                else
                {
                  
                    this.getAutomaticStackList();
                }

                if (traceListView==null)
                {
                    traceListView = new TraceListView();
                    traceListView.Owner = Application.Current.MainWindow;
              
                }
                traceListView.DataContext = this;
                traceListView.Left = 50;
                traceListView.Top = Application.Current.MainWindow.Height * 0.55;
             
                traceListView.Show();
            });

            this.getPipeList();
        }

        private void OnSavePointCommand() {
            this.AddLineData();
        }


        private void OnEditCommand(TracingLineModel obj)
        {
            ChangeTracingLineModel = new TracingLineModel();
            if (obj == null) return;

            ChangeTracingLineModel = obj;
            _updatePointView = new updatePointView();
            _updatePointView.Owner = traceListView;
            _updatePointView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _updatePointView.DataContext = this;
            _updatePointView.Show();
        }
        /// <summary>
        /// 清除所有
        /// </summary>
        private void OnDeletePointCommand()
        {
            if (Messages.ShowMessageDialog("提示", "是否要清除所有描点信息？"))
            {
                TracingLineModels = new ObservableCollection<TracingLineModel>();
                this.DelObjs();
            }
        }
        private void OnUpdateCancelCommand()
        {
            _updatePointView.Close();
        }
        private void OnUpdateSaveCommand()
        {
            TracingLineModel tracingLineModel = TracingLineModels.FirstOrDefault(t => t.Id == changeTracingLineModel.Id);
            tracingLineModel = ChangeTracingLineModel;
            polylines = new List<IPolyline>();
            DelObjs();
            DrawAutoLine();
            _updatePointView.Close();
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
                TracingLineModels = new ObservableCollection<TracingLineModel>();
                Geom = "";
                this.ShowParentsWin();
            }
        }
        public void ShowDrawWin()
        {
            if (newDrawLineView != null)
            {
                newDrawLineView.Close();
            }
            newDrawLineView = new NewDrawLineView();
            newDrawLineView.DataContext = this;
            newDrawLineView.Owner = Application.Current.MainWindow;
            HideWin = newDrawLineView.CloseWindow;
            newDrawLineView.Left = 50;
            newDrawLineView.Top = Application.Current.MainWindow.Height * 0.05;
            newDrawLineView.Show();
        }

        private void OnSearchCommand(object obj)
        {
            StakeModels = new List<StakeModel>();
            if (obj == null) return;

            string text = obj.ToString();
            if (string.IsNullOrEmpty(text)) return;
            getStackList(text);
            if (StakeModels.Count > 0)
            {
                StartIsDropDownOpen = true;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="obj"></param>
        private void OnDeleteItemCommang(object obj)
        {
            if (obj == null) return;
            TracingLineModel tracingLineModel = obj as TracingLineModel;
            if (Messages.ShowMessageDialog("提示", "是否删除当前描点？"))
            {
                //删除数据
                if(points!=null&& points.Count()>0)
                {
                    List<string> list = points.ToList();
                    list.RemoveAt(tracingLineModel.Index);
                }
              
                TracingLineModels.Remove(tracingLineModel);
                //清楚地图重新画
                polylines = new List<IPolyline>();
                DelObjs();
                DrawAutoLine();
            }
        }

        private void OnEndSearchCommand(object obj)
        {
            StakeModels2 = new List<StakeModel>();
            if (obj == null) return;

            string text = obj.ToString();
            if (string.IsNullOrEmpty(text)) return;
            getStackList2(text);
            if (StakeModels2.Count > 0)
            {
                EndIsDropDownOpen = true;
            }
        }
        /// <summary>
        /// 获取描点列表
        /// </summary>
        private void gettracingline()
        {
            Task.Run(() =>
            {
                TracingLineModels = new ObservableCollection<TracingLineModel>();
                string param = "?page=0&page_size=100&start=" + StartPoi.Id + "&end=" + EndPoi.Id;
                string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.tracinglineList + param);
                this.TracingLineModels = (JsonUtil.DeserializeFromString<ObservableCollection<TracingLineModel>>(resStr));
            });
        }
        private LineItem ChangedItem = null;
        public void ChangedData(LineItem lineItem)
        {
            ChangedItem = lineItem;
            this.SelectPipeModel = this.PipeModels.FirstOrDefault(t => t.Id == lineItem.pipe_id);
            this.Sn = lineItem.sn;
            this.PipeName = lineItem.name;
            this.SelectedItem = lineItem.type_id;
            getStackList(lineItem.start_sn);
            getStackList2(lineItem.end_sn);
            StartPoi = this.StakeModels.FirstOrDefault(t => t.Sn == lineItem.start_sn); 
            EndPoi = this.StakeModels2.FirstOrDefault(t => t.Sn == lineItem.end_sn);

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
        /// 自动获取中线桩
        /// </summary>
        private void getAutomaticStackList()
        {
            this.TracingLineModels = new ObservableCollection<TracingLineModel>();
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.stakeindex + "?limits=" + StartPoi.Id+","+EndPoi.Id);
            this.TracingLineModels = (JsonUtil.DeserializeFromString<ObservableCollection<TracingLineModel>>(resStr));
            this.DrawAutoLine();
        }

        /// <summary>
        /// 手动获取中线桩
        /// </summary>
        private void gettracinglineList()
        {
            this.TracingLineModels = new ObservableCollection<TracingLineModel>();
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.tracinglineList + "?start=" + StartPoi.Id + "&end=" + EndPoi.Id);
            this.TracingLineModels = (JsonUtil.DeserializeFromString<ObservableCollection<TracingLineModel>>(resStr));
            DrawAutoLine();
        }
        /// <summary>
        /// 手动获取中线桩
        /// </summary>
        private void getSelectStakeList()
        {
            this.SelectStakeModels = new List<StakeModel>();
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.stakeindex + "?limits=" + StartPoi.Id + "," + EndPoi.Id);
            this.SelectStakeModels = (JsonUtil.DeserializeFromString<List<StakeModel>>(resStr));


        }
        List<IPolyline> polylines = new List<IPolyline>();
        ObservableCollection<IPoint> _problemPoints = new ObservableCollection<IPoint>();
        private void DrawAutoLine()
        {
            string header = "linestring z (";
            string end = ")";
            string line = "";

            for (int i = 0; i < TracingLineModels.Count; i++)
            {
                if (i > this.SelectStakeModels.Count-1) return;
                TracingLineModels[i].Poi = this.SelectStakeModels[i];//默认赋值
                if (i== TracingLineModels.Count-1)
                {
                    line += (TracingLineModels[i].Lng + " " + TracingLineModels[i].Lat + " " + (string.IsNullOrEmpty(TracingLineModels[i].Height) ? "0" : TracingLineModels[i].Height));
                }
                else
                {
                    line += (TracingLineModels[i].Lng + " " + TracingLineModels[i].Lat + " " + (string.IsNullOrEmpty(TracingLineModels[i].Height) ? "0" : TracingLineModels[i].Height) + ",");

                }
            }
             Geom = header + line + end;
     
            var polyLine = GviMap.GeoFactory.CreatePolyline(Geom, GviMap.SpatialCrs);
            CurveSymbol curveSymbol = new CurveSymbol();
            curveSymbol.Color = ColorConvert.Argb(100, 238, 103, 35);//GviMap.LinePolyManager.CurveSym
            curveSymbol.Width = 20f;
            var rLine = GviMap.ObjectManager.CreateRenderPolyline(polyLine, curveSymbol, GviMap.ProjectTree.RootID);

            rLine.VisibleMask = gviViewportMask.gviViewAllNormalView;
            //GviMap.Camera.FlyToObject(rLine.Guid, gviActionCode.gviActionFlyTo);
            GviMap.Camera.GetCamera2(out IPoint pointCamera, out IEulerAngle eulerAngle);
            ////GviMap.Camera.FlyToEnvelope(point.Envelope);
            eulerAngle.Tilt = -45;
            eulerAngle.Heading = 210;
            pointCamera.X = rLine.Envelope.MaxX;
            pointCamera.Y = rLine.Envelope.MaxY;
            pointCamera.Z = 2000;
            GviMap.Camera.SetCamera2(pointCamera, eulerAngle, 0);
            //var polyLine = GviMap.GeoFactory.CreatePolyline(Geom, GviMap.SpatialCrs);
            //ICurveSymbol CurveSym = null;
            //CurveSym = GviMap.LinePolyManager.CurveSym;
            //var rLine = GviMap.ObjectManager.CreateRenderPolyline(polyLine, CurveSym);
            //rLine.VisibleMask = gviViewportMask.gviViewAllNormalView;


            //guids.Add(rLine.Guid);
            //GviMap.Camera.FlyToObject(rLine.Guid, gviActionCode.gviActionFlyTo);
            //var poly0 = GviMap.GeoFactory.CreateFromWKT(Geom) as IPolyline;

            if (polyLine != null)
            {
                polylines.Add(polyLine);
            }
            SetVideo();

        }

        public Dictionary<string, Guid> poiList = new Dictionary<string, Guid>();

        private void SetVideo()
        {
            if (polylines.Count > 0)
            {
                for (int i = 0; i < polylines[0].PointCount; i++)
                {
                    var point = polylines[0].GetPoint(i);

                    var poi = GviMap.GeoFactory.CreateGeometry(gviGeometryType.gviGeometryPOI, gviVertexAttribute.gviVertexAttributeZ) as IPOI;

                    poi.SetPostion(point.X, point.Y);
                    poi.Size = 50;
                    poi.ShowName = false;
                    poi.ImageName = string.Format("项目数据\\shp\\IMG_POI\\{0}.png", "中线桩");
                    poi.SpatialCRS = GviMap.SpatialCrs;
                    var rPoi = GviMap.ObjectManager.CreateRenderPOI(poi);
                    rPoi.DepthTestMode = gviDepthTestMode.gviDepthTestAlways;
                    this.poiList.Add(TracingLineModels[i].Id, rPoi.Guid);
                }
            }
        }
        /// <summary>
        /// 获取中
        /// </summary>
        private void getStackList(string sn)
        {
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.stakeindex + "?sn=" + sn);
            this.StakeModels = (JsonUtil.DeserializeFromString<List<StakeModel>>(resStr));
        }
        /// <summary>
        /// 获取中
        /// </summary>
        private void getStackList2(string sn)
        {
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.stakeindex + "?sn=" + sn);
            this.StakeModels2 = (JsonUtil.DeserializeFromString<List<StakeModel>>(resStr));
        }
        public void getPipeList()
        {
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.PipeList);
            this.PipeModels = (JsonUtil.DeserializeFromString<List<PipeModel>>(resStr));
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

            if(ChangedItem!=null)
            {
                url = PipelineInterface.tracingupdate+"?id="+ ChangedItem.id;
            }

            string resStr = HttpServiceHelper.Instance.PostRequestForData(url, jsonData);
            if (resStr == "")
            {
                Messages.ShowMessage("新增失败，请检查数据是否存在！");
                return;
            }
            var list = JsonUtil.DeserializeFromString<dynamic>(resStr);
            string id = list["id"];

            if(string.IsNullOrEmpty(id))
            {
                Messages.ShowMessage("新增失败，请检查数据");
                return;
            }
            AddPipe(lineItem);
            List<TracingModel> tracingModels = new List<TracingModel>();
            for (int i = 0; i < TracingLineModels.Count; i++)
            {
                TracingModel tracingModel = new TracingModel();
                tracingModel.sn = TracingLineModels[i].Sn;
                tracingModel.lng = TracingLineModels[i].Lng;
                tracingModel.lat = TracingLineModels[i].Lat;
                tracingModel.height = TracingLineModels[i].Height;
                tracingModel.stake = TracingLineModels[i].Poi.Id;
                TracingLineModels[i].Stake = TracingLineModels[i].Poi.Id;
                TracingLineModels[i].Traces = TracingLineModels[i].Poi.Id;
                
                tracingModels.Add(tracingModel);
            }
            string resStr1 = HttpServiceHelper.Instance.PostRequestForData(PipelineInterface.tracinglinebatch, JsonUtil.SerializeToString(tracingModels));
            if (resStr1 == "")
            {
                Messages.ShowMessage("新增描点失败，请检查中线桩描点信息！");
                return;
            }
            Messages.ShowMessage("新增成功");
            AddPipe(lineItem);
            newDrawLineView.Hide();
            traceListView.Hide();
            ChangedItem = null;
            DelObjs();
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

        private string[] points = null;
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
                points = Ru.Split(',');
                ObservableCollection<TracingLineModel> list = new ObservableCollection<TracingLineModel>();
                for (int i = 0; i < points.Count(); i++)
                {
                    string[] xyz =  points[i].Split(' ');
                    //int index = (Convert.ToInt32(StartPoi.Sn.Substring(2, StartPoi.Sn.Length - 2) ) + i);
                    TracingLineModel tracingLineModel = new TracingLineModel()
                    {
                        Poi = SelectStakeModels[i],
                        Sn = SelectStakeModels[i].Sn,
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
        public void ClearPatrolList()
        {
            Dictionary<string, Guid> expr_07 = this.poiList;
            bool flag = expr_07 == null || expr_07.Count > 0;
            if (flag)
            {
                foreach (KeyValuePair<string, Guid> current in this.poiList)
                {
                    GviMap.ObjectManager.DeleteObject(current.Value);
                }
            }
            this.poiList = new Dictionary<string, Guid>();
        }
        private void DelObjs()
        {
            ClearPatrolList();
            foreach (var item in guids)
            {
                GviMap.ObjectManager.DeleteObject(item);
            }
            guids.Clear();
       
        }


    }
}
