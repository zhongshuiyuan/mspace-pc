using Mmc.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.RegularInspectionModule.model
{
   public class StakeModel : BindableBase
    {

            //"id": "6",
            //    "sn": "1~22",
            //    "name": "",
            //    "prefix": "1~22",
            //    "pipe_id": "6",
            //    "section_id": "2",
            //    "created_at": "2021-01-04 18:43:12",
            //    "updated_at": "2021-01-04 18:43:12",
            //    "is_del": "0",
            //    "lng": "0",
            //    "lat": "0",
            //    "period_id": "1",
            //    "file": "123123",
            //    "time": "2020-12-30 00:00:00",
            //    "start": "1",
            //    "end": "22",
            //    "file_type": "0",
            //    "level": 4
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
        private string _time;
        public string Time
        {
            get { return _time; }
            set { _time = value;
                NotifyPropertyChanged("Time");
            }
        }

        private string _pipe_id;

        public string Pipe_id
        {
            get { return _pipe_id; }
            set { _pipe_id = value;
                NotifyPropertyChanged("Pipe_id");
            }
        }

        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value;
                NotifyPropertyChanged("Id");
            }
        }


        private string _sn;

        public string Sn
        {
            get { return _sn; }
            set { _sn = value; NotifyPropertyChanged("Id"); }
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; NotifyPropertyChanged("IsChecked"); }
        }

    }
}
