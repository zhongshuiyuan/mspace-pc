using ApplicationConfig;
using Gvitech.CityMaker.FdeCore;
using Microsoft.Win32;
using Mmc.DataSourceAccess;
using Mmc.Mspace.Common;
using Mmc.Mspace.Common.Cache;
using Mmc.Mspace.Common.Dto;
using Mmc.Mspace.Common.Messenger;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Common.Models.pipelines;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.Models.HttpResult;
using Mmc.Mspace.Models.Inspection;
using Mmc.Mspace.PoiManagerModule.Dto;
using Mmc.Mspace.PoiManagerModule.Models;
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
using System.Xml.Serialization;
using static Mmc.Mspace.Common.CommonContract;

namespace Mmc.Mspace.RegularInspectionModule.ViewModels
{
    public class NewInspectionVModel : BaseViewModel
    {
        private NewInspectionView _newInspectionView;
        public Action<IRenderLayer> addRenderLayer = null;
        private InspectRegion _selectedItem;
        private UserInfo _userInfo;
        public Action HideWin;
        public Action<PipeModel> updateData;
        public Dictionary<Guid, string> newrender = new Dictionary<Guid, string>();
        public string typeString = "";

        public NewInspectionVModel()
        {
            _userInfo = CacheData.UserInfo;
            if (_userInfo.mspace_config?.is_administrator == "1")
            {

            }
        }
        private ObservableCollection<TaskModel> _taskAll = new ObservableCollection<TaskModel>();
        public ObservableCollection<TaskModel> TaskAll
        {
            get { return _taskAll; }
            set
            {
                _taskAll = value;
                OnPropertyChanged("TaskAll");
            }
        }

        private TaskModel _TaskSelectItem;
        public TaskModel TaskSelectItem
        {
            get { return _TaskSelectItem; }
            set
            {
                _TaskSelectItem = value;
                OnPropertyChanged("TaskSelectItem");
            }
        }
        private RelayCommand<object> _startSearchCommand;

        public RelayCommand<object> StartSearchCommand
        {

            get { return _startSearchCommand ?? (_startSearchCommand = new RelayCommand<object>(OnSearchCommand)); }
            set { _startSearchCommand = value; }
        }
        private RelayCommand<object> _endSearchCommand;

        public RelayCommand<object> EndSearchCommand
        {

            get { return _endSearchCommand ?? (_endSearchCommand = new RelayCommand<object>(OnEndSearchCommand)); }
            set { _endSearchCommand = value; }
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
        private bool _startIsDropDownOpen;

        public bool StartIsDropDownOpen
        {
            get { return _startIsDropDownOpen; }
            set { _startIsDropDownOpen = value; OnPropertyChanged("StartIsDropDownOpen"); }
        }
        private bool _endIsDropDownOpen;

        public bool EndIsDropDownOpen
        {
            get { return _endIsDropDownOpen; }
            set { _endIsDropDownOpen = value; OnPropertyChanged("EndIsDropDownOpen"); }
        }
        private List<StakeModel> _stakeModels = new List<StakeModel>();
        public List<StakeModel> StakeModels
        {
            get { return _stakeModels; }
            set
            {
                _stakeModels = value;
                OnPropertyChanged("StakeModels");
            }
        }
        private List<StakeModel> _stakeModels2 = new List<StakeModel>();
        public List<StakeModel> StakeModels2
        {
            get { return _stakeModels2; }
            set
            {
                _stakeModels2 = value;
                OnPropertyChanged("StakeModels2");
            }
        }

        private DateTime _createTime = DateTime.Now;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; OnPropertyChanged("CreateTime"); }
        }
        private StakeModel _startPoi;
        public StakeModel StartPoi
        {
            get { return _startPoi; }
            set
            {
                _startPoi = value; OnPropertyChanged("StartPoi");
            }
        }
        private StakeModel _endPoi;
        public StakeModel EndPoi
        {
            get { return _endPoi; }
            set
            {
                _endPoi = value; OnPropertyChanged("EndPoi");
            }
        }

        private string _name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        private PipeModel _editItem;

        public PipeModel EditItem
        {
            get { return _editItem; }
            set { _editItem = value; }
        }


        private string _uploadText = "请选择文件";

        public string UploadText
        {
            get { return _uploadText; }
            set { _uploadText = value; OnPropertyChanged("UploadText"); }
        }



