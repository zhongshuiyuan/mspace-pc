using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.RenderControl;
using Mmc.Framework.Draw;
using Mmc.Framework.Services;
using Mmc.MathUtil;
using Mmc.Mspace.Common.Messenger;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Common.Models.pipelines;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.Const.ConstPath;
using Mmc.Mspace.IntelligentAnalysisModule.MidPointCheck;
using Mmc.Mspace.IntelligentAnalysisModule.Models;
using Mmc.Mspace.PoiManagerModule.Models;
using Mmc.Mspace.Services.HttpService;
using Mmc.Mspace.Theme.Pop;
using Mmc.Windows.Services;
using Mmc.Windows.Utils;
using Mmc.Wpf.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Mmc.Mspace.IntelligentAnalysisModule.AreaWidth
{
     public class AreaWidthVModel: CheckedToolItemModel
     {
        public Action CancelWin;
        List<IPolyline> polylines = new List<IPolyline>();
        ObservableCollection<IPoint> _problemPoints = new ObservableCollection<IPoint>();
        List<Guid> guids = new List<Guid>();
        public List<LineItem> lineItems = new List<LineItem>();
        private bool isSuccessReport = false;



        //当从阶段飞过来看数据时  只看

        private bool _showType=false;

        public bool ShowType
        {
            get { return _showType; }
            set
            {
                _showType = value;
                NotifyPropertyChanged("ShowType");
            }
        }

        public ObservableCollection<IPoint> ProblemPoints
        {
            get { return _problemPoints; }
            set
            {
                base.SetAndNotifyPropertyChanged<ObservableCollection<IPoint>>(ref this._problemPoints, value, "ProblemPoints");
            }
        }
        public AreaWidthVModel()
        {
            BindingStatus = "";
            isSuccessReport = false;
            TaskSelectItem = null;
            SelectPeriodModel = null;
            SelectPipeModel = null;
            SelectSectionModel = null;
            this.getTaskAll();
        }

        private string _titleText="边界宽度预警";

        public string TitleText
        {
            get { return _titleText; }
            set { _titleText = value; base.SetAndNotifyPropertyChanged<string>(ref this._titleText, value, "TitleText"); }
        }

        private string _radius = "30";
        public string Radius
        {
            get { return _radius; }
            set
            {
                base.SetAndNotifyPropertyChanged<string>(ref this._radius, value, "Radius");
            }
        }
        ICurveSymbol curveSymbol;
        //  List<IPoint> ProblemPoints2 = new List<IPoint>();
        private DrawCustomerUC drawCustomer;
       // private DrawCustomerUC editCustomerWidth = null;
        AreaWidthView areaWidthView = new AreaWidthView();
        public ICommand DrawSide { get; set; }
        public ICommand ClearDraw { get; set; }
        public ICommand Calculate { get; set; }
        public ICommand CloseCmd { get; set; }


        private RelayCommand _saveCmd;
        public RelayCommand SaveCmd
        {
            get { return _saveCmd ?? (_saveCmd = new RelayCommand(OnSaveCmd)); }
            set { _saveCmd = value; }
        }
        private RelayCommand _boundTaskCommand;
        public RelayCommand BoundTaskCommand
        {
            get { return _boundTaskCommand ?? (_boundTaskCommand = new RelayCommand(OnBoundTaskCommand)); }
            set { _boundTaskCommand = value; }
        }
        
        public override void Initialize()
        {
            base.Initialize();
            base.ViewType = ViewType.CheckedIcon;
            areaWidthView.DataContext = this;
            this.DrawSide = new RelayCommand(RegisterDrawLine);
            this.ClearDraw = new RelayCommand(ClearList);
            this.Calculate = new RelayCommand(Ca);
            this.CloseCmd = new RelayCommand(Hide);
            //Messenger.Messengers.Register("AreaWidth", (bool b) =>
            //{
            //    if (b)
            //    {
            //        Messenger.Messengers.Notify("ShowHiddenMenu", true);
            //        //MineListView?.Show();
            //        OnChecked();
            //    }
            //    else
            //    {
            //        Messenger.Messengers.Notify("ShowHiddenMenu", false);
            //        // MineListView?.Hide();
            //        OnUnchecked();
            //    }
            //});
        }

        private ObservableCollection<PipeModel> _pipeModels = new ObservableCollection<PipeModel>();
        public ObservableCollection<PipeModel> PipeModels
        {
            get { return _pipeModels; }
            set
            {
                _pipeModels = value;
                NotifyPropertyChanged("PipeModels");
            }
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
                NotifyPropertyChanged("SelectPipeModel");
                if (SelectPipeModel != null)
                    getSectionList();
            }
        }
        private ObservableCollection<PeriodModel> _periods = new ObservableCollection<PeriodModel>();
        /// <summary>
        /// 阶段
        /// </summary>
        public ObservableCollection<PeriodModel> Periods
        {
            get { return _periods; }
            set { _periods = value; NotifyPropertyChanged("Periods"); }
        }
        private PeriodModel _selectPeriodModel;
        /// <summary>
        /// 阶段选中
        /// </summary>
        public PeriodModel SelectPeriodModel
        {
            get { return _selectPeriodModel; }
            set
            {
                _selectPeriodModel = value;
                NotifyPropertyChanged("SelectPeriodModel");
            }
        }

        private ObservableCollection<SectionModel> _sections = new ObservableCollection<SectionModel>();
        /// <summary>
        /// 标段
        /// </summary>
        public ObservableCollection<SectionModel> Sections
        {
            get { return _sections; }
            set { _sections = value; NotifyPropertyChanged("Sections"); }
        }

        private SectionModel _selectSectionModel;
        /// <summary>
        /// 标段选中
        /// </summary>
        public SectionModel SelectSectionModel
        {
            get { return _selectSectionModel; }
            set
            {
                _selectSectionModel = value;
                NotifyPropertyChanged("SelectSectionModel");
            }
        }
        private string _bindingStatus;

        public string BindingStatus
        {
            get { return _bindingStatus; }
            set { _bindingStatus = value;
                NotifyPropertyChanged("BindingStatus");
            }
        }


        private ObservableCollection<TracingLineModel> _tracingLineModels = new ObservableCollection<TracingLineModel>();
        public ObservableCollection<TracingLineModel> TracingLineModels
        {
            get { return _tracingLineModels; }
            set
            {
                _tracingLineModels = value;
                NotifyPropertyChanged("TracingLineModels");
            }
        }
        private ObservableCollection<TaskModel> _taskAll = new ObservableCollection<TaskModel>();
        public ObservableCollection<TaskModel> TaskAll
        {
            get { return _taskAll; }
            set
            {
                _taskAll = value;
                NotifyPropertyChanged("TaskAll");
            }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private TaskModel _TaskSelectItem;
        public TaskModel TaskSelectItem
        {
            get { return _TaskSelectItem; }
            set { _TaskSelectItem = value;
                base.SetAndNotifyPropertyChanged<TaskModel>(ref this._TaskSelectItem, value, "TaskSelectItem");
            }
        }

        public string _currentFileName = null;
        public string _currentFilePath = null;
        private string NavigationImgPath = System.Windows.Forms.Application.LocalUserAppDataPath + "\\NavigationImage\\";
        private void OnSaveCmd()
        {
            if (!isCal) {
                Messages.ShowMessage("暂无计算数据，请先计算后再尝试导出！");
                return;
            }
            List<TracingLineModel> list = new List<TracingLineModel>();
            for (int i = 0; i < lineItems.Count; i++)
            {
                gettracinglineList(lineItems[i].id);
                list.AddRange(TracingLineModels);
            }
           
            List<TracingModel> tracingModels = new List<TracingModel>();
            for (int i = 0; i < _problemPoints.Count; i++)
            {
                TracingModel tracingModel = new TracingModel();
                var res = list.Where(t => t.Lng == _problemPoints[i].X.ToString() && t.Lat == _problemPoints[i].Y.ToString()).ToList();
               
                tracingModel.sn = lineItems[0].IsRoot? lineItems[1].sn: lineItems[0].sn;
                tracingModel.lng = _problemPoints[i].X.ToString();
                tracingModel.lat = _problemPoints[i].Y.ToString();
                tracingModel.start = lineItems[0].IsRoot ? lineItems[1].start_sn : lineItems[0].start_sn;
                tracingModel.end = lineItems[0].IsRoot ? lineItems[1].end_sn : lineItems[0].end_sn;
                tracingModels.Add(tracingModel);
            }
            //生成图片
            string imgName = GetTimeStamp() + ".png";
            string NavigationImgCompletePath = NavigationImgPath + imgName;
            bool b = GviMap.MapControl.ExportManager.ExportImage(NavigationImgCompletePath, 1280, 1280, true);
         
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = FileFilterStrings.FDB;
            saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.Threading.Thread.Sleep(2000);
                //上传图片 
                string updatestake = string.Format("{0}", PipelineInterface.taskupload);
                string result = HttpServiceHelper.Instance.PostImageFile(updatestake, NavigationImgCompletePath);
                List<string> imgs = new List<string>();
                imgs.Add(result.Substring(1, result.Length - 2));

               var item = new
                {
                    list = JsonUtil.SerializeToString(tracingModels),
                    images = JsonUtil.SerializeToString(imgs),
                };
                _currentFileName = saveFileDialog.FileName;
                string filepath= saveFileDialog.FileName.Substring(0, saveFileDialog.FileName.LastIndexOf('\\') + 1);
                string filename = saveFileDialog.FileName.Substring(saveFileDialog.FileName.LastIndexOf('\\') + 1);

                System.Threading.Tasks.Task.Run(() =>
                {
                    string path = HttpServiceHelper.Instance.PostRequestForString(PipelineInterface.tracingexport, JsonUtil.SerializeToString(item), filepath, filename, DownloadResult);
                });
              
            }
        }

        private void OnBoundTaskCommand()
        {
            if(!isSuccessReport)
            {
                Messages.ShowMessage("暂未生成报告文档，请先生成后再尝试关联任务！");
                return;
            }
            if (string.IsNullOrEmpty(Name))
            {
                Messages.ShowMessage("请输入名称！");
                return;
            }
            if (SelectPipeModel == null)
            {
                Messages.ShowMessage("请选择对应的管线后重试！");
                return;
            }
            if (SelectSectionModel == null)
            {
                Messages.ShowMessage("请选择对应的标段后重试！");
                return;
            }
            if (SelectPeriodModel == null)
            {
                Messages.ShowMessage("请选择对应的阶段后重试！");
                return;
            }
            if (TaskSelectItem==null)
            {
                Messages.ShowMessage("请选择对应的任务后重试！");
                return;
            }
            var data = new
            {
                task_id = TaskSelectItem.Id,
                name = Name,
                pipe_id = SelectPipeModel.Id,
                section_id = SelectSectionModel.Id,
                period_id = SelectPeriodModel.Id,
                file = _currentFilePath,
                stake_start = lineItems[0].IsRoot ? lineItems[1].start_sn : lineItems[0].start_sn,
                stake_end = lineItems[0].IsRoot ? lineItems[1].end_sn : lineItems[0].end_sn,
                width = Radius,
                lng = "",
                lat = "",
                trace_id= lineItems[0].IsRoot ? lineItems[1].id : lineItems[0].id,
                master_trace = lineItems[0].IsRoot ? lineItems[0].id : lineItems[1].id,
            };
            var jsonData = JsonUtil.SerializeToString(data);

            string resStr = HttpServiceHelper.Instance.PostRequestForData(PipelineInterface.flycreate, jsonData);

            if(string.IsNullOrEmpty(resStr))
            {
                Messages.ShowMessage("关联失败，请稍后重新尝试关联！");
            }
            BindingStatus = "已绑定";
            Messages.ShowMessage("关联成功！");
        }
        //public bool WordToPDF(string sourcePath, string targetPath)
        //{
        //    bool result = false;
        //    Microsoft.Office.Interop.Word.Application application = new Microsoft.Office.Interop.Word.Application();
        //    Document document = null;
        //    try
        //    {
        //        application.Visible = false;
        //        document = application.Documents.Open(sourcePath);
        //        document.ExportAsFixedFormat(targetPath, WdExportFormat.wdExportFormatPDF);
        //        result = true;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        result = false;
        //    }
        //    finally
        //    {
        //        document.Close();
        //    }
        //    return result;
        //}
        /// <summary>
        /// 手动获取中线桩
        /// </summary>
        private void gettracinglineList(string id)
        {
         
            this.TracingLineModels = new ObservableCollection<TracingLineModel>();
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.tracinglineList + "?traces=" + id);
            this.TracingLineModels = (JsonUtil.DeserializeFromString<ObservableCollection<TracingLineModel>>(resStr));
        }
        private void getTaskAll()
        {
            Task.Run(() =>
            {
                getPipeList2();
                this.TaskAll = new ObservableCollection<TaskModel>();
                string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.taskall);
                this.TaskAll = (JsonUtil.DeserializeFromString<ObservableCollection<TaskModel>>(resStr));
            });
  
        }
        public void getPipeList2()
        {
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.PipeList);
            this.PipeModels = new ObservableCollection<PipeModel>((JsonUtil.DeserializeFromString<List<PipeModel>>(resStr)));
        }
        private void getSectionList()
        {
            Task.Run(() =>
            {
                string sectionList = HttpServiceHelper.Instance.GetRequest(PipelineInterface.SectionList);
                ObservableCollection<SectionModel> list = new ObservableCollection<SectionModel>(JsonUtil.DeserializeFromString<List<SectionModel>>(sectionList));

                SectionModel se = SelectSectionModel;
                this.Sections = new ObservableCollection<SectionModel>((list.Where(t => t.Pipe_id == SelectPipeModel.Id).ToList()));
                if (se != null)
                {
                    SelectSectionModel = this.Sections.SingleOrDefault(t => t.Id == se.Id);
                }

                //获取阶段
                string periodList = HttpServiceHelper.Instance.GetRequest(PipelineInterface.PeriodList);

                this.Periods = new ObservableCollection<PeriodModel>(JsonUtil.DeserializeFromString<List<PeriodModel>>(periodList));

            });
        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
        public void DownloadResult(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                _currentFilePath = result;
                if (File.Exists(_currentFileName))
                {
                    Messages.ShowMessage("导出成功！");
                    isSuccessReport = true;
                    //WordToPDF(_currentFileName,_currentFileName);
                    System.Diagnostics.Process.Start(_currentFileName);
                   
                }
                //Messages.ShowMessage(Helpers.ResourceHelper.FindKey("SAVESUCCESSED"));
            }
            else
            {
                Messages.ShowMessage(Helpers.ResourceHelper.FindKey("SAVEFAILED"));
            }
        }

        //public void SaveFileDialog()
        //{
        //    string localFilePath, filepath;
        //    SaveFileDialog fileDialog = new SaveFileDialog();
        //    fileDialog.Filter = " Save to Result Files(*.)|*.*";//*.kml|All files(*.*)|
        //    fileDialog.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".kml";
        //    fileDialog.FilterIndex = 2;
        //    fileDialog.AddExtension = true;
        //    fileDialog.RestoreDirectory = true;
        //    if (fileDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        localFilePath = fileDialog.FileName.ToString();
        //        _fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);
        //        filepath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));
        //        this.SaveToFile(localFilePath);
        //    }
        //}

        public override void OnChecked()
        {
            base.OnChecked();
            areaWidthView.Left = 250;
            areaWidthView.Top = 220;
            areaWidthView.Show();
        }

        public override void OnUnchecked()//关闭事件
        {
            isCal = false;
            base.OnUnchecked();
            areaWidthView.Hide();
            _currentFileName = null;
            _currentFilePath = null;
        }

        public bool isCal = false;
        private void Ca()
        {
            ClearList();
            setData();
            if(lineItems[0].IsRoot)
            {
                Cal2();
            }
            else
            {
                Cal();
            }
            Messages.ShowMessage("计算完成，问题点数目为:"+ Convert.ToString(_problemPoints.Count));
            isCal = true;
        }
        public void Hide()
        {
            ClearList();
            areaWidthView.Hide();
            CancelWin();
            base.IsChecked = false;
        }
        private void RegisterDraw()
        {
            try
            {
                if (drawCustomer == null)
                {
                    drawCustomer = new DrawCustomerUC(Helpers.ResourceHelper.FindKey("AreaPolygonWidthMarkerKey"),
                        DrawCustomerType.MenuCommand);
                    //注册绘制多边形事件
                }
                //CreateTempRObjdrawCustomer
                RCDrawManager.Instance.PolygonDraw.Register(GviMap.AxMapControl, drawCustomer, RCMouseOperType.PickPoint);
                RCDrawManager.Instance.PolygonDraw.OnDrawFinished -= Rone_PolygonDraw_OnDrawFinished;
                RCDrawManager.Instance.PolygonDraw.OnDrawFinished += Rone_PolygonDraw_OnDrawFinished;

            }
            catch (Exception e)
            {
                SystemLog.Log(e);
            }
        }
        private void Rone_PolygonDraw_OnDrawFinished(object sender, object result)
        {
            var polygon = GviMap.GeoFactory.CreateGeometry(gviGeometryType.gviGeometryPolygon,
                         gviVertexAttribute.gviVertexAttributeZ) as IPolygon;
            polygon.SpatialCRS = GviMap.SpatialCrs;
            var render = GviMap.ObjectManager.CreateRenderPolygon(polygon, GviMap.LinePolyManager.SurfaceSym,
                GviMap.ProjectTree.RootID);
            try
            {
                //if (!AreaSelectFlag)
                //{
                //    return;
                //}
                var rPolygon = result as IRenderPolygon;
                polygon = rPolygon.GetFdeGeometry() as IPolygon;
                polygon.SpatialCRS = GviMap.SpatialCrs;

                if (polygon == null || polygon.ExteriorRing.PointCount < 4)
                {
                    RCDrawManager.Instance.PolygonDraw.OnDrawFinished -= Rone_PolygonDraw_OnDrawFinished;
                    RCDrawManager.Instance.PolygonDraw.UnRegister(GviMap.AxMapControl);
                    return;
                }
                try
                {
                    var rpolygon1 = render;// GviMap.TempRObjectPool[AreaMarkerKey] as IRenderPolygon;
                    rpolygon1?.SetFdeGeometry(polygon);
                    rPolygon.Symbol.BoundarySymbol.Color = Color.FromArgb(255, 0, 255, 255);
                    rpolygon1.VisibleMask = gviViewportMask.gviViewAllNormalView;
                    // GviMap.TempRObjectPool[AreaMarkerKey] = rpolygon1;                    
                    polygon = rpolygon1.GetFdeGeometry() as IPolygon;
                    if (rpolygon1 != null)
                    {
                        //rPolygonList.Add(rpolygon1);
                    }
                    polygon.SpatialCRS = GviMap.SpatialCrs;
                    var json = JsonUtil.DeserializeFromFile<dynamic>(AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
                    string _poiHost = json.poiUrl;
                    string wkt_poly = polygon.AsWKT();
                    //  _geom = wkt_poly;
                    // wktList.Add(_geom);
                    //   ChangeListCountNum(wktList);
                    // AreaIsSelected = true;
                    AreaPoiSelectedModel areaPoiSelected = new AreaPoiSelectedModel();
                    areaPoiSelected.AreaSelectedPolygon = rpolygon1;
                    areaPoiSelected.WktPoly = wkt_poly;
                    // WktPoly = wkt_poly;
                    // AreaPoiDic.Clear();
                    // AreaPoiDic.Add(AreaIsSelected, areaPoiSelected);
                    RCDrawManager.Instance.PolygonDraw.OnDrawFinished -= Rone_PolygonDraw_OnDrawFinished;
                    RCDrawManager.Instance.PolygonDraw.UnRegister(GviMap.AxMapControl);
                    //AreaSelectFlag = false;
                }
                catch (Exception e)
                {
                    SystemLog.Log(e);
                }
            }
            catch (Exception e)
            {
                SystemLog.Log(e);
            }
        }
        private void RegisterDrawLine()
        {
            if(polylines.Count<2)
            {
                try
                {
                    if (drawCustomer == null)
                    {
                        drawCustomer = new DrawCustomerUC(Helpers.ResourceHelper.FindKey("AreaWidthMarkerKey"), DrawCustomerType.MenuCommand);
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
          else
            {
                Messages.ShowMessage("请先清除上次规划结果");
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
            guids.Add(renderPolyline.Guid);
            // var rpolygon1 = GviMap.TempRObjectPool["plan"] as IRenderPolyline;
            // rpolygon1?.SetFdeGeometry(polyLine);
            //ILabel label = GviMap.ObjectManager.CreateLabel();
            //label.Position = polyLine.Midpoint;
            // label.Text = "就是线段呀";
            // SetColor(rpolygon1.Symbol.Color);
            //  this.LineWidth = (-rpolygon1.Symbol.Width).ToString();
            RCDrawManager.Instance.PolylineDraw.OnDrawFinished -= PlanPolylineDraw_OnDrawFinished;
            RCDrawManager.Instance.PolylineDraw.UnRegister(GviMap.AxMapControl);
            //this.SelectedHeightType = HeightTypes.FirstOrDefault(p => p.HeightStyle == rpolygon1.HeightStyle);
            //this.Lng = label.Position.X;
            //this.Lat = label.Position.Y;
            //this.Alt = label.Position.Z;
            var pt = polyLine.GetPoint(0);
            var prjWkt = Wgs84UtmUtil.GetWkt(pt.X);
            if (!string.IsNullOrEmpty(prjWkt))
                polyLine.ProjectEx(prjWkt);
            // this.Len = polyLine.Length;
            polyLine.Project(GviMap.SpatialCrs);
          //  label.VisibleMask = gviViewportMask.gviViewAllNormalView;
            renderPolyline.VisibleMask = gviViewportMask.gviViewAllNormalView;
            polylines.Add(polyLine);          
        }
        //protected void SetColor(Color c)
        //{
        //    var view = this.View as PathPlanView;
        //    view.ColorPicker.SelectedColor = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        //}
        private void ClearList()
        {

            polylines.Clear();
            _problemPoints.Clear();
            delObjs();
            isCal = false;
           // ProblemPoints2.Clear();
        }
        private void delObjs()
        {
            ClearPatrolList();
            foreach (var item in guids)
            {
                GviMap.ObjectManager.DeleteObject(item);
            }
            guids.Clear();
        }
        private void setData()
        {
            polylines = new List<IPolyline>();
            var poly0 = GviMap.GeoFactory.CreateFromWKT(lineItems[0].geom) as IPolyline;
            var poly1 = GviMap.GeoFactory.CreateFromWKT(lineItems[1].geom) as IPolyline;
            if (poly0 != null && poly1 != null)
            {
                polylines.Add(poly0);
                polylines.Add(poly1);
            }
            else
            {
                Messages.ShowMessage("线路中缺少必要的地理信息，无法计算");
                polylines.Clear();
                delObjs();
                areaWidthView.Hide();
            }
            DrawLineCommand();
        }
        private void Cal()
        {
            if(polylines.Count==2)
            {
                //ITopologicalOperator3D topologicalOperator3D =polylines[0]
                IGeometry geo_1 = Buffer(polylines[1], Convert.ToDouble(_radius) / 100000);
                geo_1.SpatialCRS = GviMap.SpatialCrs;
        
                IRenderPolygon render = GviMap.ObjectManager.CreateRenderPolygon(geo_1 as IPolygon, GviMap.LinePolyManager.SurfaceSym, GviMap.ProjectTree.RootID);
           
                render?.SetFdeGeometry(geo_1);
                render.VisibleMask = gviViewportMask.gviViewAllNormalView;
                render.Symbol.BoundarySymbol.Color = Color.FromArgb(255, 0, 255, 255);
                geo_1 = render.GetFdeGeometry() as IPolygon;//IMultiPolygon;
                geo_1.SpatialCRS = GviMap.SpatialCrs;
                guids.Add(render.Guid);
               // GviMap.Camera.FlyToEnvelope(render.Envelope);
            
                for(int i=0;i<polylines[0].PointCount;i++)
                {
                    var point = polylines[0].GetPoint(i);
                    var topoPoi = point as ITopologicalOperator2D;
                    if (topoPoi.Intersection2D(geo_1) == null)
                    {
                    _problemPoints.Add(point);
                    CreatRenPoi(point);
                    }
                }
            }
            else
            {
                Messages.ShowMessage("边界不足两条，请检查");
            }
        }

        private void DrawLineCommand()
        {
            for (int i = 0; i < lineItems.Count; i++)
            {
                var polyLine = GviMap.GeoFactory.CreatePolyline(lineItems[i].geom, GviMap.SpatialCrs);
                if (polyLine == null) return;

                if (polyLine.EndPoint == null) return;
                CurveSymbol curveSymbol = new CurveSymbol();
                curveSymbol.Color = ColorConvert.Argb(100, 238, 103, 35);
                curveSymbol.Width = -10;
                var rLine = GviMap.ObjectManager.CreateRenderPolyline(polyLine, curveSymbol, GviMap.ProjectTree.RootID);

                if (rLine == null) return;
                guids.Add(rLine.Guid);
                rLine.VisibleMask = gviViewportMask.gviViewAllNormalView;
                rLine.MinVisibleDistance = 1.0;
                rLine.MaxVisibleDistance = 10000.0;
                //GviMap.Camera.FlyToObject(rLine.Guid, gviActionCode.gviActionFlyTo);
                //var poly0 = GviMap.GeoFactory.CreateFromWKT(lineItem.geom) as IPolyline;

                ////GviMap.Camera.FlyToEnvelope(point.Envelope);
                GviMap.Camera.GetCamera2(out IPoint pointCamera, out IEulerAngle eulerAngle);
                eulerAngle.Tilt = -60;
                eulerAngle.Heading = 220;
                pointCamera.X = rLine.Envelope.MaxX;
                pointCamera.Y = rLine.Envelope.MaxY;
                pointCamera.Z = 1100;
                GviMap.Camera.SetCamera2(pointCamera, eulerAngle, 0);

                gettracinglineList(lineItems[i].id);
                SetVideo();
            }
        }
   
        public Dictionary<string, Guid> poiList = new Dictionary<string, Guid>();
        private void SetVideo()
        {
            if (TracingLineModels.Count > 0)
            {
                for (int i = 0; i < TracingLineModels.Count; i++)
                {
                    var point = TracingLineModels[i];

                    var poi = GviMap.GeoFactory.CreateGeometry(gviGeometryType.gviGeometryPOI, gviVertexAttribute.gviVertexAttributeZ) as IPOI;
                    poi.SetPostion(Convert.ToDouble(point.Lng), Convert.ToDouble(point.Lat), string.IsNullOrEmpty(point.Height) ? 0 : Convert.ToDouble(point.Height));
                    poi.Size = 30;
                    poi.ShowName = true;
                    poi.Name = string.IsNullOrEmpty(point.Stake_sn)? point.Sn: point.Stake_sn;
                    poi.MaxVisibleDistance = 10000.0;
                    poi.MinVisibleDistance = 1.0;
                    poi.ImageName = string.Format(AppDomain.CurrentDomain.BaseDirectory + "项目数据\\shp\\IMG_POI\\{0}.png", "stake");
                    poi.SpatialCRS = GviMap.SpatialCrs;
                    var rPoi = GviMap.ObjectManager.CreateRenderPOI(poi);
                    rPoi.DepthTestMode = gviDepthTestMode.gviDepthTestAlways;
                    this.poiList.Add(rPoi.Guid.ToString(), rPoi.Guid);
                }
            }
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
        private void Cal2()
        {
            if (polylines.Count == 2)
            {
                IGeometry geo_1 = Buffer(polylines[0], (Convert.ToDouble(_radius)) /100000);
                geo_1.SpatialCRS = GviMap.SpatialCrs;
                IRenderPolygon render = GviMap.ObjectManager.CreateRenderPolygon(geo_1 as IPolygon, GviMap.LinePolyManager.SurfaceSym, GviMap.ProjectTree.RootID);
                
                render?.SetFdeGeometry(geo_1);
                render.VisibleMask = gviViewportMask.gviViewAllNormalView;
                render.Symbol.BoundarySymbol.Color = Color.FromArgb(255, 0, 255, 255);
                geo_1 = render.GetFdeGeometry() as IPolygon;//IMultiPolygon;
                geo_1.SpatialCRS = GviMap.SpatialCrs;
                guids.Add(render.Guid);
               // GviMap.Camera.FlyToEnvelope(render.Envelope);

                for (int i = 0; i < polylines[1].PointCount; i++)
                {
                    var point = polylines[1].GetPoint(i);
                    var topoPoi = point as ITopologicalOperator2D;
                    if (topoPoi.Intersection2D(geo_1) == null)
                    {
                        _problemPoints.Add(point);
                        CreatRenPoi(point);
                    }
                }

            }          
        }
        private IGeometry Buffer(IPolyline polyline, double dis)
        {
            var poly = polyline.Clone2(gviVertexAttribute.gviVertexAttributeNone);
          
            var topo = poly as ITopologicalOperator2D;
            return topo.Buffer2D(dis, gviBufferStyle.gviBufferCapround);
        }

        private void CreatRenPoi(IPoint point)
        {
            var poi = GviMap.GeoFactory.CreateGeometry(gviGeometryType.gviGeometryPOI, gviVertexAttribute.gviVertexAttributeZ) as IPOI;
            poi.SetPostion(point.X, point.Y, point.Z);
            poi.Size = 30;
            poi.ShowName = true;
            poi.MaxVisibleDistance = 10000;
            poi.MinVisibleDistance = 1;
            //poi.Name = onePerson.name;
            poi.SpatialCRS = GviMap.SpatialCrs;
            poi.ImageName = string.Format(AppDomain.CurrentDomain.BaseDirectory + "项目数据\\shp\\IMG_POI\\{0}.png", "alphabet_P");
            IRenderPOI rpoi = GviMap.ObjectManager.CreateRenderPOI(poi);
            rpoi.DepthTestMode = gviDepthTestMode.gviDepthTestAdvance;
            guids.Add(rpoi.Guid);
        }




        ////如果是跳转过来 就查数据
        /// <summary>
        /// 手动获取中线桩
        /// </summary>
        private void getlineList(string jizhunxian)
        {
            //this.TracingLineModels = new ObservableCollection<TracingLineModel>();
            //string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.tracinglineList + "?traces=" + ChangedItem.id);
            //this.TracingLineModels = (JsonUtil.DeserializeFromString<ObservableCollection<TracingLineModel>>(resStr));
        }
    }
}
