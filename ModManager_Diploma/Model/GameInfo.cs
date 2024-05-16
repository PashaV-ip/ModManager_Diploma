using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;
using System.Windows.Input;
using ModManager_Diploma.ViewModel;
using System.IO;

namespace ModManager_Diploma.Model
{
    public delegate void DeleteGame(GameInfo game);
    public class GameInfo : BaseViewModel
    {
        private string _nameGame = "";
        private string _pathGame = "";
        private Visibility _visible = Visibility.Collapsed;
        private bool _isReadOnly = false;
        public string NameGame 
        {
            get => _nameGame;
            set
            {
                _nameGame = value;
                OnPropertyChanged(nameof(NameGame));
            }
        }
        public string PathGame
        {
            get => _pathGame;
            set
            {
                _pathGame = value;
                OnPropertyChanged(nameof(PathGame));
            }
        }

        public Visibility Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                OnPropertyChanged(nameof(Visible));
            }
        }
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
            }
        }

        public DeleteGame DeleteGameFromList;


        public ICommand BrowseGamePath
        {
            get
            {
                return new RelayCommand(() => {
                    var dialog = new FolderBrowserDialog();
                    if (PathGame != "" && Directory.Exists(PathGame))
                    {
                        dialog.InitialDirectory = PathGame;
                        dialog.SelectedPath = PathGame;
                    }
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        PathGame = dialog.SelectedPath;
                    }
                });
            }
        }

        public ICommand DeleteGame
        {
            get
            {
                return new RelayCommand(() => {
                    if (IsReadOnly)
                    {
                        DialogResult result = (DialogResult)System.Windows.MessageBox.Show("Вы точно хотите удалить " + NameGame + " из списка?\n" +
                        "Все сборки зависящие от неё, будут удалены!", "Вы уверены?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == DialogResult.Yes)
                        {
                            MainWindowViewModel.Ini.DeleteKey(NameGame, "GameList");
                            DeleteGameFromList(this);
                        }
                    }
                    else
                    {
                        DeleteGameFromList(this);
                    }
                });
            }
        }

        public ICommand CompleteGame
        {
            get
            {
                return new RelayCommand(() => {
                    if(NameGame != "" && PathGame != "" && Directory.Exists(PathGame))
                    {   
                        MainWindowViewModel.Ini.Write(NameGame, PathGame, "GameList");
                        string pathFolder = MainWindowViewModel.GetPathToAssemblersFolder();
                        //if (pathFolder != "" && Directory.Exists(pathFolder))
                        //{
                        //    Directory.CreateDirectory(Path.Combine(pathFolder, NameGame));
                        //}
                        Visible = Visibility.Collapsed;
                        IsReadOnly = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Заполните все поля!", "Пустые поля...", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                });
            }
        }

        public GameInfo(DeleteGame deleteGameFromList, string name, string path)
        {
            NameGame = name;
            PathGame = path;
            IsReadOnly = true;
            Visible = Visibility.Collapsed;
            DeleteGameFromList = deleteGameFromList;
        }
        public GameInfo(DeleteGame deleteGameFromList)
        {
            NameGame = "";
            PathGame = "";
            IsReadOnly = false;
            Visible = Visibility.Visible;
            DeleteGameFromList = deleteGameFromList;
        }
    }
}