        private bool _localCheck = true;
        /// <summary>
        /// 本地上传
        /// </summary>
        public bool LocalCheck
        {
            get { return _localCheck; }
            set { _localCheck = value; OnPropertyChanged("LocalCheck"); }
        }
        private string _loadFiles = "";

        public string LoadFiles
        {
            get { return _loadFiles; }
            set { _loadFiles = value; OnPropertyChanged("LoadFiles"); }
        }

        private bool _serverCheck;
        /// <summary>
        /// 服务器上传
        /// </summary>
        public bool ServerCheck
        {
            get { return _serverCheck; }
            set {
                _serverCheck = value;
                LoadFiles = "";
                if (_serverCheck)
                {
                    UploadText = "请输入网络上传地址";
                }
                else
                {
                    UploadText = "请选择文件";
                }
                OnPropertyChanged("ServerCheck");
            }
        }

        private PipeModel _selectPipeModel;
        /// <summary>
        /// 管线选中
        /// </summary>
        public PipeModel SelectPipeModel
        {
            get { return _selectPipeModel; }
            set { _selectPipeModel = value;
             
                OnPropertyChanged("SelectPipeModel");
                if (SelectPipeModel != null)
                    getSectionList();
            }
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
        private PeriodModel _selectPeriodModel;
        /// <summary>
        /// 阶段选中
        /// </summary>
        public PeriodModel SelectPeriodModel
        {
            get { return _selectPeriodModel; }
            set
            {
                _selectPeriodModel = value;
                OnPropertyChanged("SelectPeriodModel");
            }
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

        private SectionModel _selectSectionModel;
        /// <summary>
        /// 标段选中
        /// </summary>
        public SectionModel SelectSectionModel
        {
            get { return _selectSectionModel; }
            set
            {
                _selectSectionModel = value;
                OnPropertyChanged("SelectSectionModel");
            }
        }


        public InspectRegion SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value;
                OnPropertyChanged("SelectedItem"); }
        }

        private string _newName;

