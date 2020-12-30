using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.IntelligentAnalysisModule.MidPointCheck
{
    public class LineItem
    {
        public bool ischecked { get; set; }
        public string id { get; set; }
        public string Number { get; set; }

        public string name { get; set; }

        public string pipe_id { get; set; }

        public string type_id { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string geom { get; set; }
        public Guid guid { get; set; }
        public bool isVisible { get; set; }
    }
}
