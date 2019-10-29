﻿using System;
using System.Collections.Generic;
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
using BCLabManager.ViewModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.ObjectModel;
using BCLabManager.View;

namespace BCLabManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Repositories _repositories = new Repositories();    //model实例全部放在这里。viewmodel和database的source

        public AllBatteryTypesViewModel allBatteryTypesViewModel { get; set; }  //其中需要显示BatteryTypes和Batteries
        public AllBatteriesViewModel allBatteriesViewModel { get; set; }  //其中需要显示Batteries和Records

        public AllTestersViewModel allTestersViewModel { get; set; }  //其中需要显示Testers和Channels
        public AllChannelsViewModel allChannelsViewModel { get; set; }  //其中需要显示Channels和Records

        public AllChambersViewModel allChambersViewModel { get; set; }  //其中需要显示Chambers和Records

        public AllSubProgramTemplatesViewModel allSubProgramTemplatesViewModel { get; set; }  //其中需要显示SubPrograms

        public AllChargeTemperaturesViewModel allChargeTemperaturesViewModel { get; set; }
        public AllChargeCurrentsViewModel allChargeCurrentsViewModel { get; set; }

        public AllDischargeTemperaturesViewModel allDischargeTemperaturesViewModel { get; set; }
        public AllDischargeCurrentsViewModel allDischargeCurrentsViewModel { get; set; }

        public AllProgramsViewModel allProgramsViewModel { get; set; }  //其中需要显示Programs, SubPrograms, Test1, Test2, TestSteps

        //public DashBoardViewModel dashBoardViewModel { get; set; }

        //public List<BatteryTypeClass> BatteryTypes { get; set; }
        //public ObservableCollection<BatteryClass> Batteries { get; set; }
        //public List<TesterClass> Testers { get; set; }
        //public ObservableCollection<ChannelClass> Channels { get; set; }
        //public ObservableCollection<ChamberClass> Chambers { get; set; }
        //public List<SubProgramTemplate> SubProgramTemplates { get; set; }
        //public List<ChargeTemperatureClass> ChargeTemperatures { get; set; }
        //public List<ChargeCurrentClass> ChargeCurrents { get; set; }
        //public List<DischargeTemperatureClass> DischargeTemperatures { get; set; }
        //public List<DischargeCurrentClass> DischargeCurrents { get; set; }
        //public ObservableCollection<ProgramClass> Programs { get; set; }
        private DomainDataClass _domainData;

        public DomainDataClass DomainData
        {
            get { return _domainData; }
            set { _domainData = value; }
        }

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                InitializeConfiguration();
                InitializeDatabase();
                LoadFromDB(ref _domainData);
                InitializeNavigator();
                CreateViewModels();
                BindingVMandView();
                //MainTab.SelectedIndex = 3;
                //AllProgramsViewInstance.Programlist.SelectedItem = allProgramsViewModel.AllPrograms[4];
                //AllProgramsViewInstance.SubProgramlist.SelectedItem = allProgramsViewModel.AllPrograms[4].SubPrograms[1];
                //AllProgramsViewInstance.FirstTestRecordList.SelectedItem = allProgramsViewModel.AllPrograms[4].SubPrograms[1].Test1Records[0];
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                App.Current.Shutdown();
            }
        }

        private void InitializeNavigator()
        {
            Navigator.Initialize(this);
        }

        void InitializeConfiguration()
        {
            if (!File.Exists(GlobalSettings.ConfigurationFilePath))
            {
                ConfigureDBPath();
                CreateConfigurationFile();
            }
            else
            {
                LoadDBPathFromConfigurationFile();
            }
        }
        void InitializeDatabase()
        {
            if(!File.Exists(GlobalSettings.DbPath))
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Database.Migrate();
                }
                DatabasePopulator.PopulateHistoricData();
            }
        }

        private void ConfigureDBPath()
        {
            DBConfigureWindow dBConfigureWindow = new DBConfigureWindow();
            //dBConfigureWindow.Owner = this;
            dBConfigureWindow.ShowDialog();
            if (dBConfigureWindow.DialogResult == false)
            {
                throw new Exception("Database is not ready.");
            }

        }

        private void CreateConfigurationFile()
        {
            var fs = File.Create(GlobalSettings.ConfigurationFilePath);
            var sw = new StreamWriter(fs);
            sw.WriteLine(GlobalSettings.DbPath);
            sw.Close();
            fs.Close();
        }

        private void LoadDBPathFromConfigurationFile()
        {
            var fs = new FileStream(GlobalSettings.ConfigurationFilePath, FileMode.Open);
            var sr = new StreamReader(fs);
            GlobalSettings.DbPath = sr.ReadLine();
        }

        void LoadFromDB(ref DomainDataClass domainData)
        {
            using (var dbContext = new AppDbContext())
            {
                domainData.BatteryTypes = new ObservableCollection<BatteryTypeClass>(dbContext.BatteryTypes.ToList());

                domainData.Batteries = new ObservableCollection<BatteryClass>(
                    dbContext.Batteries
                    .Include(i => i.BatteryType)
                    .Include(o => o.Records)
                    .ToList()
                    );

                domainData.Testers = new LiObservableCollectionst<TesterClass>(dbContext.Testers.ToList());

                domainData.Channels = new ObservableCollection<ChannelClass>(
                    dbContext.Channels
                    .Include(i => i.Tester)
                    .Include(o => o.Records)
                    .ToList()
                    );

                domainData.Chambers = new ObservableCollection<ChamberClass>(
                    dbContext.Chambers
                    .Include(o => o.Records)
                    .ToList()
                    );

                domainData.SubProgramTemplates = new ObservableCollection<SubProgramTemplate>(dbContext.SubProgramTemplates.ToList());

                domainData.ChargeTemperatures = new ObservableCollection<ChargeTemperatureClass>(dbContext.ChargeTemperatures.ToList());

                domainData.ChargeCurrents = new ObservableCollection<ChargeCurrentClass>(dbContext.ChargeCurrents.ToList());

                domainData.DischargeTemperatures = new ObservableCollection<DischargeTemperatureClass>(dbContext.DischargeTemperatures.ToList());

                domainData.DischargeCurrents = new ObservableCollection<DischargeCurrentClass>(dbContext.DischargeCurrents.ToList());

                domainData.Programs = new ObservableCollection<ProgramClass>(dbContext.Programs
                    .Include(pro => pro.Group)
                    .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.FirstTestRecords)
                            .ThenInclude(tr => tr.RawDataList)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.FirstTestRecords)
                            .ThenInclude(tr => tr.AssignedBattery)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.FirstTestRecords)
                            .ThenInclude(tr => tr.AssignedChamber)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.FirstTestRecords)
                            .ThenInclude(tr => tr.AssignedChannel)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.SecondTestRecords)
                            .ThenInclude(tr => tr.RawDataList)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.SecondTestRecords)
                            .ThenInclude(tr => tr.AssignedBattery)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.SecondTestRecords)
                            .ThenInclude(tr => tr.AssignedChamber)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.SecondTestRecords)
                            .ThenInclude(tr => tr.AssignedChannel)
                    .ToList());

                foreach (var pro in domainData.Programs)
                {
                    foreach (var sub in pro.SubPrograms)
                    {
                        foreach (var tr in sub.FirstTestRecords)
                            sub.AssociateEvent(tr);
                        foreach (var tr in sub.SecondTestRecords)
                            sub.AssociateEvent(tr);
                    }
                }
            }
        }
        void CreateViewModels()
        {
            allBatteryTypesViewModel = new AllBatteryTypesViewModel(BatteryTypes);    //ViewModel初始化

            allBatteriesViewModel = new AllBatteriesViewModel(Batteries, BatteryTypes);    //ViewModel初始化

            allTestersViewModel = new AllTestersViewModel(Testers);    //ViewModel初始化

            allChannelsViewModel = new AllChannelsViewModel(Channels, Testers);    //ViewModel初始化

            allChambersViewModel = new AllChambersViewModel(Chambers);    //ViewModel初始化

            allSubProgramTemplatesViewModel = 
                new AllSubProgramTemplatesViewModel(
                    SubProgramTemplates, 
                    ChargeTemperatures,
                    ChargeCurrents,
                    DischargeTemperatures,
                    DischargeCurrents
                    );    //ViewModel初始化

            allChargeTemperaturesViewModel = new AllChargeTemperaturesViewModel(ChargeTemperatures);

            allChargeCurrentsViewModel = new AllChargeCurrentsViewModel(ChargeCurrents);

            allDischargeTemperaturesViewModel = new AllDischargeTemperaturesViewModel(DischargeTemperatures);

            allDischargeCurrentsViewModel = new AllDischargeCurrentsViewModel(DischargeCurrents);

            allProgramsViewModel = new AllProgramsViewModel
                (
                Programs,
                SubProgramTemplates,
                BatteryTypes,
                Batteries,
                Testers,
                Channels,
                Chambers
                );    //ViewModel初始化

            dashBoardViewModel = new DashBoardViewModel(Programs, Batteries, Channels, Chambers);
        }
        void BindingVMandView()
        {

            this.AllBatteryTypesViewInstance.DataContext = allBatteryTypesViewModel;                                                            //ViewModel跟View绑定


            this.AllBatteriesViewInstance.DataContext = allBatteriesViewModel;                                                            //ViewModel跟View绑定


            this.AllTestersViewInstance.DataContext = allTestersViewModel;                                                            //ViewModel跟View绑定


            this.AllChannelsViewInstance.DataContext = allChannelsViewModel;                                                            //ViewModel跟View绑定


            this.AllChambersViewInstance.DataContext = allChambersViewModel;                                                            //ViewModel跟View绑定

            this.AllSubProgramTemplatesViewInstance.DataContext = allSubProgramTemplatesViewModel;                                                            //ViewModel跟View绑定

            this.AllSubProgramTemplatesViewInstance.AllChargeTemperaturesViewInstance.DataContext = allChargeTemperaturesViewModel;

            this.AllSubProgramTemplatesViewInstance.AllChargeCurrentsViewInstance.DataContext = allChargeCurrentsViewModel;

            this.AllSubProgramTemplatesViewInstance.AllDischargeTemperaturesViewInstance.DataContext = allDischargeTemperaturesViewModel;

            this.AllSubProgramTemplatesViewInstance.AllDischargeCurrentsViewInstance.DataContext = allDischargeCurrentsViewModel;

            this.AllProgramsViewInstance.DataContext = allProgramsViewModel;                                                            //ViewModel跟View绑定

            this.DashBoardViewInstance.DataContext = dashBoardViewModel;
        }
    }
}
