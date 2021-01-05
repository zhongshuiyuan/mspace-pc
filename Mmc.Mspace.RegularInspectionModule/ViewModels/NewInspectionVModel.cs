using ApplicationConfig;
using Gvitech.CityMaker.FdeCore;
using Microsoft.Win32;
using Mmc.DataSourceAccess;
using Mmc.Mspace.Common;
using Mmc.Mspace.Common.Cache;
using Mmc.Mspace.Common.Dto;
using Mmc.Mspace.Common.Messenger;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Const.ConstDataInterface;
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
        public Action updateData;

        public Dictionary<Guid, string> newrender = new Dictionary<Guid, string>();

      
        private string typeString = "";

        public NewInspectionVModel()
        {
            _userInfo = CacheData.UserInfo;
            if (_userInfo.mspace_config?.is_administrator == "1")
            {

            }

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

        private DateTime _createTime=DateTime.Now;   
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; OnPropertyChanged("CreateTime"); }
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

        private string _uploadText="请选择文件";

        public string UploadText
        {
            get { return _uploadText; }
            set { _uploadText = value; OnPropertyChanged("UploadText"); }
        }



        private bool _localCheck=true;
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

        private int _startStake;

        public int StartStake
        {
            get { return _startStake; }
            set { _startStake = value; OnPropertyChanged("StartStake"); }
        }

        private int _endStake;

        public int EndStake
        {
            get { return _endStake; }
            set { _endStake = value; OnPropertyChanged("EndStake"); }
        }
        private PipeModel _selectPipeModel;
        /// <summary>
        /// 管线选中
        /// </summary>
        public PipeModel SelectPipeModel
        {
            get { return _selectPipeModel; }
            set { _selectPipeModel = value; OnPropertyChanged("SelectPipeModel");
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
                _selectPeriodModel = value; OnPropertyChanged("SelectPeriodModel");
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
                _selectSectionModel = value; OnPropertyChanged("SelectSectionModel");
            }
        }

        public InspectRegion SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); }
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

        private DateTime _inspectionDate=DateTime.Now;

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

        

        //关闭新增窗口
        private void OnCancelCommand()
        {
            //清楚数据
            this.LoadFiles = "";
            this.LocalCheck = true;
            this.Name = "";
            this.HideWin();
        }
        public void LoadData()
        {
            NewName = string.Empty;
            InspectionDate = DateTime.Now;
            InspectRegions = new ObservableCollection<InspectRegion>(InspectionService.Instance.GetAllRegion());

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
                foreach (string file in openFile.FileNames)
                {
                    LoadFiles= file;
                }
            }
        }
     

        private void loadLayer(string _fileAddress, double cycleTime, int _index)
        {
            //string filetype = _filetype;
            // bool status = false;
            string guid = string.Empty;

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

        private  void AddData(string filetype, string fileAddress, string guid, double cycleTime, int index)
        {
            newrender = new Dictionary<Guid, string>();
            if (index >= 0)
            {
                switch (filetype)
                {
                    case "ShpGroupLayer":
                        AddShpDataSource(fileAddress, index);
                        break;
                    case "ImageGroupLayer":
                         AddImageDataSource(fileAddress, index, cycleTime: cycleTime);
                        break;
                    case "TileGroupLayer":
                         AddTileDataSource(fileAddress, index);
                        break;
                    case "DataSetGroupLayer":
                        AddFdbDataSource(fileAddress, index);
                        break;
                }
            }
            if (index == -1)
            {
                switch (filetype)
                {
                    case "WFS":
                        if (_userInfo.mspace_config.is_administrator == "1")
                            AddShpDataSource(fileAddress, index, false, guid);
                        else
                        {
                            ShowAddDataStatus(OperateDataStatus.NOPERMISSION);

                        }
                        break;
                    case "WMTS":
                        if (_userInfo.mspace_config.is_administrator == "1")
                        {
                            AddImageDataSource(fileAddress, index, isLocal: false);
                        }
                        else
                        {
                            ShowAddDataStatus(OperateDataStatus.NOPERMISSION);

                        }
                        break;
                    case "TILE":
                        if (_userInfo.mspace_config.is_administrator == "1")
                        {
                            AddTileDataSource(fileAddress, index, false);
                        }
                        else
                        {
                            ShowAddDataStatus(OperateDataStatus.NOPERMISSION);
                        }
                        break;
                    case "MODEL":
                        if (_userInfo.mspace_config.is_administrator == "1")
                        {
                            AddFdbDataSource(fileAddress, index, false);
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
                    typeString = "ShpGroupLayer";
                    break;
                case ".SHP":
                    typeString = "ShpGroupLayer";
                    break;
                case ".tdbx":
                    typeString = "TileGroupLayer";
                    break;
                case ".TDBX":
                    typeString = "TileGroupLayer";
                    break;
                case ".tif":
                    typeString = "ImageGroupLayer";
                    break;
                case ".TIF":
                    typeString = "ImageGroupLayer";
                    break;
                case ".tds":
                    typeString = "ImageGroupLayer";
                    break;
                case ".TDS":
                    typeString = "ImageGroupLayer";
                    break;
                case ".fdb":
                    typeString = "DataSetGroupLayer";
                    break;
                case ".FDB":
                    typeString = "DataSetGroupLayer";
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
        private  void ParseUpload()
        {
            //本地加载
            if (LocalCheck)
            {
                //  bool loadstatus = false;
                loadLayer(LoadFiles, 20, 0);
                //loadLayer(LoadFiles, 20, 0);//默认值
            }
            else
            {
                string guid = "";
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
        private void AddShpDataSource(string fileAddress, int _index = -1, bool isLocal = true, string guid = "")
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
                    newrender.Add(new Guid(renderLayers[0].Guid),typeString);

                    addRenderLayer(RenderLayer.CreateRenderLayer(renderLayers[0] as IRenderLayer));
                    if (!isLocal)
                    {
                        //dataView.LoadImage.Visibility = Visibility.Collapsed;
                        Messages.ShowMessage("网络图层加载成功");
                    }
                }
                if (_index != -1)
                {
                    //ChangeStatue(status, _index);
                }
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
                if (_index != -1)
                {
                    //ChangeStatue(status, _index);
                }
            }
        }

        private  void AddImageDataSource(string fileAddress, int _index, double cycleTime = 0, bool isLocal = true)
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

        private void AddTileDataSource(string fileAddress, int _index, bool isLocal = true)
        {
            OperateDataStatus status = OperateDataStatus.LOADFAILED;
            //打开文件
            try
            {
             
                    //   BeginLoadDsProcess();

                    string name = string.Empty;
                    //string hashCode = string.Empty;
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
                        IsLocal = isLocal,
                        //HashCode = hashCode
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
                                //ChangeStatue(status, _index);
                                Messages.ShowMessage("数据加载成功");
                            }
                        });
                    }
                    else
                    {
                        if (_index != -1)
                        {
                            //ChangeStatue(status, _index);
                        }
                    }
               
                //
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
                //ChangeStatue(status, _index);
            }

        }
        private void AddFdbDataSource(string fileAddress, int _index, bool isLocal = true)
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
                        //ChangeStatue(status, _index);
                        Messages.ShowMessage("数据加载成功");

                    }
                }
                if (status == OperateDataStatus.DATAEXISTED)
                {
                    if (_index != -1)
                    {
                        //ChangeStatue(status, _index);
                    }
                }
            }
            catch (Exception ex)
            {
                SystemLog.Log(ex);
                if (_index != -1)
                {
                    //ChangeStatue(status, _index);
                }
            }

        }
    
        #endregion
        private void OnCreateCommand()
        {
            //if (string.IsNullOrEmpty(_newName) && _selectedItem == null)
            //{
            //    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("InputName"));
            //    return;
            //}
            //if (_newName.Length>21)
            //{
            //    Messages.ShowMessage(Helpers.ResourceHelper.FindKey("InputNameLength"));
            //    return;
            //}

            Task.Run(()=>{

                this.ParseUpload();
                if (newrender.Count <= 0)
                {
                    Messages.ShowMessage("当前上传文件已存在！");
                    return;
                }
                var aa = newrender.ToString();
                var txtjson = JsonUtil.SerializeToString(new
                {
                    name = this.Name,
                    pipe_id = this.SelectPipeModel.Id,
                    section_id = this.SelectSectionModel.Id,
                    file_type = LocalCheck ? 0 : 1,//文件类型
                    map = newrender.First().Key+"&"+ newrender.First().Value,
                    period_id = this.SelectPeriodModel.Id,
                    time = this.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"),
                    file = LoadFiles,
                    start = this.StartStake,
                    end = this.EndStake,
                }); 

                bool success = HttpServiceHelper.Instance.PostRequestForStatus(PipelineInterface.createstake, txtjson);

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
