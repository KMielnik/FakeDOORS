﻿using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReqTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FakeDOORS.UpdaterControls
{
    /// <summary>
    /// Interaction logic for DoorsExporterWindow.xaml
    /// </summary>
    public partial class DoorsExporterWindow : MetroWindow
    {
        readonly private IDatabaseService databaseService;
        public DoorsExporterWindow()
        {
            InitializeComponent();

            this.databaseService = App.ServiceProvider.GetRequiredService<IDatabaseService>();
        }

        private void InputButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".htm",
                Filter = "HTML Files (*.htm)|*.htm"
            };

            if (dlg.ShowDialog() == true)
            {
                InputTextBox.Text = dlg.FileName;
                InputTextBox.CaretIndex = InputTextBox.Text.Length;
            }
        }

        private void OutputButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".json",
                Filter = "Json Files (*.json)|*.json"
            };

            if (dlg.ShowDialog() == true)
            {
                OutputTextBox.Text = dlg.FileName;
                OutputTextBox.CaretIndex = OutputTextBox.Text.Length;
            }
        }
        private Task parseTask = null;
        private async void ParseButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputTextBox.Text) || string.IsNullOrWhiteSpace(OutputTextBox.Text))
            {
                ProgressTextBlock.Text = "Select input/output files";
                return;
            }

            try
            {
                ParseButton.IsEnabled = false;
                parseTask = databaseService.ParseToFileAsync(
                    new Progress<string>(x => ProgressTextBlock.Text = x),
                    InputTextBox.Text,
                    OutputTextBox.Text);
                await parseTask;
            }
            catch
            {
                ProgressTextBlock.Text = "ERROR!";
                MessageBox.Show("Error while parsing the file", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ParseButton.IsEnabled = true;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = "cached_reqs.json"; //TODO default path in databaseservice
            OutputButton.IsEnabled = false;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = "";
            OutputButton.IsEnabled = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (parseTask != null && !parseTask.IsCompleted)
            {
                MessageBox.Show("You are still parsing a file.\nThe task will continue as long as any app window is open.");
                if (MessageBox.Show("Still want to close this window?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            base.OnClosing(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Support_Tools\FakeDOORS\export_instructions.pdf")
            {
                UseShellExecute = true
            };
            p.Start();
        }
    }
}
