﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mmc.Mspace.Project
{
    /// <summary>
    /// LoadImageLayerView.xaml 的交互逻辑
    /// </summary>
    public partial class LoadImageLayerView : Window
    {


        public LoadImageLayerView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public string GetRasterType()
        {
            return this.selectType.SelectionBoxItem.ToString();

        }




    }
}