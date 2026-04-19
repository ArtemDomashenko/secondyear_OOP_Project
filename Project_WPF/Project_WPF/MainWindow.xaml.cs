using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Project_WPF.Models;

namespace Project_WPF
{
    public partial class MainWindow : Window
    {
        private MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = (MainViewModel)DataContext;
            tabMain.SelectionChanged += OnTabChanged;
        }

        private void OnTabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pageAnalyze == null) return;
            pageAnalyze.Visibility = Visibility.Collapsed;
            pageHistory.Visibility = Visibility.Collapsed;
            pageSettings.Visibility = Visibility.Collapsed;
            switch (tabMain.SelectedIndex)
            {
                case 0: pageAnalyze.Visibility = Visibility.Visible; break;
                case 1: pageHistory.Visibility = Visibility.Visible; break;
                case 2: pageSettings.Visibility = Visibility.Visible; break;
            }
        }

        private void BtnWatch_Click(object sender, RoutedEventArgs e)
        {
            if (_vm?.LastResult != null)
                OpenInBrowser(_vm.LastResult.VideoId);
        }

        private void BtnWatchSelected_Click(object sender, RoutedEventArgs e)
        {
            if (_vm?.Selected != null)
                OpenInBrowser(_vm.Selected.VideoId);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is VideoInfo video)
                _vm.DeleteItem(video);
        }

        private void OpenInBrowser(string videoId)
        {
            Process.Start(new ProcessStartInfo(
                "https://www.youtube.com/watch?v=" + videoId)
            { UseShellExecute = true });
        }
    }
}