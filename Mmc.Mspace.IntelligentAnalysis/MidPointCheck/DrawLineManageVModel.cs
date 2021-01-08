using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.RenderControl;
using Mmc.Framework.Services;
using Mmc.Mspace.Common.Messenger;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.Const.ConstPath;
using Mmc.Mspace.IntelligentAnalysisModule.AreaWidth;
using Mmc.Mspace.Services.HttpService;
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
                var TempItemList = new List<LineItem>();
                foreach (var item in DrawLineListCollection)
                {
                    if(item.IsChecked == true)
                    {
                        TempItemList.Add(item);
                    }
                }
                if(TempItemList.Count==2)
                {
                    AreaWidthVModel areaWidthVModel = new AreaWidthVModel();
                    areaWidthVModel.lineItems = TempItemList;
                    areaWidthVModel.OnChecked();
                }
                else
                {
                    Messages.ShowMessage("请选择两条线路进行对比！");
                }
                //Messenger.Messengers.Notify("AreaWidth", true);
              
            });
            this.MidPositionCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                var TempItemList = new List<LineItem>();
                foreach (var item in DrawLineListCollection)
                {
                    if (item.IsChecked == true)
                    {
                        TempItemList.Add(item);
                    }
                }
                if (TempItemList.Count == 2)
                {
                    AreaWidthVModel areaWidthVModel = new AreaWidthVModel();
                    areaWidthVModel.lineItems = TempItemList;
                    areaWidthVModel.OnChecked();
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
    
        private void OnSelectCommand(object obj)
        {
            DelObjs();
            polylines = new List<IPolyline>();
            var lineItem = obj as LineItem;
            var poly0 = GviMap.GeoFactory.CreateFromWKT(lineItem.geom) as IPolyline;
            if (poly0 != null )
            {
                polylines.Add(poly0);
            }
            SetVideo();
        }

        public override void OnUnchecked()
        {
            drawLineManageView.Hide();
            newDrawLineVModel?.HideWin();
            base.OnUnchecked();
            Messenger.Messengers.Notify("DrawLineManage", false);
        }
        private IGeometry Buffer(IPolyline polyline, double dis)
        {
            var poly = polyline.Clone2(gviVertexAttribute.gviVertexAttributeNone);
            var topo = poly as ITopologicalOperator2D;
            return topo.Buffer2D(dis, gviBufferStyle.gviBufferCapround);
        }
        private string _radius = "1";
        private void SetVideo()
        {
            if (polylines.Count >0)
            {
                //ITopologicalOperator3D topologicalOperator3D =polylines[0]
                IGeometry geo_1 = Buffer(polylines[0], Convert.ToDouble(_radius) / 100000);
                geo_1.SpatialCRS = GviMap.SpatialCrs;
           
                IRenderPolygon render = GviMap.ObjectManager.CreateRenderPolygon(geo_1 as IPolygon, GviMap.LinePolyManager.SurfaceSym, GviMap.ProjectTree.RootID);
                ////polygon.SpatialCRS = GviMap.SpatialCrs;
                render?.SetFdeGeometry(geo_1);
                render.VisibleMask = gviViewportMask.gviViewAllNormalView;
                render.Symbol.BoundarySymbol.Color = Color.FromArgb(255, 0, 255, 255);
                geo_1 = render.GetFdeGeometry() as IPolygon;//IMultiPolygon;
                geo_1.SpatialCRS = GviMap.SpatialCrs;
                guids.Add(render.Guid);
                GviMap.Camera.FlyToObject(render.Guid, gviActionCode.gviActionFlyTo);
                for (int i = 0; i < polylines[0].PointCount; i++)
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
        }

        private void DelObjs()
        {
            foreach (var item in guids)
            {
                GviMap.ObjectManager.DeleteObject(item);
            }
            guids.Clear();
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
                lineItem.start = item["start"];
                lineItem.end = item["end"]; 
                lineItem.start_sn = item["start_sn"];
                lineItem.end_sn = item["end_sn"];
                lineItem.isVisible = false;
                lineItem.type_id = item["type_name"];//TypenameToNum(Convert.ToString(item["type_name"]));
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
            if(deleteString =="?ids=")
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
