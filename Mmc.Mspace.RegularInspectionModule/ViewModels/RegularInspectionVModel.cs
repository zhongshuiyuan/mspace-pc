using Gvitech.CityMaker.FdeCore;
using Gvitech.CityMaker.RenderControl;
using Helpers;
using Mmc.DataSourceAccess;
using Mmc.Framework.Services;
using Mmc.Mspace.Common.Messenger;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Common.ShellService;
using Mmc.Mspace.Const.ConstDataInterface;
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
    public class RegularInspectionVModel:BaseViewModel
    {
        private NewInspectionView _newInspectionView;

        private NewInspectionVModel newInspectionVModel;

        private ObservableCollection<RenderLayerModel> _mapSource;
        public ObservableCollection<RenderLayerModel> MapSource
        {
            get { return _mapSource ?? (_mapSource = new ObservableCollection<RenderLayerModel>()); }
            set { _mapSource = value; OnPropertyChanged("MapSource"); }
        }
        ObservableCollection<RenderLayerModel> tempMapSource = null;

        private ObservableCollection<InspectModel> _inspectRegions;
        public ObservableCollection<InspectModel> InspectRegions
        {
            get { return _inspectRegions ?? (_inspectRegions = new ObservableCollection<InspectModel>()); }
            set
            {
                _inspectRegions = value;
                OnPropertyChanged("InspectRegions");
                BiaoduanSource.Clear();
                foreach ( var item in InspectRegions)
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

        private ObservableCollection<PeriodModel> _periods=new ObservableCollection<PeriodModel>();
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

        private DateTime startTime = DateTime.Now.AddDays(-90);

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; OnPropertyChanged("StartTime"); }
        }
        private DateTime endTime= DateTime.Now;

        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; OnPropertyChanged("EndTime"); }
        }

        private List<PipeModel> _pipeModels = new List<PipeModel>();
        public List<PipeModel> PipeModels
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
            get { return _searchCommand??(_searchCommand =new RelayCommand(OnSearchCommand)); }
            set { _searchCommand = value; }
        }

        private RelayCommand<object> _selectCommand;

        public RelayCommand<object> SelectCommand
        {
            get { return _selectCommand ?? (_selectCommand = new RelayCommand<object>(OnSelectCommand)); }
            set { _selectCommand = value; }
        }


        private RelayCommand<object> _importCommand;

        public RelayCommand<object> ImportCommand
        {
            get { return _importCommand??(_importCommand=new RelayCommand<object>(OnImportCommand)); }
            set { _importCommand = value; }
        }

        private RelayCommand<object> _addCommand;

        public RelayCommand<object> AddCommand
        {
            get { return _addCommand??(_addCommand=new  RelayCommand<object> (OnAddCommand)); }
            set { _addCommand = value; }
        }

        private RelayCommand<object> _checkedCommand;

        public RelayCommand<object> CheckedCommand
        {
            get { return _checkedCommand ?? (_checkedCommand = new RelayCommand<object>(OnCheckedCommand)); }
            set { _checkedCommand = value; }
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
            get { return _deleteCommand??(_deleteCommand=new RelayCommand<object>(OnDeleteCommand)); }
            set { _deleteCommand = value; }
        }
        private RelayCommand<object> _reNameCmd;

        public RelayCommand<object> ReNameCmd
        {
            get { return _reNameCmd ?? (_reNameCmd = new RelayCommand<object>(OnReNameCommand)); }
            set { _reNameCmd = value; }
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
            List<RenderLayerModel> list = new List<RenderLayerModel>();
            //三维模型
            var modelRenderLayers = new RenderLayerModel()
            {
                ChildCount = 0,
                MenuLevel = "1",
                IsDisplay = false,
                IsChecked = true,
                DataPath = (ImageSource)Helpers.ResourceHelper.FindResourceByKey("3dIcon"),
                Name = Helpers.ResourceHelper.FindKey("3dmodel") + "(0)",
                AliasName = Helpers.ResourceHelper.FindKey("3dmodel"),
                Guid = Guid.Parse("00000000-0000-0000-0000-000000000100").ToString(),
                LayerType = Common.CommonContract.RenderLayerType.DataSetGroupLayer,
                Rederlayers = new List<RenderLayerModel>(),
            };

            list.Add(modelRenderLayers);

            var actualLayers = DataBaseService.Instance.GetActualityLayers();
            if (actualLayers != null)
            {
                Dictionary<string, Tuple<IDataSource, List<IDisplayLayer>>> dicModel = new Dictionary<string, Tuple<IDataSource, List<IDisplayLayer>>>();

                foreach (var item in actualLayers)
                {
                    var key = item.Fc.DataSource.Guid.ToString();
                    if (!dicModel.ContainsKey(key))
                    {
                        dicModel.Add(key, new Tuple<IDataSource, List<IDisplayLayer>>(item.Fc.DataSource, new List<IDisplayLayer>()));

                    }
                    dicModel[key].Item2.Add(item);
                }
                modelRenderLayers.Name = Helpers.ResourceHelper.FindKey("3dmodel") + "(" + dicModel.Count + ")";
                foreach (var key in dicModel.Keys)
                {
                    modelRenderLayers.HasPathData = true;
                    modelRenderLayers.ChildCount++;
                    var dsName = dicModel[key].Item1.ConnectionInfo.GetDataSourceName();
                    var dataSourceRenderLayers = new RenderLayerModel()
                    {
                        HasPathData = true,
                        MenuLevel = "2",
                        IsDisplay = true,
                        IsChecked = true,
                        ChildCount = 0,
                        ParentName = modelRenderLayers.Name,
                        IsLocal = !dicModel[key].Item1.IsNetServer(),
                        Name = (!dicModel[key].Item1.IsNetServer() ? Helpers.ResourceHelper.FindKey("LocalDataMark") : Helpers.ResourceHelper.FindKey("ServerDataMark")) + dsName,
                        AliasName = (!dicModel[key].Item1.IsNetServer() ? Helpers.ResourceHelper.FindKey("LocalDataMark") : Helpers.ResourceHelper.FindKey("ServerDataMark")) + dsName,
                        Guid = key,
                        LayerType = Common.CommonContract.RenderLayerType.GroupLayer,
                        Rederlayers = new List<RenderLayerModel>()
                    };
                    list.Add(dataSourceRenderLayers);
                    foreach (var item in dicModel[key].Item2)
                    {
                        dataSourceRenderLayers.ChildCount++;
                        list.Add(RenderLayerModel.CreateRenderLayer(RenderLayerDto.RenderLayerConvert(item as DisplayLayer), dataSourceRenderLayers.Name));
                    }
                }


                //   modelRenderLayers.Name = Helpers.ResourceHelper.FindKey("3dmodel") + "(" +"0" + ")";


            }
            var imgRenderLayers = new RenderLayerModel()
            {
                DataPath = (ImageSource)Helpers.ResourceHelper.FindResourceByKey("image"),
                MenuLevel = "1",
                ChildCount = 0,
                IsDisplay = false,
                IsChecked = true,
                HasPathData = false,
                Name = Helpers.ResourceHelper.FindKey("Screenage") + "(0)",
                AliasName = Helpers.ResourceHelper.FindKey("Screenage"),
                Guid = Guid.Parse("00000000-0000-0000-0000-000000000300").ToString(),
                LayerType = Common.CommonContract.RenderLayerType.ImageGroupLayer,
                Rederlayers = new List<RenderLayerModel>()
            };
            list.Add(imgRenderLayers);
            var imageLayers = DataBaseService.Instance.GetImageLayers();
            if (imageLayers != null)
            {
                imgRenderLayers.Name = Helpers.ResourceHelper.FindKey("Screenage") + "(" + imageLayers.Count + ")";

                foreach (var item in imageLayers)
                {
                    imgRenderLayers.HasPathData = true;
                    imgRenderLayers.ChildCount++;
                    list.Add(new RenderLayerModel()
                    {
                        HasPathData = false,
                        AlphaBtnOn = true,
                        AlphaStation = "1",
                        MenuLevel = "2",
                        IsDisplay = true,
                        IsChecked = true,
                        ParentName = imgRenderLayers.Name,
                        IsLocal = item.IsLocal,
                        Name = (item.IsLocal ? Helpers.ResourceHelper.FindKey("LocalDataMark") : Helpers.ResourceHelper.FindKey("ServerDataMark")) + item.Name,
                        AliasName = (item.IsLocal ? Helpers.ResourceHelper.FindKey("LocalDataMark") : Helpers.ResourceHelper.FindKey("ServerDataMark")) + item.AliasName,
                        Guid = item.Guid,
                        LayerType = RenderLayerType.ImageLayer
                    });
                }
            }
            var tileRenderLayers = new RenderLayerModel()
            {
                DataPath = (ImageSource)Helpers.ResourceHelper.FindResourceByKey("photography"),
                MenuLevel = "1",
                IsDisplay = false,
                IsChecked = true,
                ChildCount = 0,
                HasPathData = false,
                Name = Helpers.ResourceHelper.FindKey("Obliquephotography") + "(0)",
                AliasName = Helpers.ResourceHelper.FindKey("Obliquephotography"),
                Guid = Guid.Parse("00000000-0000-0000-0000-000000000200").ToString(),
                LayerType = Common.CommonContract.RenderLayerType.TileGroupLayer,
                Rederlayers = new List<RenderLayerModel>()
            };
            list.Add(tileRenderLayers);
            var tileLayers = DataBaseService.Instance.GetTileLayers();
            if (tileLayers != null)
            {
                tileRenderLayers.Name = Helpers.ResourceHelper.FindKey("Obliquephotography") + "(" + tileLayers.Count + ")";

                foreach (var item in tileLayers)
                {
                    tileRenderLayers.HasPathData = true;
                    tileRenderLayers.ChildCount++;
                    list.Add(new RenderLayerModel()
                    {
                        HasPathData = false,
                        MenuLevel = "2",
                        IsDisplay = true,
                        IsChecked = true,
                        IsSymbolic = true,
                        ParentName = tileRenderLayers.Name,
                        IsLocal = item.IsLocal,
                        Name = (item.IsLocal ? Helpers.ResourceHelper.FindKey("LocalDataMark") : Helpers.ResourceHelper.FindKey("ServerDataMark")) + item.Name,
                        AliasName = (item.IsLocal ? Helpers.ResourceHelper.FindKey("LocalDataMark") : Helpers.ResourceHelper.FindKey("ServerDataMark")) + item.AliasName,
                        Guid = item.Guid,
                        LayerType = RenderLayerType.TileLayer,

                    });
                }
            }
            //二维矢量
            var shpRenderLayers = new RenderLayerModel()
            {
                DataPath = (ImageSource)Helpers.ResourceHelper.FindResourceByKey("2dIcon"),
                MenuLevel = "1",
                IsDisplay = false,
                IsChecked = true,
                ChildCount = 0,
                HasPathData = false,
                Name = Helpers.ResourceHelper.FindKey("2dvector") + "(0)",
                AliasName = Helpers.ResourceHelper.FindKey("2dvector"),
                Guid = Guid.Parse("00000000-0000-0000-0000-000000000400").ToString(),
                LayerType = Common.CommonContract.RenderLayerType.ShpGroupLayer,
                Rederlayers = new List<RenderLayerModel>()
            };
            list.Add(shpRenderLayers);
            var shpLayers = DataBaseService.Instance.GetShpLayers();
            if (shpLayers != null)
            {
                shpRenderLayers.Name = Helpers.ResourceHelper.FindKey("2dvector") + "(" + shpLayers.Count + ")";
                foreach (var item in shpLayers)
                {
                    shpRenderLayers.HasPathData = true;
                    shpRenderLayers.ChildCount++;
                    var shpRen = RenderLayerModel.CreateRenderLayer(RenderLayerDto.RenderLayerConvert(item as DisplayLayer), shpRenderLayers.Name);
                    shpRen.HasPathData = false;
                    shpRen.MenuLevel = "2";
                    shpRen.IsDisplay = true;
                    shpRen.IsChecked = true;
                    shpRen.IsSymbolic = true;
                    shpRen.ParentName = shpRenderLayers.Name;
                    shpRen.IsLocal = !item.Fc.DataSource.IsNetServer();
                    shpRen.Name = (!item.Fc.DataSource.IsNetServer() ? Helpers.ResourceHelper.FindKey("LocalDataMark") : Helpers.ResourceHelper.FindKey("ServerDataMark")) + item.Name;
                    shpRen.AliasName = (!item.Fc.DataSource.IsNetServer() ? Helpers.ResourceHelper.FindKey("LocalDataMark") : Helpers.ResourceHelper.FindKey("ServerDataMark")) + item.Name;
                    list.Add(shpRen);
                }
            }
            MapSource = new ObservableCollection<RenderLayerModel>(list);

            tempMapSource = new ObservableCollection<RenderLayerModel>(list);

        }


        private void OnSearchCommand()
        {
            //add  by hengda 
            //InspectRegions = new ObservableCollection<InspectModel>(InspectionService.Instance.GetAllRegion(_searchText).Select(t => RegInsModelConvert.InspectRegionConvert(t)).ToList());
            this.getPipeList();
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

        public void getPipeList()
        {
            Task.Run(() => {
                string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.PipeList);
                this.PipeModels = (JsonUtil.DeserializeFromString<List<PipeModel>>(resStr));
            });
        }
        public void CloseAddWin()
        {
            if (newInspectionVModel != null)
                newInspectionVModel.HideWin();
        }

        private void OnAddCommand( object obj)
        {
            if (_newInspectionView == null)
            {
                _newInspectionView = new NewInspectionView();
                 newInspectionVModel = new NewInspectionVModel();
                _newInspectionView.DataContext = newInspectionVModel;
                newInspectionVModel.HideWin = _newInspectionView.CloseWindow;
            }
            this.newInspectionVModel.Sections = this.Sections;
            this.newInspectionVModel.Periods = this.Periods;
            this.newInspectionVModel.PipeModels = this.PipeModels;
            this.newInspectionVModel.SelectPipeModel = this.PipeModels[0];
            this.newInspectionVModel.addRenderLayer = AddData;
            this.newInspectionVModel.updateData = getPipeList;
            if (obj.ToString() != "0")
            {
              
            }
            _newInspectionView.Owner = Application.Current.MainWindow;
            _newInspectionView.Left = 400;
            _newInspectionView.Top = Application.Current.MainWindow.Height * 0.2;
            _newInspectionView.Show();
        }

        private void AddData(IRenderLayer renderLayer)
        {
            _renderLayers.Add(renderLayer);
            GetMapSource();
            this.getPipeList();
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


        private void OnCheckedCommand(object parameter)
        {
            var inspect = parameter as InspectModel;
            if (inspect == null) return;
            RegInsDataRenderManager.Instance.OpenInspectData(inspect);

        }

        private void OnDownloadCommand(object parameter)
        {
            var inspect = parameter as InspectModel;
            if (inspect == null) return;
            RegInsDataRenderManager.Instance.OpenInspectData(inspect);
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
                PipeModel periodModel = parameter as PipeModel;
                bool success = HttpServiceHelper.Instance.PostRequestForStatus(PipelineInterface.deletestake + "?id=" + periodModel.Id, "");
                if(success)
                {
                    Messages.ShowMessage("删除成功");
                    DeleteDataSource(popemodel.Map.Split('&')[0]);;
                    this.getPipeList();
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
        private void DeleteNode(PipeModel obj,List<PipeModel> PipeModels)
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

                this.Periods = new ObservableCollection<PeriodModel>(JsonUtil.DeserializeFromString<List<PeriodModel>>(periodList));

                //获取标段
                string sectionList = HttpServiceHelper.Instance.GetRequest(PipelineInterface.SectionList);

                this.Sections = new ObservableCollection<SectionModel>(JsonUtil.DeserializeFromString<List<SectionModel>>(sectionList));
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
