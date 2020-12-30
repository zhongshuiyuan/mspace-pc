using System;
using System.Collections.Generic;
using Mmc.Wpf.Mvvm;
using System.Linq;
using System.Text;

namespace Mmc.Mspace.UavModule.Dto
{

    public class DeviceInfo : BindableBase
    {
        public VehicleInfo vehicleInfo { get; set; }

        //existence
        public string goods_id { get; set; }
        public string goods_name { get; set; }
        public string deviceType { get; set; }
        public string deviceName { get; set; }
        public string deviceSerial { get; set; }
        public string deviceHardId { get; set; }
        public string httpStatus { get; set; }
        public string socketStatus { get; set; }
        public string max_voltage { get; set; }
        public string return_voltage { get; set; }
        public string addtime { get; set; }

        //non-existent
        //public string mount_type { get; set; }
    }

    public class VehicleInfo
    {
        //统一输出
        public Int64 unmannedId { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double altitude { get; set; }
        public double height { get; set; }
        public double roll { get; set; }
        public double yaw { get; set; }
        public double pitch { get; set; }
        public double climbRate { get; set; }
        public double distanceToHome { get; set; }
        public double distanceToNext { get; set; }
        public double imuTemp { get; set; }
        public double barometerTemp { get; set; }
        public double satCount { get; set; }
        public Int64 dateTime { get; set; }
        public double groundSpeed { get; set; }
        public double airSpeed { get; set; }
        public double flightDistance { get; set; }
        public double flightTime { get; set; }
        public string flightMode { get; set; }
        public string flightSortie { get; set; }
        public string flightState { get; set; }        
        public double voltage { get; set; }
        public double current { get; set; }
        public double battaryRemain { get; set; }
        public double isLocation { get; set; }

        public double taskId { get; set; }
        public double uid { get; set; }
        public double platformType { get; set; }
        public List<CustomDataInfo> customData { get; set; }
        public dynamic currentMountType { get; set; }
        public dynamic mountInfo { get; set; }
        
        //none
        //public int uavState { get; set; }
        //public string mountType { get; set; }
        //public string videoType { get; set; }
        //public string vUrl { get; set; }
    }

    public class CustomDataInfo
    {
        public string mount { get; set; }
        public string groundStation { get; set; }
        public string battery { get; set; }
    }


}
