using Mmc.Mspace.Common.Models;
using Mmc.Mspace.IotModule.Views;
using Mmc.Wpf.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Mmc.Mspace.IotModule.ViewModels
{
   public  class EventTypeVModel:CheckedToolItemModel
   {
        public Action OnDfeipai;
        public Action OnDshenhe;
        public Action OnDshoulie;
        public Action OnDbanjie;
        public Action OnDwanjie;
        public Action OnDYguidang;
        public Action OnShowAll;
        EventTypeView eventTypeView = new EventTypeView();
        public ICommand DfeipaiCmd { get; set; }
        public ICommand DshenheCmd { get; set; }
        public ICommand DshoulieCmd { get; set; }
        public ICommand DbanjieCmd { get; set; }
        public ICommand DwanjieCmd { get; set; }
        public ICommand DYguidangCmd { get; set; }
        public ICommand ShowAllCmd { get; set; }        
        public EventTypeVModel()
        {
            this.DfeipaiCmd = new RelayCommand(OnShowDfeipai);
            this.DshenheCmd = new RelayCommand(OnShowDshenhe);
            this.DshoulieCmd = new RelayCommand(OnShowDshoulie);
            this.DbanjieCmd = new RelayCommand(OnShowDbanjie);
            this.DwanjieCmd = new RelayCommand(OnShowDwanjie);
            this.DYguidangCmd = new RelayCommand(OnShowDYguidang);
            this.ShowAllCmd = new RelayCommand(OnShowAllEvents);
        }
        public void ShowEventTypeView()
        {
            eventTypeView.DataContext = this;
            eventTypeView.Show();
            eventTypeView.Left = Application.Current.MainWindow.Width * 0.7;
            eventTypeView.Top = Application.Current.MainWindow.Height * 0.8;
        }
        public void HideEventTypeView()
        {
            eventTypeView.Hide();
        }
        public void OnShowDfeipai()
        {
            OnDfeipai();
            ResetImg();
            eventTypeView.DfeipaiImg.Height = 36;
            eventTypeView.DfeipaiImg.Width = 36;
            // (ImageSource)Helpers.ResourceHelper.FindResourceByKey("Dshenhe");
        }
        public void OnShowDshenhe()
        {
            OnDshenhe();
            ResetImg();
            eventTypeView.DshenheImg.Height = 36;
            eventTypeView.DshenheImg.Width = 36;
        }
        public void OnShowDshoulie()
        {
            OnDshoulie();
            ResetImg();
            eventTypeView.DshouliImg.Height = 36;
            eventTypeView.DshouliImg.Width = 36;
        }
        public void OnShowDbanjie()
        {
            OnDbanjie();
            ResetImg();
            eventTypeView.DbanjieImg.Height = 36;
            eventTypeView.DbanjieImg.Width = 36;
        }
        public void OnShowDwanjie()
        {
            OnDwanjie();
            ResetImg();
            eventTypeView.DwanjieImg.Height = 36;
            eventTypeView.DwanjieImg.Width = 36;
        }
        public void OnShowDYguidang()
        {
            OnDYguidang();
            ResetImg();
            eventTypeView.DYguidangImg.Height = 36;
            eventTypeView.DYguidangImg.Width = 36;
        }
        public void OnShowAllEvents()
        {
            OnShowAll();
            ResetImg();
            eventTypeView.ShowAllImg.Height = 36;
            eventTypeView.ShowAllImg.Width = 36;
        }
        public void ResetImg()
        {
            eventTypeView.DfeipaiImg.Height = 24;
            eventTypeView.DfeipaiImg.Width = 24;
            eventTypeView.DshenheImg.Height = 24;
            eventTypeView.DshenheImg.Width = 24;
            eventTypeView.DshouliImg.Height = 24;
            eventTypeView.DshouliImg.Width = 24;
            eventTypeView.DbanjieImg.Height = 24;
            eventTypeView.DbanjieImg.Width = 24;
            eventTypeView.DwanjieImg.Height = 24;
            eventTypeView.DwanjieImg.Width = 24;
            eventTypeView.DYguidangImg.Height = 24;
            eventTypeView.DYguidangImg.Width = 24;
            eventTypeView.ShowAllImg.Height = 24;
            eventTypeView.ShowAllImg.Width = 24;
        }

    }
}
