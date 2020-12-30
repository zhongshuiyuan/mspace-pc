using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.RenderControl;
using Mmc.Framework.Services;
using Mmc.Mspace.Common;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Common.ShellService;
using Mmc.Mspace.Const.ConstDataBase;
using Mmc.Mspace.Services.NetRouteAnalysisService;
using Mmc.Mspace.Theme.Pop;
using Mmc.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Mmc.Mspace.RoutePlanning
{
   public  class FlySimulate : CheckedToolItemModel
    {
        private IDynamicObject dynamicObject = null;
        private ISkinnedMesh skinMeshPlane = null;
        private IVector3 position = new Vector3();
        private System.Guid rootId = new System.Guid();
        private IPolyline fde_polyline = null;
        private IRenderPolyline rpolyline = null;
        private IGeometryFactory gfactory = null;      
        private ICurveSymbol lineSymbol = null;
        public double Flyspeed;
        IModelPoint mp = null;
        public  List<Guid> guidlist = new List<Guid>();
        FlySimulateParameterViewModel flySimulateParameterViewModel = null;
        public List<IPoint> FlyPointList = new List<IPoint>();
        public string Modelpath = @"\data\x\Uav\1800_slow.X";
        private ITableLabel dynamicTableLabel = null;
        //  public string Flystate = null;
        public void HideParameterView()
        {
            if (flySimulateParameterViewModel != null)
            {
                flySimulateParameterViewModel.HideParameterView();
            }
        }
        public void fly(List<IPoint> flyPointList)
        {
            FlyPointList = flyPointList;
            if (dynamicObject == null)
            {
                rootId = GviMap.ObjectManager.GetProjectTree().RootID;
                dynamicObject = GviMap.ObjectManager.CreateDynamicObject(rootId);
            }
            else if (dynamicObject != null)
            {
                //dynamicObject.CrsWKT
                dynamicObject = null;
                rootId = GviMap.ObjectManager.GetProjectTree().RootID;
                dynamicObject = GviMap.ObjectManager.CreateDynamicObject(rootId);
                dynamicObject.ClearWaypoints();
            }
           
            dynamicObject.CrsWKT = WKTString.WGS_84_WKT;
            dynamicObject.TurnSpeed = double.Parse("5000");
            if(flySimulateParameterViewModel==null)
            {
                flySimulateParameterViewModel = new FlySimulateParameterViewModel();
            }
            
            flySimulateParameterViewModel.ObjectStartFly -= StartFly;
            flySimulateParameterViewModel.ObjectStartFly += StartFly;
            flySimulateParameterViewModel.ObjectPauseFly -= PauseFly;
            flySimulateParameterViewModel.ObjectPauseFly += PauseFly;
            flySimulateParameterViewModel.ObjectReSetFly -= StopFly;
            flySimulateParameterViewModel.ObjectReSetFly += StopFly;
            flySimulateParameterViewModel.ObjecReStartFly -= ReSetObjectspeed;
            flySimulateParameterViewModel.ObjecReStartFly += ReSetObjectspeed;
            flySimulateParameterViewModel.ObjectFollowerPer -= FlyFollowerPer;
            flySimulateParameterViewModel.ObjectFollowerPer += FlyFollowerPer;
            flySimulateParameterViewModel.ObjectOverLook -= FlyOverLook;
            flySimulateParameterViewModel.ObjectOverLook += FlyOverLook;
            flySimulateParameterViewModel.ObjectReSetSpeed -= ReSetSpeed;
            flySimulateParameterViewModel.ObjectReSetSpeed += ReSetSpeed;
            flySimulateParameterViewModel.ObjectRemove -= RemoveFlight;
            flySimulateParameterViewModel.ObjectRemove += RemoveFlight;
            flySimulateParameterViewModel.ShowParameterView(flyPointList);
           
            for (int i = 0; i < flyPointList.Count; i++)
            {
                var point = flyPointList[i];               
                dynamicObject.AddWaypoint2(point,Convert.ToDouble( flySimulateParameterViewModel._flyspeed));
              //  GviMap.ObjectManager.CreateRenderPoint(point, null, rootId);

            }
          
           
            LoadPath(flyPointList);
            LoadPlane(flyPointList);
            LoadTableLabel();
            //  LoadTableLabel();
            // GviMap.Camera.FlyToObject(this.skinMeshPlane.Guid, gviActionCode.gviActionFollowBehindAndAbove);
            GviMap.Camera.LookAtEnvelope(rpolyline.Envelope);
            //  this.cbFlyMode.SelectedIndex = 6;

            // 获取设置参数
            // cbAutoRepeat.Checked = dynamicObject.AutoRepeat;//

            //  comboxMotionStyle.SelectedIndex = getIndexFromEnum(dynamicObject.MotionStyle);

            dynamicObject.MotionStyle = getMotionStyleFromString("Hover");
            dynamicObject.Index = 0;
            dynamicObject.AutoRepeat = true;
            dynamicObject.Play();
            UpdateTableLabel();
        }
        private void LoadPath(List<IPoint> FlyPointList)
        {
            GviMap.TraceLinePolyManager.Clear();
          
            if (gfactory == null)
                gfactory = new GeometryFactory();

            fde_polyline = (IPolyline)gfactory.CreateGeometry(gviGeometryType.gviGeometryPolyline,
                gviVertexAttribute.gviVertexAttributeZ);
            fde_polyline.SpatialCRS = GviMap.SpatialCrs;
          
            for (int i = 0; i < FlyPointList.Count; i++)
            {
                var point = FlyPointList[i];
              
                fde_polyline.AppendPoint(point);
            }
            lineSymbol = new CurveSymbol();
            //  Color color = color.Red;
            //  if (string.IsNullOrEmpty(style))
            //  CurveSym = GviMap.TraceLinePolyManager.CreateCurveSymbol(-3f, color);
            //  lineSymbol.Color = Color.Red;// GviMap.LinePolyManager.CurveSym.Color;  
            lineSymbol.Width = 0;
            rpolyline = GviMap.ObjectManager.CreateRenderPolyline(fde_polyline, lineSymbol, rootId);
            var label = GviMap.ObjectManager.CreateLabel(fde_polyline.Midpoint);
            GviMap.TraceLinePolyManager.AddPoi(rpolyline.Guid.ToString(), 2, label, rpolyline);
        }
        bool isUpdate = true;
       
        private void UpdateTableLabel()
        {
            Random rand = new Random();          
            var shell = ServiceManager.GetService<IShellService>(null).ShellWindow;
            //if(isUpdate == true)
            //{

            //}
            Thread thread = new Thread(() =>
            {
                if(dynamicTableLabel!=null)
                 {
                    dynamicTableLabel.VisibleMask = gviViewportMask.gviViewAllNormalView;
                }               
                while (isUpdate)
                {
                   Thread.Sleep(1000);
                   shell.Dispatcher.Invoke(() =>
                    {

                        if (dynamicTableLabel != null)
                        {
                            dynamicTableLabel.SetRecord(0, 1, (Math.Round(dynamicTableLabel.Position.X, 2)+ rand.Next(100) / 10000.0).ToString());

                            dynamicTableLabel.SetRecord(1, 1, (Math.Round(dynamicTableLabel.Position.Y, 2) + rand.Next(100) / 10000.0).ToString());

                            dynamicTableLabel.SetRecord(2, 1, (Math.Round(dynamicTableLabel.Position.Z, 2) + rand.Next(100) / 10000.0).ToString());
                        }
                    });
                    if (!isUpdate)
                        break;

                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
        public void StartFly()
        {
            dynamicObject?.Play();
            isUpdate = true;
            UpdateTableLabel();
        }
        public void ReSetSpeed(double speed)
        {
            IPoint waypoint = null;
            double OriSpeed;
            for(int i = 0;i < dynamicObject.WaypointsNumber;i++)
            {
                dynamicObject.GetWaypoint2(i,out waypoint, out OriSpeed);
                dynamicObject.ModifyWaypoint2(i,waypoint,speed);
            }
        }
        public void StopFly()
        {
            dynamicObject?.Stop();
            if (dynamicObject != null)
            {
                dynamicObject.Index = 0;
            }
          
        }
        public void PauseFly()
        {
            dynamicObject?.Pause();
            isUpdate = false;
        }
        public void FlyFollowerPer()
        {
            if (skinMeshPlane!=null)
            {
                if (dynamicObject != null)
                {
                    dynamicObject.ViewingDistance = 40;
                }
                GviMap.Camera.FlyToObject(this.skinMeshPlane.Guid, gviActionCode.gviActionFollowBehindAndAbove);
            }
        }
        public void FlyOverLook()
        {
            if (rpolyline != null)
            { 
            GviMap.Camera.LookAtEnvelope(rpolyline.Envelope);
            }
        }
        public void ReSetObjectspeed(List<IPoint> _reFlyPointList,bool viewPerspective)
        {
            isUpdate = false;
            if(dynamicTableLabel!=null)
            {
                dynamicTableLabel.VisibleMask = gviViewportMask.gviViewNone;
            }            
            dynamicObject?.Stop();
            if (dynamicObject != null)
            {
                dynamicObject.Index = 0;
            }
            dynamicObject?.ClearWaypoints();
            for (int i = 0; i < _reFlyPointList.Count; i++)
            {
                var point = _reFlyPointList[i];
                dynamicObject?.AddWaypoint2(point, Convert.ToDouble(flySimulateParameterViewModel._flyspeed));
                //  GviMap.ObjectManager.CreateRenderPoint(point, null, rootId);
            }           
            LoadPath(_reFlyPointList);
            LoadPlane(_reFlyPointList);
            LoadTableLabel();
            //  LoadTableLabel();
            dynamicObject.ViewingDistance = 30;
            if (viewPerspective == true)
            {
                if (rpolyline != null)
                {
                    GviMap.Camera.LookAtEnvelope(rpolyline.Envelope);
                }
            }
            else
            {
                GviMap.Camera.FlyToObject(this.skinMeshPlane.Guid, gviActionCode.gviActionFollowBehindAndAbove);
            }
           
           // 
           
            //  this.cbFlyMode.SelectedIndex = 6;

            // 获取设置参数
            // cbAutoRepeat.Checked = dynamicObject.AutoRepeat;//

            //  comboxMotionStyle.SelectedIndex = getIndexFromEnum(dynamicObject.MotionStyle);

            dynamicObject.MotionStyle = getMotionStyleFromString("Hover");
            dynamicObject.Index = 0;
            dynamicObject.AutoRepeat = true;
            dynamicObject.Play();
            isUpdate = true;
            UpdateTableLabel();
        }
        public void RemoveFlight()
        {
            if(dynamicObject!=null)
            {
                dynamicObject.Stop();
            }          
            GviMap.TraceLinePolyManager.Clear();
           
                for(int i=0;i<guidlist.Count;i++)
                {
                    ISkinnedMesh skin = GviMap.ObjectManager.GetObjectById(guidlist[i]) as ISkinnedMesh;
                    skin.VisibleMask = gviViewportMask.gviViewNone;                     
                }
            guidlist.Clear();
            dynamicTableLabel.VisibleMask = gviViewportMask.gviViewNone;
            flySimulateParameterViewModel?.HideParameterView();
            isUpdate = false;
        }
        private gviDynamicMotionStyle getMotionStyleFromString(string value)
        {
            switch (value)
            {
                case "GroundVehicle":
                    return gviDynamicMotionStyle.gviDynamicMotionGroundVehicle;
                case "Airplane":
                    return gviDynamicMotionStyle.gviDynamicMotionAirplane;
                case "Helicopter":
                    return gviDynamicMotionStyle.gviDynamicMotionHelicopter;
                case "Hover":
                    return gviDynamicMotionStyle.gviDynamicMotionHover;
            }
            return gviDynamicMotionStyle.gviDynamicMotionHover;
        }
        //AppDomain.CurrentDomain.BaseDirectory + @"\data\x\Uav\1550_low3A.x";
        private void LoadPlane(List<IPoint> FlyPointList)
        {
            // 加载直升飞机
            string fileName = AppDomain.CurrentDomain.BaseDirectory + Modelpath;
            if (skinMeshPlane == null|| skinMeshPlane.VisibleMask== gviViewportMask.gviViewNone)
            {
                // 构造ModelPoint
                IGeometryFactory gf = new GeometryFactory();
                mp = gf.CreateGeometry(gviGeometryType.gviGeometryModelPoint, gviVertexAttribute.gviVertexAttributeZ) as IModelPoint;
                mp.ModelName = fileName;
                //var wkt = Wgs84UtmUtil.GetWkt(113);
                mp.SelfScale(20, 20, 20);
                mp.SpatialCRS = (ISpatialCRS)GviMap.CrsFactory.CreateFromWKT(WKTString.WGS_84_WKT);
                // 设置位置
                IMatrix matrix = new Matrix();
                matrix.MakeIdentity();
              //  matrix.SetTranslate(FlyPointList[0].Position);
                mp.FromMatrix(matrix);
                // 创建骨骼动画
                skinMeshPlane = GviMap.ObjectManager.CreateSkinnedMesh(mp, rootId);
                if (skinMeshPlane == null)
                {
                    System.Windows.MessageBox.Show("骨骼动画创建失败！");
                    return;
                }
                skinMeshPlane.Loop = true;
                skinMeshPlane.Play();
                skinMeshPlane.MaxVisibleDistance = 1000;
                skinMeshPlane.ViewingDistance = 100;
                guidlist.Add(skinMeshPlane.Guid);
                // 绑定到运动路径
                IMotionable motion = skinMeshPlane as IMotionable;
                // position.Set(polyLine.GetPoint(0).Position.X, polyLine.GetPoint(0).Position.Y, polyLine.GetPoint(0).Position.Z);
                position.Set(0, 0, 0);
                motion.Bind2(dynamicObject, position, 0, 0, 0);
            }
        }

        private void LoadTableLabel()
        {
            dynamicTableLabel = GviMap.ObjectManager.CreateTableLabel(3, 2, rootId);

            dynamicTableLabel.TitleText = "无人机当前位置";
            dynamicTableLabel.SetRecord(0, 0, "经度:");
            dynamicTableLabel.SetRecord(0, 1, fde_polyline.GetPoint(0).X.ToString());
            dynamicTableLabel.SetRecord(1, 0, "纬度:");
            dynamicTableLabel.SetRecord(1, 1, fde_polyline.GetPoint(0).Y.ToString());
            dynamicTableLabel.SetRecord(2, 0, "高度:");
            dynamicTableLabel.SetRecord(2, 1, fde_polyline.GetPoint(0).Z.ToString());

            dynamicTableLabel.Position = fde_polyline.GetPoint(0);

            dynamicTableLabel.BorderColor = System.Drawing.Color.Black;
            dynamicTableLabel.BorderWidth = 2;
            dynamicTableLabel.TableBackgroundColor = System.Drawing.Color.Red;//;ColorHelper.ARGBToUint(200, 255, 255, 165);
            dynamicTableLabel.TitleBackgroundColor = System.Drawing.Color.Black ;//ColorHelper.ARGBToUint(180, 122, 122, 122);

            // 第一列文本样式
            TextAttribute headerTextAttribute = new TextAttribute();
            headerTextAttribute.TextColor = System.Drawing.Color.White;
            headerTextAttribute.OutlineColor = System.Drawing.Color.Black;
            headerTextAttribute.Font = "细黑";
            headerTextAttribute.Bold = true;
            headerTextAttribute.MultilineJustification = gviMultilineJustification.gviMultilineLeft;
            dynamicTableLabel.SetColumnTextAttribute(0, headerTextAttribute);

            // 第二列文本样式
            TextAttribute contentTextAttribute = new TextAttribute();
            contentTextAttribute.TextColor = System.Drawing.Color.White;
            contentTextAttribute.OutlineColor = System.Drawing.Color.Black;
            contentTextAttribute.Font = "黑体";
            contentTextAttribute.Bold = false;
            contentTextAttribute.MultilineJustification = gviMultilineJustification.gviMultilineLeft;
            dynamicTableLabel.SetColumnTextAttribute(1, contentTextAttribute);

            // 标题文本样式
            TextAttribute capitalTextAttribute = new TextAttribute();
            capitalTextAttribute.TextColor = System.Drawing.Color.White;
            capitalTextAttribute.OutlineColor = System.Drawing.Color.Black;
            capitalTextAttribute.Font = "华文新魏";
            capitalTextAttribute.TextSize = 14;
            capitalTextAttribute.MultilineJustification = gviMultilineJustification.gviMultilineCenter;
            capitalTextAttribute.Bold = true;
            dynamicTableLabel.TitleTextAttribute = capitalTextAttribute;

            // 绑定到运动路径
            IMotionable m = dynamicTableLabel as IMotionable;
            position.Set(0, 0, 2);
            m.Bind2(dynamicObject, position, 0, 0, 0);
        }
    }
}
