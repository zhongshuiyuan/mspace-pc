using Mmc.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.IntelligentAnalysisModule.Models
{
    public class TracingLineModel : BindableBase
    {
        //"id": "1",
        //    "stake": "1",
        //    "lng": "123.152676",
        //    "lat": "20.156451",
        //    "created_at": "2020-12-31 11:12:23",
        //    "updated_at": "2020-12-31 11:12:23",
        //    "is_del": "0",
        //    "traces": "1,2",
        //    "height": "1"
        private int index;

        public int Index
        {
            get { return index; }
            set { index = value; NotifyPropertyChanged("Index"); }
        }


        private string stake;
        public string Stake
        {
            get => stake;
            set
            {
                stake = value;
                NotifyPropertyChanged("Stake");
            }
        }
        private string lng;
        public string Lng
        {
            get { return lng; }
            set
            {
                lng = value;
                NotifyPropertyChanged("Lng");
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

        private string lat;
        public string Lat
        {
            get => lat;
            set
            {
                lat = value;
                NotifyPropertyChanged("Lat");
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

        private StakeModel _poi;

        public StakeModel Poi
        {
            get { return _poi; }
            set { _poi = value; NotifyPropertyChanged("Poi"); }
        }

        private string traces;

        public string Traces
        {
            get => traces;
            set
            {
                traces = value;
                NotifyPropertyChanged("Traces");
            }
        }


        private string height;

        public string Height
        {
            get { return height; }
            set
            {
                height = value;
                NotifyPropertyChanged("Height");
            }
        }

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


        private string sn;

        public string Sn
        {
            get { return sn; }
            set { sn = value; NotifyPropertyChanged("Sn"); }
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; NotifyPropertyChanged("IsChecked"); }
        }

    }


    public class TracingModel
    {
        public string stake;
        public string sn;
        public string id;
        public string height;
        public string lat;
        public string lng;
        public string traces;
        public string start;
        public string end;

    }
}
