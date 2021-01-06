using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.PoiManagerModule.Models;
using Mmc.Mspace.RegularInspectionModule.model;
using Mmc.Mspace.Services.HttpService;
using Mmc.Mspace.Theme.Pop;
using Mmc.Windows.Utils;
using Mmc.Wpf.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.RegularInspectionModule.ViewModels
{
    public class ComparisonVModel : BaseViewModel
    {
        private ObservableCollection<PipeModel> _periods = new ObservableCollection<PipeModel>();
        /// <summary>
        /// 阶段
        /// </summary>
        public ObservableCollection<PipeModel> Periods
        {
            get { return _periods; }
            set { _periods = value; OnPropertyChanged("Periods"); }
        }

        private List<StakeModel> _staticStakes = new List<StakeModel>();
        public List<StakeModel> StaticStakes
        {
            get { return _staticStakes; }
            set { _staticStakes = value; }
        }
        private List<StakeModel> _filterStakes = new List<StakeModel>();
        public List<StakeModel> FilterStakes
        {
            get { return _filterStakes; }
            set { _filterStakes = value; OnPropertyChanged("FilterStakes"); }
        }
        private ObservableCollection<StakeModel> _stakes = new ObservableCollection<StakeModel>();
        /// <summary>
        /// 中线桩
        /// </summary>
        public ObservableCollection<StakeModel> Stakes
        {
            get { return _stakes; }
            set { _stakes = value; OnPropertyChanged("Stakes"); }
        }
        private PipeModel _selectPipeModel;
        /// <summary>
        /// 管线选中
        /// </summary>
        public PipeModel SelectPipeModel
        {
            get { return _selectPipeModel; }
            set
            {
                _selectPipeModel = value;
                this.FilterStake();
                OnPropertyChanged("SelectPipeModel");
            }
        }

        private StakeModel _selectStakeModel;

        public StakeModel SelectStakeModel
        {
            get { return _selectStakeModel; }
            set 
            {
                _selectStakeModel = value;

                if(_selectStakeModel != null)
                {
                    getStakelist();
                }
                OnPropertyChanged("SelectStakeModel");

            }
        }
        private RelayCommand<object> _getSourceCommand;

        public RelayCommand<object> getSourceCommand
        {
            get { return _getSourceCommand ?? (_getSourceCommand = new RelayCommand<object>((obj)=> {
                if (_selectStakeModel != null)
                {
                    getStakelist();
                }
            })); }
            set { _getSourceCommand = value; }
        }

        private RelayCommand<object> _selectCommand;

        public RelayCommand<object> SelectCommand
        {
            get { return _selectCommand ?? (_selectCommand = new RelayCommand<object>(OnSelectCommand)); }
            set { _selectCommand = value; }
        }
        private RelayCommand<object> _comparisonCommand;

        public RelayCommand<object> ComparisonCommand
        {
            get { return _comparisonCommand ?? (_comparisonCommand = new RelayCommand<object>(OnComparisonCommand)); }
            set { _comparisonCommand = value; }
        }

        public ComparisonVModel()
        {
            this.getSource();
        }
        private void FilterStake()
        {
            this.Stakes = new ObservableCollection<StakeModel>(this.StaticStakes.Where(t => t.Pipe_id == SelectPipeModel.Id).ToList());
        }

        private void OnSelectCommand(object stakeModel)
        {
            if (stakeModel == null) return;
            StakeModel stake = stakeModel as StakeModel;
            stake.IsChecked = !stake.IsChecked;
        }
        /// <summary>
        /// 多期对比
        /// </summary>
        /// <param name="obj"></param>
        private void OnComparisonCommand(object obj)
        {
            if(FilterStakes.Count<1)
            {
                Messages.ShowMessage("当前视角过高或当前地图中没有相关模型！");
                return;
            }
            List<StakeModel> list = FilterStakes.Where(t => t.IsChecked).ToList();
            if(list.Count<1)
            {
                Messages.ShowMessage("请选中正射或倾斜相关模型！");
                return;

            }
        }
        /// <summary>
        /// 获取分屏显示
        /// </summary>
        private void getStakelist()
        {
            Task.Run(() =>
            {
                string stakeList = HttpServiceHelper.Instance.GetRequest(PipelineInterface.stakelist+ "?pipe_id="+SelectPipeModel.Id+"&sn="+SelectStakeModel.Sn);
                this.FilterStakes = JsonUtil.DeserializeFromString<List<StakeModel>>(stakeList);
            });
        }
        private void getSource()
        {
            Task.Run(() =>
            {
                //获取管线
                string periodList = HttpServiceHelper.Instance.GetRequest(PipelineInterface.pipeindex);
                this.Periods = new ObservableCollection<PipeModel>(JsonUtil.DeserializeFromString<List<PipeModel>>(periodList));
                //获取中线桩
                string stakeList = HttpServiceHelper.Instance.GetRequest(PipelineInterface.stakeindex);
                this.StaticStakes = JsonUtil.DeserializeFromString<List<StakeModel>>(stakeList);
                this.Stakes =new ObservableCollection<StakeModel>(StaticStakes);
            });
        }
    }

}
