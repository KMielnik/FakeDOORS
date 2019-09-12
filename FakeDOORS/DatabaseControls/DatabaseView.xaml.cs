using FakeDOORS.DatabaseControls.RequirementsControls;
using FakeDOORS.DatabaseControls.TestCasesControls;
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
using System.Linq;

namespace FakeDOORS
{
    /// <summary>
    /// Interaction logic for DatabaseView.xaml
    /// </summary>
    public partial class DatabaseView : UserControl
    {
        private RequirementsView requirementsView;
        private TestCasesView testCasesView;

        private IDatabaseService databaseService;

        public DatabaseView(IDatabaseService databaseService)
        {
            InitializeComponent();

            this.databaseService = databaseService;

            requirementsView = App.ServiceProvider.GetRequiredService<RequirementsView>();
            requirementsView.Loaded += RequirementsView_Loaded;

            RequirementsViewControl.Content = requirementsView;

            testCasesView = App.ServiceProvider.GetRequiredService<TestCasesView>();

            testCasesView.SelectionChanged += TestCasesView_SelectionChanged;

            TestCasesViewControl.Content = testCasesView;
        }

        private async void TestCasesView_SelectionChanged(object sender, EventArgs e)
        {
            await requirementsView.SetSelectedTestCases(testCasesView.SelectedTCs.ToList());
        }

        private async void RequirementsView_Loaded(object sender, RoutedEventArgs e)
        {
            var loadingReqsTask = databaseService.Init();
            var settings = new ReqViewSettingsBuilder()
                .AddDefaultSettings()
                .Build();

            requirementsView.SetReqView(settings);
            await loadingReqsTask;
        }
    }
}
