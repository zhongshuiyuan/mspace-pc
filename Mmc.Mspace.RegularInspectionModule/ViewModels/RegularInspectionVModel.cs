﻿using Gvitech.CityMaker.FdeCore;
using Gvitech.CityMaker.RenderControl;
using Helpers;
using Mmc.DataSourceAccess;
using Mmc.Framework.Services;
using Mmc.Mspace.Common.Messenger;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Common.Models.pipelines;
using Mmc.Mspace.Common.ShellService;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.IntelligentAnalysisModule.AreaWidth;
using Mmc.Mspace.Models.Inspection;
using Mmc.Mspace.PoiManagerModule.Dto;
using Mmc.Mspace.PoiManagerModule.Models;
using Mmc.Mspace.PoiManagerModule.ViewModels;
using Mmc.Mspace.RegularInspectionModule.Dto;
using Mmc.Mspace.RegularInspectionModule.model;
using Mmc.Mspace.RegularInspectionModule.Views;
using Mmc.Mspace.Services.DataSourceServices;
using Mmc.Mspace.Services.HttpService;
using Mmc.Mspace.Services.LocalConfigService;
using Mmc.Mspace.Theme.Pop;
using Mmc.Windows.Services;
using Mmc.Windows.Utils;
using Mmc.Wpf.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static Mmc.Mspace.Common.CommonContract;

namespace Mmc.Mspace.RegularInspectionModule.ViewModels
{
    /// <summary>
    /// 常态化巡检列表构造器
    /// </summary>
    public class RegularInspectionVModel : BaseViewModel
    {
        private NewInspectionView _newInspectionView;
        public Action updateRenderLayer = null;
        private NewInspectionVModel newInspectionVModel;

        private ObservableCollection<InspectModel> _inspectRegions;
        public ObservableCollection<InspectModel> InspectRegions
        {
            get { return _inspectRegions ?? (_inspectRegions = new ObservableCollection<InspectModel>()); }
            set
            {
                _inspectRegions = value;
                OnPropertyChanged("InspectRegions");
                BiaoduanSource.Clear();
                foreach (var item in InspectRegions)
                {
                    BiaoduanSource.Add(item.Name);
                }
            }
        }

        private ObservableCollection<string> _pipelist;
        /// <summary>
        /// 管道层级列表
        /// </summary>
        public ObservableCollection<string> Pipelist
        {
            get { return _pipelist; }
            set { _pipelist = value; OnPropertyChanged("Pipelist"); }
        }

        private ObservableCollection<PeriodModel> _periods = new ObservableCollection<PeriodModel>();
        /// <summary>
        /// 阶段
        /// </summary>
        public ObservableCollection<PeriodModel> Periods
        {
            get { return _periods; }
            set { _periods = value; OnPropertyChanged("Periods"); }
        }
        private ObservableCollection<SectionModel> _sections = new ObservableCollection<SectionModel>();
        /// <summary>
        /// 标段
        /// </summary>
        public ObservableCollection<SectionModel> Sections
        {
            get { return _sections; }
            set { _sections = value; OnPropertyChanged("Sections"); }
        }

