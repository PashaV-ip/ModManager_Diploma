using GalaSoft.MvvmLight.Command;
using ModManager_Diploma.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace ModManager_Diploma.Model
{
    public delegate void OpenAssembler(AssemblerInfo assembler);
    public class AssemblerInfo : BaseViewModel
    {
        private IniFile _iniSettings;
        private bool _isLoadedAssembler;
        private bool _isNewAssembler;
        private string _name;
        private string _gameName;
        private bool _isCheckedSaveConfigs;
        private bool _isCheckedSaveWorlds;
        private string _pathToAssemblerConfigs;
        private string _pathToAssemblerWorlds;
        private ObservableCollection<ModInfo> _modList;
        private BitmapImage _pathToImg;
        public IniFile IniSettings
        {
            get => _iniSettings;
            set
            {
                _iniSettings = value;
            }
        }
        public bool IsLoadedAssembler
        {
            get => _isLoadedAssembler;
            set
            {
                _isLoadedAssembler = value;
            }
        }
        public bool IsNewAssembler
        {
            get => _isNewAssembler;
            set
            {
                _isNewAssembler = value;
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string GameName
        {
            get => _gameName;
            set
            {
                _gameName = value;
            }
        }
        public bool IsCheckedSaveConfigs
        {
            get => _isCheckedSaveConfigs;
            set
            {
                _isCheckedSaveConfigs = value;
                OnPropertyChanged(nameof(IsCheckedSaveConfigs));
                if (value)
                {
                    string path = new IniFile(Path.Combine(MainWindowViewModel.PathToAssemblersFolder, GameName, "Settings.ini")).Read("ConfigsPath", "Config");
                    if (!string.IsNullOrEmpty(path)) PathToAssemblerConfigs = path;
                }
                else PathToAssemblerConfigs = "";
                OnPropertyChanged(nameof(PathToConfigsVisible));
            }
        }
        public bool IsCheckedSaveWorlds
        {
            get => _isCheckedSaveWorlds;
            set
            {
                _isCheckedSaveWorlds = value;
                OnPropertyChanged(nameof(IsCheckedSaveWorlds));
                if (value)
                {
                    string path = new IniFile(Path.Combine(MainWindowViewModel.PathToAssemblersFolder, GameName, "Settings.ini")).Read("WoldsPath", "Config");
                    if (!string.IsNullOrEmpty(path)) PathToAssemblerWorlds = path;
                }
                else PathToAssemblerWorlds = "";
                OnPropertyChanged(nameof(PathToWorldsVisible));
            }
        }
        public string PathToAssemblerConfigs
        {
            get
            {
                if (IsCheckedSaveConfigs)
                {
                    return _pathToAssemblerConfigs;
                }
                return null;
            }
            set
            {
                _pathToAssemblerConfigs = value;
                OnPropertyChanged(nameof(PathToAssemblerConfigs));
            }
        }
        public string PathToAssemblerWorlds
        {
            get
            {
                if (IsCheckedSaveWorlds)
                {
                    return _pathToAssemblerWorlds; 
                }
                return null;
            }
            set
            {
                _pathToAssemblerWorlds = value;
                OnPropertyChanged(nameof(PathToAssemblerWorlds));
            }
        }
        public Visibility PathToWorldsVisible
        {
            get
            {
                if (IsCheckedSaveWorlds)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        public Visibility PathToConfigsVisible
        {
            get
            {
                if (IsCheckedSaveConfigs)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        public ObservableCollection<ModInfo> ModList
        {
            get => _modList;
            set 
            { 
                _modList = value;
            }
        }
        public BitmapImage PathToImg
        {
            get => _pathToImg;
            set
            {
                _pathToImg = value;
            }
        }
        public int CoutOnMods
        {
            get
            {
                return ModList.Count(x => x.IsChecked);
            }
        }

        public static OpenAssembler OpenAssembler;

        public ICommand OpenAssemblerInfoPage
        {
            get
            {
                return new RelayCommand(() => {
                    if(OpenAssembler != null)
                    {
                        
                        OpenAssembler(this);
                    }
                });
            }
        }

        public void OnOffModInIniFile(string modName, bool value)
        {
            IniSettings.Write(modName, value.ToString(), "Mods");
            OnPropertyChanged(nameof(CoutOnMods));
        }
        public BitmapImage GetNewImage(string path)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(path, UriKind.Absolute);
            image.EndInit();
            return image;
        }
        public void GetAssemblerIsLoaded()
        {
            string pathToGameIni = Path.Combine(MainWindowViewModel.PathToAssemblersFolder, GameName, "Settings.ini");
            if (File.Exists(pathToGameIni))
            {
                IniFile ini = new IniFile(pathToGameIni);
                string nameLoadedAssemblers = ini.Read("LoadedAssembler", "Config");
                if (nameLoadedAssemblers != null && nameLoadedAssemblers == Name) IsLoadedAssembler = true;
                else IsLoadedAssembler = false;

            }
        }


        public AssemblerInfo()
        {
            IsNewAssembler = true;
        }
        public AssemblerInfo(string name, string gameName)
        {
            IsNewAssembler = false;
            Name = name;
            GameName = gameName;
            ModList = new ObservableCollection<ModInfo>();

            if (File.Exists(Path.Combine(MainWindowViewModel.PathToAssemblersFolder, GameName, "Assemblers", Name, "img.png")))
            {
                PathToImg = GetNewImage(Path.Combine(MainWindowViewModel.PathToAssemblersFolder, GameName, "Assemblers", Name, "img.png"));
            }


            else
            {
                PathToImg = GetNewImage("pack://application:,,,/Source/Images/img.png");
            }

            if(File.Exists(Path.Combine(MainWindowViewModel.PathToAssemblersFolder, GameName, "Assemblers", Name, "Config.ini")))
            {
                IniSettings = new IniFile(Path.Combine(MainWindowViewModel.PathToAssemblersFolder, GameName, "Assemblers", Name, "Config.ini"));
                //string testStr = "";
                foreach (var item in IniSettings.GetKeys("Mods"))
                {
                    if(item != "")
                    ModList.Add(new ModInfo(item, bool.Parse((IniSettings.Read(item, "Mods")).ToLower()), ModList.Count+1, OnOffModInIniFile));
                }
                string configsChecked = IniSettings.Read("SaveConfigs", "Settings").ToString();
                string worldsChecked = IniSettings.Read("SaveWorlds", "Settings").ToString();
                IsCheckedSaveConfigs = bool.Parse(string.IsNullOrEmpty(configsChecked) ? false.ToString() : configsChecked);
                IsCheckedSaveWorlds = bool.Parse(string.IsNullOrEmpty(worldsChecked) ? false.ToString() : worldsChecked);
            }
            GetAssemblerIsLoaded();
            /*string pathToGameIni = Path.Combine(MainWindowViewModel.PathToAssemblersFolder, GameName, "Settings.ini");
            if (File.Exists(pathToGameIni))
            {
                IniFile ini = new IniFile(pathToGameIni);
                string nameLoadedAssemblers = ini.Read("LoadedAssembler", "Config");
                if (nameLoadedAssemblers != null && nameLoadedAssemblers == Name) IsLoadedAssembler = true;
                else IsLoadedAssembler = false;

            }*/
        }
        public AssemblerInfo(AssemblerInfo assemblerInfo)
        {
            IniSettings = assemblerInfo.IniSettings;
            IsNewAssembler = assemblerInfo.IsNewAssembler;
            Name = assemblerInfo.Name;
            GameName = assemblerInfo.GameName;
            ModList = assemblerInfo.ModList;
            PathToImg = assemblerInfo.PathToImg;
            IsLoadedAssembler = assemblerInfo.IsLoadedAssembler;
            IsCheckedSaveConfigs = assemblerInfo.IsCheckedSaveConfigs;
            IsCheckedSaveWorlds = assemblerInfo.IsCheckedSaveWorlds;
        }
    }
}
