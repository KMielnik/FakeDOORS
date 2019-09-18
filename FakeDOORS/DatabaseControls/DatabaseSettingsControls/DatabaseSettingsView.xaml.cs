using FakeDOORS.DatabaseControls.RequirementsControls;
using ReqTools;
using System;
using System.Collections.Generic;
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

namespace FakeDOORS.DatabaseControls.DatabaseSettingsControls
{
    /// <summary>
    /// Interaction logic for DatabaseSettingsView.xaml
    /// </summary>
    public partial class DatabaseSettingsView : UserControl, IDatabaseSettingsView
    {
        private IDatabaseService databaseService;
        public DatabaseSettingsView(IDatabaseService databaseService)
        {
            InitializeComponent();

            this.databaseService = databaseService;


        }

        public event EventHandler<ViewSettingsEventArgs> ViewSettingsChanged;

        private ViewSettingsEventArgs GetEventArgs()
        {
            var reqViewSettings = new ReqViewSettingsBuilder();

            foreach(ListBoxItem column in ColumnsSelectionListBox.SelectedItems)
            {
                switch(column.Content.ToString())
                {
                    case "ID":
                        reqViewSettings = reqViewSettings.AddIDColumn();
                        break;

                    case "Text":
                        reqViewSettings = reqViewSettings.AddTextColumn();
                        break;

                    case "Functional Variants":
                        reqViewSettings = reqViewSettings.AddFVariantsColumn();
                        break;

                    case "Status":
                        reqViewSettings = reqViewSettings.AddStatusColumn();
                        break;

                    case "Valid From/To":
                        reqViewSettings = reqViewSettings.AddValidFromToColumn();
                        break;
                }
            }

            foreach (ListBoxItem column in MiscSettingsSelectionListBox.SelectedItems)
            {
                switch (column.Content.ToString())
                {
                    case "Bold Headers":
                        reqViewSettings = reqViewSettings.SetBoldHeaders();
                        break;
                }
            }

            return new ViewSettingsEventArgs()
            {
                NewFilterVersion = ValidIn.SelectedItem is null ? "-" : (ValidIn.SelectedItem as ComboBoxItem).Content.ToString(),
                Settings = reqViewSettings.Build()
            };
        }

        private void SetSettings_Click(object sender, RoutedEventArgs e)
        {
            ViewSettingsChanged?.Invoke(this, GetEventArgs());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewSettingsChanged?.Invoke(this, GetEventArgs());
        }
    }
}