        private DateTime startTime;

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; OnPropertyChanged("StartTime"); }
        }
        private DateTime endTime;

        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; OnPropertyChanged("EndTime"); }
        }
        private PeriodModel _selectPeriodModel;
        /// <summary>
        /// 阶段选中
        /// </summary>
        public PeriodModel SelectPeriodModel
        {
            get { return _selectPeriodModel; }
            set
            {
                _selectPeriodModel = value; OnPropertyChanged("SelectPeriodModel");
            }
        }
        private bool _isNew;
        public bool IsNew
        {
            get { return _isNew; }
            set { _isNew = value; OnPropertyChanged("IsNew"); }
        }

        private SectionModel _selectSectionModel;
        /// <summary>
        /// 标段选中
        /// </summary>
        public SectionModel SelectSectionModel
        {
            get { return _selectSectionModel; }
            set
            {
                _selectSectionModel = value; OnPropertyChanged("SelectSectionModel");
            }
        }

        private ObservableCollection<PipeModel> _pipeModels = new ObservableCollection<PipeModel>();
        public ObservableCollection<PipeModel> PipeModels
        {
            get { return _pipeModels; }
            set
            {
                _pipeModels = value;
                OnPropertyChanged("PipeModels");
            }
        }

        private ObservableCollection<string> _biaoduanSource;

        public ObservableCollection<string> BiaoduanSource
        {
            get { return _biaoduanSource ?? (_biaoduanSource = new ObservableCollection<string>()); }
            set
            {
                _biaoduanSource = value;
                OnPropertyChanged("BiaoduanSource");
            }
        }
        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged("SearchText"); }
        }


        private RelayCommand _searchCommand;

        public RelayCommand SearchCommand
        {
            get { return _searchCommand ?? (_searchCommand = new RelayCommand(OnSearchCommand)); }
            set { _searchCommand = value; }
        }
        private RelayCommand _cancelCommand;

        public RelayCommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(OnCancelCommand)); }
            set { _cancelCommand = value; }
        }


        private RelayCommand<object> _selectCommand;

        public RelayCommand<object> SelectCommand
        {
            get { return _selectCommand ?? (_selectCommand = new RelayCommand<object>(OnSelectCommand)); }
            set { _selectCommand = value; }
        }
        private RelayCommand<object> _checkVideoCommand;

        public RelayCommand<object> CheckVideoCommand
        {
            get { return _checkVideoCommand ?? (_checkVideoCommand = new RelayCommand<object>(OnCheckVideoCommand)); }
            set { _checkVideoCommand = value; }
        }

        private RelayCommand<object> _picCommand;

        public RelayCommand<object> PicCommand
        {
            get { return _picCommand ?? (_picCommand = new RelayCommand<object>(OnPicCommand)); }
            set { _picCommand = value; }
        }
        

        private RelayCommand<object> _importCommand;

        public RelayCommand<object> ImportCommand
        {
            get { return _importCommand ?? (_importCommand = new RelayCommand<object>(OnImportCommand)); }
            set { _importCommand = value; }
        }

        private RelayCommand<object> _addCommand;

        public RelayCommand<object> AddCommand
        {
            get { return _addCommand ?? (_addCommand = new RelayCommand<object>(OnAddCommand)); }
            set { _addCommand = value; }
        }

        private RelayCommand<object> _checkedCommand;

        public RelayCommand<object> CheckedCommand
        {
            get { return _checkedCommand ?? (_checkedCommand = new RelayCommand<object>(OnCheckedCommand)); }
            set { _checkedCommand = value; }
        }
        private RelayCommand<object> _checkReportCommand;

        public RelayCommand<object> CheckReportCommand
        {
            get { return _checkReportCommand ?? (_checkReportCommand = new RelayCommand<object>(OnCheckReportCommand)); }
            set { _checkReportCommand = value; }
        }


        
        private RelayCommand<object> _downloadCommand;

        public RelayCommand<object> DownloadCommand
        {
            get { return _downloadCommand ?? (_checkedCommand = new RelayCommand<object>(OnDownloadCommand)); }
            set { _downloadCommand = value; }
        }


        private RelayCommand<object> _deleteCommand;

        public RelayCommand<object> DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new RelayCommand<object>(OnDeleteCommand)); }
            set { _deleteCommand = value; }
        }


        private RelayCommand<object> _editCommand;

        public RelayCommand<object> EditCommand
        {
            get { return _editCommand ?? (_editCommand = new RelayCommand<object>(OnEditCommand)); }
            set { _editCommand = value; }
        }
        private RelayCommand<object> _reNameCmd;

        public RelayCommand<object> ReNameCmd
        {
            get { return _reNameCmd ?? (_reNameCmd = new RelayCommand<object>(OnReNameCommand)); }
            set { _reNameCmd = value; }
        }

        private RelayCommand<PipeModel> _checkMapCommand;

        public RelayCommand<PipeModel> CheckMapCommand
        {
            get { return _checkMapCommand?? (_checkMapCommand = new RelayCommand<PipeModel>(OnCheckMapCommand)); }
            set { _checkMapCommand = value; }
        }


        private List<IRenderLayer> _renderLayers;

        public RegularInspectionVModel()
        {
            Messenger.Messengers.Register<bool>("AddRegion", (t) =>
              {
                  if (t)
                      UpdateData();
              });

            Messenger.Messengers.Register("LeftListRefresh", () =>
            {
                LoadData();
            });
            if (_renderLayers == null)
            {
                _renderLayers = new List<IRenderLayer>();
                Task.Run(() =>
                {
                    GetMapSource();
                });
            }
        }

        public void MapControlEventManagement(bool OnEvent)
        {
            RegInsDataRenderManager.Instance.MapControlEventManagement(OnEvent);
        }

        private void GetMapSource()
        {
            var tileLayers = DataBaseService.Instance.GetTileLayers();
            var imageLayers = DataBaseService.Instance.GetImageLayers();
            var shpLayers = DataBaseService.Instance.GetShpLayers();
            var actualLayers = DataBaseService.Instance.GetActualityLayers();
            if (shpLayers != null)
            {
                foreach (var item in shpLayers)
                    _renderLayers.Add(item as IRenderLayer);
            }
            if (actualLayers != null)
            {
                foreach (var item in actualLayers)
                    _renderLayers.Add(item as IRenderLayer);
            }
            _renderLayers.AddRange(tileLayers);
            _renderLayers.AddRange(imageLayers);
        }

        private void OnSearchCommand()
        {
            //add  by hengda 
            //InspectRegions = new ObservableCollection<InspectModel>(InspectionService.Instance.GetAllRegion(_searchText).Select(t => RegInsModelConvert.InspectRegionConvert(t)).ToList());
            this.getPipeList();
        }

        private void OnCancelCommand()
        {
            IsNew = false;
            SelectPeriodModel = null;
            SelectSectionModel = null;
            StartTime = DateTime.Now.AddDays(-90);
            EndTime = DateTime.Now;
            this.getPipeList();

           
        }

        private void CloseAdd()
        {
            oneItem = null;
            twoItem = null;
            threeItem = null;
            fatherItem = "";
        }
        /// <summary>
        /// 飞入图层
        /// </summary>
        /// <param name="LayerGuid">图层id</param>
        private void flyToRederLayer(string LayerGuid)
        {
            try
            {
                if (_renderLayers.Count > 0)
                {
                    foreach (var layer in _renderLayers)
                    {
                        if (layer?.Guid == LayerGuid && layer?.Renderable != null)
                        {
                            GviMap.Camera.FlyToObject(layer.Renderable.Guid, gviActionCode.gviActionFlyTo);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SystemLog.Log(e);
            }
        }
        //RenderLayerModel
        private void OnSelectCommand(object renderlayermodel)
        {
            try
            {
                if (renderlayermodel == null) return;
                PipeModel popemodel = renderlayermodel as PipeModel;
             
                if (popemodel.Level!="4") return;
                //定位到图层
                flyToRederLayer(popemodel.Map.Split('&')[0]);
            }
            catch (Exception e)
            {
                SystemLog.Log(e);
            }
        }

        private void OnPicCommand(object obj)
        {
            if (obj == null) return;

            ImageDisplayVModel ImageDisplayVModel = new ImageDisplayVModel();
            PipeModel popemodel = obj as PipeModel;

            if (WarnFileExist(popemodel.File))
                ImageDisplayVModel.ShowImageView(popemodel.File);
        }
        private void OnCheckVideoCommand(object obj)
        {
            if (obj == null) return;

            VideoPlayViewVMdel videoPlayViewVMdel = new VideoPlayViewVMdel();
            PipeModel popemodel = obj as PipeModel;

            if (WarnFileExist(popemodel.File))
                videoPlayViewVMdel.ShowVideoView(popemodel.File);
        }
        private bool WarnFileExist(string filepath)
        {
            if (File.Exists(filepath))
            {
                return true;
            }
            else
            {
                Messages.ShowMessage(Helpers.ResourceHelper.FindKey("FileNotExists"));
                return false;
            }
        }

        public void getPipeList()
        {
            Task.Run(() => {
                string param = "";
                    param = "?section_id=" + SelectSectionModel?.Id+ "&period_id="+SelectPeriodModel?.Id+ "&new="+(IsNew ? 1 : 0) +"&time="+StartTime.ToString("yyyy-MM-dd hh:mm:ss") + "~" + EndTime.ToString("yyyy-MM-dd hh:mm:ss");
                string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.PipeList+ param);
                this.PipeModels = new  ObservableCollection<PipeModel>((JsonUtil.DeserializeFromString<List<PipeModel>>(resStr)));
                this.SetData(PipeModels);
            });
        }

        public void getPipeList2()
        {
            string param = "";
            param = "?section_id=" + SelectSectionModel?.Id + "&period_id=" + SelectPeriodModel?.Id + "&new=" + (IsNew ? 1 : 0) + "&time=" + StartTime.ToString("yyyy-MM-dd hh:mm:ss") + "~" + EndTime.ToString("yyyy-MM-dd hh:mm:ss");
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.PipeList + param);
            this.PipeModels = new ObservableCollection<PipeModel>((JsonUtil.DeserializeFromString<List<PipeModel>>(resStr)));
            this.SetData(PipeModels);
        }
        public void CloseAddWin()
        {
            if (newInspectionVModel != null)
                newInspectionVModel.HideWin();
        }
        private PipeModel addSelect = null;
        private void OnAddCommand( object obj)
        {
            if (_newInspectionView == null)
            {
                _newInspectionView = new NewInspectionView();
                 newInspectionVModel = new NewInspectionVModel();
                newInspectionVModel.HideWin = _newInspectionView.CloseWindow;
                newInspectionVModel.HideWin += CloseAdd;
            }

            this.newInspectionVModel.Sections = this.Sections;
            this.newInspectionVModel.Periods = this.Periods;
            this.newInspectionVModel.PipeModels = this.PipeModels;
          
            this.newInspectionVModel.addRenderLayer = AddData;
            this.newInspectionVModel.updateData = AddUpdate;
            addSelect = obj as PipeModel;
            this.SetSelectItem(PipeModels, obj as PipeModel);

          
            this.newInspectionVModel.SelectPeriodModel = this.Periods.SingleOrDefault(t => t.Id == threeItem.Id);
            this.newInspectionVModel.SelectSectionModel = this.Sections.SingleOrDefault(t => t.Id == twoItem.Id);
            this.newInspectionVModel.SelectPipeModel = oneItem;

            newInspectionVModel.LoadData();
            _newInspectionView.TitleName.Text = "新增影像";
            _newInspectionView.DataContext = newInspectionVModel;
            _newInspectionView.Owner = Application.Current.MainWindow;
            _newInspectionView.Left = 400;
            _newInspectionView.Top = Application.Current.MainWindow.Height * 0.2;
            _newInspectionView.Show();

        }


        private PipeModel oneItem = null;
        private PipeModel twoItem = null;
        private PipeModel threeItem = null;
        private string fatherItem = "";
        private void SetSelectItem( ObservableCollection<PipeModel> list, PipeModel pipeModel)
        {
            //newInspectionVModel.SelectPipeModel = pipeModel;
            //newInspectionVModel.SelectPeriodModel = pipeModel;
            //newInspectionVModel.SelectSectionModel = pipeModel;

            foreach (var item in list)//1级
            {
                if (threeItem != null) return;
                if(item.Level=="1")
                {
                    oneItem = item;
                }
                if (item.Level == "2"&& item.Id== pipeModel.Father)
                {
                    twoItem = item;
                    fatherItem = item.Id;
                }
                if (item.Id == pipeModel.Id && item.Level == "3"&& fatherItem!="")
                {
                    threeItem = item;
                    return;
                }
                if (item.Child!=null&&item.Child.Count>0)
                {
                    SetSelectItem(new ObservableCollection<PipeModel>(item.Child), pipeModel);
                }
            }
        }
        private void AddUpdate( PipeModel pipeModel)
        {
            pipeModel.Father = addSelect.Id;
            pipeModel.Level = "4";
            AddItem(PipeModels, pipeModel);
            addSelect = null;
        }
        private void EditUpdate(PipeModel pipeModel)
        {
             getPipeList2();
             addSelect = null;
        }
        private void EditItem(ObservableCollection<PipeModel> list, PipeModel pipeModel)
        {
            Application.Current.Dispatcher.Invoke(() => {
                foreach (var item in list)//1级
                {

                    if (item.Child != null && item.Child.Count > 0)
                    {
                        var re = item.Child.Where(t => t.Id == pipeModel.Id).ToList();
                        if (re.Count > 0)
                        {
                            pipeModel.Father = item.Id;
                            re[0].Name= pipeModel.Name;
                            break;
                        }
                        EditItem(new ObservableCollection<PipeModel>(item.Child), pipeModel);
                    }
                }
            });
        }
        private void AddItem(ObservableCollection<PipeModel> list, PipeModel pipeModel)
        {
            Application.Current.Dispatcher.Invoke(()=> {
                foreach (var item in list)//1级
                {
                    if (item.Child != null && item.Child.Count > 0)
                    {
                        var re = item.Child.Where(t => t.Id == pipeModel.Father && t.Level == "3").ToList();
                        if (re.Count > 0)
                        {
                            re[0].Child.Add(pipeModel);
                            break;
                        }
                        AddItem(new ObservableCollection<PipeModel>(item.Child), pipeModel);
                    }
                }
            });
        }
        private void DeleteItem(ObservableCollection<PipeModel> list, PipeModel pipeModel)
        {
            foreach (var item in list)//1级
            {
                if (item.Child != null && item.Child.Count > 0)
                {
                    var re = item.Child.Where(t => t.Id == pipeModel.Id).ToList();
                    if (re.Count>0)
                    {
                        item.Child.Remove(re[0]);
                        break;
                    }
                    DeleteItem(new ObservableCollection<PipeModel>(item.Child), pipeModel);
                }
            }
        }
        private void SetEyesItem(ObservableCollection<PipeModel> list, PipeModel pipeModel)
        {
            foreach (var item in list)//1级
            {
             
                if (item.Id == pipeModel.Id )
                {
                    item.FirstEyeStatus = !item.FirstEyeStatus;
                    return;
                }
                if (item.Child != null && item.Child.Count > 0)
                {
                    SetSelectItem(new ObservableCollection<PipeModel>(item.Child), pipeModel);
                }
            }
        }
        private string FatherId = "";
        private void SetData(ObservableCollection<PipeModel> list)
        {
            foreach (var item in list)//1级
            {
                if (item.Level == "2")
                {
                    FatherId = item.Id;
                }
                if (item.Level == "3")
                {
                    item.Father = FatherId;
                }
                if (item.Child!=null&&item.Child.Count > 0)
                {
                    SetData(new ObservableCollection<PipeModel>(item.Child));
                }
            }
        }
        private void AddData(IRenderLayer renderLayer)
        {
            _renderLayers.Add(renderLayer);
            GetMapSource();
            this.updateRenderLayer();
        }
        private void OnImportCommand(object parameter)
        {
            //if (parameter == null) return;
            //InspectModel inspectModel = parameter as InspectModel;
            //if (_regInsImportDataView==null)
            //{
            //    _regInsImportDataView = new RegInsImportDataView();
            //    _regInsImportDataVModel = new RegInsImportDataVModel();
            //    _regInsImportDataVModel.CloseWindow = CloseWindow;
            //    _regInsImportDataVModel.UpdateDataList = UpdateData;
            //    _regInsImportDataView.DataContext = _regInsImportDataVModel;
            //}
            //_regInsImportDataVModel.currentUnitId = inspectModel.Id;
            //_regInsImportDataVModel.currentUnitName = inspectModel.Name;
            //_regInsImportDataView.Owner= Application.Current.MainWindow;
            //_regInsImportDataView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //_regInsImportDataView.Show();

            //if (_regInsImportDataView == null)
            //    _regInsImportDataVModel = new RegInsImportDataVModel();
            var inspect = parameter as InspectModel;
            if (inspect == null) return;
           RegInsDataRenderManager.Instance.ImportInspectData(inspect);
            //if (success)
            //    LoadData();
        }

        /// <summary>
        /// 点击按钮显示
        /// </summary>
        /// <param name="parameter"></param>
        private void OnCheckedCommand(object parameter)
        {
            var inspect = parameter as InspectModel;
            if (inspect == null) return;
            RegInsDataRenderManager.Instance.OpenInspectData(inspect);

        }
        /// <summary>
        /// 查看报告
        /// </summary>
        /// <param name="parameter"></param>
        private void OnCheckReportCommand(object parameter)
        {
            var inspect = parameter as PipeModel;
            if (inspect == null) return;
            AreaWidthVModel areaWidthVModel = new AreaWidthVModel();
            areaWidthVModel.Radius = inspect.Width;
            areaWidthVModel.Master_trace = inspect.Master_trace;
            areaWidthVModel.Trace_id = inspect.Trace_id;
            areaWidthVModel.Files = inspect.File;
            areaWidthVModel.IsChecked = true;
        }

        private void OnDownloadCommand(object parameter)
        {
            var inspect = parameter as InspectModel;
            if (inspect == null) return;
            RegInsDataRenderManager.Instance.OpenInspectData(inspect);
        }

        private void OnEditCommand(object parameter)
        {
            if (parameter == null)
            {
                Messages.ShowMessage(ResourceHelper.FindKey("SelectedDeleteItem"));
                return;
            }
            PipeModel popemodel = parameter as PipeModel;
            if (popemodel.Level != "4") return;

            if (_newInspectionView == null)
            {
                _newInspectionView = new NewInspectionView();
                newInspectionVModel = new NewInspectionVModel();
                newInspectionVModel.HideWin = _newInspectionView.CloseWindow;
                newInspectionVModel.HideWin += CloseAdd;
            }
            newInspectionVModel.Name = popemodel.Name;
            newInspectionVModel.UploadText = popemodel.File;
            newInspectionVModel.LoadFiles = popemodel.File;
            
            newInspectionVModel.EditItem = popemodel;

            this.newInspectionVModel.Sections = this.Sections;
            this.newInspectionVModel.Periods = this.Periods;
            this.newInspectionVModel.PipeModels = this.PipeModels;
            this.newInspectionVModel.addRenderLayer = AddData;
            this.newInspectionVModel.updateData = EditUpdate;
            this.newInspectionVModel.typeString = popemodel.Type;
            this.newInspectionVModel.SelectPeriodModel = this.Periods.SingleOrDefault(t => t.Id == popemodel.Pipe_id);
            this.newInspectionVModel.SelectSectionModel = this.Sections.SingleOrDefault(t => t.Id == popemodel.Section_id);
            this.newInspectionVModel.SelectPipeModel = this.PipeModels[0];
            newInspectionVModel.LoadData();
            this.newInspectionVModel.StartPoi = this.newInspectionVModel.StakeModels.Where(t => t.Id == popemodel.Start).ToList()[0];
            this.newInspectionVModel.EndPoi = this.newInspectionVModel.StakeModels2.Where(t => t.Id == popemodel.End).ToList()[0];
            this.newInspectionVModel.TaskSelectItem = this.newInspectionVModel.TaskAll.Where(t => t.Id == popemodel.Task_id).ToList()[0];
            _newInspectionView.TitleName.Text = "编辑影像";
            _newInspectionView.DataContext = newInspectionVModel;
            _newInspectionView.Owner = Application.Current.MainWindow;
            _newInspectionView.Left = 400;
            _newInspectionView.Top = Application.Current.MainWindow.Height * 0.2;
            _newInspectionView.Show();
        }

        private void OnDeleteCommand(object parameter)
        {
            if (parameter == null)
            {
                Messages.ShowMessage(ResourceHelper.FindKey("SelectedDeleteItem"));
                return;
            }
            PipeModel popemodel = parameter as PipeModel;
            if (popemodel.Level != "4") return;
            //定位到图层
            var dr = Messages.ShowMessageDialog("提示", "是否确定删除当前选中项？");
            if (dr)
            {
                bool success = HttpServiceHelper.Instance.PostRequestForStatus(PipelineInterface.deletestake + "?id=" + popemodel.Id, "");
                if(success)
                {
                    Messages.ShowMessage("删除成功");
                    DeleteDataSource(popemodel.Map.Split('&')[0]);;
                    //this.getPipeList();
                    DeleteItem(PipeModels, popemodel);
                    GetMapSource();
                }
            }
        }
        /// <summary>
        /// 删除数据源
        /// </summary>
        /// <param name="LayerGuid">图层id</param>
        public void DeleteDataSource(string LayerGuid)
        {
            try
            {
                if (_renderLayers.Count > 0)
                {
                    foreach (var layer in _renderLayers)
                    {
                        if (layer?.Guid == LayerGuid) // remove sigle data
                        {
                            if (Messages.ShowMessageDialog(Helpers.ResourceHelper.FindKey("Deletelayer"), string.Format(Helpers.ResourceHelper.FindKey("Deletelayerconfirm"), layer.AliasName)))
                            {
                                DataBaseService.Instance.RemoveSingleLayer(layer);
                                _renderLayers.Remove(layer as IRenderLayer);
                                //var tileLayers = DataBaseService.Instance.GetTileLayers();
                                // add by hengda this.UpdateLeftViewLayer();
                                break;
                            }
                        }
                        else if (layer is DisplayLayer) // remove collection data 
                        {
                            var disLyr = layer as DisplayLayer;
                            if (disLyr != null && disLyr.Fc.DataSource.Guid.ToString() == LayerGuid)
                            {
                                string[] tempArr = disLyr.Fc.DataSource.ConnectionInfo.Database.Split('\\');
                                if (Messages.ShowMessageDialog(Helpers.ResourceHelper.FindKey("Deletedatasource"), string.Format(Helpers.ResourceHelper.FindKey("Deletedatasourceconfirm"), tempArr[tempArr.Length - 1])))
                                {
                                    var disPlayLayers = new List<IDisplayLayer>();
                                    var disPlayGuids = new List<string>();
                                    foreach (var item in _renderLayers)
                                    {
                                        if (item.LayerType == RenderLayerType.FeatureLayer)
                                        {
                                            var disLyr1 = item as DisplayLayer;
                                            if (disLyr1 != null && disLyr1.Fc.DataSource.Guid.ToString() == LayerGuid)
                                            {
                                                disPlayLayers.Add(disLyr1);
                                                disPlayGuids.Add(disLyr1.Guid);
                                            }
                                        }
                                    }
                                    DataBaseService.Instance.RemoveFDbDataSource(disPlayLayers);
                                    foreach (var item in disPlayLayers)
                                        _renderLayers.Remove(item as IRenderLayer);

                                    // add  by hengda  this.UpdateLeftViewLayer();
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
            }

        }

        /// <summary>
        /// 去掉node 上面的展示 
        /// </summary>
        /// <param name="obj"></param>
        private void DeleteNode(PipeModel obj,ObservableCollection<PipeModel> PipeModels)
        {
            for (int i = 0; i < PipeModels.Count; i++)
            {
                if (PipeModels[i].Child.Count > 0)
                {
                    DeleteNode(obj, PipeModels[i].Child);
                }
                //管线ID
                if (PipeModels[i].Id == obj.Id)
                {
                    PipeModels.Remove(PipeModels[i]);
                }
            }
        }

        private void OnCheckMapCommand(PipeModel obj)
        {
            if (obj == null) return;
            SetEyesItem(PipeModels,obj);
            //图层
            setRederLayerVisible(obj.Map.Split('&')[0], obj.FirstEyeStatus);
        }

     
        /// <summary>
        /// 设置图层是否可见
        /// </summary>
        /// <param name="LayerGuid">图层guid</param>
        /// <param name="viewPortIndex">图层视口序号，单屏状态为15,多屏状态下为视口序号，第一屏为0，第二为1，依此类推</param>
        /// <param name="isVisilbe"></param>
        public void setRederLayerVisible(string LayerGuid,  bool isVisilbe)
        {
            if (_renderLayers.Count > 0)
            {
                foreach (var layer in _renderLayers)
                {

                    if (layer.Guid == LayerGuid)
                    {
                        var renderable = layer.Renderable;
                        renderable?.SetVisibleMask(GviMap.Viewport.ViewportMode,0, isVisilbe);
                    }
                }
            }
        }
        ChangeRegInsNameVModel ChangeRegInsNameVModel = null;
        private void OnReNameCommand(object parameter)
        {
            if (parameter == null)
            {
                Messages.ShowMessage("该数据不存在");//ResourceHelper.FindKey("SelectedDeleteItem")
                return;
            }
            InspectModel inspectModel = parameter as InspectModel;
            if (ChangeRegInsNameVModel != null)
            {
                ChangeRegInsNameVModel?.changeRegInsNameView.Close();
            }
            ChangeRegInsNameVModel = new ChangeRegInsNameVModel(inspectModel, inspectModel.Name);
            ChangeRegInsNameVModel.UpdateName -= EditQueryName;
            ChangeRegInsNameVModel.UpdateName += EditQueryName;
            ChangeRegInsNameVModel.OpenEditName();
            //tree1
            //int index = InspectRegions.IndexOf(inspectModel);
            //InspectRegions.Remove(inspectModel);
            //RegInsDataRenderManager.Instance.DeleteInspectData(inspectModel);
            //inspectModel.Name = "121";
            //InspectRegions.Insert(index,inspectModel);
          
        }
        public void EditQueryName(InspectModel _inspectModel,string _name)
        {
            RegInsDataRenderManager.Instance.ChangeInspectDataName(_inspectModel, _name);
            UpdateData();
            ChangeRegInsNameVModel?.CloseEditName();
        }
        public void UpdateData(InspectModel inspectModel)
        {
            UpdateData();
        }

        protected override void Loaded()
        {
            //InspectRegions.Add(MockNewInspectRegionData());
            LoadData();
            base.Loaded();
        }

        private void LoadData()
        {
            UpdateData();
            this.getPipeList();
            if (InspectRegions.Count == 0)
                Messenger.Messengers.Notify("CreateNewInspection", true);

            Task.Run(() =>
            {
                //获取阶段
                string periodList = HttpServiceHelper.Instance.GetRequest(PipelineInterface.PeriodList);
                List<PeriodModel> res1 = JsonUtil.DeserializeFromString<List<PeriodModel>>(periodList);
                res1.Insert(0, new PeriodModel
                {
                    Name = "全部"
                });
                this.Periods = new ObservableCollection<PeriodModel>(res1);
                //获取标段
                string sectionList = HttpServiceHelper.Instance.GetRequest(PipelineInterface.SectionList);
                List<SectionModel> res = JsonUtil.DeserializeFromString<List<SectionModel>>(sectionList);
                res.Insert(0, new SectionModel
                {
                    Name = "全部"
                });
                this.Sections = new ObservableCollection<SectionModel>(res);
            });
        }

        private void UpdateData()
        {
            InspectRegions = new ObservableCollection<InspectModel>(InspectionService.Instance.GetAllRegion().Select(t => RegInsModelConvert.InspectRegionConvert(t)).ToList());
        }

        protected override void Unloaded()
        {
            base.Unloaded();
        }

    }
}
