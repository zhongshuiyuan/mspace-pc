using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.RenderControl;
using Mmc.Framework.Services;
using Mmc.Mspace.Common.Messenger;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.Const.ConstPath;
using Mmc.Mspace.IntelligentAnalysisModule.AreaWidth;
using Mmc.Mspace.IntelligentAnalysisModule.Models;
using Mmc.Mspace.Services.HttpService;
using Mmc.Mspace.Services.NetRouteAnalysisService;
using Mmc.Mspace.Theme.Pop;
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
using System.Windows;
using System.Windows.Input;

namespace Mmc.Mspace.IntelligentAnalysisModule.MidPointCheck
{
    public class DrawLineManageVModel: CheckedToolItemModel
    {
        DrawLineManageView drawLineManageView = new DrawLineManageView();
        ObservableCollection<LineItem> _drawLineListCollection = new ObservableCollection<LineItem>();
        public ObservableCollection<LineItem> DrawLineListCollection
        {
            get { return _drawLineListCollection; }
            set
            {
                _drawLineListCollection = value;
                NotifyPropertyChanged("DrawLineListCollection");
            }
        }

        private int _pageCount=1;

        public int PageCount
        {
            get { return _pageCount; }
            set 
            {
                _pageCount = value;
                NotifyPropertyChanged("PageCount");
            }
        }

        private int _selectCount;

        public int SelectCount
        {
            get { return _selectCount; }
            set { _selectCount = value; NotifyPropertyChanged("SelectCount"); }
        }


        private int _pageNum=1;

        public int PageNum
        {
            get { return _pageNum; }
            set {
                _pageNum = value;
                NotifyPropertyChanged("PageNum");
            }
        }
        private int _total;

        public int Total
        {
            get { return _total; }
            set { _total = value; NotifyPropertyChanged("Total"); }
        }


        private string _ReportSearchText;

        public string ReportSearchText
        {
            get { return _ReportSearchText; }
            set { _ReportSearchText = value;
                base.SetAndNotifyPropertyChanged<string>(ref this._ReportSearchText, value, "ReportSearchText");
            }
        }

        ObservableCollection<LineItem> _tempItemList = new ObservableCollection<LineItem>();
        public ObservableCollection<LineItem> TempItemList
        {
            get { return _tempItemList; }
            set
            {
                _tempItemList = value;
                base.SetAndNotifyPropertyChanged<ObservableCollection<LineItem>>(ref this._tempItemList, value, "TempItemList");
            }
        }
        private ObservableCollection<TracingLineModel> _tracingLineModels = new ObservableCollection<TracingLineModel>();
        public ObservableCollection<TracingLineModel> TracingLineModels
        {
            get { return _tracingLineModels; }
            set
            {
                _tracingLineModels = value;
                base.SetAndNotifyPropertyChanged<ObservableCollection<TracingLineModel>>(ref this._tracingLineModels, value, "TracingLineModels");
            }
        }

        private SelectView selectView = null;
        private NewDrawLineVModel newDrawLineVModel = null;

        public ICommand CreatLineCmd { get; set; }
        public ICommand CloseCmd { get; set; }
        public ICommand DelItemsCmd { get; set; }
        public ICommand SearchCmd { get; set; }
        public ICommand IsOpenCmd { get; set; }
        public ICommand AreaWidthCmd { get; set; }
        public ICommand MidPositionCmd { get; set; }
        public ICommand ChangeCmd { get; set; }
        public ICommand VisualCmd { get; set; }

        private RelayCommand<LineItem> _selectCommand;

        public RelayCommand<LineItem> SelectCommand
        {
            get { return _selectCommand ?? (_selectCommand = new RelayCommand<LineItem>(OnSelectCommand)); }
            set { _selectCommand = value; }
        }
        private RelayCommand<LineItem> _selectCommand2;

        public RelayCommand<LineItem> SelectCommand2
        {
            get { return _selectCommand2 ?? (_selectCommand2 = new RelayCommand<LineItem>(OnSelectCommand2)); }
            set { _selectCommand2 = value; }
        }
        
        private RelayCommand _updateSaveCommand;

