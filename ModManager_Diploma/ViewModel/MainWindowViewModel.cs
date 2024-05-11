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

namespace ModManager_Diploma.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        public IniFile ini;
        private string _modList;
        private ImageBrush _backgroundWindow;
        private Uri _page = new Uri("pack://application:,,,/View/Pages/AboutTheProgram.xaml");
        public string ModList
        {
            get => _modList;
            set
            {
                _modList = value;
                OnPropertyChanged(nameof(ModList));
            }
        }
        public ImageBrush BackgroundWindow
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
                    Page = new Uri("pack://application:,,,/View/Pages/AboutTheProgram.xaml");
                });
            }
        }


        public void GetMods()
        {
            /*string path = "D://Minecraft//Minecraft//game//mods";
            DirectoryInfo directory = new DirectoryInfo(path);
            foreach (var mods in directory.GetFiles())
            {
                ModList += mods.Name + "\n";
            }
            ModList += "\n\n" + Directory.GetCurrentDirectory() + "\n\n" +
                new Uri("pack://application:,,,/Source/Images/img.png", UriKind.Absolute).AbsolutePath + "\n\n" +
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            //FileInfo file = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestModManagerFolder/Configs"));
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestModManagerFolder/Images"));

            if(!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestModManagerFolder", "Configs/Settings.ini")))
            {
                ini = new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestModManagerFolder", "Configs/Settings.ini"));
                ini.Write("Num", "0");
                Integer = 0;
            }
            else
            {
                ini = new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestModManagerFolder", "Configs/Settings.ini"));
                Integer = int.Parse(ini.Read("Num")) + 1;
                ini.Write("Num", Integer.ToString());
            }*/
            

            if(File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestModManagerFolder/Images/img.png")))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                //image.UriSource = new Uri("../../../Source/Images/Backgrounds/Background.png", UriKind.Relative);
                image.UriSource = new Uri(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestModManagerFolder/Images/img.png"), UriKind.Relative);
                image.EndInit();
                BackgroundWindow = new ImageBrush(image);
            }
            //Directory.CreateDirectory("../../../Test");
            //DirectoryInfo dir = new DirectoryInfo("D:\\4 Курс\\Диплом\\ModManager_Diploma\\ModManager_Diploma\\TestFolder");
            //new Uri("pack://application:,,,/Source/Images/img.png", UriKind.Absolute)

        }
    }
}
