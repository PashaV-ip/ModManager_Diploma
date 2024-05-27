using ModManager_Diploma.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace ModManager_Diploma.ViewModel
{
    public class AssemblerInfoPageViewModel : BaseViewModel
    {
        public static event EventHandler AssemblerChanged;
        public delegate void OpenSettingsAssembler(AssemblerInfo assembler);

        private static AssemblerInfo _assembler;
        private static double _opacityPanels;
        private static SolidColorBrush? _colorPanels;
        //private static string _name;
        private double _modsWeight = 0;

        public AssemblerInfo Assembler
        {
            get => _assembler;
            set
            {
                _assembler = value;
                OnPropertyChanged(nameof(Assembler));
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
        public string Name
        {
            get => Assembler.Name;
            set
            {
                Assembler.Name = value;
                OnPropertyChanged(nameof(Name));
            }
            /*get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }*/
        }
        public double ModsWeight
        {
            get => _modsWeight;
            set
            {
                _modsWeight = value;
                OnPropertyChanged(nameof(ModsWeight));
            }
        }
        public string ButtonLoadUnloadText
        {
            get
            {
                if (Assembler.IsLoadedAssembler) return "Выгрузить";
                else return "Загрузить";
            }
        }
        public static OpenSettingsAssembler OpenSettings;



        public ICommand EditImageAssembler
        {
            get
            {
                return new RelayCommand(() => {
                    var dialog = new OpenFileDialog();
                    dialog.Filter = "PNG (*.png)|*.png";
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        string path = Path.Combine(MainWindowViewModel.PathToAssemblersFolder, Assembler.GameName, "Assemblers", Name, "img.png");
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        File.Copy(dialog.FileName, path);
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        image.UriSource = new Uri(path, UriKind.Absolute);
                        image.EndInit();
                        Assembler.PathToImg = image;
                        MainWindowViewModel.ChangeImageInAssembler(Assembler.GameName, Assembler.Name);
                        AssemblerChanged?.Invoke(null, EventArgs.Empty);
                    }

                    //MessageBox.Show("EditImage");
                });
            }
        }

        public ICommand LoadUnloadAssembler
        {
            get
            {
                return new RelayCommand(() => {
                    string pathToGameIni = Path.Combine(MainWindowViewModel.PathToAssemblersFolder, Assembler.GameName, "Settings.ini");
                    if (File.Exists(pathToGameIni))
                    {
                        IniFile ini = new IniFile(pathToGameIni);
                        string nameLoadedAssemblers = ini.Read("LoadedAssembler", "Config");
                        string pathToModsFolderThisGame = MainWindowViewModel.Ini.Read(Assembler.GameName, "GameList");
                        string pathToModsThisGame = Path.Combine(MainWindowViewModel.Ini.Read("PathToTheAssemblersFolder", "ModManager"), Assembler.GameName, "mods");
                        if (nameLoadedAssemblers == Assembler.Name)
                        {
                            foreach (var item in Directory.GetFiles(pathToModsFolderThisGame))
                            {
                                File.Delete(item);
                            }
                            ini.Write("LoadedAssembler", "", "Config");
                            
                            string pathToThisAssembler = Path.Combine(MainWindowViewModel.PathToAssemblersFolder, Assembler.GameName, "Assemblers", Assembler.Name);
                            string path = Path.Combine(pathToThisAssembler, "Worlds");
                            string path2 = Path.Combine(pathToThisAssembler, "Configs");
                            if (Directory.Exists(path) && Assembler.IsCheckedSaveWorlds)
                            {
                                MainWindowViewModel.ClearDirectory(path);
                                //MessageBox.Show(Assembler.PathToAssemblerWorlds + "\n" + path);//----------------------
                                foreach (var item in Directory.GetDirectories(Assembler.PathToAssemblerWorlds))
                                {
                                    MainWindowViewModel.CopyDirectory(item, Path.Combine(path, new DirectoryInfo(item).Name));
                                }
                                MainWindowViewModel.ClearDirectory(Assembler.PathToAssemblerWorlds);
                            }
                            if (Directory.Exists(path2) && Assembler.IsCheckedSaveConfigs)
                            {
                                MainWindowViewModel.ClearDirectory(path2);
                                //MessageBox.Show(Assembler.PathToAssemblerWorlds + "\n" + path2);//----------------------
                                foreach (var item in Directory.GetFiles(Assembler.PathToAssemblerConfigs))
                                {
                                    File.Copy(item, Path.Combine(path2, new DirectoryInfo(item).Name));
                                }
                                MainWindowViewModel.ClearDirectory(Assembler.PathToAssemblerConfigs);
                            }

                            Assembler.IsLoadedAssembler = false;
                            MainWindowViewModel.ChangeAssembler(Assembler);
                        }
                        else
                        {
                            MessageBox.Show("Сборка: " + Name + "\n" + "Загружена");//----------------------
                            string pathToThisAssembler = Path.Combine(MainWindowViewModel.PathToAssemblersFolder, Assembler.GameName, "Assemblers", Assembler.Name);
                            string path = Path.Combine(pathToThisAssembler, "Worlds");
                            string path2 = Path.Combine(pathToThisAssembler, "Configs");
                            if(Directory.Exists(path) && Assembler.IsCheckedSaveWorlds)
                            {
                                MainWindowViewModel.ClearDirectory(Assembler.PathToAssemblerWorlds);
                                foreach (var item in Directory.GetDirectories(path))
                                {
                                    MainWindowViewModel.CopyDirectory(item, Path.Combine(Assembler.PathToAssemblerWorlds, new DirectoryInfo(item).Name));
                                }
                            }
                            if (Directory.Exists(path2) && Assembler.IsCheckedSaveConfigs)
                            {
                                MainWindowViewModel.ClearDirectory(Assembler.PathToAssemblerConfigs);
                                foreach (var item in Directory.GetFiles(path2))
                                {
                                    File.Copy(item, Path.Combine(Assembler.PathToAssemblerConfigs, new DirectoryInfo(item).Name));
                                }
                            }
                            foreach (var item in Directory.GetFiles(pathToModsFolderThisGame))
                            {
                                if (!Assembler.ModList.Any(x => x.Name == new FileInfo(item).Name) || !Assembler.ModList.First(x => x.Name == new FileInfo(item).Name).IsChecked)
                                {
                                    File.Delete(item);
                                }

                            }
                            foreach (var item in Directory.GetFiles(pathToModsThisGame))
                            {
                                if (Assembler.ModList.Any(x => x.Name == new FileInfo(item).Name) && !File.Exists(Path.Combine(pathToModsFolderThisGame, new FileInfo(item).Name)) && Assembler.ModList.First(x => x.Name == new FileInfo(item).Name).IsChecked)
                                {
                                    File.Copy(item, Path.Combine(pathToModsFolderThisGame, new FileInfo(item).Name));
                                }

                            }
                            ini.Write("LoadedAssembler", Name, "Config");
                            Assembler.IsLoadedAssembler = true;
                            MainWindowViewModel.ChangeAssembler(Assembler);
                        }
                        OnPropertyChanged(nameof(ButtonLoadUnloadText));
                    }
                });
            }
        }

        public ICommand OpenSettingsThisAssembler
        {
            get
            {
                return new RelayCommand(() => {
                    OpenSettings(Assembler);
                });
            }
        }
        public ICommand DeleteAssembler
        {
            get
            {
                return new RelayCommand(() => {
                    if((DialogResult)MessageBox.Show("Вы точно хотите удалить данную сборку?\n" +
                        "Данное действие невозможно отменить!", "Вы уверены?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        string path = Path.Combine(MainWindowViewModel.PathToAssemblersFolder, Assembler.GameName);
                        IniFile ini = new IniFile(Path.Combine(path, "Settings.ini"));
                        if (ini.Read("LoadedAssembler", "Config") == Name)
                        {
                            ini.Write("LoadedAssembler", "", "Config");
                        }
                        //Path.Combine(MainWindowViewModel.PathToAssemblersFolder, Assembler.GameName, "Assemblers", Name)
                        Directory.Delete(Path.Combine(path, "Assemblers", Name), true);
                        MainWindowViewModel.DeleteAssemblerFromList(Assembler.GameName, Assembler.Name);
                        //Сделать удаление сборки из бокового списка!!!
                        //Папка благополучно удаляется!
                    }
                });
            }
        }


        private void GetWeightAssembler()
        {
            string pathToModsFolderInGame = Path.Combine(MainWindowViewModel.PathToAssemblersFolder, Assembler.GameName, "mods");
            double weight = 0;
            foreach (var item in Assembler.ModList)
            {
                weight += Math.Ceiling((double)new FileInfo(Path.Combine(pathToModsFolderInGame, item.Name)).Length);
            }
            ModsWeight = Math.Ceiling((weight / (1024 * 1024)));
            //ModsWeight = 1000;
            //ModsWeight = weight;
        }
        private void HandleAssemblerChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Assembler));
            OnPropertyChanged(nameof(ButtonLoadUnloadText));
            GetWeightAssembler();
        }
        public static void SetBaseValues(double opacityPanels, SolidColorBrush colorPanels, AssemblerInfo assembler)
        {
            _assembler = assembler;
            _opacityPanels = opacityPanels;
            _colorPanels = colorPanels;
            //_name = assembler.Name;

            AssemblerChanged?.Invoke(null, EventArgs.Empty);
            /*string test = "";
            foreach (var item in _assembler.ModList)
            {
                //ModList.Add(new ModInfo(item, bool.Parse((IniSettings.Read(item, "Mods")).ToLower()), ModList.Count + 1));
                test += item.Name + "  :  " + _assembler.IniSettings.Read(item.Name, "Mods") + "\n";
            }
            MessageBox.Show(test);*/
            //Возникла проблема, программа не хочет при переключении на другую сборку - переключать информацию о ней на экране
        }
        public static void SetBaseValues(AssemblerInfo assembler)
        {
            _assembler = assembler;
            AssemblerChanged?.Invoke(null, EventArgs.Empty);
        }
        public AssemblerInfoPageViewModel()
        {
            //MessageBox.Show("Создался новый экземпляр AssemblerInfoPageViewModel");
            if(AssemblerChanged == null)
            AssemblerChanged += HandleAssemblerChanged;
            GetWeightAssembler();
        }
    }
}
