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
        public const string stakelist = "/api/stake/list";
        public const string pipeindex = "/api/pipe/index";//管道列表
        public const string stakeindex = "/api/stake/index";//中线桩列表
        public const string tracinglineList = "/api/tracing-line/index";//描点列表
        public const string tracinglineCreate= "/api/tracing-line/create";//添加
        public const string tracinglinebatch = "/api/tracing-line/batch";//批量添加
        public const string tracingupdate = "/api/tracing/update";//批量添加
        public const string tracingexport = "/api/tracing/export-warn";//导出
        public const string tracingexportfly = "/api/tracing/export-fly";//导出
        public const string taskupload = "/api/task/upload";//上传图片
        public const string taskall = "/api/task/all";//任务列表
        public const string flycreate = "/api/warn/create";//
        






    }

}
