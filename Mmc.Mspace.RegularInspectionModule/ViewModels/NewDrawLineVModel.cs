using Microsoft.Win32;
using Mmc.Mspace.Common.Models;
using Mmc.Mspace.RegularInspectionModule.Dto;
using Mmc.Mspace.RegularInspectionModule.Views;
using Mmc.Windows.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.RegularInspectionModule.ViewModels
{
    public class NewDrawLineVModel: BaseViewModel
    {
        public Action<InspectModel, string> AddTIF;
        private NewDrawLineView newDrawLineView = null;
        InspectModel inspect = null;
        public  NewDrawLineVModel()
        {
            newDrawLineView = new NewDrawLineView();
            newDrawLineView.DataContext = this;
        }
        public void ShowDrawWin(InspectModel inspectModel)
        {
            inspect = inspectModel;
            newDrawLineView?.Show();
        }
       
        public void AddDom()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            string filePath = string.Empty;
          
            dialog.Multiselect = false;
            dialog.Title = Helpers.ResourceHelper.FindKey("PleaseChoose") + Helpers.ResourceHelper.FindKey("InspectionDom") + Helpers.ResourceHelper.FindKey("File");
            dialog.Filter = FileFilterStrings.TIF;
            if (dialog.ShowDialog() == true)
            {
                filePath = dialog.FileName;
                if(inspect!=null)
                {
                    //AddTIF(inspect, filePath);
                }
                //updateDataToSave(inModel, filePath);
            }
        }
        

    }
}
