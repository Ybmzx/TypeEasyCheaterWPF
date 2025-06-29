using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TypeEasyCheaterWPF.Services;
using WindowsInput;

namespace TypeEasyCheaterWPF.ViewModels
{
    public partial class MainWindowVM : ObservableObject
    {
        private IFileDialogService fileDialogService;
        private IMessageBoxService messageBoxService;
        private ITypeEasyCoursesService typeEasyCoursesService;
        private ISettingService settingService;
        private ITypeEasyProgramMemoryModifyService typeEasyProgramMemoryModifyService;
        private ITypeEasyRecordModifyService typeEasyRecordModifyService;
        private ISelectMidwayPositionWindowService selectMidwayService;
        public MainWindowVM(IFileDialogService fileDialogService,
                            IMessageBoxService messageBoxService,
                            ITypeEasyCoursesService typeEasyCoursesService,
                            ISettingService settingService,
                            ITypeEasyProgramMemoryModifyService typeEasyProgramMemoryModifyService,
                            ITypeEasyRecordModifyService typeEasyRecordModifyService,
                            ISelectMidwayPositionWindowService selectMidwayService)
        {
            this.fileDialogService = fileDialogService;
            this.messageBoxService = messageBoxService;
            this.typeEasyCoursesService = typeEasyCoursesService;
            this.settingService = settingService;
            this.typeEasyProgramMemoryModifyService = typeEasyProgramMemoryModifyService;
            this.typeEasyRecordModifyService = typeEasyRecordModifyService;
            this.selectMidwayService = selectMidwayService;

            PathsAndCourseNamesView = CollectionViewSource.GetDefaultView(_pathsAndCourseNames);
            PathsAndCourseNamesView.Filter = (item) =>
            {
                if (item is null || string.IsNullOrEmpty(SearchText)) return true;
                return item is KeyValuePair<string, string> pair && pair.Value.ToLower().Contains(SearchText.ToLower());
            };

            TypeEasyPath = settingService.Setting.TypeEasyExePath;
            LoadTypeEasyExePath(TypeEasyPath);

            InputDelay = settingService.Setting.InputDelay;
        }

