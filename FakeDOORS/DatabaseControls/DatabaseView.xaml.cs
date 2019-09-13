using FakeDOORS.DatabaseControls.ChapterSelectionControls;
using FakeDOORS.DatabaseControls.RequirementsControls;
using FakeDOORS.DatabaseControls.TestCasesControls;
using Microsoft.Extensions.DependencyInjection;
using ReqTools;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FakeDOORS
{
    /// <summary>
    /// Interaction logic for DatabaseView.xaml
    /// </summary>
    public partial class DatabaseView : UserControl, IDatabaseView
    {
        private IRequirementsView requirementsView;
        private ITestCasesView testCasesView;
        private IChapterSelectionView chapterSelectionView;

        private IDatabaseService databaseService;

        public DatabaseView(IDatabaseService databaseService)
        {
            InitializeComponent();

            this.databaseService = databaseService;

            RequirementViewInit();
            TestCasesViewInit();
            ChapterSelectionViewInit();
        }

        private void RequirementViewInit()
        {
            requirementsView = App.ServiceProvider.GetRequiredService<IRequirementsView>();
            requirementsView.Loaded += RequirementsView_Loaded;

            RequirementsViewControl.Content = requirementsView;
        }
        private void TestCasesViewInit()
        {
            testCasesView = App.ServiceProvider.GetRequiredService<ITestCasesView>();

            testCasesView.SelectionChanged += TestCasesView_SelectionChanged;

            TestCasesViewControl.Content = testCasesView;
        }
        private void ChapterSelectionViewInit()
        {
            chapterSelectionView = App.ServiceProvider.GetRequiredService<IChapterSelectionView>();
            chapterSelectionView.SelectionChanged += ChapterSelectionView_SelectionChanged;
        }

        private void ChapterSelectionView_SelectionChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
