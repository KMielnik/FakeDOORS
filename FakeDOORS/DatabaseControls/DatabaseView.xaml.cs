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

namespace FakeDOORS
{
    /// <summary>
    /// Interaction logic for DatabaseView.xaml
    /// </summary>
    public partial class DatabaseView : UserControl
    {
        private RequirementsView requirementsView;
        private TestCasesView testCasesView;
        public DatabaseView()
        {
            InitializeComponent();

            requirementsView = new RequirementsView();
            requirementsView.Loaded += RequirementsView_Loaded;

            RequirementsViewControl.Content = requirementsView;

            testCasesView = new TestCasesView();

            TestCasesViewControl.Content = testCasesView;
        }

        private async void RequirementsView_Loaded(object sender, RoutedEventArgs e)
        {
            var loadingReqsTask = App.ServiceProvider.GetService<IDatabaseService>().Init();
            var settings = new ReqViewSettingsBuilder()
                .AddDefaultSettings()
                .Build();

            requirementsView.SetReqView(settings);
            await loadingReqsTask;
        }
    }
}
