using Gvitech.CityMaker.RenderControl;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.Services.HttpService;
using Mmc.Wpf.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mmc.Mspace.IotModule.Mission
{
    public class MissionVModel : CheckedToolItemModel
    {  
        private ObservableCollection<MissionItem> _missionCollection = new ObservableCollection<MissionItem>();
        public ObservableCollection<MissionItem> MissionCollection
        {
            get { return _missionCollection; }
            set
            {
                base.SetAndNotifyPropertyChanged<ObservableCollection<MissionItem>>(ref this._missionCollection, value, "MissionCollection");
            }
        }
        MissionView missionView = new MissionView();
        NewMissionView newMissionView = null;
        public ICommand CreateCmd { get; set; }
        public override void Initialize()
        {
            base.Initialize();
            base.ViewType = ViewType.CheckedIcon;
            missionView.DataContext = this;
            this.CreateCmd = new RelayCommand(OnCreatMission);
        }
        public override void OnChecked()
        {
            base.OnChecked();
            GetMissionList();
            missionView.Show();
        }

        public override void OnUnchecked()//关闭事件
        {
            base.OnUnchecked();
            missionView.Hide();     
        }
        private string _missionNum;
        public string MissionNum
        {
            get { return this._missionNum; }
            set
            {
                _missionNum = value; NotifyPropertyChanged("MissionNum");
            }
        }
        private void GetMissionList()
        {
            // List<ReportType> result = new List<ReportType>();
            string url = MarkInterface.MissionList;

            //if (ReportSearchText == "")
            //{
            //    url = string.Format("{0}?page_size={1}&page={2}", MarkInterface.Getbglist, pageSize, page);
            //}
            //else
            //{
            //    url = string.Format("{0}?page_size={1}&page={2}&keyword={3}", MarkInterface.Getbglist, pageSize, page, ReportSearchText);
            //}                  
            string resStr = HttpServiceHelper.Instance.GetRequest(url);
            GetMissions(resStr);
            //  var list = JsonUtil.DeserializeFromString<List<ReportType>>(resStr);          
            //if (result?.Count > 0)
            //{
            //    //ReportCollection.Clear();
            //    foreach (var account in result)
            //    {                    
            //        ReportCollection.Add(account);
            //    }
            //}
           
        }
        public void GetMissions(string _input)
        {

            using (JsonTextReader reader = new JsonTextReader(new StringReader(_input)))
            {
                JObject obj = (JObject)JToken.ReadFrom(reader);               
                JArray jArray = (JArray)obj["data"];
                MissionCollection = jArray.ToObject<ObservableCollection<MissionItem>>();              
            }
        }
        private void OnCreatMission()
        {
           newMissionView = new NewMissionView();
           newMissionView.Show();
        }
    }
}
