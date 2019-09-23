using MahApps.Metro.Controls;
using Microsoft.Extensions.Options;
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
using System.Windows.Shapes;

namespace FakeDOORS.SettingsControls
{
    /// <summary>
    /// Interaction logic for ChangelogWindow.xaml
    /// </summary>
    public partial class ChangelogWindow : MetroWindow
    {
        private string changelogPath;
        public ChangelogWindow(AppSettings options)
        {
            InitializeComponent();

            changelogPath = options.ServerPath + "changelog.rtf";
        }

        private void RichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var richTextBox = (RichTextBox)sender;

            try
            {
                var changelogText = File.ReadAllBytes(changelogPath);
                MemoryStream stream = new MemoryStream(changelogText);

                richTextBox.Selection.Load(stream, DataFormats.Rtf);
            }
            catch
            {
                MessageBox.Show($"Error with opening changelog, to see it manually go to {changelogPath}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
