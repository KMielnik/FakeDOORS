using FakeDOORS.SettingsControls;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;
using System.Windows;

namespace FakeDOORS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private IDatabaseView databaseView;
        private IUpdaterView updaterView;
        private ISettingsView settingsView;
        private AppSettings settings;
        public MainWindow(IDatabaseView databaseView, IUpdaterView updaterView, ISettingsView settingsView, AppSettings options)
        {
            InitializeComponent();

            this.databaseView = databaseView;
            DatabaseViewControl.Content = databaseView;

            this.updaterView = updaterView;
            UpdaterViewControl.Content = updaterView;

            this.settingsView = settingsView;
            SettingsViewControl.Content = settingsView;

            this.settings = options;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var version = $"{settings.VersionMajor}.{settings.VersionMinor}";
            this.Title += $" - {version}";

            try
            {
                var serverVersion = File.ReadAllText(settings.ServerPath + @"actual_version.txt");
                if (version != serverVersion)
                    this.Title += $" - NEW VERSION AVAILABLE ({serverVersion})";
            }
            catch { }
        }

        private void SettingsToggle_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(SettingsViewControl.Visibility == Visibility.Collapsed)
            {
                SettingsViewControl.Visibility = Visibility.Visible;
                DatabaseViewControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                SettingsViewControl.Visibility = Visibility.Collapsed;
                DatabaseViewControl.Visibility = Visibility.Visible;
            }
        }
    }
}
