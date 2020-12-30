using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;
using Mmc.Mspace.Theme.Pop;
using Mmc.Mspace.UavModule.Dto;
using Mmc.Framework.Services;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Common.ShellService;
using Mmc.Mspace.Const.ConstDataBase;
using Mmc.Mspace.Const.ConstPath;
using Mmc.Mspace.Services.HttpService;
using Mmc.Windows.Services;
using Mmc.Windows.Utils;
using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.Utils;

namespace Mmc.Mspace.UavModule.UavTracing
{
    public class UavTracingModel : CheckedToolItemModel
    {

        private const double EARTH_RADIUS = 6378137;
        private const double LIMIT_DISTANCE = 200;
        private ICurveSymbol _curveSymbol = null;
        private IDynamicObject _dynamicObject = null;
        private bool _isFollow = true;
        private bool _isStop = false;
        private IPolyline _line = null;
        private IMotionable _motionable = null;
        private IMotionable _motionableTableLabel = null;
        private IMotionPath _motionPath = null;
        private IModelPoint _mp;

        private IRenderPolyline _rLine = null;
        private ISkinnedMesh _skinMesh = null;
        //private IRenderModelPoint _skinMesh = null;
        private IMultiPoint _multiPoint = null;
        private IRenderMultiPoint _rpoint = null;
        private ISimplePointSymbol _pointSym = null;

        //choose altitude model
        private bool _isAbsoluteAltitude = true;

        private ITableLabel _dynamicTableLabel = null;

        public DeviceInfo deviceInfo;

        [XmlIgnore]
        public DeviceInfo deviceInfoRealTime
        {
            get { return deviceInfo; }
            set { deviceInfo = value; }
        }

        [XmlIgnore]
        public bool IsFollow
        {
            get { return this._isFollow; }
            set
            {
                base.SetAndNotifyPropertyChanged<bool>(ref this._isFollow, value, "IsFollow");
                this.FollowToUav(_isFollow);
            }
        }

        public Action<DeviceInfo> OnTracking { get; set; }

        public void FollowToUav(bool isFollow)
        {
            // _isFollow = isFollow;
            if (_isFollow)
            {
                if (_skinMesh != null)
                    GviMap.Camera.FlyToObject(_skinMesh.Guid, gviActionCode.gviActionFollowBehindAndAbove);
            }
            else
            {
                IVector3 v3 = null; IEulerAngle angle = null;
                GviMap.Camera.GetCamera(out v3, out angle);
                GviMap.Camera.SetCamera(v3, angle, gviSetCameraFlags.gviSetCameraNoFlags);
            }
        }

        public override void Initialize()
        {
            deviceInfo = new DeviceInfo();
            base.Initialize();
            base.ViewType = ViewType.CheckedIcon;
            _curveSymbol = new CurveSymbol();
        }

        public override void OnChecked()
        {
            if (deviceInfo?.httpStatus == "1")
            {
                base.OnChecked();
                StartMotionPath();
            }
            else
                Messages.ShowMessage(Helpers.ResourceHelper.FindKey("DeviceNotOnline"));
        }

        public override void OnUnchecked()
        {
            try
            {
                base.OnUnchecked();
                _isStop = true;
                ReleaseRObj();
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
            }
        }

        private IPoint CreateLocalPoint(DeviceInfo deviceInfo)
        {
            var startPt = GviMap.GeoFactory.CreatePoint(gviVertexAttribute.gviVertexAttributeZ);
            if (_isAbsoluteAltitude)
                startPt.SetPostion(deviceInfo.vehicleInfo.longitude, deviceInfo.vehicleInfo.latitude, Math.Round(deviceInfo.vehicleInfo.height, 2));
            else
                startPt.SetPostion(deviceInfo.vehicleInfo.longitude, deviceInfo.vehicleInfo.latitude, Math.Round(deviceInfo.vehicleInfo.altitude, 2));
            return startPt;
        }

