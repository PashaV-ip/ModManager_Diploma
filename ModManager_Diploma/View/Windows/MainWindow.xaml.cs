using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModManager_Diploma.ViewModel;

namespace ModManager_Diploma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //(DataContext as MainWindowViewModel).StartApplication();
            //MainWindowViewModel.StartApp();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            (sender as Frame).NavigationService.RemoveBackEntry();
        }
    }
}
