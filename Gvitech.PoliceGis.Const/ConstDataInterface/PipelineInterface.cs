using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.Const.ConstDataInterface
{
    public class PipelineInterface
    {
        public const string PipeList = "/api/pipe/list";
        public const string SectionList = "/api/section/index";
        public const string PeriodList = "/api/period/index";
        public const string createstake = "/api/stake/create";
        public const string updatestake = "/api/stake/update";// id =${id}  中线桩 编辑
        public const string deletestake = "/api/stake/delete";// id =${id}

   
    }
}
