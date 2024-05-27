using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace ModManager_Diploma.Model
{
    public class ModInfo
    {
        public delegate void OnOffModInIniFile(string nameMod, bool value);

        private string _name;
        private bool _isChecked;
        private int _number;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
            }
        }
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
            }
        }
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
            }
        }

        public OnOffModInIniFile OnOffMod;
        public ICommand TestCommand
        {
            get
            {
                return new RelayCommand(() => {
                    /*if (IsChecked) MessageBox.Show(Name + " Enabled");
                    else MessageBox.Show(Name + " Disable");*/
                    if (OnOffMod != null)
                    {
                        OnOffMod(Name, IsChecked);
                    }
                });
            }
        }
        public ModInfo(string name, bool isChecked, int number)
        {
            Name = name;
            IsChecked = isChecked;
            Number = number;
        }
        public ModInfo(string name, bool isChecked, int number, OnOffModInIniFile onOffMod)
        {
            Name = name;
            IsChecked = isChecked;
            Number = number;
            OnOffMod += onOffMod;
        }
    }
}