        public RelayCommand UpdateSaveCommand
        {
            get { return _updateSaveCommand ?? (_updateSaveCommand = new RelayCommand(OnUpdateSaveCommand)); }
            set { _updateSaveCommand = value; }
        }
        private RelayCommand _selectCancelCommand;

        public RelayCommand SelectCancelCommand
        {
            get { return _selectCancelCommand ?? (_selectCancelCommand = new RelayCommand(OnSelectCancelCommand)); }
            set { _selectCancelCommand = value; }
        }
        
        /// <summary>
        /// 边界预警 0  中线桩位置1
        /// </summary>
        private int AreaWithStatus = 0;
        
        public override void Initialize()
        {
            base.Initialize();
            base.ViewType = ViewType.CheckedIcon;
            drawLineManageView.DataContext = this;
            this.CloseCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                IsChecked = false;
                this.DelObjs();
                drawLineManageView.Hide();
            }); 
            this.CreatLineCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                if(newDrawLineVModel==null)
                {
                    newDrawLineVModel = new NewDrawLineVModel();
                    newDrawLineVModel.HideParentsWin = drawLineManageView.Hide;
                    newDrawLineVModel.ShowParentsWin = ShowWin;
                    newDrawLineVModel.AddPipe = AddLinePipe;
                }
                newDrawLineVModel.ClearData();
                newDrawLineVModel.ShowDrawWin();
               // GetLineData();
            });
            this.DelItemsCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                if (DrawLineListCollection.Count<1||DrawLineListCollection.Where(t => t.IsChecked).Count() < 1)
                {
                    Messages.ShowMessage("没有可删除项！");
                    return;
                }
                if (Messages.ShowMessageDialog("提示", "是否确认删除选中项？"))
                {
                    DelItems();
                }
          
            });
            this.SearchCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                DelItems();
            });
            this.IsOpenCmd = new Mmc.Wpf.Commands.RelayCommand<LineItem>((lineitm) => ChangeIsChecked(lineitm));
            this.AreaWidthCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                 TempItemList = new ObservableCollection<LineItem>( DrawLineListCollection.Where(t => t.IsChecked).ToList());
                if(TempItemList.Count==2)
                {
                     AreaWithStatus = 0;
                    selectView = new SelectView();
                    selectView.Owner = drawLineManageView;
                    selectView.DataContext = this;
                    selectView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectView.ShowDialog();
                }
                else
                {
                    Messages.ShowMessage("请选择两条线路进行对比！");
                }
                //Messenger.Messengers.Notify("AreaWidth", true);
              
            });
            this.MidPositionCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                TempItemList = new ObservableCollection<LineItem>(DrawLineListCollection.Where(t => t.IsChecked).ToList());
                if (TempItemList.Count == 2)
                {
                    AreaWithStatus = 1;
                    selectView = new SelectView();
                    selectView.DataContext = this;
                    selectView.Owner = drawLineManageView;
                    selectView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectView.ShowDialog();
                }
                else
                {
                    Messages.ShowMessage("请选择两条线路进行对比！");
                }//
            });    
            this.ChangeCmd = new Mmc.Wpf.Commands.RelayCommand<object>((obj) =>
            {
                //DelItems();
                if(newDrawLineVModel==null)
                {
                    newDrawLineVModel = new NewDrawLineVModel();
                    newDrawLineVModel.HideParentsWin = drawLineManageView.Hide;
                    newDrawLineVModel.ShowParentsWin = ShowWin;
                    newDrawLineVModel.AddPipe = AddLinePipe;
                }
                DelObjs();
                newDrawLineVModel.ChangedData(obj as LineItem);
                newDrawLineVModel.ShowDrawWin();
            });
            this.VisualCmd = new Mmc.Wpf.Commands.RelayCommand<LineItem>((lineitm) => VisualChecked(lineitm));           
        }

        public override void OnChecked()
        {
            Messenger.Messengers.Notify("DrawLineManage", true);
            GetLineData();
            drawLineManageView.DataContext = this;
            drawLineManageView.Owner = Application.Current.MainWindow;
            drawLineManageView.Left = 700;
            drawLineManageView.Top = Application.Current.MainWindow.Height * 0.2;
            drawLineManageView.Show();
            base.OnChecked();          
        }
        private void ShowWin()
        {
            drawLineManageView.Show();
        }
        private void OnSelectCancelCommand()
        {
            selectView.Hide();
        }
        private void OnUpdateSaveCommand() {
            if(TempItemList.Where(t=>t.IsRoot).Count()<1)
            {
                Messages.ShowMessage("您未选择自动描线作为基准线，无法进行核准！！");
                return;
            }
            DelObjs();
            selectView.Hide();
            if(AreaWithStatus==0)
            {
                AreaWidthVModel areaWidthVModel = new AreaWidthVModel();
                areaWidthVModel.ShowType = true;
                areaWidthVModel.TitleText = "边界宽度预警";
                areaWidthVModel.lineItems = TempItemList.ToList();
                areaWidthVModel.CancelWin = ShowWin;
                areaWidthVModel.OnChecked();
            }
            else
            {
                AreaWidthVModel areaWidthVModel = new AreaWidthVModel();
                areaWidthVModel.ShowType = true;
                areaWidthVModel.TitleText = "中线桩位置核准";
                areaWidthVModel.lineItems = TempItemList.ToList();
                areaWidthVModel.CancelWin = ShowWin;
                areaWidthVModel.OnChecked();
            }
            drawLineManageView.Hide();
        }

        private LineItem selectLineItem = null;

        private void OnSelectCommand2(LineItem obj)
        {
            if (obj == null) return;
            if (obj.EyeStatus) return;
            OnSelectCommand(obj);
        }
        private void OnSelectCommand(LineItem obj)
        {
            if (obj == null) return;
            //DelObjs();
            //polylines = new List<IPolyline>();
            selectLineItem = obj ;
            if(guids.Count>0&& guids.ContainsKey(selectLineItem.id))
            {
                SetVisibleMask(selectLineItem, selectLineItem.EyeStatus);
                if (!selectLineItem.EyeStatus)
                {
                    Guid id = guids[selectLineItem.id];
                    gettracinglineList(id, selectLineItem.id);
                    SetVideo(rLines.SingleOrDefault(t => t.Guid == id).Guid);
                    selectLineItem.EyeStatus2 = false;
                    selectLineItem.EyeStatus = true;
                }
                else
                {
                    Guid id = guids[selectLineItem.id];
                    ClearPatrolList(rLines.SingleOrDefault(t => t.Guid == id).Guid.ToString());
                    selectLineItem.EyeStatus =false;
                    selectLineItem.EyeStatus2 =true;
                }
          
          
                return;
            }
          
            if (guids.ContainsKey(selectLineItem.id)) return;
            var polyLine = GviMap.GeoFactory.CreatePolyline(selectLineItem.geom, GviMap.SpatialCrs);
            if (polyLine == null) return;
       
            if (polyLine.EndPoint == null) return;
            CurveSymbol curveSymbol = new CurveSymbol();
            if(selectLineItem.type=="自动")
            {
                curveSymbol.Color = ColorConvert.Argb(100, 0, 0, 0);
            }
            else
            {
                curveSymbol.Color = ColorConvert.Argb(100, 238, 103, 35);
            }
            curveSymbol.Width = 10f;
          

            var rLine = GviMap.ObjectManager.CreateRenderPolyline(polyLine, curveSymbol, GviMap.ProjectTree.RootID);
         
            if (rLine == null) return;
            rLine.MinVisibleDistance = 1.0;
            rLine.MaxVisibleDistance = 10000.0;
            rLines.Add(rLine);
            gettracinglineList(rLine.Guid,selectLineItem.id);
            guids.Add(selectLineItem.id,rLine.Guid);
            rLine.VisibleMask = gviViewportMask.gviViewAllNormalView;
            //GviMap.Camera.FlyToObject(rLine.Guid, gviActionCode.gviActionFlyTo);
            //var poly0 = GviMap.GeoFactory.CreateFromWKT(lineItem.geom) as IPolyline;

            GviMap.Camera.GetCamera2(out IPoint pointCamera, out IEulerAngle eulerAngle);
            ////GviMap.Camera.FlyToEnvelope(point.Envelope);
            eulerAngle.Tilt = -60;
            eulerAngle.Heading = 0;

            pointCamera.X = rLine.Envelope.MinX;
            pointCamera.Y = rLine.Envelope.MinY;
            pointCamera.Z = 2100;
            GviMap.Camera.SetCamera2(pointCamera, eulerAngle, 0);
            //Messenger.Messengers.Notify("zhibeiCommand", true);

            if (polyLine != null)
            {
                polylines.Add(rLine.Guid,polyLine);
            }
        
            selectLineItem.EyeStatus = true;
            selectLineItem.EyeStatus2 = false;
        }

        private void SetVisibleMask(LineItem line,bool visi)
        {
            Guid id = guids[line.id];
            rLines.SingleOrDefault(t => t.Guid == id)?.SetVisibleMask(GviMap.Viewport.ViewportMode, 0, !visi);
        }
     
        /// <summary>
        /// 手动获取中线桩
        /// </summary>
        private void gettracinglineList(Guid guid,string id)
        {
            this.TracingLineModels = new ObservableCollection<TracingLineModel>();
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.tracinglineList + "?traces=" + id);
            this.TracingLineModels = (JsonUtil.DeserializeFromString<ObservableCollection<TracingLineModel>>(resStr));
            SetVideo(guid);
        }

        public override void OnUnchecked()
        {
            drawLineManageView.Hide();
            newDrawLineVModel?.HideWin();
          
            if(newDrawLineVModel!=null&& newDrawLineVModel.HideAdd!=null)
            {
                newDrawLineVModel?.HideAdd();
            }
            base.OnUnchecked();
            DelObjs();
            Messenger.Messengers.Notify("DrawLineManage", false);
        }
        private IGeometry Buffer(IPolyline polyline, double dis)
        {
            var poly = polyline.Clone2(gviVertexAttribute.gviVertexAttributeNone);
            var topo = poly as ITopologicalOperator2D;
            return topo.Buffer2D(dis, gviBufferStyle.gviBufferCapround);
        }
        public Dictionary<string, Guid> poiList = new Dictionary<string, Guid>();
        private void SetVideo(Guid guid)
        {
            try
            {

                if (TracingLineModels.Count > 0)
                {
                    for (int i = 0; i < TracingLineModels.Count; i++)
                    {
                        var point = TracingLineModels[i];

                        var poi = GviMap.GeoFactory.CreateGeometry(gviGeometryType.gviGeometryPOI, gviVertexAttribute.gviVertexAttributeZ) as IPOI;

                        poi.MaxVisibleDistance = 10000.0;
                        poi.MinVisibleDistance = 0;
                        poi.Name = string.IsNullOrEmpty(point.Stake_sn) ? point.Sn : point.Stake_sn;
                        poi.ShowName = true;
                        poi.SetPostion(Convert.ToDouble(point.Lng), Convert.ToDouble(point.Lat), string.IsNullOrEmpty(point.Height) ? 0 : Convert.ToDouble(point.Height));
                        poi.Size = 50;
                        poi.ImageName = string.Format(AppDomain.CurrentDomain.BaseDirectory + "项目数据\\shp\\IMG_POI\\{0}.png", "stake");
                        poi.SpatialCRS = GviMap.SpatialCrs;
                        var rPoi = GviMap.ObjectManager.CreateRenderPOI(poi);
                        rPoi.DepthTestMode = gviDepthTestMode.gviDepthTestAlways;
                        this.poiList.Add(rPoi.Guid.ToString(), guid);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
          
        }
        public IPolygon UpdateZ(IPolygon geo, double Z)
        {
            bool flag = geo == null;
            IPolygon result;
            if (flag)
            {
                result = geo;
            }
            else
            {
                geo = (geo.Clone2(gviVertexAttribute.gviVertexAttributeZ) as IPolygon);
                int num;
                for (int i = 0; i < geo.ExteriorRing.PointCount; i = num + 1)
                {
                    IPoint point = geo.ExteriorRing.GetPoint(i);
                    point.Z = Z;
                    geo.ExteriorRing.UpdatePoint(i, point);
                    num = i;
                }
                result = geo;
            }
            return result;
        }

        private void DelObjs()
        {
            ClearPatrolList(null);
            foreach (var item in guids)
            {
                GviMap.ObjectManager.DeleteObject(item.Value);
            }
            guids.Clear();
        }
        public void ClearPatrolList(string str)
        {
            Dictionary<string, Guid> expr_07 = null;

            if (str == null)
            {
                expr_07 = this.poiList;
            }
            else
            {
                Guid guid = Guid.Parse(str);
                expr_07 = this.poiList.Where(t => t.Value == guid).ToDictionary(product => product.Key, product => product.Value);

            }
            bool flag = expr_07 == null || expr_07.Count > 0;
            if (flag)
            {
                foreach (KeyValuePair<string, Guid> current in expr_07)
                {
                    GviMap.ObjectManager.DeleteObject(Guid.Parse(current.Key));
                    if (str != null)
                        this.poiList.Remove(current.Key);
                }
            }
            if (str == null)
                this.poiList = new Dictionary<string, Guid>();
        }
        Dictionary<Guid,IPolyline> polylines = new Dictionary<Guid,IPolyline>();
        List<IRenderPolyline> rLines = new List<IRenderPolyline>();
        ObservableCollection<IPoint> _problemPoints = new ObservableCollection<IPoint>();
        Dictionary<string,Guid> guids = new Dictionary<string,Guid>();
        public List<LineItem> lineItems = new List<LineItem>();

        private void GetLineData()
        {
            DrawLineListCollection.Clear();
            var json = JsonUtil.DeserializeFromFile<dynamic>(AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
            string url = string.Format("{0}/api/tracing/index?page=1&page_size=100&name="+ ReportSearchText, json.poiUrl);
            var httpservice = new HttpService();
            httpservice.Token = HttpServiceUtil.Token;
            var uavResult = string.Empty;
            uavResult = httpservice.RequestService(url, method: "GET");
            var templist = JsonUtil.DeserializeFromString<dynamic>(uavResult);
             dynamic list = templist.data;
            Total = templist.total;
            foreach (var item in list)
            {
                LineItem lineItem = new LineItem();
                lineItem.id = item["id"];
                lineItem.sn = item["sn"];
                lineItem.name = item["name"];
                lineItem.pipe_id = item["pipe_id"];
                lineItem.pipe_name = item["pipe_name"];
                lineItem.start = item["start"];
                lineItem.end = item["end"];
                lineItem.start_sn = item["start_sn"];
                lineItem.end_sn = item["end_sn"];
                lineItem.isVisible = false;
                lineItem.type = item["type"] == "1" ? "自动" : "手动";
                lineItem.geom = item["geom"];
                lineItem.IsChecked = false;
                DrawLineListCollection.Add(lineItem);
            }
            if (DrawLineListCollection.Count > 0)
                OnSelectCommand(DrawLineListCollection[0]);
        }
      
        private void AddLinePipe(LineItem lineItem)
        {
            if(lineItem != null)
            {
                drawLineManageView.Show();
                this.GetLineData();
            }            
        }
        private void DelItems()
        {
            string deleteString = "?ids=";
            foreach(var item in DrawLineListCollection)
            {
                if(item?.IsChecked == true)
                {
                    deleteString = deleteString + Convert.ToString(item.id)+",";
                }
            }
            deleteString = deleteString.Substring(0, deleteString.Length - 1);
            string url = MarkInterface.DeleteLine + deleteString;
            string resStr = HttpServiceHelper.Instance.GetRequest(url);

            GetLineData();
            if (DrawLineListCollection.Count == 0)
            {
                DelObjs();
            }
        }
        private void ChangeIsChecked(LineItem lineItem)
        {
            SelectCount = DrawLineListCollection.Where(t => t.IsChecked).Count();
        }
        private void VisualChecked(LineItem lineItem)
        {
            if (lineItem.guid != null)
            {
                IRenderPolyline obj = GviMap.ObjectManager.GetObjectById(lineItem.guid) as IRenderPolyline;
                if(obj != null)
                {
                    if (obj.VisibleMask == gviViewportMask.gviViewAllNormalView)
                    {
                        obj.VisibleMask = gviViewportMask.gviViewNone;
                    }
                    else
                    {
                        obj.VisibleMask = gviViewportMask.gviViewAllNormalView;
                    }
                }                
            }
            
        }
    }
}
