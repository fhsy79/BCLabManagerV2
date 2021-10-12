﻿#define Show
//#define Requester
//#define StartWindow
using System;
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
using Microsoft.Win32;
using Npgsql;
using Path = System.IO.Path;
using System.Threading;
using System.Text.Json;

namespace BCLabManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mainWindowViewModel;
        public MainWindowViewModel mainWindowViewModel { get { return _mainWindowViewModel; } set { _mainWindowViewModel = value; } }
        public MainWindow()
        {
#if StartWindow
            var startupWindow = new StartupWindow();
            startupWindow.Show();
#endif
            try
            {
#if Show
                InitializeComponent();
#if !Requester
#endif
                InitializeNavigator();
                mainWindowViewModel = new MainWindowViewModel();
                DataContext = mainWindowViewModel;
#if Requester
                UpdateUIForRequester();
#endif
#else
                App.Current.Shutdown();
#endif
            }
            catch (DatabaseAccessException e)
            {
#if StartWindow
                startupWindow.Close();
#endif
                MessageBox.Show($"{e.Message}\n" +
                    $"{e.InnerException}\n" +
                    $"\n" +
                    $"Please setup database in the configuration window correctly!");
            }
            catch (Exception e)
            {
#if StartWindow
                startupWindow.Close();
#endif
                MessageBox.Show($"{e.Message}\n" +
                    $"{e.InnerException}");
                App.Current.Shutdown();
            }
            finally
            {
#if StartWindow
                if (startupWindow.IsActive)
                    startupWindow.Close();
#endif
            }
        }
        private void InitializeNavigator()
        {
            Navigator.Initialize(this);
        }

        private void UpdateUIForRequester()
        {
            AllTestersViewInstance.ButtonPanel.IsEnabled = false;
            AllChannelsViewInstance.ButtonPanel.IsEnabled = false;
            AllBatteriesViewInstance.ButtonPanel.IsEnabled = false;
            AllChambersViewInstance.ButtonPanel.IsEnabled = false;
            AllProgramTypesViewInstance.Visibility = Visibility.Collapsed;
            AllTableMakerProductTypesViewInstance.Visibility = Visibility.Collapsed;
            AllProgramsViewInstance.RecipeButtonPanel.IsEnabled = false;
            AllProgramsViewInstance.FreeTestRecordButtonPanel.IsEnabled = false;
            AllProgramsViewInstance.DirectCommitBtn.IsEnabled = false;
            AllProgramsViewInstance.ExecuteBtn.IsEnabled = false;
            AllProgramsViewInstance.CommitBtn.IsEnabled = false;
            AllProgramsViewInstance.InvalidateBtn.IsEnabled = false;
            AllProgramsViewInstance.AddBtn.IsEnabled = false;
            Grid.SetColumnSpan(ProjectBorder, 3);
            Grid.SetColumn(ProjectSettingBorder, 4);
            Grid.SetColumnSpan(ProjectSettingBorder, 5);
            //Title = "BCLM-R v0.2.0.5";
            var array = Title.Split();
            Title = $"{array[0]}-R {array[1]}";
        }

        private void Event_Click(object sender, RoutedEventArgs e)
        {
            AllEventsView allEventsView = new AllEventsView();
            var vm = new AllEventsViewModel
                 (
                 mainWindowViewModel.ProgramService,
                 mainWindowViewModel.ProjectService,
                 mainWindowViewModel.ProgramTypeService,
                 mainWindowViewModel.BatteryService,
                 mainWindowViewModel.TesterService,
                 mainWindowViewModel.ChannelService,
                 mainWindowViewModel.ChamberService,
                 mainWindowViewModel.BatteryTypeService
                 );
            allEventsView.DataContext = vm;// new AllEventsViewModel(/*EventService*/);
            allEventsView.ShowDialog();
        }


        private void Config_Click(object sender, RoutedEventArgs e)
        {
            Configuration();
        }
        private void Configuration()
        {
            Configuration conf = new Configuration();
            conf.RemotePath = GlobalSettings.RemotePath;
            conf.EnableTest = GlobalSettings.EnableTest;
            conf.MappingPath = GlobalSettings.MappingPath;
            conf.DatabaseHost = GlobalSettings.DatabaseHost;
            conf.DatabaseName = GlobalSettings.DatabaseName;
            conf.DatabaseUser = GlobalSettings.DatabaseUser;
            conf.DatabasePassword = GlobalSettings.DatabasePassword;
            ConfigurationView configView = new ConfigurationView();
            var vm = new ConfigurationViewModel(conf);
            configView.DataContext = vm;// new AllEventsViewModel(/*EventService*/);
            configView.ShowDialog();
        }
        private void LocalFileExistenceCheck_Click(object sender, RoutedEventArgs e)    //将本地缺失文件列表放入Running Log
        {
            uint existNum;
            List<string[]> list = FileExistenceCheck(FileTransferHelper.Remote2Local, out existNum);
            if (list.Count > 0)
            {
                string str = $"{list.Count} Missing Files:\n";
                foreach (var arr in list)
                {
                    str += $"{arr[0]}\n";
                }
                RuningLog.Write(str);
                MessageBox.Show($"{list.Count} files are missing. Check running log for the details.");
            }
            else
                MessageBox.Show($"All {existNum.ToString()} files are existed.");
        }
        private void RemoteFileExistenceCheck_Click(object sender, RoutedEventArgs e)   //将远程缺失文件列表放入Running Log
        {
            uint existNum;
            List<string[]> list = FileExistenceCheck(FileTransferHelper.Remote2Universal, out existNum);
            if (list.Count > 0)
            {
                string str = $"{list.Count} Missing Files:\n";
                foreach (var arr in list)
                {
                    str += $"{arr[0]}\n";
                }
                RuningLog.Write(str);
                MessageBox.Show($"{list.Count} files are missing. Check running log for the details.");
            }
            else
                MessageBox.Show($"All {existNum.ToString()} files are existed.");
        }

        private void LocalFileMD5Check_Click(object sender, RoutedEventArgs e)  //将本地损坏文件和没有MD5的文件列表放入Running Log
        {
            List<string[]> list = FileMD5Check(FileTransferHelper.Remote2Local);
            if (list.Count > 0)
            {
                var emptylist = list.Where(o => o[1] == null || o[1] == string.Empty).ToList();
                var brokenlist = list.Where(o => o[1] != null && o[1] != string.Empty).ToList();
                string str = $"{brokenlist.Count} MD5 Broken Files:\n";
                foreach (var arr in brokenlist)
                {
                    str += $"{arr[0]}, {arr[1]}\n";
                }
                str += $"{emptylist.Count} MD5 Empty Files:\n";
                foreach (var arr in emptylist)
                {
                    str += $"{arr[0]}\n";
                }
                RuningLog.Write(str);
                MessageBox.Show($"{brokenlist.Count} files' MD5 are broken.\n" +
                    $"{emptylist.Count} files' MD5 are empty.\n" +
                    $" Check running log for the details.");
            }
            else
                MessageBox.Show($"All files are fine.");
        }

        private void RemoteFileMD5Check_Click(object sender, RoutedEventArgs e) //将远程损坏文件和没有MD5的文件列表放入Running Log
        {
            if (MessageBoxResult.Cancel == MessageBox.Show("It may take a long time to download, are you sure to proceed?", "Remote File MD5 Check", MessageBoxButton.OKCancel))
                return;
            Thread t = new Thread(() =>
            {
                List<string[]> list;
                try
                {
                    list = FileMD5Check(FileTransferHelper.Remote2Universal);
                }
                catch (Exception error)
                {
                    MessageBox.Show($"MD5 Check Failed.\n{error.Message}");
                    return;
                }
                if (list != null && list.Count > 0)
                {
                    var emptylist = list.Where(o => o[1] == null || o[1] == string.Empty).ToList();
                    var brokenlist = list.Where(o => o[1] != null && o[1] != string.Empty).ToList();
                    string str = $"{brokenlist.Count} MD5 Broken Files:\n";
                    foreach (var arr in brokenlist)
                    {
                        str += $"{arr[0]}, {arr[1]}\n";
                    }
                    str = $"{emptylist.Count} MD5 Empty Files:\n";
                    foreach (var arr in emptylist)
                    {
                        str += $"{arr[0]}\n";
                    }
                    RuningLog.Write(str);
                    MessageBox.Show($"{brokenlist.Count} files' MD5 are broken.\n" +
                        $"{emptylist.Count} files' MD5 are empty.\n" +
                        $" Check running log for the details.");
                }
                else
                    MessageBox.Show($"All files are fine.");
            });
            t.Start();
        }
        private void LocalMissingFileDownload_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Cancel == MessageBox.Show("It may take a long time to download, are you sure to proceed?", "Local File Restore", MessageBoxButton.OKCancel))
                return;
            Thread t = new Thread(() =>
            {
                try
                {
                    List<string> RestoreList = new List<string>();
                    uint existNum;
                    List<string[]> list = FileExistenceCheck(FileTransferHelper.Remote2Local, out existNum);
                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            if (FileTransferHelper.FileDownload(item[0], item[1]))
                                RestoreList.Add(item[0]);
                        }
                        string str = $"{RestoreList.Count} files restored:\n";
                        foreach (var file in RestoreList)
                        {
                            str += $"{file}\n";
                        }
                        RuningLog.Write(str);
                        MessageBox.Show($"{RestoreList.Count} files restored.\n" +
                            $" Check running log for the details.");
                    }
                    else
                        MessageBox.Show($"All {existNum.ToString()} files are existed.");
                }
                catch (Exception error)
                {
                    MessageBox.Show($"Local File Restore Failed.\n{error.Message}");
                }
            });
            t.Start();
        }
        private void RemoteMissingFileRestore_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Cancel == MessageBox.Show("It may take a long time to download, are you sure to proceed?", "Remote File Restore", MessageBoxButton.OKCancel))
                return;
            Thread t = new Thread(() =>
            {
                try
                {
                    List<string> RestoreList = new List<string>();
                    uint existNum;
                    List<string[]> list = FileExistenceCheck(FileTransferHelper.Remote2Universal, out existNum);
                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            if (FileTransferHelper.FileRestore(item[0], item[1]))
                                RestoreList.Add(item[0]);
                        }
                        string str = $"{RestoreList.Count} files restored:\n";
                        foreach (var file in RestoreList)
                        {
                            str += $"{file}\n";
                        }
                        RuningLog.Write(str);
                        MessageBox.Show($"{RestoreList.Count} files restored.\n" +
                            $" Check running log for the details.");
                    }
                    else
                        MessageBox.Show($"All {existNum.ToString()} files are existed.");
                }
                catch (Exception error)
                {
                    MessageBox.Show($"Remote File Restore Failed.\n{error.Message}");
                }
            });
            t.Start();
        }
        private void RemoteFileMD5Commit_Click(object sender, RoutedEventArgs e)    //如果远程文件存在，但没有MD5，则计算远程文件的MD5，提交到数据库
        {
            Thread t = new Thread(() =>
            {
                List<string> RestoreMD5List = new List<string>();
                foreach (var tmr in mainWindowViewModel.TableMakerRecordService.Items)
                {
                    foreach (var tmp in tmr.Products)
                    {
                        if (tmp.FilePath == string.Empty || tmp.FilePath == null)   //一般不会有这样的情况
                            continue;
                        string filepath = FileTransferHelper.Remote2Universal(tmp.FilePath);
                        if (tmp.MD5 == null || tmp.MD5 == string.Empty)
                        {
                            if (File.Exists(filepath))
                            {
                                using (var context = new AppDbContext())
                                {
                                    var dbtmp = context.TableMakerProducts.SingleOrDefault(o => o.Id == tmp.Id);
                                    dbtmp.MD5 = FileTransferHelper.GetMD5(filepath);
                                    context.SaveChanges();
                                }
                                RestoreMD5List.Add(filepath);
                            }
                        }
                    }
                }

                foreach (var tr in mainWindowViewModel.ProgramService.RecipeService.TestRecordService.Items)
                {
                    if (tr.TestFilePath == null || tr.TestFilePath == string.Empty) //有些test record还没有数据
                        continue;
                    string filepath = FileTransferHelper.Remote2Universal(tr.TestFilePath);
                    if (tr.MD5 == null || tr.MD5 == string.Empty)
                    {
                        if (File.Exists(filepath))
                        {
                            using (var context = new AppDbContext())
                            {
                                var dbtr = context.TestRecords.SingleOrDefault(o => o.Id == tr.Id);
                                dbtr.MD5 = FileTransferHelper.GetMD5(filepath);
                                context.SaveChanges();
                            }
                            RestoreMD5List.Add(filepath);
                        }
                    }
                }
                MessageBox.Show($"{RestoreMD5List.Count} files' MD5 is restored");
            });
            t.Start();
        }

        private List<string[]> FileExistenceCheck(Func<string, string> relocate, out uint existNum)//检查所有文件是否存在，如果不存在，就放入返回值里。返回数组元素0是文件地址，1是MD5
        {
            List<string[]> MissingList = new List<string[]>();
            existNum = 0;
            foreach (var tmr in mainWindowViewModel.TableMakerRecordService.Items)
            {
                foreach (var tmp in tmr.Products)
                {
                    if (tmp.FilePath == string.Empty || tmp.FilePath == null)   //一般不会有这样的情况
                        continue;
                    string filepath = relocate(tmp.FilePath);
                    if (!File.Exists(filepath))
                    {
                        MissingList.Add(new string[] { tmp.FilePath, tmp.MD5 });
                    }
                    else
                        existNum++;
                }
            }

            foreach (var tr in mainWindowViewModel.ProgramService.RecipeService.TestRecordService.Items)
            {
                if (tr.TestFilePath == string.Empty || tr.TestFilePath == null || tr.Status == TestStatus.Abandoned) //有些test record还没有数据
                    continue;
                string filepath = relocate(tr.TestFilePath);
                if (!File.Exists(filepath))
                {
                    MissingList.Add(new string[] { tr.TestFilePath, tr.MD5 });
                }
                else
                    existNum++;
            }
            return MissingList;
        }

        private List<string[]> FileMD5Check(Func<string, string> relocate)  //检查所有文件的MD5，如果MD5是空的，或者MD5是错的，就放入返回值里。不考虑文件不存在的情况。返回数组元素0是文件地址，1是数据库中记录的MD5
        {
            List<string[]> BrokenList = new List<string[]>();
            //List<string> RestoreList = new List<string>();
            foreach (var tmr in mainWindowViewModel.TableMakerRecordService.Items)
            {
                foreach (var tmp in tmr.Products)
                {
                    if (tmp.FilePath == string.Empty || tmp.FilePath == null)   //一般不会有这样的情况
                        continue;
                    string filepath = relocate(tmp.FilePath);
                    if (tmp.MD5 != null && tmp.MD5 != string.Empty)
                    {
                        if (File.Exists(filepath))
                            if (!FileTransferHelper.CheckFileMD5(filepath, tmp.MD5))
                            {
                                BrokenList.Add(new string[] { filepath, tmp.MD5 }); //Wrong MD5
                            }
                    }
                    else
                    {
                        BrokenList.Add(new string[] { filepath, tmp.MD5 }); //Empty MD5
                    }
                }
            }

            foreach (var tr in mainWindowViewModel.ProgramService.RecipeService.TestRecordService.Items)
            {
                if (tr.TestFilePath == string.Empty || tr.TestFilePath == null || tr.Status == TestStatus.Abandoned) //有些test record还没有数据
                    continue;
                string filepath = relocate(tr.TestFilePath);
                if (tr.MD5 != null && tr.MD5 != string.Empty)
                {
                    if (File.Exists(filepath))
                        if (!FileTransferHelper.CheckFileMD5(filepath, tr.MD5))
                        {
                            BrokenList.Add(new string[] { filepath, tr.MD5 }); //Wrong MD5
                        }
                }
                else
                {
                    BrokenList.Add(new string[] { filepath, tr.MD5 }); //Empty MD5
                }
            }
            return BrokenList;
        }

        private void DischargeVoltageRasingCheck_Click(object sender, RoutedEventArgs e)
        {
            //List<ChromaNode> nodes = new List<ChromaNode>();
            List<TestRecord> trs;
            List<Battery> batts;
            List<Channel> chnls;
            using (var dbContext = new AppDbContext())
            {
                trs = dbContext.TestRecords
                    .Include(tr => tr.Recipe.Program.Project.BatteryType)
                    .Include(tr => tr.Recipe.Program.Type)
                                //.Include(tr => tr.Recipe)
                                //    .ThenInclude(rec => rec.Program)
                                //        .ThenInclude(pro => pro.Project)
                                //            .ThenInclude(prj => prj.BatteryType).ToList()
                                .Where(tr => tr.Status!=TestStatus.Abandoned && tr.TesterStr == "17200" && tr.TestFilePath != string.Empty).ToList();
                batts = dbContext.Batteries.Include(batt => batt.BatteryType).ToList();
                chnls = dbContext.Channels.Include(chnl => chnl.Tester).ToList();
            }
            var batteryTypes = trs.Select(tr => tr.Recipe.Program.Project.BatteryType).Distinct().ToList();
            var projects = trs.Select(tr => tr.Recipe.Program.Project).Distinct().ToList();
            var programs = trs.Select(tr => tr.Recipe.Program).Distinct().ToList();
            var recipes = trs.Select(tr => tr.Recipe).Distinct().ToList();
            Dictionary<TestRecord, List<List<ChromaNode>>> ErrorDetailLogs = new Dictionary<TestRecord, List<List<ChromaNode>>>();
            Dictionary<TestRecord, List<ErrorDescriptor>> ErrorBriefLogs = new Dictionary<TestRecord, List<ErrorDescriptor>>();
            if (trs != null)
            {
                foreach (var tr in trs)
                {
                    //dischargeVoltageRasingLogs.AddRange(GetDischargeVoltageRaisingLogs(tr));
                    try
                    {
                        List<List<ChromaNode>> nodesList;
                        var err = GetDischargeVoltageRaisingNodes(tr, out nodesList);
                        if (err != ErrorCode.NORMAL)
                        {
                            MessageBox.Show($"GetDischargeVoltageRaisingNodes failed.{tr.TestFilePath}");
                        }
                        if (err == ErrorCode.NORMAL && nodesList.Count > 0)
                        {
                            ErrorDetailLogs.Add(tr, nodesList);
                        }
                    }
                    catch (Exception err)
                    {
                        RuningLog.Write($"{tr.TestFilePath}\n{err.Message}!\n");
                    }
                }
                foreach (var key in ErrorDetailLogs.Keys)
                {
                    List<ErrorDescriptor> BriefList = new List<ErrorDescriptor>();
                    int i = 1;
                    foreach (var frame in ErrorDetailLogs[key])
                    {
                        ErrorDescriptor ed = GetErrorFrameBrief(frame, i);
                        BriefList.Add(ed);
                        i++;
                    }
                    ErrorBriefLogs.Add(key, BriefList);
                }
                var newBriefLogs = new Dictionary<TestRecord, List<ErrorDescriptor>>();
                foreach (var ebl in ErrorBriefLogs)
                {
                    var BriefList = ebl.Value;
                    var newBriefList = BriefList.Where(ed => ed.FrameLength > 2 && ed.RaisedVoltage > 0.0003 && ed.MaxDelta > 0.0003).ToList();
                    if (newBriefList.Count > 0)
                        newBriefLogs.Add(ebl.Key, newBriefList);
                }
                ErrorBriefLogs = newBriefLogs;
                var newDetailLogs = new Dictionary<TestRecord, List<List<ChromaNode>>>();
                foreach (var edl in ErrorDetailLogs)
                {
                    if (!ErrorBriefLogs.Keys.Contains(edl.Key))
                        continue;
                    var briefs = ErrorBriefLogs[edl.Key];
                    var nodesList = edl.Value;
                    var newFrameList = new List<List<ChromaNode>>();
                    foreach (var ed in briefs)
                    {
                        newFrameList.Add(nodesList[ed.Index-1]);
                    }
                    if (newFrameList.Count > 0)
                        newDetailLogs.Add(edl.Key, newFrameList);
                }
                ErrorDetailLogs = newDetailLogs;
            }
            RuningLog.NewLog("Summary");
            RuningLog.Write($"----------------------------Summary--------------------------\n");
            RuningLog.Write($"Total Test Records: {trs.Count}\n");
            RuningLog.Write($"Total Error Test Records: {ErrorDetailLogs.Count}\n");
            var errorFrameNumber = ErrorDetailLogs.Sum(o => o.Value.Sum(p => p.Count));
            RuningLog.Write($"Total Error Frames: {errorFrameNumber}\n");
            RuningLog.NewLog("Occur Rate");
            RuningLog.Write($"----------------------------Occur Rate--------------------------\n");
            foreach (var bt in batteryTypes)
            {
                var a = trs.Count(tr => tr.Recipe.Program.Project.BatteryType == bt);
                var b = ErrorDetailLogs.Keys.Count(tr => tr.Recipe.Program.Project.BatteryType == bt);
                if (b == 0)
                    continue;
                var c = bt.Projects.Sum(prj => prj.Programs.Sum(pro => pro.Recipes.Count));
                var d = bt.Projects.Sum(prj => prj.Programs.Sum(pro => pro.Recipes.Count(rec => rec.TestRecords.Any(tr => ErrorDetailLogs.Keys.Contains(tr)))));
                var E = bt.Projects.Sum(prj => prj.Programs.Count);
                var f = bt.Projects.Sum(prj => prj.Programs.Count(pro => pro.Recipes.Any(rec => rec.TestRecords.Any(tr => ErrorDetailLogs.Keys.Contains(tr)))));
                var g = bt.Projects.Count;
                var h = bt.Projects.Count(prj => prj.Programs.Any(pro => pro.Recipes.Any(rec => rec.TestRecords.Any(tr => ErrorDetailLogs.Keys.Contains(tr)))));
                RuningLog.Write($"Battery Type:{bt.Name},Project OR:{h}\\{g}, Program OR:{f}\\{E},Recipe OR:{d}\\{c},TR OR:{b}\\{a}\n");
                foreach (var prj in bt.Projects)
                {
                    a = trs.Count(tr => tr.Recipe.Program.Project == prj);
                    b = ErrorDetailLogs.Keys.Count(tr => tr.Recipe.Program.Project == prj);
                    if (b == 0)
                        continue;
                    c = prj.Programs.Sum(pro => pro.Recipes.Count);
                    d = prj.Programs.Sum(pro => pro.Recipes.Count(rec => rec.TestRecords.Any(tr => ErrorDetailLogs.Keys.Contains(tr))));
                    E = prj.Programs.Count;
                    f = prj.Programs.Count(pro => pro.Recipes.Any(rec => rec.TestRecords.Any(tr => ErrorDetailLogs.Keys.Contains(tr))));
                    RuningLog.Write($"\tProject:{prj.Name},Program OR:{f}\\{E},Recipe OR:{d}\\{c},TR OR:{b}\\{a}\n");
                    foreach (var pro in prj.Programs)
                    {
                        a = trs.Count(tr => tr.Recipe.Program == pro);
                        b = ErrorDetailLogs.Keys.Count(tr => tr.Recipe.Program == pro);
                        if (b == 0)
                            continue;
                        c = pro.Recipes.Count;
                        d = pro.Recipes.Count(rec => rec.TestRecords.Any(tr => ErrorDetailLogs.Keys.Contains(tr)));
                        RuningLog.Write($"\t\tProgram: {pro.Name},Recipe OR:{d}\\{c},TR OR:{b}\\{a}\n");
                        foreach (var rec in pro.Recipes)
                        {
                            a = trs.Count(tr => tr.Recipe == rec);
                            b = ErrorDetailLogs.Keys.Count(tr => tr.Recipe == rec);
                            if (b == 0)
                                continue;
                            RuningLog.Write($"\t\t\tRecipe: {rec.Name}, OR:{b}\\{a}\n");
                            foreach (var tr1 in rec.TestRecords)
                            {
                                a = trs.Count(tr => tr == tr1);
                                b = ErrorDetailLogs.Keys.Count(tr => tr == tr1);
                                if (b == 0)
                                    continue;
                                RuningLog.Write($"\t\t\t\tTest Record: {tr1.TestFilePath}, OR:{b}\\{a}\n");
                                if (ErrorDetailLogs.Keys.Contains(tr1))
                                {
                                    int i = 1;
                                    foreach (var log in ErrorBriefLogs[tr1])
                                    {
                                        RuningLog.Write($"\t\t\t\t\t{i},Length:{log.FrameLength},Raised Vol:{log.RaisedVoltage},Avrerage Vol:{log.AvrVoltage},Average Curr:{log.AvrCurrent},Average Temp:{log.AvrTemperature},Min Delta:{log.MinDelta},Max Delta:{log.MaxDelta}, Avrerage Delta:{log.AvrDelta}\n");
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            RuningLog.NewLog("Assets Occur Rate");
            RuningLog.Write($"----------------------------Assets Occur Rate--------------------------\n");
            foreach (var batt in batts)
            {
                var a = trs.Count(tr => tr.BatteryTypeStr == batt.BatteryType.Name && tr.BatteryStr == batt.Name);
                var b = ErrorDetailLogs.Keys.Count(tr => tr.BatteryTypeStr == batt.BatteryType.Name && tr.BatteryStr == batt.Name);
                if (b == 0)
                    continue;
                RuningLog.Write($"\tBattery:{batt.BatteryType.Name}-{batt.Name},{b}\\{a}\n");
            }
            foreach (var chnl in chnls)
            {
                var a = trs.Count(tr => tr.TesterStr == chnl.Tester.Name && tr.ChannelStr == chnl.Name);
                var b = ErrorDetailLogs.Keys.Count(tr => tr.TesterStr == chnl.Tester.Name && tr.ChannelStr == chnl.Name);
                if (b == 0)
                    continue;
                RuningLog.Write($"\tChannl:{chnl.Tester.Name}-{chnl.Name},{b}\\{a}\n");
            }
            RuningLog.NewLog("Details");
            RuningLog.Write($"----------------------------Details--------------------------\n");
            foreach (var log in ErrorDetailLogs)
            {
                var tr = log.Key;
                var nodesList = log.Value;
                RuningLog.Write($"{nodesList.Count} error frames in {tr.TestFilePath}\n");
                int i = 1;
                foreach (var nodes in nodesList)
                {
                    RuningLog.Write($"\tThe {i}st frame({nodes.Count}):\n");
                    foreach (var node in nodes)
                    {
                        RuningLog.Write($"\t\t{node.StepNo},{node.Step},{node.TimeInMS},{node.Time},{node.Cycle},{node.Loop},{node.StepMode},{node.Mode},{node.Current},{node.Voltage},{node.Temperature},{node.Capacity},{node.TotalCapacity},{node.Status}\n");
                    }
                    i++;
                }
            }

            /*RuningLog.Write($"----------------------------Briefs--------------------------\n");
            int errorNumber = 0;
            RuningLog.Write($"No,Length,MinVol,MaxVol,RaisedVol,AvrVol,MinCurr,MaxCurr,AvrCurr,MinTemp,MaxTemp,AvrTemp,MinDelta,MaxDelta,AvrDelta\n");
            foreach (var log in ErrorBriefLogs)
            {
                var tr = log.Key;
                var briefs = log.Value;
                errorNumber += briefs.Count;
                int i = 1;
                foreach (var brief in briefs)
                {
                    RuningLog.Write($"{i},{brief.FrameLength},{brief.MinVoltage},{brief.MaxVoltage},{brief.RaisedVoltage},{brief.AvrVoltage},{brief.MinCurrent},{brief.MaxCurrent},{brief.AvrCurrent},{brief.MinTemperature},{brief.MaxTemperature},{brief.AvrTemperature},{brief.MinDelta},{brief.MaxDelta},{brief.AvrDelta}\n");
                    i++;
                }
            }*/
        }

        private ErrorDescriptor GetErrorFrameBrief(List<ChromaNode> frame, int index)
        {
            ErrorDescriptor ed = new ErrorDescriptor();
            ed.Index = index;
            ed.FrameLength = frame.Count;
            ed.MinCurrent = Math.Round(frame.Min(o => o.Current), 5);
            ed.MaxCurrent = Math.Round(frame.Max(o => o.Current), 5);
            ed.AvrCurrent = Math.Round(frame.Average(o => o.Current), 5);
            ed.MinTemperature = Math.Round(frame.Min(o => o.Temperature), 5);
            ed.MaxTemperature = Math.Round(frame.Max(o => o.Temperature), 5);
            ed.AvrTemperature = Math.Round(frame.Average(o => o.Temperature), 5);
            ed.MinVoltage = Math.Round(frame.Min(o => o.Voltage), 5);
            ed.MaxVoltage = Math.Round(frame.Max(o => o.Voltage), 5);
            ed.AvrVoltage = Math.Round(frame.Average(o => o.Voltage), 5);
            ed.RaisedVoltage = Math.Round(ed.MaxVoltage - ed.MinVoltage, 5);
            ed.MinDelta = 9999;
            ed.MaxDelta = 0;
            ed.AvrDelta = 0;
            for (int i = 1; i < frame.Count; i++)
            {
                var delta = frame[i].Voltage - frame[i - 1].Voltage;
                ed.AvrDelta += delta;
                if (delta < ed.MinDelta)
                    ed.MinDelta = delta;
                if (delta > ed.MaxDelta)
                    ed.MaxDelta = delta;
            }
            ed.AvrDelta /= (frame.Count - 1);
            ed.MinDelta = Math.Round(ed.MinDelta, 5);
            ed.MaxDelta = Math.Round(ed.MaxDelta, 5);
            ed.AvrDelta = Math.Round(ed.AvrDelta, 5);
            return ed;
        }

        private void UpdateProjectIdForTMP_Click(object sender, RoutedEventArgs e)
        {
            using (var dbContext = new AppDbContext())
            {
                //trs = dbContext.TestRecords
                //    .Include(tr => tr.Recipe.Program.Project.BatteryType)
                //    .Include(tr => tr.Recipe.Program.Type)
                //                .Where(tr => tr.TesterStr == "17200" && tr.TestFilePath != string.Empty).ToList();
                var tmrs = dbContext.TableMakerRecords.Include(tmr => tmr.Project).Include(tmr => tmr.Products).ToList();
                foreach (var tmr in tmrs)
                {
                    foreach (var tmp in tmr.Products)
                    {
                        tmp.Project = tmr.Project;
                        tmp.IsValid = tmr.IsValid;
                    }
                }
                dbContext.SaveChanges();

            }
        }

        private uint GetDischargeVoltageRaisingNodes(TestRecord tr, out List<List<ChromaNode>> nodesList)
        {
            nodesList = new List<List<ChromaNode>>();
            //List<ChromaNode> nodes;
            SourceData sd = new SourceData();
            var localPath = FileTransferHelper.Remote2Local(tr.TestFilePath);
            if (!File.Exists(localPath)) //本地不存在
            {
                if (!FileTransferHelper.FileDownload(tr.TestFilePath, tr.MD5))  //下载不成功
                    return ErrorCode.UNDEFINED;
            }

            FileStream fs = new FileStream(localPath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            int lineIndex = 0;
            for (; lineIndex < 10; lineIndex++)     //第十行以后都是数据
            {
                sr.ReadLine();
            }
            bool isInErrFrame = false;
            double previousVoltage, voltage = 9999;
            ChromaNode previousNode, node = null;
            while (true)
            {
                lineIndex++;
                var line = sr.ReadLine();
                if (line == null)
                    break;
                previousNode = node;
                previousVoltage = voltage;
                node = DataPreprocesser.GetNodeFromeString(line);
                if (node == null)
                {
                    RuningLog.Write($"----------{tr.TestFilePath} line:{lineIndex}\n");
                    break;
                }
                voltage = node.Voltage;
                if (previousNode == null)
                    continue;
                if (node.Current >= 0 && !isInErrFrame)
                    continue;
                //if (previousVoltage == 0)
                //{
                //    previousVoltage = node.Voltage;
                //    continue;
                //}
                if (node.StepMode == ActionMode.REST)
                    continue;
                if (previousVoltage < voltage & !isInErrFrame) //刚刚遇到错误
                {
                    isInErrFrame = true;
                    var nodes = new List<ChromaNode>();
                    nodes.Add(previousNode);
                    nodes.Add(node);
                    nodesList.Add(nodes);
                }
                else if (previousVoltage < voltage & isInErrFrame)  //继续遇到错误
                {
                    var nodes = nodesList.Last();
                    nodes.Add(node);
                }
                else if (previousVoltage >= voltage && isInErrFrame)    //离开错误
                {
                    isInErrFrame = false;
                }
                else
                {
                }
            }

            return ErrorCode.NORMAL;
        }

        #region Temporary Method
        private void TemporaryMethod_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new AppDbContext())
            {
                var project = context.Projects.SingleOrDefault(o => o.Name == "O2Sim4");
                var programs = context.Programs.Include(o => o.Project).ToList();
                var programNames = programs.Where(o => o.Project.Id == project.Id).Select(o => o.Name);
                //var programs = context.Programs.Include(o=>o.Project).Where(o => o.Project.Id == project.Id);
                foreach (var proName in programNames)
                {
                    //if (proName == "EV-Static-N2")
                    //    continue;
                    var program = context.Programs.SingleOrDefault(o => o.Project.Id == project.Id && o.Name == proName);
                    //foreach (var program in programs)
                    {
                        context.Entry(program).Collection(p => p.Recipes).Load();
                        foreach (var rec in program.Recipes)
                        {
                            context.Entry(rec).Collection(r => r.TestRecords).Load();
                            context.Entry(rec).Reference(r => r.RecipeTemplate).Load();
                        }
                        foreach (var rec in program.Recipes)
                        {
                            if (rec.Name.Contains('_'))
                                rec.Name = rec.Name.Replace('_', '-');
                            //else
                            //    continue;
                            if (rec.RecipeTemplate.Name.Contains('_'))
                                rec.RecipeTemplate.Name = rec.RecipeTemplate.Name.Replace('_', '-');
                            //else
                            //    continue;
                            foreach (var tr in rec.TestRecords)
                            {
                                tr.RecipeStr = $"{tr.Temperature}Deg-{rec.Name}";
                                if (tr.Status != TestStatus.Waiting && tr.TestFilePath.Contains("csv"))
                                {
                                    if (!tr.TestFilePath.Contains(tr.RecipeStr))
                                    {
                                        tr.TestFilePath = ReplaceRecipeString(tr);
                                    }
                                    UpdateFileName(tr);
                                }
                            }
                        }
                        program.RecipeTemplates = program.Recipes.Select(o => o.Name).Distinct().ToList();
                    }
                    context.SaveChanges();
                }
            }
        }

        private void TemporaryMethod1_Click(object sender, RoutedEventArgs e)  //查MD5错误文件的MD5码
        {
            List<string[]> list = FileMD5Check(FileTransferHelper.Remote2Local);
            if (list.Count > 0)
            {
                string str = string.Empty;
                foreach (var item in list)
                {
                    var localMD5 = FileTransferHelper.GetMD5(item[0]);
                    var remotePath = FileTransferHelper.Local2Remote(item[0]);
                    var remoteMD5 = FileTransferHelper.GetMD5(remotePath);
                    str += $"Local File Path: {item[0]}, Local File MD5: {localMD5}\n" +
                        $"Remote File Path: {remotePath}, Remote File MD5: {remoteMD5}\n" +
                        $"MD5 in database: {item[1]}\n";
                }
                RuningLog.Write(str);
                MessageBox.Show($"{list.Count} local files are broken.\n" +
                    $" Check running log for the details.");
            }
            else
                MessageBox.Show($"All files are fine.");
        }
        private void TemporaryMethod2_Click(object sender, RoutedEventArgs e)  //查单个文件的MD5码
        {
            string filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select the file you want to check.";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                var MD5 = FileTransferHelper.GetMD5(filePath);
                MessageBox.Show(MD5);
            }
        }

        private void UpdateFileName(TestRecord tr)
        {
            var testFilePath = tr.TestFilePath;
            var folder = Path.GetDirectoryName(testFilePath);
            var timestamp = tr.StartTime.ToString("yyyyMMddHHmmss");
            var files = Directory.GetFiles(folder, $"*{timestamp}*");
            if (files.Length == 1)
            {
                if (files[0] != testFilePath)
                    File.Move(files[0], testFilePath);
            }
            else if (files.Length > 1)
            {
                MessageBox.Show("Same timestamp!");
            }
            else
            {
                MessageBox.Show("Cannot find file!");
            }
        }

        private string ReplaceRecipeString(TestRecord tr)
        {
            var a = tr.TestFilePath.IndexOf(tr.ProgramStr) + tr.ProgramStr.Length + 1;
            var b = tr.TestFilePath.IndexOf(tr.TesterStr) - 1;
            var recname = tr.TestFilePath.Substring(a, b - a);
            return tr.TestFilePath.Replace(recname, tr.RecipeStr);
        }
        #endregion
    }

    public class ErrorDescriptor
    {
        public int Index { get; set; }
        public int FrameLength { get; set; }
        public double MinDelta { get; set; }
        public double MaxDelta { get; set; }
        public double AvrDelta { get; set; }
        public double MinCurrent { get; set; }
        public double MaxCurrent { get; set; }
        public double AvrCurrent { get; set; }
        public double MinVoltage { get; set; }
        public double MaxVoltage { get; set; }
        public double AvrVoltage { get; set; }
        public double RaisedVoltage { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public double AvrTemperature { get; set; }
    }
}
