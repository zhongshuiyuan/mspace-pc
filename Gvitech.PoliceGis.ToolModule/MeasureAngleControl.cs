using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.RenderControl;
using Mmc.Framework.Draw;
using Mmc.Framework.Services;
using Mmc.MathUtil;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Theme.Pop;
using Mmc.Windows.Framework.Commands;
using Mmc.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.ToolModule
{
   public class MeasureAngleControl: ToolItemModel
    {
        ICurveSymbol curveSymbol;
        private DrawCustomerUC drawCustomer;
        IPolyline GeometryPolyline = null;
        bool start = true;
        List<Guid> guids = new List<Guid>();
        public override void Initialize()
        {
            base.Initialize();          
            //ase.ViewType = ViewType.Custom;
            base.Command = new RelayCommand(() =>
            {
                //(ServiceManager.GetService<IShellService>(null).ShellWindow.Content as FrameworkElement).Visibility = Visibility.Collapsed;
                //ServiceManager.GetService<IShellService>(null).LeftPanel.Visibility = System.Windows.Visibility.Collapsed;
                //ServiceManager.GetService<IShellService>(null).ShellMenu.Visibility = System.Windows.Visibility.Collapsed;
                //InkPadWindow inkPadWindow = new InkPadWindow();
                //inkPadWindow.Owner = ServiceManager.GetService<IShellService>(null).ShellWindow;
                //inkPadWindow.Width = (inkPadWindow.MaxWidth = inkPadWindow.Owner.MaxWidth);
                //inkPadWindow.Height = (inkPadWindow.MaxHeight = inkPadWindow.Owner.MaxHeight);
                //inkPadWindow.Background = (Brush)new BrushConverter().ConvertFromString("#0F000000");
                //inkPadWindow.WindowStyle = WindowStyle.None;
                //inkPadWindow.ResizeMode = ResizeMode.NoResize;
                //inkPadWindow.AllowsTransparency = true;
                //inkPadWindow.ShowInTaskbar = false;
                //inkPadWindow.Show();
                //inkPadWindow.InkPadCloseCompleted += delegate (object sender, EventArgs ars)
                //{
                //    ServiceManager.GetService<IShellService>(null).LeftPanel.Visibility = System.Windows.Visibility.Visible;
                //    (ServiceManager.GetService<IShellService>(null).ShellWindow.Content as FrameworkElement).Visibility = Visibility.Visible;
                //    ServiceManager.GetService<IShellService>(null).ShellWindow.WindowState = WindowState.Maximized;
                //    ServiceManager.GetService<IShellService>(null).ShellWindow.Activate();
                //    ServiceManager.GetService<IShellService>(null).ShellMenu.Visibility = System.Windows.Visibility.Visible;
                //};
                if(start)
                {
                    RegisterDrawLine();                   
                    start = !start;
                }
                else
                {
                    Clear();                    
                    start = !start;
                }                              
            });
        }
        private void RegisterDrawLine()
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
        private void PlanPolylineDraw_OnDrawFinished(object sender, object result)
        {
            var rPolyline = result as IRenderPolyline;
            var polyLine = rPolyline.GetFdeGeometry() as IPolyline;
            GeometryPolyline = polyLine;
            if (polyLine == null || polyLine.PointCount < 2)
            {
                RCDrawManager.Instance.PolylineDraw.OnDrawFinished -= PlanPolylineDraw_OnDrawFinished;
                RCDrawManager.Instance.PolylineDraw.UnRegister(GviMap.AxMapControl);
                return;
            }

            curveSymbol = GviMap.TraceLinePolyManager.CreateCurveSymbol(0.1f, System.Drawing.Color.Yellow, gviDashStyle.gviDashSmall);
            IRenderPolyline renderPolyline = GviMap.ObjectManager.CreateRenderPolyline(polyLine, curveSymbol);
           // guids.Add(renderPolyline.Guid);
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
            var point3 = polyLine.GetPoint(0);
            if(polyLine.GetPoint(0).Z> polyLine.GetPoint(1).Z)
            {
                point3.X = polyLine.GetPoint(0).X;
                point3.Y = polyLine.GetPoint(0).Y;
                point3.Z = polyLine.GetPoint(1).Z;
            }
            else
            {
                point3.X = polyLine.GetPoint(1).X;
                point3.Y = polyLine.GetPoint(1).Y;
                point3.Z = polyLine.GetPoint(0).Z;
            }
            polyLine.AddPointAfter(1, point3);
            var pt = polyLine.GetPoint(0);
            var prjWkt = Wgs84UtmUtil.GetWkt(pt.X);
            if (!string.IsNullOrEmpty(prjWkt))
                polyLine.ProjectEx(prjWkt);
            // this.Len = polyLine.Length;
            polyLine.Project(GviMap.SpatialCrs);
            //  label.VisibleMask = gviViewportMask.gviViewAllNormalView;
            renderPolyline.VisibleMask = gviViewportMask.gviViewAllNormalView;
            guids.Add(renderPolyline.Guid);
            CacluteAngle();
            //ShellB_Angle
        }
        private void CacluteAngle()
        {
            if(GeometryPolyline?.PointCount!=3)
            {
                Messages.ShowMessage("描点数不等于2，无法计算，请重新绘制！");
                Clear();
                start = true;
                return;
            }
            else
            {
                for(int i = 1;i< GeometryPolyline.PointCount-1;i++)
                {
                    IPoint FirstPoi = GeometryPolyline.GetPoint(i-1);
                    IPoint SecondPoi = GeometryPolyline.GetPoint(i);
                    IPoint ThirdPoi = GeometryPolyline.GetPoint(i + 1);
                    IPoint FP = FirstPoi.Clone2(gviVertexAttribute.gviVertexAttributeZ) as IPoint;
                    IPoint SP = SecondPoi.Clone2(gviVertexAttribute.gviVertexAttributeZ) as IPoint;
                    IPoint TP = ThirdPoi.Clone2(gviVertexAttribute.gviVertexAttributeZ) as IPoint;
                    var prjWkt = Wgs84UtmUtil.GetWkt(FirstPoi.X);
                    if (!string.IsNullOrEmpty(prjWkt))
                    FP.ProjectEx(prjWkt);
                    SP.ProjectEx(prjWkt);
                    TP.ProjectEx(prjWkt);

                    double c = DisTanceTwoPoint(FP ,SP);
                    double a = DisTanceTwoPoint(SP, TP);
                    double b = DisTanceTwoPoint(FP, TP);                  
                    var angle = (a*a + c*c - b*b) / (2 * a* c);
                    var temp = Math.Acos(angle)*180/Math.PI;
                    temp =  Math.Round(temp, 2);
                    ILabel label = GviMap.ObjectManager.CreateLabel();
                    label.Position = SecondPoi;
                    label.Text = Convert.ToString(temp)+"°";
                    guids.Add(label.Guid);
                }                
                //GeometryPolyline.
            }
            
        }
        private void Clear()
        {
            foreach (var item in guids)
            {
                GviMap.ObjectManager.DeleteObject(item);
            }
            guids.Clear();
        }
        private double DisTanceTwoPoint(IPoint poi, IPoint poi2)
        {
            return Math.Sqrt((poi.X - poi2.X) * (poi.X - poi2.X) + (poi.Y - poi2.Y) * (poi.Y - poi2.Y) + (poi.Z - poi2.Z) * (poi.Z - poi2.Z));
        }
    }
}
