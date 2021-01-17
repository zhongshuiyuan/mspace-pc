using Mmc.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.PoiManagerModule.Models
{
    public class PipeModel : BindableBase
    {
        //"id": "1",
        //    "sn": "GX65432514564",
        //    "name": "中俄线",
        //    "prefix": "ZDFF",
        //    "created_at": "2020-11-19 16:15:25",
        //    "updated_at": "2020-11-19 16:15:25",
        //    "is_del": "0",



        private string _id;
        private string _sn;
        private string _name;
        private string _map;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
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
        public string Map
        {
            get => _map;
            set
            {
                _map = value;
                NotifyPropertyChanged("Map");
            }
        }

        private string _file;

        public string File
        {
            get { return _file; }
            set { _file  = value; NotifyPropertyChanged("File"); }
        }

        private bool _firstEyeStatus=true;

        public bool FirstEyeStatus
        {
            get { return _firstEyeStatus; }
            set { _firstEyeStatus = value; NotifyPropertyChanged("FirstEyeStatus"); }
        }

        private string _fileType;
        public string FileType
        {
            get =>string.IsNullOrEmpty(Map)?"": Map?.Split('&').Length>1? Map?.Split('&')[1]: "";
            set
            {
                _fileType = value;
                NotifyPropertyChanged("FileType");
            }
        }

        private string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value; NotifyPropertyChanged("Type"); }
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
        private string _nodeName;

        public string NodeName
        {
            get => this.Name;
            set
            {
                _nodeName = this.Name;
                NotifyPropertyChanged("NodeName");
            }
        }
        private string _father;

        public string Father
        {
            get { return _father; }
            set { _father = value; NotifyPropertyChanged("Father"); }
        }

        private string _level;

        public string Level
        {
            get => _level;
            set
            {
                _level = value;
                NotifyPropertyChanged("Level");
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
        
        private ObservableCollection<PipeModel> _child;
        public ObservableCollection<PipeModel> Child
        {
            get { return _child; }
            set { _child = value; NotifyPropertyChanged("Child"); }
        }
    }

}
