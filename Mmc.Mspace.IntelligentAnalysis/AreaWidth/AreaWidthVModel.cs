using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.RenderControl;
using Mmc.Framework.Draw;
using Mmc.Framework.Services;
using Mmc.MathUtil;
using Mmc.Mspace.Common.Messenger;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Const.ConstPath;
using Mmc.Mspace.IntelligentAnalysisModule.MidPointCheck;
using Mmc.Mspace.PoiManagerModule.Models;
using Mmc.Mspace.Theme.Pop;
using Mmc.Windows.Services;
using Mmc.Windows.Utils;
using Mmc.Wpf.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Mmc.Mspace.IntelligentAnalysisModule.AreaWidth
{
     public class AreaWidthVModel: CheckedToolItemModel
     {
        List<IPolyline> polylines = new List<IPolyline>();
        ObservableCollection<IPoint> _problemPoints = new ObservableCollection<IPoint>();
        List<Guid> guids = new List<Guid>();
        public List<LineItem> lineItems = new List<LineItem>();
        public ObservableCollection<IPoint> ProblemPoints
        {
            get { return _problemPoints; }
            set
            {
                base.SetAndNotifyPropertyChanged<ObservableCollection<IPoint>>(ref this._problemPoints, value, "ProblemPoints");
            }
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
        public override void OnChecked()
        {
            base.OnChecked();
            areaWidthView.Show();
        }

        public override void OnUnchecked()//关闭事件
        {
            base.OnUnchecked();
            areaWidthView.Hide();
        }
        private void Ca()
        {
            setData();
            Cal();
            Cal2();
            Messages.ShowMessage("计算完成，问题点数目为:"+ Convert.ToString(_problemPoints.Count));
        }
        public void Hide()
        {
            ClearList();
            areaWidthView.Hide();           
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
           // ProblemPoints2.Clear();
        }
        private void delObjs()
        {
            foreach (var item in guids)
            {
                GviMap.ObjectManager.DeleteObject(item);
            }
            guids.Clear();
        }
        private void setData()
        {
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
        }
        private void Cal()
        {
            if(polylines.Count==2)
            {
                //ITopologicalOperator3D topologicalOperator3D =polylines[0]
                IGeometry geo_1 = Buffer(polylines[1], Convert.ToDouble(_radius) / 100000);
                geo_1.SpatialCRS = GviMap.SpatialCrs;
                //var type = geo_1.GeometryType;
                //IRenderMultiPolygon render = GviMap.ObjectManager.CreateRenderMultiPolygon(geo_1 as IMultiPolygon, GviMap.LinePolyManager.SurfaceSym, GviMap.ProjectTree.RootID);
                IRenderPolygon render = GviMap.ObjectManager.CreateRenderPolygon(geo_1 as IPolygon, GviMap.LinePolyManager.SurfaceSym, GviMap.ProjectTree.RootID);

                ////polygon.SpatialCRS = GviMap.SpatialCrs;
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
        private void Cal2()
        {
            if (polylines.Count == 2)
            {
                //ITopologicalOperator3D topologicalOperator3D =polylines[0]
                IGeometry geo_1 = Buffer(polylines[0], Convert.ToDouble(_radius)/100000);
                geo_1.SpatialCRS = GviMap.SpatialCrs;
                //var type = geo_1.GeometryType;
                //IRenderMultiPolygon render = GviMap.ObjectManager.CreateRenderMultiPolygon(geo_1 as IMultiPolygon, GviMap.LinePolyManager.SurfaceSym, GviMap.ProjectTree.RootID);
                IRenderPolygon render = GviMap.ObjectManager.CreateRenderPolygon(geo_1 as IPolygon, GviMap.LinePolyManager.SurfaceSym, GviMap.ProjectTree.RootID);
                
                ////polygon.SpatialCRS = GviMap.SpatialCrs;
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
    }
}
