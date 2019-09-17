using FakeDOORS.DatabaseControls.ChapterSelectionControls;
using FakeDOORS.DatabaseControls.DatabaseSettingsControls;
using FakeDOORS.DatabaseControls.RequirementsControls;
using FakeDOORS.DatabaseControls.TestCasesControls;
using Microsoft.Extensions.DependencyInjection;
using ReqTools;
using System;
using System.Collections.Generic;
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
        private IDatabaseSettingsView databaseSettingsView;

        private IDatabaseService databaseService;

        public DatabaseView(IDatabaseService databaseService)
        {
            InitializeComponent();

            this.databaseService = databaseService;

            RequirementViewInit();
            TestCasesViewInit();
            ChapterSelectionViewInit();
            DatabaseSettingsViewInit();
        }

        private void DatabaseSettingsViewInit()
        {
            databaseSettingsView = App.ServiceProvider.GetRequiredService<IDatabaseSettingsView>();
            databaseSettingsView.ViewSettingsChanged += DatabaseSettingsView_ViewSettingsChanged;

            DatabaseSettingsViewControl.Content = databaseSettingsView;
        }

        private void DatabaseSettingsView_ViewSettingsChanged(object sender, ViewSettingsEventArgs e)
        {
            chapterSelectionView.ResetView();
            databaseService.ChangeVersionFilter(e.NewFilterVersion);
            requirementsView.SetReqView(e.Settings);
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
            testCasesView.SelectTCsForSelectedReqsButtonClicked += TestCasesView_SelectTCsForSelectedReqsButtonClicked;

            TestCasesViewControl.Content = testCasesView;
        }

        private void TestCasesView_SelectTCsForSelectedReqsButtonClicked(object sender, EventArgs e)
        {
            var selectedReqs = requirementsView.GetSelectedRequirements();
            var testCases = databaseService.GetTestCasesFromReqList(selectedReqs)
                .Select(x => x.IDValue)
                .ToList();

            testCasesView.AddSelectedTCs(testCases);
        }

        private void ChapterSelectionViewInit()
        {
            chapterSelectionView = App.ServiceProvider.GetRequiredService<IChapterSelectionView>();
            chapterSelectionView.SelectionChanged += ChapterSelectionView_SelectionChanged;

            ChapterSelectionViewControl.Content = chapterSelectionView;
        }

        private void ChapterSelectionView_SelectionChanged(object sender, EventArgs e)
        {
            if(chapterSelectionView.SelectedChapter.chapter == "-")
                requirementsView.LimitScrollingToOneChapter(0);
            else
            {
                if (chapterSelectionView.ClearAllTCs)
                    testCasesView.SetSelectedTCs(new List<int>());
                if (chapterSelectionView.SelectChaptersTCs)
                    testCasesView.AddSelectedTCs(databaseService
                        .GetTestCasesFromChapter(chapterSelectionView.SelectedChapter.id)
                        .Select(x => x.IDValue)
                        .ToList());

                requirementsView.LimitScrollingToOneChapter(chapterSelectionView.SelectedChapter.id);
            }
        }

        private async void TestCasesView_SelectionChanged(object sender, EventArgs e)
        {
            await requirementsView.SetSelectedTestCases(testCasesView.SelectedTCs.ToList());
        }

        private async void RequirementsView_Loaded(object sender, RoutedEventArgs e)
        {
            await databaseService.Init();
        }
    }
}
