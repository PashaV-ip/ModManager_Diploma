using GalaSoft.MvvmLight.Command;
using ModManager_Diploma.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ModManager_Diploma.ViewModel
{
    public class ReferenceViewModel : BaseViewModel
    {
        public static event EventHandler ParametersChanged;

        private static double _opacityPanels;
        private static SolidColorBrush? _colorPanels;
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

        public ICommand OpenBrowser
        {
            get
            {
                return new RelayCommand(() => {
                    Process.Start(new ProcessStartInfo("https://github.com/PashaV-ip/ModManager_Diploma") { UseShellExecute = true });
                });
            }
        }
        public static void SetBaseValues(double opacityPanels, SolidColorBrush colorPanels)
        {
            _opacityPanels = opacityPanels;
            _colorPanels = colorPanels;
            ParametersChanged?.Invoke(null, EventArgs.Empty);
            //MessageBox.Show("Создаётся экземпляр СправкиViewModel");
        }

        private void HandleParametersChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(OpacityPanels));
            OpacityPanels = _opacityPanels;
            OnPropertyChanged(nameof(ColorPanels));
            ColorPanels = _colorPanels;
        }

        public ReferenceViewModel()
        {
            ParametersChanged += HandleParametersChanged;
        }
    }
}
