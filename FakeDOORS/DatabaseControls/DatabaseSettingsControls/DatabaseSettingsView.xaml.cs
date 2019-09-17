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

        private void ValidIn_Loaded(object sender, RoutedEventArgs e)
        {
            ValidIn.SelectedIndex = 0;
            ValidIn.UpdateLayout();
        }

        private void ValidIn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var eventArgs = new ViewSettingsEventArgs()
            {
                NewFilterVersion = (e.AddedItems[0] as ComboBoxItem).Content.ToString()
            };

            ViewSettingsChanged?.Invoke(this, eventArgs);
        }
    }
}
