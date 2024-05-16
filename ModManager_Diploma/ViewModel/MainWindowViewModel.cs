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

namespace ModManager_Diploma.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Delegates
        
        #endregion


        private static IniFile _ini;
        private WindowState _stateWindow;
        private static Brush _backgroundWindow;
        private Uri _page;
        private Uri _mainPage;
        private static string? _pathToAssemblersFolder;
        private static double _opacityPanels;
        private static SolidColorBrush _colorPanels;

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
        public string PathToAssemblersFolder
        {
            get => _pathToAssemblersFolder;
            set
            {
                _pathToAssemblersFolder = value;
                OnPropertyChanged(nameof(PathToAssemblersFolder));
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

        public static SolidColorBrush ColorPanelsStatic { 
            get => _colorPanels;
            set
            {
                _colorPanels = value;
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
                    MainPage = new Uri("pack://application:,,,/View/Pages/CreateAssemblerPage.xaml");
                });
            }
        }

        public ICommand OpenInfo
        {
            get
            {
                return new RelayCommand(() => {
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
            
            CreateAssemblerPageViewModel.SetOpacityPanels(OpacityPanels);
            CreateAssemblerPageViewModel.SetColorPanels(ColorPanels);
            //CreateAssemblerPageViewModel.ColorPanels = ColorPanels;
            if (!Directory.Exists(PathToAssemblersFolder)) PathToAssemblersFolder = "";
            if (pathToBackground != null)
            {
                ReplaceBackground(pathToBackground);
            }
            Ini.Write("PathToTheAssemblersFolder", PathToAssemblersFolder, "ModManager");
            Ini.Write("ColorPanels", ColorPanels.Color.ToString(), "ModManager");
            Ini.Write("OpacityPanels", OpacityPanels.ToString(), "ModManager");

            MainPage = null;
        }

        private void ReplaceBackground(string pathToBackground)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModManager/Images/img.png");
            //MessageBox.Show(path + "\n" + pathToBackground);
            File.Delete(path);
            File.Copy(pathToBackground, path);
        }

        private void SetPathToAssemblers(string path)
        {
            PathToAssemblersFolder = path;
        }

        private void SetNewBackground(Brush brush)
        {
            BackgroundWindow = brush;
        }
        #endregion


        public MainWindowViewModel()
        {
            StateWindow = WindowState.Normal;
            StartApp();
            Ini = GetIniFileSettings();
            BackgroundWindow = GetBackground();
            Page = new Uri("pack://application:,,,/View/Pages/AboutTheProgram.xaml");
            PathToAssemblersFolder = GetPathToAssemblersFolder();
            OpacityPanels = GetOpacityPanels();
            ColorPanels = GetColorPanels();
            OptionsPageViewModel.CloseOptions += CloseMainPage;
            OptionsPageViewModel.SaveOptions += SaveMainOptions;
            OptionsPageViewModel.SetNewBackground += SetNewBackground;
            CreateAssemblerPageViewModel.CloseAssemblerOptions += CloseMainPage;
        }
    }
}
