﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BCLabManager.Model;
using BCLabManager.ViewModel;

namespace BCLabManager.View
{
    /// <summary>
    /// Interaction logic for AllProjectSettingType.xaml
    /// </summary>
    public partial class AllProjectSettingsView : UserControl
    {
        public AllProjectSettingsView()
        {
            InitializeComponent();
            //ProjectSettinglist.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
        }
    }
}
