using Mmc.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.IotModule.Mission
{
    public class MissionItem: BindableBase
    {
        public string id { get; set; }
        public string type_id { get; set; }
        public string sn { get; set; }
        public string period_id { get; set; }
        public string cycle { get; set; }
        public string address { get; set; }
        public string geom { get; set; }
        public string stake_start { get; set; }
        public string stake_end { get; set; }
        public string real_stake_start { get; set; }
        public string real_stake_end { get; set; }
        public string file { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string started_at { get; set; }
        public string end_at { get; set; }
        public string flow { get; set; }
        public string is_del { get; set; }
        public string user_id { get; set; }
        public string audit_id { get; set; }
        public string status_name { get; set; }
        public string type_name { get; set; }
        public string audit_name { get; set; }
        public string self_state { get; set; }          
    }
}
