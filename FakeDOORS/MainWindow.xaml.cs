using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FakeDOORS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IDatabaseView databaseView;
        private IUpdaterView updaterView;
        public MainWindow(IDatabaseView databaseView, IUpdaterView updaterView)
        {
            InitializeComponent();

            this.databaseView = databaseView;
            DatabaseViewControl.Content = databaseView;

            this.updaterView = updaterView;
            UpdaterViewControl.Content = updaterView;
        }

        private void DisplayChangeLog()
        {
            //if (!ApplicationDeployment.IsNetworkDeployed)
            //    return;

            //Title += " - " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();

            //if (!ApplicationDeployment.CurrentDeployment.IsFirstRun)
            //    return;

            //new ChangelogWindow().Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayChangeLog();
        }
    }
}
