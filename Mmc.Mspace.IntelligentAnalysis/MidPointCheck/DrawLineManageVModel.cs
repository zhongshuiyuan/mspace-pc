using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.RenderControl;
using Mmc.Framework.Services;
using Mmc.Mspace.Common.Messenger;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.Const.ConstPath;
using Mmc.Mspace.IntelligentAnalysisModule.AreaWidth;
using Mmc.Mspace.Services.HttpService;
using Mmc.Mspace.Services.NetRouteAnalysisService;
using Mmc.Mspace.Theme.Pop;
using Mmc.Windows.Utils;
using Mmc.Wpf.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
                base.SetAndNotifyPropertyChanged<ObservableCollection<LineItem>>(ref this._drawLineListCollection, value, "DrawLineListCollection");               
            }
        }

        List<LineItem> _tempItemList = new List<LineItem>();
        public List<LineItem> TempItemList
        {
            get { return _tempItemList; }
            set
            {
                _tempItemList = value;
                base.SetAndNotifyPropertyChanged<List<LineItem>>(ref this._tempItemList, value, "TempItemList");
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

        private RelayCommand<object> _selectCommand;

        public RelayCommand<object> SelectCommand
        {
            get { return _selectCommand ?? (_selectCommand = new RelayCommand<object>(OnSelectCommand)); }
            set { _selectCommand = value; }
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
                    newDrawLineVModel.AddPipe += AddLinePipe;
                }
                newDrawLineVModel.ClearData();
                newDrawLineVModel.ShowDrawWin();
               // GetLineData();
            });
            this.DelItemsCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                DelItems();
            });
            this.SearchCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                //DelItems();
            });
            this.IsOpenCmd = new Mmc.Wpf.Commands.RelayCommand<LineItem>((lineitm) => ChangeIsChecked(lineitm));
            this.AreaWidthCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                 TempItemList = DrawLineListCollection.Where(t => t.IsChecked).ToList();
                if(TempItemList.Count==2)
                {
                     AreaWithStatus = 0;
                    if (selectView == null)
                        selectView = new SelectView();
                    selectView.DataContext = this;
                    selectView.Owner = drawLineManageView;
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
                TempItemList = DrawLineListCollection.Where(t => t.IsChecked).ToList();
                if (TempItemList.Count == 2)
                {
                    AreaWithStatus = 1;
                    if (selectView == null)
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
                    newDrawLineVModel.AddPipe += AddLinePipe;
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
                areaWidthVModel.TitleText = "边界宽度预警";
                areaWidthVModel.lineItems = TempItemList;
                areaWidthVModel.CancelWin = ShowWin;
                areaWidthVModel.OnChecked();
            }
            else
            {
                AreaWidthVModel areaWidthVModel = new AreaWidthVModel();
                areaWidthVModel.TitleText = "中线桩位置核准";
                areaWidthVModel.lineItems = TempItemList;
                areaWidthVModel.CancelWin = ShowWin;
                areaWidthVModel.OnChecked();
            }
            drawLineManageView.Hide();
        }

        private LineItem selectLineItem = null;
        private void OnSelectCommand(object obj)
        {
            if (obj == null) return;
            DelObjs();
            polylines = new List<IPolyline>();
            selectLineItem = obj as LineItem;

            var polyLine = GviMap.GeoFactory.CreatePolyline(selectLineItem.geom, GviMap.SpatialCrs);
            if (polyLine == null) return;
           
            if (polyLine.EndPoint == null) return;
            CurveSymbol curveSymbol = new CurveSymbol();
            curveSymbol.Color = ColorConvert.Argb(100, 238, 103, 35);//GviMap.LinePolyManager.CurveSym
            curveSymbol.Width = 20f;
            var rLine = GviMap.ObjectManager.CreateRenderPolyline(polyLine, curveSymbol, GviMap.ProjectTree.RootID);
        
            if (rLine == null) return;
            guids.Add(rLine.Guid);
            rLine.VisibleMask = gviViewportMask.gviViewAllNormalView;
            //GviMap.Camera.FlyToObject(rLine.Guid, gviActionCode.gviActionFlyTo);
            //var poly0 = GviMap.GeoFactory.CreateFromWKT(lineItem.geom) as IPolyline;

            GviMap.Camera.GetCamera2(out IPoint pointCamera, out IEulerAngle eulerAngle);
            ////GviMap.Camera.FlyToEnvelope(point.Envelope);
            eulerAngle.Tilt = -90;
            eulerAngle.Heading = 110;
            pointCamera.X = rLine.Envelope.MaxX;
            pointCamera.Y = rLine.Envelope.MaxY;
            pointCamera.Z = 2000;
            GviMap.Camera.SetCamera2(pointCamera, eulerAngle, 0);
            if (polyLine != null)
            {
                polylines.Add(polyLine);
            }
            SetVideo();
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
        private void SetVideo()
        {
            if (polylines.Count >0)
            {
                for (int i = 0; i < polylines[0].PointCount; i++)
                {
                    var point = polylines[0].GetPoint(i);

                    var poi = GviMap.GeoFactory.CreateGeometry(gviGeometryType.gviGeometryPOI, gviVertexAttribute.gviVertexAttributeZ) as IPOI;
                    if(i==0)
                    {
                        poi.Name = selectLineItem.start_sn;
                        poi.ShowName = true;
                    }
                    if (i == polylines[0].PointCount-1)
                    {
                        poi.Name = selectLineItem.end_sn;
                        poi.ShowName = true;
                    }
                    poi.SetPostion(point.X, point.Y);
                    poi.Size = 50;
                  
                    poi.ShowName = false;
                    poi.ImageName = string.Format("项目数据\\shp\\IMG_POI\\{0}.png", "中线桩");
                    poi.SpatialCRS = GviMap.SpatialCrs;
                    var rPoi = GviMap.ObjectManager.CreateRenderPOI(poi);
                    rPoi.DepthTestMode = gviDepthTestMode.gviDepthTestAlways;
                    this.poiList.Add(rPoi.Guid.ToString(), rPoi.Guid);
                }
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
            ClearPatrolList();
            foreach (var item in guids)
            {
                GviMap.ObjectManager.DeleteObject(item);
            }
            guids.Clear();
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
        List<IPolyline> polylines = new List<IPolyline>();
        ObservableCollection<IPoint> _problemPoints = new ObservableCollection<IPoint>();
        List<Guid> guids = new List<Guid>();
        public List<LineItem> lineItems = new List<LineItem>();
        private void CreatRenPoi(IPoint point)
        {
            var poi = GviMap.GeoFactory.CreateGeometry(gviGeometryType.gviGeometryPOI, gviVertexAttribute.gviVertexAttributeZ) as IPOI;
            poi.SetPostion(point.X, point.Y, 2);
            poi.Size = 30;
            poi.ShowName = true;
            poi.MaxVisibleDistance = 5000;
            poi.MinVisibleDistance = 100;
            //poi.Name = onePerson.name;
            poi.SpatialCRS = GviMap.SpatialCrs;
            poi.ImageName = string.Format("项目数据\\shp\\IMG_POI\\{0}.png", "alphabet_P");//Helpers.ResourceHelper.FindResourceByKey("userImg").ToString();//
            IRenderPOI rpoi = GviMap.ObjectManager.CreateRenderPOI(poi);
            rpoi.DepthTestMode = gviDepthTestMode.gviDepthTestAdvance;
            guids.Add(rpoi.Guid);
        }
        private void GetLineData()
        {
            DrawLineListCollection.Clear();
            var json = JsonUtil.DeserializeFromFile<dynamic>(AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
            string url = string.Format("{0}/api/tracing/index", json.poiUrl);
            var httpservice = new HttpService();
            httpservice.Token = HttpServiceUtil.Token;
            var uavResult = string.Empty;
            uavResult = httpservice.RequestService(url, method: "GET");
            var templist = JsonUtil.DeserializeFromString<dynamic>(uavResult);
            dynamic list = templist.data;
            foreach( var item in list)
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
                lineItem.type = item["type"]=="1"?"自动":"手动";
                lineItem.geom = item["geom"];
                lineItem.IsChecked = false;
                DrawLineListCollection.Add(lineItem);
            }
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

       
            if (deleteString =="?ids=")
            {
                //Messages.ShowMessage("");
            }
            else
            {
                deleteString =  deleteString.Substring(0,deleteString.Length-1);
                string url = MarkInterface.DeleteLine + deleteString;
                string resStr = HttpServiceHelper.Instance.GetRequest(url);
            }
            GetLineData();
            if (DrawLineListCollection.Count == 0)
            {
                DelObjs();

            }
        }
        private void ChangeIsChecked(LineItem lineItem)
        {
            lineItem.IsChecked = !lineItem.IsChecked;
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