        private IModelPoint CreateModelPoint(IVector3 startPt, int catId = 1)
        {
            string fileName;
            var mp = GviMap.GeoFactory.CreateGeometry(gviGeometryType.gviGeometryModelPoint, gviVertexAttribute.gviVertexAttributeZ) as IModelPoint;
            var matrix = new Matrix();

            switch (catId)
            {
                case 1:
                    fileName = AppDomain.CurrentDomain.BaseDirectory + @"\data\x\Uav\1800_slow.X";
                    break;
                case 20:
                    fileName = AppDomain.CurrentDomain.BaseDirectory + @"\data\x\Uav\ship.X";
                    //设置缩放比例
                    mp.SelfScale(0.005, 0.005, 0.005);
                    mp.SelfScale(0.05, 0.05, 0.05);
                    break;
                default:
                    fileName = AppDomain.CurrentDomain.BaseDirectory + @"\data\x\Uav\1800_slow.X";
                    break;
            }
            mp.ModelName = fileName;
            mp.SpatialCRS = GviMap.SpatialCrs;
            // 设置位置
            matrix.MakeIdentity();
            matrix.SetTranslate(startPt);
            mp.FromMatrix(matrix);
            return mp;
        }

        private IEulerAngle GetEulerAngle(DeviceInfo deviceInfo)
        {
            var v3 = new EulerAngle();
            if (!(string.IsNullOrEmpty(deviceInfo.vehicleInfo.yaw.ToString()) || string.IsNullOrEmpty(deviceInfo.vehicleInfo.pitch.ToString()) || string.IsNullOrEmpty(deviceInfo.vehicleInfo.roll.ToString())))
            {
                v3.Set(double.Parse(deviceInfo.vehicleInfo.yaw.ToString()), double.Parse(deviceInfo.vehicleInfo.pitch.ToString()), double.Parse(deviceInfo.vehicleInfo.roll.ToString()));
                return v3;
            }
            return null;
        }

        private bool GetUavInfo(UavTrace uavTrace, out IVector3 location, out IEulerAngle angle)
        {
            location = null;
            angle = null;
            if (uavTrace == null)
                return false;
            location = new Vector3();
            angle = new EulerAngle();
            if (_isAbsoluteAltitude)
                location.Set(uavTrace.longitude, uavTrace.latitude, uavTrace.height);
            else
                location.Set(uavTrace.longitude, uavTrace.latitude, uavTrace.altitude);

            if (!(string.IsNullOrEmpty(uavTrace.yaw) || string.IsNullOrEmpty(uavTrace.pitch) || string.IsNullOrEmpty(uavTrace.roll)))
            {
                angle.Set(double.Parse(uavTrace.yaw), double.Parse(uavTrace.pitch), double.Parse(uavTrace.roll));
            }
            else
            {
                return false;
            }
            return true;
        }

        private bool IsSamePoint(IVector3 ptA, IVector3 ptB)
        {
            return ptA.X == ptB.X && ptA.Y == ptB.Y && ptA.Z == ptB.Z;
        }

        private void ReleaseRObj()
        {
            try
            {
                //释放内存
                _multiPoint?.ReleaseComObject();
                _multiPoint = null;
                if (_rpoint != null)
                {
                    GviMap.ObjectManager.DeleteObject(_rpoint.Guid);
                    GviMap.ObjectManager.ReleaseRenderObject(_rpoint);
                    _rpoint = null;
                }
                if (_dynamicTableLabel != null)
                {
                    GviMap.ObjectManager.DeleteObject(_dynamicTableLabel.Guid);
                    GviMap.ObjectManager.ReleaseRenderObject(_dynamicTableLabel);
                    _dynamicTableLabel = null;
                }

                if (_line != null)
                {
                    //_line?.RemovePoints(0, _line.PointCount - 1);
                    for (int i = 0; i < _line.PointCount; i++)
                    {
                        var pt = _line.GetPoint(i);
                        pt.ReleaseComObject();
                    }
                    _line?.ReleaseComObject();
                    _line = null;
                }
                if (_rLine != null)
                {
                    GviMap.ObjectManager.DeleteObject(_rLine.Guid);
                    GviMap.ObjectManager.ReleaseRenderObject(_rLine);
                    _rLine = null;
                }
                _mp?.ReleaseComObject();
                if (_motionPath != null)
                {
                    _motionable?.Unbind();
                    _motionableTableLabel?.Unbind();
                    _motionPath.Stop();
                    //_motionPath.ClearWaypoints();//add by hengda 暂时注释，当打开多个无人机轨迹时，关闭无人机界面时报内存损坏
                    GviMap.ObjectManager.DeleteObject(_motionPath.Guid);
                    // GviMap.ObjectManager.ReleaseRenderObject(_motionPath);
                    _motionPath = null;
                }
                if (_skinMesh != null)
                {
                    GviMap.ObjectManager.DeleteObject(_skinMesh.Guid);
                    GviMap.ObjectManager.ReleaseRenderObject(_skinMesh);
                    _skinMesh = null;
                }
            }
            catch (Exception ex)
            {
                SystemLog.WriteLog(ex.ToString());
            }

        }