        public string NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;
                OnPropertyChanged("NewName");
            }
        }

        private DateTime _inspectionDate = DateTime.Now;

        public DateTime InspectionDate
        {
            get { return _inspectionDate; }
            set { _inspectionDate = value; OnPropertyChanged("InspectionDate"); }
        }

        private ObservableCollection<InspectRegion> _inspectRegions;

        public ObservableCollection<InspectRegion> InspectRegions
        {
            get { return _inspectRegions; }
            set { _inspectRegions = value; OnPropertyChanged("InspectRegions"); }
        }

        private RelayCommand _cancelCommand;
        [XmlIgnore]
        public RelayCommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(OnCancelCommand)); }
            set { _cancelCommand = value; }
        }

        private RelayCommand _createCommand;
        [XmlIgnore]
        public RelayCommand CreateCommand
        {
            get { return _createCommand ?? (_createCommand = new RelayCommand(OnCreateCommand)); }
            set { _createCommand = value; }
        }

        private RelayCommand _uploadFileCommand;
        [XmlIgnore]
        public RelayCommand UploadFileCommand
        {
            get { return _uploadFileCommand ?? (_uploadFileCommand = new RelayCommand(OnUploadFileCommand)); }
            set { _uploadFileCommand = value; }
        }
        private void OnEndSearchCommand(object obj)
        {
            StakeModels2 = new List<StakeModel>();
            if (obj == null) return;

            string text = obj.ToString();
            if (string.IsNullOrEmpty(text)) return;
            getStackList2(text);
            if (StakeModels2.Count > 0)
            {
                EndIsDropDownOpen = true;
            }
        }
        private void OnSearchCommand(object obj)
        {
            StakeModels = new List<StakeModel>();
            if (obj == null|| obj.ToString() == "") return;

            string text = obj.ToString();
            if (string.IsNullOrEmpty(text)) return;
            getStackList(text);
            if (StakeModels.Count > 0)
            {
                StartIsDropDownOpen = true;
            }
        }

        //关闭新增窗口
        private void OnCancelCommand()
        {
            //清楚数据
            this.LoadFiles = "";
            this.LocalCheck = true;
            this.Name = "";
            this.StartPoi = null;
            this.EndPoi = null;
            this.SelectPeriodModel = null;
            this.SelectPipeModel = null;
            this.SelectSectionModel = null;
            this.CreateTime = DateTime.Now;
            this.HideWin();
        }
        public void LoadData()
        {
            NewName = string.Empty;
            InspectionDate = DateTime.Now;
            InspectRegions = new ObservableCollection<InspectRegion>(InspectionService.Instance.GetAllRegion());
            getStackList("");
            getStackList2("");
            getTaskAll();

        }
        /// <summary>
        /// 获取中
        /// </summary>
        private void getStackList(string sn)
        {
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.stakeindex + "?sn=" + sn);
            this.StakeModels = (JsonUtil.DeserializeFromString<List<StakeModel>>(resStr));
        }
        /// <summary>
        /// 获取中
        /// </summary>
        private void getStackList2(string sn)
        {
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.stakeindex + "?sn=" + sn);
            this.StakeModels2 = (JsonUtil.DeserializeFromString<List<StakeModel>>(resStr));
        }
        #region //上传文件

        private void ClearCache()
        {
            LoadFiles = "";
        }
        public void OnUploadFileCommand()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = false;
            openFile.Filter = FileFilterStrings.Support;
            if (openFile.ShowDialog() == true)
            {
                Name = openFile.SafeFileName.Split('.')[0];
                foreach (string file in openFile.FileNames)
                {
                    LoadFiles = file;
                   
                }
            }
        }

        private void loadLayer(string _fileAddress, double cycleTime, int _index)
        {
            //string filetype = _filetype;
            // bool status = false;
            string guid = Guid.NewGuid().ToString();

            if (_fileAddress == null || _fileAddress == "")
            {
                Messages.ShowMessage(Helpers.ResourceHelper.FindKey("EmptyFilePath")); //("请选择图层");

            }
            else
            {
                string filetype = ExtensionConfirm(System.IO.Path.GetExtension(_fileAddress));
                AddData(filetype, _fileAddress, guid, cycleTime, _index);
            }

        }

        private void AddData(string filetype, string fileAddress, string guid, double cycleTime, int index)
        {
            newrender = new Dictionary<Guid, string>();
            if (index >= 0)
            {
                switch (filetype)
                {
                    case "ShpGroupLayer":
                        AddShpDataSource(fileAddress, guid, index);
                        break;
                    case "ImageGroupLayer":
                        AddImageDataSource(fileAddress, guid, index, cycleTime: cycleTime);
                        break;
                    case "TileGroupLayer":
                        AddTileDataSource(fileAddress, guid, index);
                        break;
                    case "DataSetGroupLayer":
                        AddFdbDataSource(fileAddress, guid, index);
                        break;
                    case "Video":
                    case "Img":
                        break;
                }
            }
            if (index == -1)
            {
                switch (filetype)
                {
                    case "WFS":
                        if (_userInfo.mspace_config.is_administrator == "1")
                            AddShpDataSource(fileAddress, guid, index, false);
                        else
                        {
                            ShowAddDataStatus(OperateDataStatus.NOPERMISSION);

                        }
                        break;
                    case "WMTS":
                        if (_userInfo.mspace_config.is_administrator == "1")
                        {
                            AddImageDataSource(fileAddress, guid, index, isLocal: false);
                        }
                        else
                        {
                            ShowAddDataStatus(OperateDataStatus.NOPERMISSION);

                        }
                        break;
                    case "TILE":
                        if (_userInfo.mspace_config.is_administrator == "1")
                        {
                            AddTileDataSource(fileAddress, guid, index, false);
                        }
                        else
                        {
                            ShowAddDataStatus(OperateDataStatus.NOPERMISSION);
                        }
                        break;
                    case "MODEL":
                        if (_userInfo.mspace_config.is_administrator == "1")
                        {
                            AddFdbDataSource(fileAddress, guid, index, false);
                        }
                        else
                        {
                            ShowAddDataStatus(OperateDataStatus.NOPERMISSION);
                        }
                        break;
                }

            }
        }


        private string ExtensionConfirm(string _extension)
        {
            typeString = "";
            switch (_extension)
            {
                case ".shp":
                case ".SHP":
                    typeString = "ShpGroupLayer";
                    break;
                case ".tdbx":
                case ".TDBX":
                    typeString = "TileGroupLayer";
                    break;
                case ".tif":
                case ".TIF":
                    typeString = "ImageGroupLayer";
                    break;
                case ".tds":
                case ".TDS":
                    typeString = "ImageGroupLayer";
                    break;
                case ".fdb":
                case ".FDB":
                    typeString = "DataSetGroupLayer";
                    break;
                case ".mp4":
                case ".MP4":
                    typeString = "Video";
                    break;
                case ".jpg":
                case ".png":
                case ".bmp":
                case ".JPG":
                case ".PNG":
                case ".BMP":
                    typeString = "Img";
                    break;
                default:
                    typeString = "";
                    break;
            }
            return typeString;
        }
        /// <summary>
        /// 解析上传文件
        /// </summary>
        private void ParseUpload()
        {
            //本地加载
            if (LocalCheck)
            {
                loadLayer(LoadFiles, 20, 0);
                //loadLayer(LoadFiles, 20, 0);//默认值
            }
            else
            {
                string guid = Guid.NewGuid().ToString();
                if (_userInfo.mspace_config.is_administrator != "1")
                {
                    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("NoPermission"));
                    return;
                }
                var filetype = "WFS";//多种网络方式选择
                var fileAddress = LoadFiles;

                switch (filetype)
                {
                    case "WFS":
                        AddData(filetype, fileAddress, guid, 20, -1);//-1代表网络图层
                        break;
                    case "WMTS":
                        AddData(filetype, fileAddress, "", 20, -1);
                        break;
                    case "TILE":
                        AddData(filetype, fileAddress, "", 20, -1);
                        break;
                    case "MODEL":
                        AddData(filetype, fileAddress, "", 20, -1);
                        break;
                }
            }
        }

        private void ShowAddDataStatus(OperateDataStatus status)
        {
            switch (status)
            {
                case OperateDataStatus.NOPERMISSION:
                    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("NoPermission"));
                    break;
                case OperateDataStatus.LOADFAILED:
                    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("LoadDataFailed"));
                    break;
                case OperateDataStatus.LOADSUCCESSED:
                    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("LoadDataSucceed"));
                    break;
                case OperateDataStatus.DATAEXISTED:
                    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("LoadDataRepeat"));
                    break;
            }
        }
        private void AddShpDataSource(string fileAddress, string guid, int _index = -1, bool isLocal = true)
        {
            OperateDataStatus status = OperateDataStatus.LOADFAILED;
            try
            {
                var con = new ConnectionInfo();
                if (isLocal)
                {
                    con.ConnectionType = gviConnectionType.gviConnectionShapeFile;
                }
                else
                {
                    con.ConnectionType = gviConnectionType.gviConnectionWFS;
                }
                con.Database = fileAddress;

                LibraryConfig layerConfig = new LibraryConfig()
                {
                    ConnInfoString = con.ToConnectionString(),
                    Guid = guid,
                    Is2DData = true,
                    IsLocal = isLocal,
                };
                List<IDisplayLayer> renderLayers;
                renderLayers = DataBaseService.Instance.AddFeatureDatasource(layerConfig, out status);
                if (status == OperateDataStatus.LOADSUCCESSED)//renderLayers != null
                {
                    newrender.Add(new Guid(renderLayers[0].Guid), typeString);

                    addRenderLayer(RenderLayer.CreateRenderLayer(renderLayers[0] as IRenderLayer));
                    if (!isLocal)
                    {
                        //dataView.LoadImage.Visibility = Visibility.Collapsed;
                        Messages.ShowMessage("网络图层加载成功");
                    }
                }
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
            }
        }

        private void AddImageDataSource(string fileAddress, string guid, int _index, double cycleTime = 0, bool isLocal = true)
        {
            OperateDataStatus status = OperateDataStatus.LOADFAILED;

            try
            {
                //Task.Run(() =>
                //{
                ImageLayerConfig layerConfig = new ImageLayerConfig()
                {
                    ConnInfoString = fileAddress,
                    AlphaEnabled = "false",  //启动A通道
                    Guid = guid,
                    IsLocal = isLocal
                };
                if (isLocal)
                {
                    layerConfig.AliasName = Path.GetFileNameWithoutExtension(fileAddress);
                    layerConfig.ConType = "File";
                    //layerConfig.HashCode = hashCode;
                    layerConfig.AddTime = DateTime.Today;
                    layerConfig.CycleTime = cycleTime;
                }
                else
                {
                    layerConfig.ConType = "WMTS";
                }

                var renderLayer = DataBaseService.Instance.AddImageLayer(layerConfig, out status);
                if (renderLayer != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        newrender.Add(new Guid(renderLayer.Guid), typeString);
                            //_renderLayers.Add(renderLayer);
                            addRenderLayer(renderLayer);
                        if (!isLocal)
                        {
                            Messages.ShowMessage("网络图层加载成功");
                        }
                        else if (_index != -1)
                        {
                                //ChangeStatue(status, _index);
                                Messages.ShowMessage("数据加载成功");
                        }
                    });
                }
                else
                {
                    //失败

                }
                //});
            }
            catch (Exception ex)
            {

            }

        }

        private void AddTileDataSource(string fileAddress, string guid, int _index, bool isLocal = true)
        {
            OperateDataStatus status = OperateDataStatus.LOADFAILED;
            //打开文件
            try
            {
                string name = string.Empty;
                if (isLocal)
                {
                    name = Path.GetFileNameWithoutExtension(fileAddress);
                }
                else
                {
                    name = fileAddress.Split(':', '@')[1];
                }
                TileLayerConfig layerConfig = new TileLayerConfig()
                {
                    AliasName = name,
                    ConnInfoString = fileAddress,
                    Guid = guid,
                    IsLocal = isLocal,
                };

                var renderLayer = DataBaseService.Instance.Add3DTileLayer(layerConfig, out status);
                if (renderLayer != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        newrender.Add(new Guid(renderLayer.Guid), typeString);
                        addRenderLayer(renderLayer);
                        if (!isLocal)
                        {

                            Messages.ShowMessage("网络图层加载成功");
                        }
                        else if (_index != -1)
                        {
                            Messages.ShowMessage("数据加载成功");
                        }
                    });
                }
                else
                {
                }

                //
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
            }

        }
        private void AddFdbDataSource(string fileAddress, string guid, int _index, bool isLocal = true)
        {
            OperateDataStatus status = OperateDataStatus.LOADFAILED;
            try
            {

                string name = string.Empty;
                var con = new ConnectionInfo();
                if (isLocal)
                {
                    con.ConnectionType = gviConnectionType.gviConnectionFireBird2x;
                    con.Database = fileAddress;
                    name = fileAddress.Substring(fileAddress.LastIndexOf('\\') + 1);
                }
                else
                {
                    string[] tempArr = fileAddress.Split(';');
                    Dictionary<string, string> tempDic = new Dictionary<string, string>();
                    if (tempArr.Length > 0)
                    {
                        foreach (string item in tempArr)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                tempDic.Add(item.Split('=')[0].Trim(), item.Split('=')[1].Trim());
                            }
                        }
                    }
                    con.ConnectionType = gviConnectionType.gviConnectionCms7Http;

                    con.Server = tempDic["Server"];
                    con.Port = Convert.ToUInt32(tempDic["Port"]);
                    con.Database = tempDic["DataBase"];
                    name = con.Database;
                }

                LibraryConfig layerConfig = new LibraryConfig()
                {
                    ConnInfoString = con.ToConnectionString(),
                    AliasName = name,
                    Is2DData = false,
                    IsLocal = isLocal,
                    Guid = guid,

                    //HashCode = hashCode
                };
                var renderLayers = DataBaseService.Instance.AddFeatureDatasource(layerConfig, out status);

                if (renderLayers != null && status != OperateDataStatus.DATAEXISTED)//&& alreadyExist==false//
                {
                    newrender.Add(new Guid(renderLayers[0].Guid), typeString);
                    foreach (var item in renderLayers)
                    {
                        //_renderLayers.Add(item as DisplayLayer);
                        addRenderLayer(item as DisplayLayer);
                    }
                    if (!isLocal)
                    {
                        Messages.ShowMessage("网络图层加载成功");
                    }
                    else if (_index != -1)
                    {
                        Messages.ShowMessage("数据加载成功");

                    }
                }
                if (status == OperateDataStatus.DATAEXISTED)
                {

                }
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
            }

        }

        private void getSectionList()
        {
            string sectionList = HttpServiceHelper.Instance.GetRequest(PipelineInterface.SectionList);
            ObservableCollection<SectionModel> list = new ObservableCollection<SectionModel>(JsonUtil.DeserializeFromString<List<SectionModel>>(sectionList));

            SectionModel se = SelectSectionModel;
            this.Sections = new ObservableCollection<SectionModel>((list.Where(t => t.Pipe_id == SelectPipeModel.Id).ToList()));
            if (se != null)
            {
                SelectSectionModel= this.Sections.SingleOrDefault(t => t.Id == se.Id);
            }
        }
        private void getTaskAll()
        {
            this.TaskAll = new ObservableCollection<TaskModel>();
            string resStr = HttpServiceHelper.Instance.GetRequest(PipelineInterface.taskall);
            this.TaskAll = (JsonUtil.DeserializeFromString<ObservableCollection<TaskModel>>(resStr));
        }
        #endregion
        private void OnCreateCommand()
        {
            if (SelectPeriodModel==null)
            {
                Messages.ShowMessage("请选择标段信息！");
                return;
            }
            if (SelectSectionModel == null)
            {
                Messages.ShowMessage("请选择阶段信息！");
                return;
            }
            if (string.IsNullOrEmpty(Name))
            {
                Messages.ShowMessage("请输入名称信息！");
                return;
            }
            if (StartPoi == null || EndPoi == null)
            {
                Messages.ShowMessage("请选择起始桩号！");
                return;
            }
            if (TaskSelectItem == null )
            {
                Messages.ShowMessage("请选择关联任务！");
                return;
            }
            Task.Run(()=>{
                if(EditItem==null|| EditItem.File!= UploadText)
                {
                    this.ParseUpload();
                }
                string map = "";
                if (newrender.Count <= 0&&typeString!= "Video" && typeString != "Img")
                {
                    Messages.ShowMessage("当前上传文件已存在！");
                    return;
                }
                if (newrender.Count > 0)
                {
                    map = newrender.First().Key + "&" + newrender.First().Value;
                }
                if (typeString == "Video"|| typeString == "Img")
                {
                    map = "123456&" + typeString;
                }
                var txtjson = JsonUtil.SerializeToString(new
                {
                    id= EditItem!=null?EditItem.Id:"",
                    name = this.Name,
                    pipe_id = this.SelectPipeModel.Id,
                    section_id = this.SelectSectionModel.Id,
                    file_type = LocalCheck ? 0 : 1,//文件类型
                    map = map,
                    type = typeString,//类型
                    period_id = this.SelectPeriodModel.Id,
                    time = this.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"),
                    file = LoadFiles,
                    start = this.StartPoi.Id,
                    end = this.EndPoi.Id,
                    task_id =TaskSelectItem.Id,
                });
                var url = PipelineInterface.createstake;
                if (EditItem != null)
                {
                  url = PipelineInterface.updatestake+"?id="+ EditItem.Id;
                }
                HttpResultModel success = HttpServiceHelper.Instance.PostRequestForResultModel(url, txtjson);
                if(success.status=="1")
                {
                    updateData(JsonUtil.DeserializeFromString<PipeModel>(success.data.ToString()));
                    if(EditItem!=null)
                    {
                        Messages.ShowMessage("修改成功！");
                    }
                    else
                    {
                        Messages.ShowMessage("添加成功！");
                    }
                }
                else
                {
                    Messages.ShowMessage("添加数据异常，请联系管理员！");
                }
                this.OnCancelCommand();

            });
            
            //if(_selectedItem==null|| _selectedItem.Name!=_newName)
            //{
            //  var regions=  _inspectRegions.Where(t => t.Name == _newName).ToList();
            //    if (regions.Count > 0)
            //        SelectedItem = regions.SingleOrDefault();
            //    else
            //    {
            //        _selectedItem = new InspectRegion();
            //        _selectedItem.Name = _newName;
            //    }
            //}
            //_selectedItem.InspectUnits = new List<InspectUnit>();
            //_selectedItem.InspectUnits.Add(new InspectUnit() { Name= "清点",Time=_inspectionDate});
            //_selectedItem.InspectUnits.Add(new InspectUnit() { Name = "开挖后", Time = _inspectionDate });
            //_selectedItem.InspectUnits.Add(new InspectUnit() { Name = "回填前", Time = _inspectionDate });
            //_selectedItem.InspectUnits.Add(new InspectUnit() { Name = "回填后", Time = _inspectionDate });
            //if (InspectionService.Instance.IsExistsRegion(_selectedItem.Id,_newName, _inspectionDate.ToLongDateString()))
            //{
            //    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("ReSaveName"));
            //    return;
            //}
            //var result = InspectionService.Instance.AddRegion(_selectedItem);
            //if (result != null)
            //{
            //    NewName = null;
            //    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("Savesuccess"));
            //    Messenger.Messengers.Notify("AddRegion", true);

            //    Messenger.Messengers.Notify("HistoryDomRefresh");
            //    Messenger.Messengers.Notify("PhotoTraceRefresh");
            //}
            //else
            //{
            //    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("Savefailed"));
            //}
        }
    }
}
