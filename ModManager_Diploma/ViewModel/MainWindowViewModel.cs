using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Policy;
using System.Windows.Media.Imaging;
using ModManager_Diploma.Model;
using System.Windows.Media;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using ModManager_Diploma.View.Pages;
using System.Xml.Linq;
using System.Reflection;
using System.Diagnostics;

namespace ModManager_Diploma.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Events
        public static event EventHandler ImageChanged;
        public static event EventHandler DeleteAssembler;
        #endregion


        private static IniFile _ini;
        private WindowState _stateWindow;
        private static Brush _backgroundWindow;
        private Uri _page;
        private Uri _mainPage;
        private static string? _pathToAssemblersFolder;
        private static double _opacityPanels;
        private static SolidColorBrush _colorPanels;
        private static ObservableCollection<GameInfo> _assemblersList;

        public static IniFile Ini
        {
            get => _ini;
            set
            {
                _ini = value;
            }
        }
        public WindowState StateWindow
        {
            get => _stateWindow;
            set
            {
                _stateWindow = value;
                OnPropertyChanged(nameof(StateWindow));
            }
        }
        public Brush BackgroundWindow
        {
            get => _backgroundWindow;
            set
            {
                _backgroundWindow = value;
                OnPropertyChanged(nameof(BackgroundWindow));
            }
        }
        public Uri Page
        {
            get => _page;
            set
            {
                _page = value;
                OnPropertyChanged(nameof(Page));
            }
        }
        public Uri MainPage
        {
            get => _mainPage;
            set
            {
                _mainPage = value;
                OnPropertyChanged(nameof(MainPage));
            }
        }
        public static string PathToAssemblersFolder
        {
            get => _pathToAssemblersFolder;
            set
            {
                _pathToAssemblersFolder = value;
            }
        }
        public double OpacityPanels
        {
            get => _opacityPanels;
            set
            {
                _opacityPanels = double.Parse(value.ToString("F2"));
                OnPropertyChanged(nameof(OpacityPanels));
            }
        }
        public SolidColorBrush ColorPanels
        {
            get => _colorPanels;
            set
            {
                _colorPanels = value;
                //ColorPanelsStatic = value;
                OnPropertyChanged(nameof(ColorPanels));
            }
        }

        public ObservableCollection<GameInfo> AssemblersList
        {
            get => _assemblersList;
            set
            {
                _assemblersList = value;
                OnPropertyChanged(nameof(AssemblersList));
            }
        }

        #region ControlsWindow
        public ICommand CloseApplication
        {
            get
            {
                return new RelayCommand(() => { foreach (System.Windows.Window w in App.Current.Windows) w.Close(); });
            }
        }
        public ICommand MinimizeWindow
        {
            get
            {
                return new RelayCommand(() => { StateWindow = System.Windows.WindowState.Minimized; });
            }
        }
        #endregion

        public ICommand OpenCreateAssemblerPage
        {
            get
            {
                return new RelayCommand(() => {
                    CreateAssemblerPageViewModel.SetBaseValues(OpacityPanels, ColorPanels, new AssemblerInfo());
                    MainPage = new Uri("pack://application:,,,/View/Pages/CreateAssemblerPage.xaml");
                });
            }
        }

        public ICommand OpenBrowser
        {
            get
            {
                return new RelayCommand(() => {
                    Process.Start(new ProcessStartInfo("https://github.com/PashaV-ip/ModManager_Diploma") { UseShellExecute = true });
                });
            }
        }

        public ICommand OpenInfo
        {
            get
            {
                return new RelayCommand(() => {
                    ReferenceViewModel.SetBaseValues(OpacityPanels, ColorPanels);
                    Page = new Uri("pack://application:,,,/View/Pages/Reference.xaml");
                });
            }
        }
        public ICommand OpenSettings
        {
            get
            {
                return new RelayCommand(() => {
                    OptionsPageViewModel.SetBaseValues(PathToAssemblersFolder, OpacityPanels, ColorPanels);
                    MainPage = new Uri("pack://application:,,,/View/Pages/OptionsPage.xaml");
                });
            }
        }
        public static void StartApp()
        {
            CreateFolderStructure();
            CreateMainSettingsINI();
        }
        private static IniFile GetIniFileSettings()
        {
            CreateMainSettingsINI();
            return new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModManager/Configs/Settings.ini"));
        }
        private static void CreateMainSettingsINI()
        {
            if(!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModManager/Configs/Settings.ini")))
            {
                IniFile ini = new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModManager/Configs/Settings.ini"));
                ini.Write("PathToTheAssemblersFolder", "", "ModManager");
                ini.Write("ColorPanels", "#FF000000", "ModManager");
                ini.Write("OpacityPanels", "0,7", "ModManager");
            }
        }
        private static void CreateFolderStructure()
        {
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModManager/Configs")) || !Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModManager/Images")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModManager/Configs"));
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModManager/Images"));
            }
        }

        private static Brush GetBackground()
        {
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModManager/Images/img.png")))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModManager/Images/img.png"), UriKind.Relative);
                image.EndInit();
                return new ImageBrush(image);
            }
            else
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        public static string GetPathToAssemblersFolder()
        {
            return Ini.Read("PathToTheAssemblersFolder", "ModManager");
        }

        public static double GetOpacityPanels()
        {
            return double.Parse(Ini.Read("OpacityPanels", "ModManager"));
        }

        public static SolidColorBrush GetColorPanels()
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(Ini.Read("ColorPanels", "ModManager")));
        }

        public static ObservableCollection<GameInfo> GetAssemblersList()
        {
            if(PathToAssemblersFolder != "")
            {
                ObservableCollection<GameInfo> gameList = new ObservableCollection<GameInfo>();
                string[] keys = Ini.GetKeys("GameList");
                foreach (var item in keys)
                {
                    if (Directory.Exists(Path.Combine(PathToAssemblersFolder, item, "Assemblers")) &&
                        Directory.GetDirectories(Path.Combine(PathToAssemblersFolder, item, "Assemblers")).Count() > 0)
                    {
                        ObservableCollection<AssemblerInfo> assemblersInGame = new ObservableCollection<AssemblerInfo>();
                        foreach (var item2 in new DirectoryInfo(Path.Combine(PathToAssemblersFolder, item, "Assemblers")).GetDirectories().OrderBy(x => x.CreationTime))
                        {
                            assemblersInGame.Add(new AssemblerInfo(item2.Name, item));
                        }
                        gameList.Add(new GameInfo(assemblersInGame, item, Path.Combine(PathToAssemblersFolder, item)));
                    }
                }
                return gameList;
            }
            return null;
        }

        #region PagesControlsMethods
        private void CloseMainPage()
        {
            OpacityPanels = GetOpacityPanels();
            ColorPanels = GetColorPanels();
            BackgroundWindow = GetBackground();
            MainPage = null;
        }
        #endregion

        #region MainSettings
        private void SaveMainOptions(string pathAssemblers, double opacityPanels, SolidColorBrush colorPanels, string? pathToBackground = null)
        {           
            PathToAssemblersFolder = pathAssemblers;
            OpacityPanels = opacityPanels;
            ColorPanels = colorPanels;
            
            //CreateAssemblerPageViewModel.ColorPanels = ColorPanels;
            if (!Directory.Exists(PathToAssemblersFolder)) PathToAssemblersFolder = "";
            if (pathToBackground != null)
            {
                ReplaceBackground(pathToBackground);
            }
            Ini.Write("PathToTheAssemblersFolder", PathToAssemblersFolder, "ModManager");
            Ini.Write("ColorPanels", ColorPanels.Color.ToString(), "ModManager");
            Ini.Write("OpacityPanels", OpacityPanels.ToString(), "ModManager");
            ReferenceViewModel.SetBaseValues(OpacityPanels, ColorPanels);

            MainPage = null;
        }

        private void ReplaceBackground(string pathToBackground)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModManager/Images/img.png");
            File.Delete(path);
            File.Copy(pathToBackground, path);
        }

        private void SetNewBackground(Brush brush)
        {
            BackgroundWindow = brush;
        }
        #endregion



        #region AssemblerSettings
        public void OpenAssemblerInfo(AssemblerInfo assembler)
        {
            AssemblerInfoPageViewModel.SetBaseValues(OpacityPanels, ColorPanels, assembler);
            Page = new Uri("pack://application:,,,/View/Pages/AssemblerInfoPage.xaml");
        }
        public void OpenAssemblerSettings(AssemblerInfo assembler)
        {
            MainPage = new Uri("pack://application:,,,/View/Pages/CreateAssemblerPage.xaml");
            CreateAssemblerPageViewModel.SetBaseValues(OpacityPanels, ColorPanels, assembler);
        }
        public void SaveAssemblerOptions()
        {
            AssemblersList = GetAssemblersList();
            MainPage = null;
        }
        public static void ChangeAssembler(AssemblerInfo assembler)
        {
            foreach(var item in _assemblersList.First(x => x.NameGame == assembler.GameName).Assemblers)
            {
                item.GetAssemblerIsLoaded();
            }
        }

        public static void ReplaceAssembler(AssemblerInfo assembler)
        {
            var item = _assemblersList.First(x => x.NameGame == assembler.GameName);
            item.Assemblers[item.Assemblers.IndexOf(assembler)] = assembler;
        }
        #endregion

        #region AssemblerInfoPage

        private void HandleAssemblerListChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(AssemblersList));
        }
        private void HandleDeleteAssemblerChanged(object sender, EventArgs e)
        {
            ReferenceViewModel.SetBaseValues(OpacityPanels, ColorPanels);
            Page = new Uri("pack://application:,,,/View/Pages/Reference.xaml");
        }

        public static void ChangeImageInAssembler(string gameName, string assemblerName)
        {
            var item = _assemblersList.First(x => x.NameGame == gameName);
            AssemblerInfo assembler = item.Assemblers.First(x => x.Name == assemblerName);
            assembler.PathToImg = assembler.GetNewImage(Path.Combine(PathToAssemblersFolder, assembler.GameName, "Assemblers", assembler.Name, "img.png"));
            item.Assemblers[item.Assemblers.IndexOf(assembler)] = new AssemblerInfo(assembler);
        }
        public static void DeleteAssemblerFromList(string gameName, string assemblerName)
        {
            var item = _assemblersList.First(x => x.NameGame == gameName);
            item.Assemblers.Remove(item.Assemblers.First(x => x.Name == assemblerName));
            DeleteAssembler.Invoke(null, EventArgs.Empty);
        }
        public static void DeleteGameFromList(string gameName)
        {
            _assemblersList.Remove(_assemblersList.First(x => x.NameGame == gameName));
            DeleteAssembler.Invoke(null, EventArgs.Empty);
        }

        #endregion


        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            Directory.CreateDirectory(destDirName);

            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
        }
        public static void ClearDirectory(string sourceDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                subdir.Delete(true);
            }
        }


        public MainWindowViewModel()
        {
            StateWindow = WindowState.Normal;
            StartApp();
            Ini = GetIniFileSettings();
            BackgroundWindow = GetBackground();
            OpacityPanels = GetOpacityPanels();
            ColorPanels = GetColorPanels();
            ReferenceViewModel.SetBaseValues(OpacityPanels, ColorPanels);
            Page = new Uri("pack://application:,,,/View/Pages/Reference.xaml");
            PathToAssemblersFolder = GetPathToAssemblersFolder();

            AssemblersList = GetAssemblersList();
            OptionsPageViewModel.CloseOptions += CloseMainPage;
            OptionsPageViewModel.SaveOptions += SaveMainOptions;
            OptionsPageViewModel.SetNewBackground += SetNewBackground;
            CreateAssemblerPageViewModel.CloseAssemblerOptions += SaveAssemblerOptions;
            AssemblerInfoPageViewModel.OpenSettings += OpenAssemblerSettings;
            AssemblerInfo.OpenAssembler += OpenAssemblerInfo;
            if(ImageChanged == null)
            ImageChanged += HandleAssemblerListChanged;
            if(DeleteAssembler == null)
            DeleteAssembler += HandleDeleteAssemblerChanged;
        }
    }
}
