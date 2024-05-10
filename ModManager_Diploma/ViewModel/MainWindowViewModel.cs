using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModManager_Diploma.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _modList;

        public string ModList
        {
            get => _modList;
            set
            {
                _modList = value;
                OnPropertyChanged(nameof(ModList));
            }
        }

    }
}
