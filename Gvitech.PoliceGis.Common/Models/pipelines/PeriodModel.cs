﻿using Mmc.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.Common.Models.pipelines
{
    /// <summary>
    /// 阶段实体
    /// </summary>
    public class PeriodModel: BindableBase
    {
        private string _id;
        private string _sn;
        private string _name;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }


        private string _sort;

        public string Sort
        {
            get { return _sort; }
            set { _sort = value; NotifyPropertyChanged("Sort"); }
        }

        public string Sn
        {
            get => _sn;
            set
            {
                _sn = value;
                NotifyPropertyChanged("Sn");
            }
        }

        private string _start;

        public string Start
        {
            get => _start;
            set
            {
                _start = value;
                NotifyPropertyChanged("Start");
            }
        }

        private string _end;

        public string End
        {
            get => _end;
            set
            {
                _end = value;
                NotifyPropertyChanged("End");
            }
        }


        private string _pipe_id;

        public string Pipe_id
        {
            get => _pipe_id;
            set
            {
                _pipe_id = value;
                NotifyPropertyChanged("Pipe_id");
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        private string _prefix;

        public string Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value;
                NotifyPropertyChanged("Prefix");
            }
        }


        private string _created_at;

        public string Created_at
        {
            get => _created_at;
            set
            {
                _created_at = value;
                NotifyPropertyChanged("Created_at");
            }
        }

        private string _updated_at;

        public string Updated_at
        {
            get => _updated_at;
            set
            {
                _updated_at = value;
                NotifyPropertyChanged("Updated_at");
            }
        }


        private string _is_del;
        public string Is_del
        {
            get => _is_del;
            set
            {
                _is_del = value;
                NotifyPropertyChanged("Is_del");
            }
        }
        private string _section_id;
        public string Section_id
        {
            get => _section_id;
            set
            {
                _section_id = value;
                NotifyPropertyChanged("Section_id");
            }
        }
        private string _lng;
        public string Lng
        {
            get => _lng;
            set
            {
                _lng = value;
                NotifyPropertyChanged("Lng");
            }
        }

        private string _lat;
        public string Lat
        {
            get => _lat;
            set
            {
                _lat = value;
                NotifyPropertyChanged("Lat");
            }
        }

        private string _period_id;
        public string Period_id
        {
            get => _period_id;
            set
            {
                _period_id = value;
                NotifyPropertyChanged("Period_id");
            }
        }

        private string _file;
        public string File
        {
            get => _file;
            set
            {
                _file = value;
                NotifyPropertyChanged("File");
            }
        }

        private string _time;
        public string Time
        {
            get => _time;
            set
            {
                _time = value;
                NotifyPropertyChanged("Time");
            }
        }

        private string _file_type;
        public string File_type
        {
            get => _file_type;
            set
            {
                _file_type = value;
                NotifyPropertyChanged("File_type");
            }
        }

    }
}
