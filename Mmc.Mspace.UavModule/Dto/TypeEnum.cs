/*
 string name = Enum.GetName(typeof(TypeEnum.GAS_NAME), i);
 int type = (int)TypeEnum.WEB_CONTROLLER.CMD_TYPE_SERVER_MESSAGE;

 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mmc.Mspace.UavModule.Dto
{
    class TypeEnum
    {
        //New web controller
        public enum N_WEB_CONTROLLER
        {
            //Message Type
            CMD_TYPE_CONNECT_SERVER_GROUND = 103, //控制器与服务器建立连接 
            CMD_TYPE_DISCONNECT_SERVER_GROUND = 104, //控制器与服务器断开连接

            CMD_TYPE_WEB2GROUND = 200, //系统码
            CMD_TYPE_SERVER_HEARTBEAT = 400, //服务器心跳包

            //控制类消息
            CMD_TYPE_CONNECT_SUCCESS = 210, //连接成功
            CMD_TYPE_CONNECT_FAILED = 211, //连接失败
            CMD_TYPE_SENDDATA_SUCCESS = 212, //数据发送成功
            CMD_TYPE_SENDDATA_FAILED = 213, //数据发生失败
            CMD_TYPE_DISCONNECT_SUCCESS = 214, //断开连接成功
            CMD_TYPE_FORMAT_ERROR = 216, //数据格式错误
            CMD_TYPE_REMOET_LOGIN = 217, //异地登录
            CMD_TYPE_NOPERMISSION = 218, //无权限控制
            CMD_TYPE_OFFLINE = 219, //控制方已离线
            CMD_TYPE_STATION_OFFLINE = 220, //地面站退出连接
            CMD_TYPE_DB_CONNECT_FAILED = 299, //数据库连接失败
        }

        public enum MOUNT_NAME_MAP
        {
            [Description("10倍云台")]
            CAM_10 = 267,
            [Description("20倍松下云台")]
            CAM_SONGXIA20 = 283,
            [Description("20倍索尼云台")]
            MOUNT_DROP = 298,
            [Description("智能抛投")]
            CAM_SONY20 = 299,
            [Description("30倍奇蛙云台")]
            CAM_QIWA30 = 315,
            [Description("Gopro5云台")]
            CAM_GOPRO5 = 331,
            [Description("10倍红外双光云台")]
            CAM_BIFOCAL10 = 347,
            [Description("小双光")]
            MOUNT_THUNDERBOLTFIRE = 353,
            [Description("霹雳火FA6")]
            CAM_SMALLBIFOCAL = 363,
            [Description("18倍微光")]
            MOUNT_FN1MOUNT = 369,
            [Description("霹雳火FN1")]
            CAM_LLL18 = 379,
            [Description("35倍微光带跟踪")]
            MOUNT_FB1MOUNT = 385,
            [Description("霹雳火FB1")]
            CAM_LLLFLOW35 = 395,
            [Description("5100云台")]
            MOUNT_FE3MOUNT = 401,
            [Description("霹雳火FE3")]
            CAM_5100 = 411,
            [Description("A7R云台")]
            CAM_A7R = 427,
            [Description("Filr红外")]
            CAM_Filr = 443,
            [Description("微光红外二合一短款")]
            CAM_F1T2FOCAL = 475,
            [Description("海视英科红外")]
            CAM_PGIY1 = 491,
            [Description("微光红外二合一长款")]
            CAM_PG2IF2_LT2Z35 = 507,
            [Description("Dislink")]
            CAM_NONE_CAM = 9999,
        }

        public enum GAS_NAME
        {
            EX = 1,
            CO,
            O2,
            H2,
            CH4,
            C3H8,
            CO2,
            O3,
            H2S,
            SO2,
            NH3,
            CL2,
            ETO,
            HCL,
            PH3,
            HBr,
            HCN,
            AsH3,
            HF,
            Br2,
            NO,
            NO2,
            NOX,
            CLO2,
            SiH4,
            CS2,
            F2,
            B2H6,
            GeH4,
            N2,
            THT,
            C2H2,
            C2H4,
            CH2O,
            LPG,
            HC,
            VOC,//id = 37
            H2O2,
            VOC2,//重复VOC，id = 39
            SF6,
            C7H8,
            C4H6,
            COS,
            N2H4,
            SeH2,
            C8H8,
            C4H8,
            CH2,
        }

        public enum GAS_UNIT_MAP
        {
            LEL,
            VOL,
            PPM,
            PPb,
            mg_m3
        }

        public readonly string[] GAS_UNIT_MAP_ARRY = { "%LEL", "%VOL", "PPM", "PPb", "mg/m3" };

        /// <summary>
        /// 获取枚举项描述信息 例如GetEnumDesc(Days.Sunday)
        /// </summary>
        /// <param name="en">枚举项 如Days.Sunday</param>
        /// <returns></returns>
        private static string GetEnumDesc(Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }

        public static string getMountNameList(DeviceInfo deviceInfo)
        {
            try
            {
                bool line = false;
                var mountName = new StringBuilder();
                dynamic currentMountType = deviceInfo?.vehicleInfo?.currentMountType;                

                if (currentMountType == null)
                    return "";

                foreach (dynamic type in currentMountType)
                {
                    if (line)
                        mountName.AppendLine("|");
                    if (IsNumberic((string)type))                    
                        mountName.AppendLine(getMountName((int)type).ToString());                    
                    else                    
                        mountName.AppendLine((string)type).ToString();
                   
                    line = true;
                }
                return mountName.ToString();
            }
            catch
            {
                return "";
            }
        }

        public static bool IsNumberic(string oText)
        {
            try
            {
                Decimal Number = Convert.ToDecimal(oText);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string getMountName(int mountype)
        {
            string mountName = "";
            switch (mountype)
            {
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_10:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_10);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.MOUNT_DROP:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.MOUNT_DROP);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_5100:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_5100);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_SONGXIA20:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_SONGXIA20);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_SONY20:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_SONY20);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_QIWA30:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_QIWA30);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_GOPRO5:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_GOPRO5);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_BIFOCAL10:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_BIFOCAL10);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_SMALLBIFOCAL:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_SMALLBIFOCAL);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_LLL18:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_LLL18);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_LLLFLOW35:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_LLLFLOW35);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_Filr:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_Filr);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_F1T2FOCAL:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_F1T2FOCAL);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_PGIY1:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_PGIY1);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_PG2IF2_LT2Z35:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_PG2IF2_LT2Z35);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.MOUNT_THUNDERBOLTFIRE:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.MOUNT_THUNDERBOLTFIRE);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.MOUNT_FN1MOUNT:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.MOUNT_FN1MOUNT);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.MOUNT_FB1MOUNT:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.MOUNT_FB1MOUNT);
                    break;
                case (int)TypeEnum.MOUNT_NAME_MAP.CAM_NONE_CAM:
                    mountName = GetEnumDesc(TypeEnum.MOUNT_NAME_MAP.CAM_NONE_CAM);
                    break;
                default:
                    mountName = "Nnknown";
                    break;
            }
            return mountName;
        }
    }
}
