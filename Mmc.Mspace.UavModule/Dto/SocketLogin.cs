using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.UavModule.Dto
{

    class SocketLogin : SocketItem
    {

        public int type { get; set; }//del

        public string name { get; set; }//del

        /// <summary>
        /// 公司编号 web或者客户端登录(必传)
        /// </summary>
        public new string systemCode { get; set; } = "MMC";

        /// <summary>
        /// 登录来源(必传) 0地面站,1客户端或web,2Http接口
        /// </summary>
        public new int state { get; set; } = 1;
    }
}
