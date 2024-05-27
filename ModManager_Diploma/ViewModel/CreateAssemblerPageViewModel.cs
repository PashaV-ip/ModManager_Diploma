using GalaSoft.MvvmLight.Command;
using ModManager_Diploma.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace ModManager_Diploma.ViewModel
{
    public class CreateAssemblerPageViewModel : BaseViewModel
    {
        public static event EventHandler LoadedOldAssembler;

        private static AssemblerInfo _assembler;
        private static string _pathToAssemblersFolder;
        private static double _opacityPanels;
        private static SolidColorBrush? _colorPanels;
        //private string _assemblerName;
        //private string _selectGame;
        private ObservableCollection<string> _gameList;
        //private ObservableCollection<ModInfo> _modList;


        public AssemblerInfo Assembler
        {
            get => _assembler;
            set
            {
                _assembler = value;
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
                OnPropertyChanged(nameof(ColorPanels));
            }
        }
        public string AssemblerName
        {
            get => Assembler.Name;
            set
            {
                Assembler.Name = value;
                OnPropertyChanged(nameof(AssemblerName));
            }
            /*get => _assemblerName;
            set
            {
                _assemblerName = value;
                OnPropertyChanged(nameof(AssemblerName));
            }*/
        }
        public string SelectGame
        {
            get => Assembler.GameName;
            set
            {
                Assembler.GameName = value;
                OnPropertyChanged(nameof(SelectGame));
                if(!string.IsNullOrEmpty(value))
                GetModsForGame(value);
            }
            /*get => _selectGame;
            set
            {
                _selectGame = value;
                OnPropertyChanged(nameof(SelectGame));
                GetModsForGame(value);
            }*/
        }
        public ObservableCollection<string> GameList
        {
            get => _gameList;
            set
            {
                _gameList = value;
                OnPropertyChanged(nameof(GameList));
            }
        }
        public ObservableCollection<ModInfo> ModList
        {
            get => Assembler.ModList;
            set
            {
                Assembler.ModList = value;
                OnPropertyChanged(nameof(ModList));
            }
            /*get => _modList;
            set
            {
                _modList = value;
                OnPropertyChanged(nameof(ModList));
            }*/
        }

        public static ClosePage CloseAssemblerOptions;


        public ICommand SaveSettings
        {
            get
            {
                return new RelayCommand(() => {
                    if(!string.IsNullOrEmpty(AssemblerName) && !string.IsNullOrEmpty(SelectGame))
                    {
                        string pathToThisAssembler = Path.Combine(PathToAssemblersFolder, SelectGame, "Assemblers", AssemblerName);
                        if (!Directory.Exists(pathToThisAssembler))
                        {
                            Directory.CreateDirectory(pathToThisAssembler);
                        }
                        string pathToIni = Path.Combine(pathToThisAssembler, "Config.ini");
                        IniFile ini = new IniFile(pathToIni);
                        if (Assembler.IsCheckedSaveWorlds)
                        {
                            if(Directory.Exists(Assembler.PathToAssemblerWorlds))
                            {
                                new IniFile(Path.Combine(MainWindowViewModel.PathToAssemblersFolder, Assembler.GameName, "Settings.ini")).Write("WoldsPath", Assembler.PathToAssemblerWorlds, "Config");
                                string path = Path.Combine(pathToThisAssembler, "Worlds");
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                foreach(var item in Directory.GetDirectories(Assembler.PathToAssemblerWorlds))
                                {
                                    MainWindowViewModel.CopyDirectory(item, Path.Combine(path, new DirectoryInfo(item).Name));
                                }
                            }  
                            else Assembler.IsCheckedSaveWorlds = false;
                        }
                        if (Assembler.IsCheckedSaveConfigs)
                        {
                            if (Directory.Exists(Assembler.PathToAssemblerConfigs))
                            {
                                new IniFile(Path.Combine(MainWindowViewModel.PathToAssemblersFolder, Assembler.GameName, "Settings.ini")).Write("ConfigsPath", Assembler.PathToAssemblerConfigs, "Config");
                                string path = Path.Combine(pathToThisAssembler, "Configs");
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                foreach (var item in Directory.GetFiles(Assembler.PathToAssemblerConfigs))
                                {
                                    File.Copy(item, Path.Combine(path, new DirectoryInfo(item).Name), false);
                                }
                            }
                            else Assembler.IsCheckedSaveConfigs = false;
                        }
                        ini.Write("SaveWorlds", Assembler.IsCheckedSaveWorlds.ToString(), "Settings");
                        ini.Write("SaveConfigs", Assembler.IsCheckedSaveConfigs.ToString(), "Settings");
                        foreach (var item in ModList)
                        {
                            if (!File.Exists(Path.Combine(PathToAssemblersFolder, SelectGame, "mods", item.Name)))
                            {
                                File.Copy(Path.Combine(MainWindowViewModel.Ini.Read(SelectGame, "GameList"), item.Name),
                                    Path.Combine(PathToAssemblersFolder, SelectGame, "mods", item.Name));
                            }
                            if (item.IsChecked && !ini.KeyExists(item.Name))
                            {
                                ini.Write(item.Name, true.ToString(), "Mods");
                            }
                            else if(!item.IsChecked && ini.KeyExists(item.Name, "Mods"))
                            {
                                ini.DeleteKey(item.Name, "Mods");
                            }
                        }
                        if (!Assembler.IsNewAssembler)
                        {
                            Assembler = new AssemblerInfo(AssemblerName, SelectGame);
                            //GetModListInThisAssembler(false);
                            AssemblerInfoPageViewModel.SetBaseValues(Assembler);
                        }
                        CloseAssemblerOptions();

                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Поля:\n" +
                            "\"Название сборки\" и \"Игра\" не могут быть пустыми!", "Ошибка..", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                });
            }
        }

        public ICommand CancleSettings
        {
            get
            {
                return new RelayCommand(() => {
                    CloseAssemblerOptions();
                });
            }
        }

        public ICommand TestCommand
        {
            get
            {
                return new RelayCommand(() => {
                    ////SelectGame = "Minecraft";
                    
                    //OnPropertyChanged(nameof(SelectGame));
                    //OnPropertyChanged(nameof(AssemblerName));
                    ////SelectGame = Assembler.GameName;
                    ////AssemblerName = Assembler.Name;
                    //MessageBox.Show(SelectGame + " " + AssemblerName + " GameListCout: " + GameList.Count + "\n" +
                    //    Assembler.GameName + " " + Assembler.Name + " GameListCout: " + GameList.Count);
                });
            }
        }
        public ICommand BrowseConfigsFolder
        {
            get
            {
                return new RelayCommand(() => {
                    var dialog = new FolderBrowserDialog();
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        Assembler.PathToAssemblerConfigs = dialog.SelectedPath;
                    }
                });
            }
        }
        public ICommand BrowseWorldsFolder
        {
            get
            {
                return new RelayCommand(() => {
                    var dialog = new FolderBrowserDialog();
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        Assembler.PathToAssemblerWorlds = dialog.SelectedPath;
                    }
                });
            }
        }



        public void GetModsForGame(string gameName)
        {
            string pathToModsHistory = Path.Combine(PathToAssemblersFolder, gameName, "mods");
            if (Assembler.IsNewAssembler)
            {
                if (gameName != "" && gameName != null)
                {
                    ModList = new ObservableCollection<ModInfo>();
                    
                    if (!Directory.Exists(pathToModsHistory))
                    {
                        Directory.CreateDirectory(pathToModsHistory);
                    }
                    GetModListInFolder(MainWindowViewModel.Ini.Read(gameName, "GameList"), true);
                    GetModListInFolder(pathToModsHistory, false);
                }
            }
            else
            {
                GetModListInThisAssembler(true);
                GetModListInFolder(pathToModsHistory, false);
            }
            ModList = new ObservableCollection<ModInfo>(ModList.OrderBy(x => x.Name));
            
        }
        public void GetModListInFolder(string path, bool checkedMods)
        {
            //ObservableCollection<ModInfo> modList = new ObservableCollection<ModInfo>();
            foreach (var item in Directory.GetFiles(path))
            {
                if (item != "")
                {
                    ModInfo newMod = new ModInfo(Path.GetFileName(item.ToString()), checkedMods, ModList.Count + 1);
                    if (!ModList.Any(s => s.Name == Path.GetFileName(item)))
                    {
                        ModList.Add(newMod);
                    }
                }
            }
            //return modList;
        }
        public void GetModListInThisAssembler(bool isForSettings)
        {
            ModList = new ObservableCollection<ModInfo>();
            if(Assembler.IniSettings != null)
            {
                foreach (var item in Assembler.IniSettings.GetKeys("Mods"))
                {
                    if (item != "")
                    {
                        ModInfo newMod;
                        if (isForSettings)
                            newMod = new ModInfo(item, true, ModList.Count + 1);
                        else newMod = new ModInfo(item, bool.Parse(Assembler.IniSettings.Read(item, "Mods")), ModList.Count + 1);
                        if (!ModList.Any(s => s.Name == Path.GetFileName(item)))
                        {
                            ModList.Add(newMod);
                        }
                    }
                }
            } 
        }
        public void SetAssembler(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(Assembler.Name) && !string.IsNullOrEmpty(Assembler.GameName))
            GetModsForGame(_assembler.GameName);
            //SelectGame = Assembler.GameName;
            //AssemblerName = Assembler.Name;
            //MessageBox.Show(SelectGame + "\n" + AssemblerName + "\nGameListCout: " + GameList.Count);
            //OnPropertyChanged(nameof(SelectGame));
            //OnPropertyChanged(nameof(AssemblerName));
            //GetModsForGame(SelectGame);
            //GetModListInThisAssembler();
        }
        public static void SetBaseValues(double opacityPanels, SolidColorBrush colorPanels, AssemblerInfo assembler)
        {
            _assembler = assembler;
            _opacityPanels = opacityPanels;
            _colorPanels = colorPanels;
            if(assembler.Name != "" && assembler.Name != null)
            {
                LoadedOldAssembler?.Invoke(null, EventArgs.Empty);
            }
        }

        public CreateAssemblerPageViewModel()
        {
            PathToAssemblersFolder = MainWindowViewModel.PathToAssemblersFolder;
            GameList = new ObservableCollection<string>(MainWindowViewModel.Ini.GetKeys("GameList"));
            //ModList = new ObservableCollection<ModInfo>();
            if(LoadedOldAssembler == null)
            LoadedOldAssembler += SetAssembler;
            LoadedOldAssembler?.Invoke(null, EventArgs.Empty);
            //SelectGame = GameList.First();
        }
    }
}
