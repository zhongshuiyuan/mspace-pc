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
        private int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; NotifyPropertyChanged("Index"); }
        }


        private string _stake;
        public string Stake
        {
            get => _stake;
            set
            {
                _stake = value;
                NotifyPropertyChanged("Stake");
            }
        }
        private string _lng;
        public string Lng
        {
            get { return _lng; }
            set
            {
                _lng = value;
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

        private string _traces;

        public string Traces
        {
            get => _traces;
            set
            {
                _traces = value;
                NotifyPropertyChanged("Traces");
            }
        }


        private string _height;

        public string Height
        {
            get { return _height; }
            set
            {
                _height = value;
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
