﻿using Gvitech.CityMaker.RenderControl;
using Mmc.Framework.Services;
using Mmc.Mspace.Common.Messenger;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.Const.ConstDataInterface;
using Mmc.Mspace.Const.ConstPath;
using Mmc.Mspace.IntelligentAnalysisModule.AreaWidth;
using Mmc.Mspace.Services.HttpService;
using Mmc.Mspace.Theme.Pop;
using Mmc.Windows.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Mmc.Mspace.IntelligentAnalysisModule.MidPointCheck
{
    public class DrawLineManageVModel: CheckedToolItemModel
    {
        DrawLineManageView drawLineManageView = new DrawLineManageView();
        ObservableCollection<LineItem> _drawLineListCollection = new ObservableCollection<LineItem>();
        public ObservableCollection<LineItem> DrawLineListCollection
        {
            get { return _drawLineListCollection; }
            set
            {
                _drawLineListCollection = value;
                base.SetAndNotifyPropertyChanged<ObservableCollection<LineItem>>(ref this._drawLineListCollection, value, "DrawLineListCollection");               
            }
        }
        private NewDrawLineVModel newDrawLineVModel = null;

        public ICommand CreatLineCmd { get; set; }
        public ICommand CloseCmd { get; set; }
        public ICommand DelItemsCmd { get; set; }
        public ICommand SearchCmd { get; set; }
        public ICommand IsOpenCmd { get; set; }
        public ICommand AreaWidthCmd { get; set; }
        public ICommand MidPositionCmd { get; set; }
        public ICommand ChangeCmd { get; set; }
        public ICommand VisualCmd { get; set; }
        public override void Initialize()
        {
            base.Initialize();
            base.ViewType = ViewType.CheckedIcon;
            drawLineManageView.DataContext = this;
            this.CloseCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                IsChecked = false;
                drawLineManageView.Hide();
            }); 
            this.CreatLineCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                if(newDrawLineVModel==null)
                {
                    newDrawLineVModel = new NewDrawLineVModel();
                    newDrawLineVModel.HideParentsWin = drawLineManageView.Hide;
                    newDrawLineVModel.ShowParentsWin = ShowWin;
                    newDrawLineVModel.AddPipe += AddLinePipe;
                }
                newDrawLineVModel.ClearData();
                newDrawLineVModel.ShowDrawWin();
               // GetLineData();
            });
            this.DelItemsCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                DelItems();
            });
            this.SearchCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                //DelItems();
            });
            this.IsOpenCmd = new Mmc.Wpf.Commands.RelayCommand<LineItem>((lineitm) => ChangeIsChecked(lineitm));
            this.AreaWidthCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                var TempItemList = new List<LineItem>();
                foreach (var item in DrawLineListCollection)
                {
                    if(item.IsChecked == true)
                    {
                        TempItemList.Add(item);
                    }
                }
                if(TempItemList.Count==2)
                {
                    AreaWidthVModel areaWidthVModel = new AreaWidthVModel();
                    areaWidthVModel.lineItems = TempItemList;
                    areaWidthVModel.OnChecked();
                }
                else
                {
                    Messages.ShowMessage("请选择两条线路进行对比！");
                }
                //Messenger.Messengers.Notify("AreaWidth", true);
              
            });
            this.MidPositionCmd = new Mmc.Wpf.Commands.RelayCommand(() =>
            {
                var TempItemList = new List<LineItem>();
                foreach (var item in DrawLineListCollection)
                {
                    if (item.IsChecked == true)
                    {
                        TempItemList.Add(item);
                    }
                }
                if (TempItemList.Count == 2)
                {
                    AreaWidthVModel areaWidthVModel = new AreaWidthVModel();
                    areaWidthVModel.lineItems = TempItemList;
                    areaWidthVModel.OnChecked();
                }
                else
                {
                    Messages.ShowMessage("请选择两条线路进行对比！");
                }//
            });    
            this.ChangeCmd = new Mmc.Wpf.Commands.RelayCommand<object>((obj) =>
            {
                //DelItems();
                if(newDrawLineVModel==null)
                {
                    newDrawLineVModel = new NewDrawLineVModel();
                    newDrawLineVModel.HideParentsWin = drawLineManageView.Hide;
                    newDrawLineVModel.ShowParentsWin = ShowWin;
                    newDrawLineVModel.AddPipe += AddLinePipe;
                }
                newDrawLineVModel.ChangedData(obj as LineItem);
                newDrawLineVModel.ShowDrawWin();
            });
            this.VisualCmd = new Mmc.Wpf.Commands.RelayCommand<LineItem>((lineitm) => VisualChecked(lineitm));           
        }

        public override void OnChecked()
        {
            Messenger.Messengers.Notify("DrawLineManage", true);
            GetLineData();

            drawLineManageView.Owner = Application.Current.MainWindow;
            drawLineManageView.Left = 700;
            drawLineManageView.Top = Application.Current.MainWindow.Height * 0.2;
            drawLineManageView.Show();
            base.OnChecked();          
        }
        private void ShowWin()
        {

            drawLineManageView.Show();
        }
     
        public override void OnUnchecked()
        {
            drawLineManageView.Hide();
            newDrawLineVModel?.HideWin();
            base.OnUnchecked();
            Messenger.Messengers.Notify("DrawLineManage", false);
        }
        private void GetLineData()
        {
            DrawLineListCollection.Clear();
            var json = JsonUtil.DeserializeFromFile<dynamic>(AppDomain.CurrentDomain.BaseDirectory + ConfigPath.WebConnectConfig);
            string url = string.Format("{0}/api/tracing/index", json.poiUrl);
            var httpservice = new HttpService();
            httpservice.Token = HttpServiceUtil.Token;
            var uavResult = string.Empty;
            uavResult = httpservice.RequestService(url, method: "GET");
            var templist = JsonUtil.DeserializeFromString<dynamic>(uavResult);
            dynamic list = templist.data;
            foreach( var item in list)
            {
                LineItem lineItem = new LineItem();
                lineItem.id = item["id"];
                lineItem.sn = item["sn"];
                lineItem.name = item["name"];
                lineItem.pipe_id = item["pipe_id"];
                lineItem.start = item["start"];
                lineItem.end = item["end"];
                lineItem.isVisible = false;
                lineItem.type_id = item["type_name"];//TypenameToNum(Convert.ToString(item["type_name"]));
                lineItem.geom = "";
                lineItem.IsChecked = false;
                DrawLineListCollection.Add(lineItem);
            }
        }
      
        private void AddLinePipe(LineItem lineItem)
        {
            if(lineItem != null)
            {
                drawLineManageView.Show();
                this.GetLineData();
            }            
        }
        private void DelItems()
        {
            string deleteString = "?ids=";
            foreach(var item in DrawLineListCollection)
            {
                if(item?.IsChecked == true)
                {
                    deleteString = deleteString + Convert.ToString(item.id)+",";
                }
            }
            if(deleteString =="?ids=")
            {
                //Messages.ShowMessage("");
            }
            else
            {
                deleteString =  deleteString.Substring(0,deleteString.Length-1);
                string url = MarkInterface.DeleteLine + deleteString;
                string resStr = HttpServiceHelper.Instance.GetRequest(url);
            }
            GetLineData();
        }
        private void ChangeIsChecked(LineItem lineItem)
        {
            lineItem.IsChecked = !lineItem.IsChecked;
        }
        private void VisualChecked(LineItem lineItem)
        {
            if (lineItem.guid != null)
            {
                IRenderPolyline obj = GviMap.ObjectManager.GetObjectById(lineItem.guid) as IRenderPolyline;
                if(obj != null)
                {
                    if (obj.VisibleMask == gviViewportMask.gviViewAllNormalView)
                    {
                        obj.VisibleMask = gviViewportMask.gviViewNone;
                    }
                    else
                    {
                        obj.VisibleMask = gviViewportMask.gviViewAllNormalView;
                    }
                }                
            }
            
        }
    }
}
