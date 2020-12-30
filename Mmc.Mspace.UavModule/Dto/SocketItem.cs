using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mmc.Mspace.Common.Cache;

namespace Mmc.Mspace.UavModule.Dto
{
    public class SocketItem
    {
        /// <summary>
        /// 系统码(必传)
        /// </summary>
        public int type { get; set; } = 200;

        /// <summary>
        /// 无人机硬件ID
        /// </summary>
        public string deviceHardId { get; set; }

        /// <summary>
        /// 公司编号 web或者客户端登录(必传) 客户端默认为 MMC
        /// </summary>
        public string systemCode { get; set; } = "MMC";

        /// <summary>
        /// 登录来源(必传) 0地面站,1客户端或web,2Http接口
        /// </summary>
        public int state { get; set; } = 1;

        /// <summary>
        /// 客户端或者web控制当前登录用户账号
        /// </summary>
        public string username { get; set; } = CacheData.UserInfo.username;

    }

    public class SocketControl : SocketItem
    {
        public SocketData data { get; set; }
    }
}
