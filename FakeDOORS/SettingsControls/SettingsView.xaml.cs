using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MahApps.Metro.Controls.Dialogs;

namespace FakeDOORS.SettingsControls
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl, ISettingsView
    {
        private AppSettings settings;
        public SettingsView(AppSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
        }

        private void ServerMainPath_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBlock).Text = settings.ServerPath;
        }

        private void OpenServerPathButton_Click(object sender, RoutedEventArgs e)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo(settings.ServerPath)
                {
                    UseShellExecute = true,
                }
            };
            p.Start();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var backupLocation = Directory.GetCurrentDirectory() + @"\FakeDOORS.exe.bak";
                var installLocation = Directory.GetCurrentDirectory() + @"\FakeDOORS.exe";
                var serverLocation = settings.ServerPath + "FakeDOORS.exe";
                if (File.Exists(backupLocation))
                    File.Delete(backupLocation);

                File.Move(installLocation, backupLocation, true);
                File.Copy(serverLocation, installLocation, true);
                await DialogCoordinator.Instance.ShowMessageAsync(this, "Success!", "Done, restart app to use the new version");
            }
            catch
            {
                await DialogCoordinator.Instance.ShowMessageAsync(this, "Error", $"Couldn't install the newest version.\nPlease install it manually from\n{settings.ServerPath}\nCopy it to\n{Directory.GetCurrentDirectory()}");
            }
        }

        private void OpenChangelogButton_Click(object sender, RoutedEventArgs e)
        {
            App.ServiceProvider.GetRequiredService<ChangelogWindow>().Show();
        }

        private void CreateShortcutButton_Click(object sender, RoutedEventArgs e)
        {
            ShortcutCreator.CreateShortcut();
        }

        private void InstallDirPath_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBlock).Text = Directory.GetCurrentDirectory();
        }

        private void OpenInstallDirPathButton_Click(object sender, RoutedEventArgs e)
        {
            using (var p = new Process
            {
                StartInfo = new ProcessStartInfo(Directory.GetCurrentDirectory())
                {
                    UseShellExecute = true,
                }
            })
            {
                p.Start();
            }
        }
    }
}