        [ObservableProperty]
        string _typeEasyPath = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StartCheatingCommand))]
        [NotifyCanExecuteChangedFor(nameof(StartCheatingFromHalfwayCommand))]
        bool _typeEasyPathInvalid = true;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StartCheatingCommand))]
        [NotifyCanExecuteChangedFor(nameof(StartCheatingFromHalfwayCommand))]
        bool _isCheating = false;

        List<KeyValuePair<string, string>> _pathsAndCourseNames = new();

        [ObservableProperty]
        ICollectionView _pathsAndCourseNamesView;

        [ObservableProperty]
        string _searchText = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StartCheatingCommand))]
        [NotifyCanExecuteChangedFor(nameof(StartCheatingFromHalfwayCommand))]
        KeyValuePair<string, string> _selectedPathAndCourseName;

        partial void OnSearchTextChanged(string value) => PathsAndCourseNamesView.Refresh();

        partial void OnSelectedPathAndCourseNameChanged(KeyValuePair<string, string> value)
        {
            CoursePathInvalid = !File.Exists(value.Key);
            AutoCalc(WantedSpeedAutoCalc);
        }

        [ObservableProperty]
        bool _coursePathInvalid = true;

        private bool CanStartCheating() => !TypeEasyPathInvalid && File.Exists(SelectedPathAndCourseName.Key) && !IsCheating;

        [ObservableProperty]
        double _inputDelay = 0.01;

        [ObservableProperty]
        double _progressValue = 100;

        private CancellationTokenSource? _cheatingCts;

        [ObservableProperty]
        string _originalTimeStr = "00:00:00";


        [ObservableProperty]
        string _originalSpeedStr = "0字/分";

        [ObservableProperty]
        string _wantedTimeStr = "00:00:00";

        [ObservableProperty]
        string _wantedSpeedStr = "0字/分";

        [ObservableProperty]
        int _wantedSpeedAutoCalc = 0;

        partial void OnWantedSpeedAutoCalcChanged(int value) => AutoCalc(value);

        private void AutoCalc(int value)
        {
            if (CoursePathInvalid) return;
            if (value == 0)
            {
                WantedSpeedStr = "0字/分";
                WantedTimeStr = "∞";
                return;
            }
            WantedSpeedStr = $"{value}字/分";
            WantedTimeStr = new TimeSpan(0, 0, (int)(typeEasyCoursesService.GetContent(SelectedPathAndCourseName.Key).Length / (double)value * 60)).ToString();
        }

        partial void OnInputDelayChanged(double value)
        {
            settingService.Setting.InputDelay = value;
            settingService.Save();
        }

        [RelayCommand(CanExecute = nameof(CanStartCheating))]
        private async Task StartCheating()
        {
            var content = typeEasyCoursesService.GetContent(SelectedPathAndCourseName.Key);

            var speedPerSencond = InputDelay == 0 ? content.Length : 1d / InputDelay;

            OriginalSpeedStr = $"{speedPerSencond * 60}字/分";
            OriginalTimeStr = new TimeSpan(0, 0, (int)(content.Length / speedPerSencond)).ToString();
            await Cheating(content);
        }

        [RelayCommand(CanExecute = nameof(CanStartCheating))]
        private async Task StartCheatingFromHalfway()
        {
            var content = typeEasyCoursesService.GetContent(SelectedPathAndCourseName.Key);

            int? result = selectMidwayService.ShowDialog(content);

            if (result is null)
            {
                return;
            }

            content = content.Substring((int)result);

            var speedPerSencond = InputDelay == 0 ? content.Length : 1d / InputDelay;

            OriginalSpeedStr = $"unknown字/分";
            OriginalTimeStr = new TimeSpan(0, 0, 0).ToString();
            await Cheating(content);
        }

        private async Task Cheating(string content)
        {
            if (IsCheating) return;

            StopCheating();

            IsCheating = true;

            _cheatingCts = new();

            IProgress<double> progress = new Progress<double>(value =>
            {
                ProgressValue = value;
            });

            progress.Report(0);

            try
            {
                InputSimulator simulator = new();

                await Task.Delay(3000, _cheatingCts.Token);
                for (int i = 0; i < content.Length; i++)
                {
                    char c = content[i];
                    simulator.Keyboard.TextEntry(c);
                    progress.Report(i / (double)content.Length * 100.0);
                    await Task.Delay((int)(InputDelay * 1000), _cheatingCts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
            finally
            {
                _cheatingCts.Dispose();
                _cheatingCts = null;
                IsCheating = false;
                progress.Report(100);
            }
        }

        [RelayCommand]
        void StopCheating()
        {
            _cheatingCts?.Cancel();
        }

        [RelayCommand]
        async Task ModifyProgramRecordAsync()
        {
            if (TypeEasyPathInvalid)
            {
                messageBoxService.Show("请选择TypeEasy.exe", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CoursePathInvalid)
            {
                messageBoxService.Show("请选择课程", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (OriginalSpeedStr.Length < WantedSpeedStr.Length ||
                OriginalTimeStr.Length < WantedTimeStr.Length)
            {
                messageBoxService.Show("想要修改的文字不得长于原文字", "错误", messageBoxImage: MessageBoxImage.Error);
                return;
            }

            await typeEasyProgramMemoryModifyService.ModifyAsync(Path.GetFileName(TypeEasyPath),
                                                                 OriginalSpeedStr,
                                                                 OriginalTimeStr,
                                                                 WantedSpeedStr,
                                                                 WantedTimeStr);
            messageBoxService.Show("修改成功, 大概...", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [ObservableProperty]
        int _detectedSpeed = 0;

        partial void OnWantedSpeedStrChanged(string value) => DetectedSpeed = ExtractFirstInteger(value) ?? 0;

        [RelayCommand]
        void ModifyLastRecord()
        {
            typeEasyRecordModifyService.ModifyLastRecord(DetectedSpeed);
            messageBoxService.Show("修改成功, 大概...", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        int? ExtractFirstInteger(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            // 使用正则表达式匹配字符串中的第一串正整数
            var match = Regex.Match(input, @"\d+");
            if (match.Success)
            {
                // 如果匹配成功，尝试将匹配到的部分转换为整数
                if (int.TryParse(match.Value, out int number))
                {
                    return number;
                }
            }

            // 如果没有匹配到数字或转换失败，返回 0
            return null;
        }

        [RelayCommand]
        void OpenTypeEasyExeFileDialog()
        {
            var path = fileDialogService.OpenSingleFileDialog("可执行文件 (*.exe)|*.exe") ?? TypeEasyPath;
            if (path is null) return;
            TypeEasyPath = path;

            settingService.Setting.TypeEasyExePath = TypeEasyPath;
            settingService.Save();

            LoadTypeEasyExePath(TypeEasyPath);
            
        }

        [RelayCommand]
        void ShowAboutInfo()
        {
            messageBoxService.Show($"TypeEasyCheaterWPF\n" +
                                    "Github@Ybmzx - https://github.com/Ybmzx\n" +
                                   $"编译时间: {BuildInfo.CompileTime}\n" +
                                    "本软件仅供学习交流使用, 请勿用于非法用途,\n" +
                                    "如作它用所承受的法律责任一概与作者无关.", "关于", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        void LoadTypeEasyExePath(string typeEasyPath)
        {
            if (!File.Exists(typeEasyPath)) return;

            if (!typeEasyCoursesService.Load(typeEasyPath))
            {
                messageBoxService.Show("没有找到课程文件, 可能会出现错误.", "警告", messageBoxImage: System.Windows.MessageBoxImage.Warning);
            }

            _pathsAndCourseNames = typeEasyCoursesService.GetPathsAndCourseNames().ToList();
            PathsAndCourseNamesView = CollectionViewSource.GetDefaultView(_pathsAndCourseNames);
            PathsAndCourseNamesView.Refresh();
            PathsAndCourseNamesView.Filter = (item) =>
            {
                if (item is null || string.IsNullOrEmpty(SearchText)) return true;
                return item is KeyValuePair<string, string> pair && pair.Value.ToLower().Contains(SearchText.ToLower());
            };
        }

        partial void OnTypeEasyPathChanged(string value) => TypeEasyPathInvalid = string.IsNullOrWhiteSpace(value) || !File.Exists(value);

    }
}
