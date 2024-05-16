using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.ObjectModel;
using ModManager_Diploma.Model;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Specialized;
using System.DirectoryServices;

namespace ModManager_Diploma.ViewModel
{
    public delegate void ClosePage();
    public delegate void SaveOptions(string pathAssemblers, double opacityPanels, SolidColorBrush colorPanels, string? path);
    public delegate void SetNewBackground(Brush brush);
    public class OptionsPageViewModel : BaseViewModel
    {
        private static string _pathAssemblers;
        private static double _opacityPanels;
        private static SolidColorBrush _colorPanels;

        public string PathAssemblers
        {
            get => _pathAssemblers;
            set
            {
                _pathAssemblers = value;
                OnPropertyChanged(nameof(PathAssemblers));
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


        public static string BackgroundPath;
        public static ClosePage CloseOptions;
        public static SaveOptions SaveOptions;
        public static SetNewBackground SetNewBackground;
        public ObservableCollection<GameInfo> GameList { get; set; }
        public ICommand SaveSettings
        {
            get
            {
                return new RelayCommand(() => {
                    SaveOptions(PathAssemblers, OpacityPanels, ColorPanels, BackgroundPath);
                });
            }
        }

        public ICommand CancleSettings
        {
            get
            {
                return new RelayCommand(() => {
                    CloseOptions();
                });
            }
        }

        public ICommand BrowseAssemblersFolder
        {
            get
            {
                return new RelayCommand(() => {
                    var dialog = new FolderBrowserDialog();
                    string path = MainWindowViewModel.GetPathToAssemblersFolder();
                    if (path != "" && Directory.Exists(path))
                    {
                        dialog.InitialDirectory = path;
                        dialog.SelectedPath = path;
                    }
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        PathAssemblers = dialog.SelectedPath;
                    }
                });
            }
        }

        public ICommand BrowseBackgroundImage
        {
            get
            {
                return new RelayCommand(() => {
                    var dialog = new OpenFileDialog();
                    dialog.Filter = "PNG (*.png)|*.png"; //"Image files (*.png;*.jpg)|*.png;*.jpg";
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK && new FileInfo(dialog.FileName).Exists)
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        image.UriSource = new Uri(dialog.FileName, UriKind.Relative);
                        image.EndInit();
                        SetNewBackground(new ImageBrush(image));
                        BackgroundPath = dialog.FileName;
                    }
                });
            }
        }

        public ICommand AddGame
        {
            get
            {
                return new RelayCommand(() => {
                    foreach(var item in GameList)
                    {
                        if (item.IsReadOnly == false)
                            return;
                    }
                    GameList.Add(new GameInfo(RemoveGame));
                });
            }
        }

        

        public void RemoveGame(GameInfo game)
        {
            GameList.Remove(game);
        }
        public static void SetBaseValues(string pathAssemblers, double opacityPanels, SolidColorBrush colorPanels)
        {
            _pathAssemblers = pathAssemblers;
            _opacityPanels = opacityPanels;
            _colorPanels = colorPanels;
        }
        public OptionsPageViewModel()
        {
            GameList = new ObservableCollection<GameInfo>();
            foreach(var item in MainWindowViewModel.Ini.GetKeys("GameList"))
            {
                GameList.Add(new GameInfo(RemoveGame, item.ToString(), MainWindowViewModel.Ini.Read(item, "GameList")));
            }
        }
    }
}
