using Mmc.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.Common.Models.pipelines
{
    public class TaskModel:BindableBase
    {
        //"id": "1",
        //    "sn": "202101141124505377",
        //    "type_id": "5",
        //    "name": "日常飞行任务"


        private string _id;

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }
        private string _type_id;
        public string Type_id
        {
            get => _type_id;
            set
            {
                _type_id = value;
                NotifyPropertyChanged("_type_id");
            }
        }

        private string _sn;

        public string Sn
        {
            get { return _sn; }
            set { _sn = value; NotifyPropertyChanged("Id"); }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

    }
}
