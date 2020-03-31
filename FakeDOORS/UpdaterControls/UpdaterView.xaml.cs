using FakeDOORS.UpdaterControls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ReqTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FakeDOORS
{
    /// <summary>
    /// Interaction logic for UpdaterView.xaml
    /// </summary>
    public partial class UpdaterView : UserControl, IUpdaterView
    {
        private IDatabaseService databaseService;
        public UpdaterView(IDatabaseService databaseService)
        {
            InitializeComponent();

            this.databaseService = databaseService;
            databaseService.RequirementsChanged += DatabaseService_RequirementsChanged;
        }

        private void DatabaseService_RequirementsChanged(object sender, EventArgs e)
        {
            ActualExportDateTextBlock.Text = databaseService.GetCacheCreationDate().ToShortDateString();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await DialogCoordinator.Instance.ShowMessageAsync(this, "Confirm replacement.",
                "Do you want to replace actual cached file with latest one from server?", MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative)
            {
                await databaseService.DownloadNewestVersion();
                UpdateButton.Content = "Get latest version";
                UpdateButton.Background = Brushes.Gainsboro;
            }
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ExporterButton_Click(object sender, RoutedEventArgs e)
        {
            var exporterWindow = new DoorsExporterWindow();
            exporterWindow.Closed += async (s, args) =>
            {
                if (File.Exists(exporterWindow.OutputTextBox.Text))
                    await databaseService.Init();
            };
            exporterWindow.Show();
        }

        private async void UpdateButton_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isUpdateAvailable = await databaseService.CheckForUpdates();
                if (isUpdateAvailable)
                {
                    UpdateButton.Content = "UPDATE AVAILABLE";
                    UpdateButton.Background = Brushes.Red;
                }
            }
            catch
            {
                await DialogCoordinator.Instance.ShowMessageAsync(this, "Warning", "Cannot access the server");
            }
        }
    }
}
