using GalaSoft.MvvmLight.Command;
using ModManager_Diploma.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ModManager_Diploma.ViewModel
{
    public class CreateAssemblerPageViewModel : BaseViewModel
    {
        private static double _opacityPanels;
        private static SolidColorBrush _colorPanels;
        private string _assemblerName;
        private string _selectGame;
        private ObservableCollection<string> _gameList;
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
            get => _assemblerName;
            set
            {
                _assemblerName = value;
                OnPropertyChanged(nameof(AssemblerName));
            }
        }
        public string SelectGame
        {
            get => _selectGame;
            set
            {
                _selectGame = value;
                OnPropertyChanged(nameof(SelectGame));
            }
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

        public static ClosePage CloseAssemblerOptions;


        public ICommand SaveSettings
        {
            get
            {
                return new RelayCommand(() => {
                    
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



        public static void SetOpacityPanels(double opacity)
        {
            _opacityPanels = opacity;
            
        }
        public static void SetColorPanels(SolidColorBrush color)
        {
            _colorPanels = color;
        }

        public static void SetBaseValues(double opacityPanels, SolidColorBrush colorPanels)
        {
            _opacityPanels = opacityPanels;
            _colorPanels = colorPanels;
        }

        public CreateAssemblerPageViewModel()
        {
            //OpacityPanels = MainWindowViewModel.GetOpacityPanels();
            //ColorPanels = MainWindowViewModel.GetColorPanels();
            GameList = new ObservableCollection<string>(MainWindowViewModel.Ini.GetKeys("GameList"));
        }
    }
}
