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

        private ViewSettingsEventArgs GetEventArgs((bool reqs, bool column, bool style) reload)
        {
            var reqViewSettingsBuilder = new ReqViewSettingsBuilder();

            if(reload.column)
            foreach (ListBoxItem column in ColumnsSelectionListBox.SelectedItems)
            {
                switch (column.Content.ToString())
                {
                    case "ID":
                        reqViewSettingsBuilder = reqViewSettingsBuilder.AddIDColumn();
                        break;

                    case "Text":
                        reqViewSettingsBuilder = reqViewSettingsBuilder.AddTextColumn();
                        break;

                    case "Functional Variants":
                        reqViewSettingsBuilder = reqViewSettingsBuilder.AddFVariantsColumn();
                        break;

                    case "Status":
                        reqViewSettingsBuilder = reqViewSettingsBuilder.AddStatusColumn();
                        break;

                    case "Valid From/To":
                        reqViewSettingsBuilder = reqViewSettingsBuilder.AddValidFromToColumn();
                        break;
                }
            }

            if(reload.style)
            foreach (ListBoxItem column in MiscSettingsSelectionListBox.SelectedItems)
            {
                switch (column.Content.ToString())
                {
                    case "Bold Headers":
                        reqViewSettingsBuilder = reqViewSettingsBuilder.SetBoldHeaders();
                        break;
                }
            }

            return new ViewSettingsEventArgs()
            {
                NewFilterVersion = ValidIn.SelectedItem is null ? "-" : (ValidIn.SelectedItem as ComboBoxItem).Content.ToString(),
                Settings = reqViewSettingsBuilder.Build(),
                ReqsNeedsReloading = reload.reqs,
                ColumnsNeedsReloading = reload.column,
                StyleNeedsReloading = reload.style
            };
        }

        private void SetSettings_Click(object sender, RoutedEventArgs e)
        {
            ViewSettingsChanged?.Invoke(this, GetEventArgs((true, true, true)));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewSettingsChanged?.Invoke(this, GetEventArgs((true, true, true)));
        }

        private void ValidIn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewSettingsChanged?.Invoke(this, GetEventArgs((true, false, false)));
        }

        private void ColumnsSelectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewSettingsChanged?.Invoke(this, GetEventArgs((false, true, false)));
        }

        private void MiscSettingsSelectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewSettingsChanged?.Invoke(this, GetEventArgs((false, false, true)));
        }
    }
}
