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
            ServerMainPath.Text = settings.ServerPath;
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        => await Task.Run(() =>
         {
             try
             {
                 if (File.Exists("FakeDOORS.exe.bak"))
                     File.Delete("FakeDOORS.exe.bak");

                 File.Move("FakeDOORS.exe", "FakeDOORS.exe.bak");
                 File.Copy(settings.ServerPath + "FakeDOORS.exe", "FakeDOORS.exe", true);
                 MessageBox.Show("Done, restart app to use the new version");
             }
             catch(Exception e)
             {
                 File.WriteAllText("log.txt", e.Message);
                 MessageBox.Show($"Couldn't install the newest version.\n Please install it manually from\n{settings.ServerPath}");
             }
         });

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.ServiceProvider.GetRequiredService<ChangelogWindow>().Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ShortcutCreator.CreateShortcut();
        }
    }
}
