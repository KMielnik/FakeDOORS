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
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;

namespace FakeDOORS.DatabaseControls.TestCasesControls
{
    /// <summary>
    /// Interaction logic for TestCasesView.xaml
    /// </summary>
    public partial class TestCasesView : UserControl
    {
        public ObservableCollection<int> TestCases { get; set; } = new ObservableCollection<int>();

        private IDatabaseService databaseService;
        public TestCasesView()
        {
            InitializeComponent();

            databaseService = App.ServiceProvider.GetService<IDatabaseService>();

            databaseService.RequirementsChanged += DatabaseService_RequirementsChanged;
        }

        private async void DatabaseService_RequirementsChanged(object sender, EventArgs e)
        {
            TestCases.Clear();
            (await databaseService.GetTestCases())
                .Select(x=>x.IDValue)
                .ToList()
                .ForEach(x => TestCases.Add(x));
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AllTCsListBox_LostFocus(object sender, MouseEventArgs e)
        {

        }

        private void TCFilter_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void TCFilter_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AllTCsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