        private string RequestUavTrace(HttpService httpservice, string url)
        {
            try
            {
                httpservice.Token = HttpServiceUtil.Token;
                var result = httpservice.HttpRequestAsync(url);
                return result;
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
                if (ex?.HResult == -2146233088 || ex?.HResult == -2146233079)
                {
                    throw ex;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="lat1">第一点纬度</param>
        /// <param name="lng1">第一点经度</param>
        /// <param name="lat2">第二点纬度</param>
        /// <param name="lng2">第二点经度</param>
        /// <returns>mile</returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);
            double radLng1 = Rad(lng1);
            double radLat2 = Rad(lat2);
            double radLng2 = Rad(lng2);
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
            return result;
        }

        /// <summary>
        /// To Rad
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return (double)d * Math.PI / 180d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private dynamic IsNullOrEmpty(dynamic str)
        {
            return string.IsNullOrEmpty((string)str) ? "0" : str;
        }

        /// <summary>
        /// 确定dynamic类型数据对象是否存在某属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public static bool IsPropertyExist(dynamic data, string propertyname)
        {
            if (data is ExpandoObject)
                return ((IDictionary<string, object>)data).ContainsKey(propertyname);
            return data.GetType().GetProperty(propertyname) != null;
        }

        /// <summary>
        /// 无人机实时动态路径
        /// 注意：运动物体的飞行速度不能大于实际点的飞行速度，否则三维引擎会报错
        /// </summary>
        private void StartMotionPath()
        {
            _isStop = false;
            _curveSymbol.Width = -4;//三个像素
            _curveSymbol.Color = ColorConvert.UintToColor(GviColors.Yellow);

            IEulerAngle angle = null;
            IVector3 lastPt = new Vector3();
            var shell = ServiceManager.GetService<IShellService>(null).ShellWindow;
            var json = JsonUtil.DeserializeFromFile<dynamic>(AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
            string url = string.Format("{0}/api/aircraft/new-uav-data?deviceHardId={1}", json.poiUrl, deviceInfo.deviceHardId);
            var httpservice = new HttpService();
            httpservice.Token = HttpServiceUtil.Token;

            var uavResult = string.Empty;
            dynamic uavResultJson;
            //获取无人机位置信息
            try
            {
                //uavResult = RequestUavTrace(httpservice, url);
                //获取无人机位置信息
                uavResult = httpservice.RequestService(url, method: "POST");
            }
            catch (Exception ex)
            {
                if (ex?.HResult == -2146233088 || ex?.HResult == -2146233079)//处理断网或切换网络异常
                    return;
            }

            uavResultJson = JsonUtil.DeserializeFromString<dynamic>(uavResult);

            if ((bool)uavResultJson.status)
                deviceInfo = updateVehicleinfo(uavResultJson);
            else
            {
                Messages.ShowMessage((string)uavResultJson.message);
                return;
            }

            if (deviceInfo == null)
            {
                Messages.ShowMessage(Helpers.ResourceHelper.FindKey("Nolocationretry"));
                return;
            }        

            this.OnTracking?.Invoke(deviceInfoRealTime);

            IPoint startPt = CreateLocalPoint(deviceInfo);
            lastPt = startPt.Position;
            //创建临时dian
            var tempPt = CreateLocalPoint(deviceInfo);
            //创建运动物体
            if (_motionPath == null)
            {
                _motionPath = GviMap.ObjectManager.CreateMotionPath();
                _motionPath.CrsWKT = GviMap.SpatialCrs.AsWKT();
            }
            var scale = new Vector3();
            //scale.Set(0.01, 0.01, 0.01);
            scale.Set(2, 2, 2);
            angle = GetEulerAngle(deviceInfo);
            _motionPath.AddWaypoint(startPt.Position, angle, scale, 0);

            // 创建运动轨迹线
            _line = GviMap.GeoFactory.CreateGeometry(gviGeometryType.gviGeometryPolyline, gviVertexAttribute.gviVertexAttributeZ) as IPolyline;
            _line.SpatialCRS = GviMap.SpatialCrs;
            _line.AppendPoint(startPt);
            _rLine = GviMap.ObjectManager.CreateRenderPolyline(_line, _curveSymbol);


            // 创建multipoint 
            _multiPoint = GviMap.GeoFactory.CreateGeometry(gviGeometryType.gviGeometryMultiPoint,
                gviVertexAttribute.gviVertexAttributeZ) as IMultiPoint;
            _multiPoint.AddPoint(startPt);
            _multiPoint.SpatialCRS = GviMap.SpatialCrs;
            _pointSym = new SimplePointSymbol();
            _pointSym.Size = 5;
            _pointSym.FillColor = ColorConvert.UintToColor(GviColors.OrangeRed);
            _pointSym.OutlineColor = ColorConvert.UintToColor(GviColors.Red);
            _rpoint = GviMap.ObjectManager.CreateRenderMultiPoint(_multiPoint, _pointSym, GviMap.ProjectTree.RootID);


            // 创建模型的骨骼动画
            var rotation = new EulerAngle();
            var curPt = new Vector3();
            _mp = CreateModelPoint(startPt.Position, deviceInfo == null ? 1 : int.Parse(deviceInfo.deviceType));
            {
                _skinMesh = GviMap.ObjectManager.CreateSkinnedMesh(_mp, GviMap.ProjectTree.RootID);
                if (_skinMesh == null)
                {
                    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("Creationfailed"));
                    return;
                }
                _skinMesh.Envelope.SetByEnvelope(_mp.Envelope);
                _skinMesh.Loop = true;
                _skinMesh.Play();
                _skinMesh.MaxVisibleDistance = 1000;
                _skinMesh.ViewingDistance = 100;

                // _skinMesh = GviMap.ObjectManager.CreateRenderModelPoint(_mp, new ModelPointSymbol() { }, GviMap.ProjectTree.RootID);
                if (_skinMesh == null)
                {
                    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("Creationfailed"));
                    return;
                }
                _skinMesh.Envelope.SetByEnvelope(_mp.Envelope);
                //_skinMesh.Loop = true;
                //_skinMesh.Play();
                _skinMesh.MaxVisibleDistance = 1000;
                _skinMesh.ViewingDistance = 20;

                //先直飞到运动物体
                GviMap.Camera.FlyTime = 0;
                GviMap.Camera.LookAtEnvelope(_skinMesh.Envelope);

                //设置相机跟随运动物体
                GviMap.Camera.FlyToObject(_skinMesh.Guid, gviActionCode.gviActionFollowBehindAndAbove);
            }

            // 绑定到运动路径
            _motionable = _skinMesh as IMotionable;
            var position = new Vector3();
            position.Set(0, 0, 0);
            _motionable.Bind(_motionPath, position, 0, 0, 0);
            double timeCount = 0;

            //window
            LoadTableLable();

            Thread thread = new Thread(() =>
            {
                var timeSpan = 500; //500ms
                while (true)
                {
                    Thread.Sleep(timeSpan);
                    try
                    {
                        uavResult = httpservice.RequestService(url, method: "POST");
                    }
                    catch (Exception ex)
                    {
                        if (ex?.HResult == -2146233088 || ex?.HResult == -2146233079) //处理断网或切换网络异常
                        {
                            _isStop = true;
                            break;
                        }
                    }

                    if (uavResult == null || deviceInfo == null)
                        continue;

                    uavResultJson = JsonUtil.DeserializeFromString<dynamic>(uavResult);
                    if ((bool)uavResultJson.status)
                    {
                        deviceInfo = updateVehicleinfo(uavResultJson);
                    }

                    if (deviceInfo == null)
                        continue;

                    //增加运动物体的位置点
                    tempPt = CreateLocalPoint(deviceInfo);
                    if (IsSamePoint(lastPt, tempPt.Position))
                        continue;
                    //获取与上次坐标的距离
                    var distanceTo = GetDistance(tempPt.Position.X, tempPt.Position.Y, lastPt.X, lastPt.Y);
                    //if (distanceTo >= LIMIT_DISTANCE)
                    //{
                    //    if (_isStop)
                    //        break;
                    //    else
                    //        continue;
                    //}

                    //不合理的坐标点丢弃
                    //if (!tempPt.Position.Valid() && tempPt.Position.X == 0.0)
                    //    continue;

                    timeCount += (timeSpan / 1000.0);
                    //获取姿态
                    angle = GetEulerAngle(deviceInfo);
                    _line?.AppendPoint(tempPt);
                    shell.Dispatcher.Invoke(() =>
                    {
                        if (_motionPath == null)
                            return;
                        if (_line.PointCount <= 1)
                            return;
                        this.OnTracking?.Invoke(deviceInfoRealTime);
                        ////插值做平滑
                        //var spitCount = 5;
                        //var pts = Dispase(lastPt, tempPt.Position, spitCount);
                        //foreach (var item in pts)
                        //{
                        //    tempPt = item.ToPoint(GviMap.GeoFactory, GviMap.SpatialCrs);
                        //    _multiPoint.AddPoint(tempPt);
                        //    _motionPath.AddWaypoint(tempPt.Position, angle, scale, timeCount / spitCount);
                        //}

                        _multiPoint.AddPoint(tempPt);
                        _motionPath.AddWaypoint(tempPt.Position, angle, scale, timeCount);

                        if (_motionPath.WaypointsNumber == 2)
                        {
                            GviMap.Camera.FlyTime = 8.0;//恢复默认
                            _motionPath.Index = 0;
                            _motionPath.Play();
                            //设置相机跟随运动物体
                            FollowToUav(_isFollow);
                        }
                        else if (_motionPath.WaypointsNumber > 2)
                        {
                            _motionPath.Index = _motionPath.WaypointsNumber - 1;
                            _motionPath.Play();
                        }
                        //获取姿态
                        // GviMap.Camera.FlyToObject(_skinMeshPlane.Guid, gviActionCode.gviActionFollowBehindAndAbove);//后上方
                        // GviMap.Camera.FlyToObject(_skinMeshPlane.Guid, gviActionCode.gviActionFollowAbove);//上方跟随
                        // GviMap.Camera.FlyToObject(_skinMeshPlane.Guid, gviActionCode.gviActionFollowBehindAndAbove);//上方跟随
                        //增加位置点到运动轨迹
                        if (_line.PointCount > 1)
                        {
                            if (_rLine == null)
                                _rLine = GviMap.ObjectManager.CreateRenderPolyline(_line, _curveSymbol);
                            _rLine.SetFdeGeometry(_line);
                        }
                        if (_rpoint != null)
                            _rpoint.SetFdeGeometry(_multiPoint);

                        this.OnFrame();
                    });
                    if (_isStop)
                        break;
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private DeviceInfo updateVehicleinfo(dynamic uavResultJson)
        {
            Object datav = uavResultJson.data;
            string str = uavResultJson.data.ToString();
            bool datab = string.IsNullOrEmpty(str);
            if(datab || str == "[]")
                return null;

            if (uavResultJson?.data == null )
                return null;

            var vehicleInfo = new VehicleInfo();
            deviceInfo.vehicleInfo = vehicleInfo;

            deviceInfo.vehicleInfo.unmannedId = Convert.ToInt64(IsNullOrEmpty(uavResultJson?.data?.unmannedId));
            deviceInfo.vehicleInfo.latitude = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.latitude));
            deviceInfo.vehicleInfo.longitude = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.longitude));
            deviceInfo.vehicleInfo.altitude = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.altitude));
            deviceInfo.vehicleInfo.height = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.height));
            deviceInfo.vehicleInfo.roll = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.roll));
            deviceInfo.vehicleInfo.yaw = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.yaw));
            deviceInfo.vehicleInfo.pitch = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.pitch));
            deviceInfo.vehicleInfo.climbRate = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.climbRate));
            deviceInfo.vehicleInfo.distanceToHome = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.distanceToHome));
            deviceInfo.vehicleInfo.distanceToNext = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.distanceToNext));
            deviceInfo.vehicleInfo.imuTemp = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.imuTemp));
            deviceInfo.vehicleInfo.barometerTemp = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.barometerTemp));
            deviceInfo.vehicleInfo.satCount = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.satCount));
            deviceInfo.vehicleInfo.dateTime = (Int64)IsNullOrEmpty(uavResultJson?.data?.dateTime);
            deviceInfo.vehicleInfo.groundSpeed = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.groundSpeed));
            deviceInfo.vehicleInfo.airSpeed = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.airSpeed));
            deviceInfo.vehicleInfo.flightDistance = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.flightDistance));
            deviceInfo.vehicleInfo.flightTime = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.flightTime));
            deviceInfo.vehicleInfo.flightMode = IsNullOrEmpty(uavResultJson?.data?.flightMode);
            deviceInfo.vehicleInfo.flightSortie = IsNullOrEmpty(uavResultJson?.data?.flightSortie);
            deviceInfo.vehicleInfo.flightState = IsNullOrEmpty(uavResultJson?.data?.flightState);
            deviceInfo.vehicleInfo.voltage = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.voltage));
            deviceInfo.vehicleInfo.current = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.current));
            deviceInfo.vehicleInfo.battaryRemain = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.battaryRemain));
            deviceInfo.vehicleInfo.isLocation = Convert.ToDouble(IsNullOrEmpty(uavResultJson?.data?.isLocation));
            deviceInfo.vehicleInfo.currentMountType = uavResultJson?.data?.currentMountType;
            deviceInfo.vehicleInfo.mountInfo = uavResultJson?.data?.mountInfo;

            //deviceInfo.vehicleInfo.uavState = Convert.ToInt16(IsNullOrEmpty(uavResultJson?.data?.uavState));
            //deviceInfo.vehicleInfo.mountType = IsNullOrEmpty(uavResultJson?.data?.mountType);
            //deviceInfo.vehicleInfo.videoType = IsNullOrEmpty(uavResultJson?.data?.videoType);
            //deviceInfo.vehicleInfo.vUrl = IsNullOrEmpty(uavResultJson?.data?.vUrl);

            return deviceInfo;
        }


        /// <summary>
        /// 离散平滑
        /// </summary>
        /// <param name="vStart"></param>
        /// <param name="vEnd"></param>
        /// <param name="spitCount">离散线采样（默认5个）</param>
        /// <returns></returns>
        public List<IVector3> Dispase(IVector3 vStart, IVector3 vEnd, int spitCount = 5)
        {
            List<IVector3> pts = new List<IVector3>();
            var vector = vEnd.Subtract(vStart);
            vector.Normalize();//单一化

            var length = vStart.GetDistance(vEnd);
            pts.Add(vStart);
            for (int j = 0; j < spitCount; j++)
            {
                var temVect = vStart.Add(vector.Multiply(length * (j + 1)));
                pts.Add(temVect);
            }

            return pts;
        }

        private void OnFrame()
        {
            if (this._dynamicTableLabel != null && this._skinMesh != null)
            {
                _dynamicTableLabel.SetRecord(0, 1, String.Format("{0:F8}", this._skinMesh.ModelPoint.Position.X));
                _dynamicTableLabel.SetRecord(1, 1, String.Format("{0:F8}", this._skinMesh.ModelPoint.Position.Y));
                _dynamicTableLabel.SetRecord(2, 1, String.Format("{0:F2}", this._skinMesh.ModelPoint.Position.Z));
            }
        }

        private void LoadTableLable(int catId = 1)
        {
            _dynamicTableLabel = GviMap.AxMapControl.ObjectManager.CreateTableLabelWith2Col(3, 50, 95);

            switch (catId)
            {
                case 1:
                    _dynamicTableLabel.TitleText = Helpers.ResourceHelper.FindKey("Currentposition").ToString();
                    break;
                case 20:
                    _dynamicTableLabel.TitleText = Helpers.ResourceHelper.FindKey("CurrentpositionForShip").ToString();
                    break;
                default:
                    _dynamicTableLabel.TitleText = Helpers.ResourceHelper.FindKey("Currentposition").ToString();
                    break;
            }
            _dynamicTableLabel.SetRecord(0, 0, Helpers.ResourceHelper.FindKey("Longitude"));
            _dynamicTableLabel.SetRecord(0, 1, String.Format("{0:F8}", _line.GetPoint(0).X));
            _dynamicTableLabel.SetRecord(1, 0, Helpers.ResourceHelper.FindKey("Latitude"));
            _dynamicTableLabel.SetRecord(1, 1, String.Format("{0:F8}", _line.GetPoint(0).Y));
            _dynamicTableLabel.SetRecord(2, 0, Helpers.ResourceHelper.FindKey("Height"));
            _dynamicTableLabel.SetRecord(2, 1, String.Format("{0:F2}", _line.GetPoint(0).Z));
            _dynamicTableLabel.DepthTestMode = gviDepthTestMode.gviDepthTestDisable;
            _dynamicTableLabel.Position = _line.GetPoint(0);
            _motionableTableLabel = _dynamicTableLabel as IMotionable;
            _motionableTableLabel.Bind(_motionPath, new Vector3() { X = 0, Y = 0, Z = 1 }, 0, 0, 0);
        }       

    }
}