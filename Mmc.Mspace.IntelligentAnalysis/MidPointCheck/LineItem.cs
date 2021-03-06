﻿using Mmc.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.IntelligentAnalysisModule.MidPointCheck
{
    public class LineItem: BindableBase
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; NotifyPropertyChanged("IsChecked"); }
        }

        private bool _isRoot;

        public bool IsRoot
        {
            get { return _isRoot; }
            set { _isRoot = value; NotifyPropertyChanged("IsRoot"); }
        }
        private bool _eyeStatus = false;

        public bool EyeStatus
        {
            get { return _eyeStatus; }
            set { _eyeStatus = value; NotifyPropertyChanged("EyeStatus"); }
        }
        private bool _eyeStatus2 = true;

        public bool EyeStatus2
        {
            get { return _eyeStatus2; }
            set { _eyeStatus2 = value; NotifyPropertyChanged("EyeStatus2"); }
        }


        
        public string id { get; set; }
        public string Number { get; set; }

        public string name { get; set; }
        public string sn { get; set; }

        public string pipe_id { get; set; }
        public string pipe_name { get; set; }
        
        public string type { get; set; }
        public string start { get; set; }
        public string end { get; set; }

        public string start_sn { get; set; }
        public string end_sn { get; set; }
        public string geom { get; set; }
        public Guid guid { get; set; }
        public bool isVisible { get; set; }
    }
}
