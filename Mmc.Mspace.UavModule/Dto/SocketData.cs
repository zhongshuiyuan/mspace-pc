using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.UavModule.Dto
{
    public class SocketData
    {
        /// <summary>
        /// 动作类型
        /// </summary>
        public int cmdFunction { get; set; }

        /// <summary>
        /// 正负标识
        /// </summary>
        public int cmdState { get; set; }

        /// <summary>
        /// 操作数值
        /// </summary>
        public int cmdValue { get; set; }
    }
}
