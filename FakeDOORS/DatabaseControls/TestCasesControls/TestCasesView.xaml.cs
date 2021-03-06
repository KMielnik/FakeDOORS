﻿using ReqTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FakeDOORS.DatabaseControls.TestCasesControls
{
    /// <summary>
    /// Interaction logic for TestCasesView.xaml
    /// </summary>
    public partial class TestCasesView : UserControl, ITestCasesView
    {
        public ObservableCollection<int> TestCases { get; set; } = new ObservableCollection<int>();
        private string TCFilterActual = "";

        private IDatabaseService databaseService;
        public TestCasesView(IDatabaseService databaseService)
        {
            InitializeComponent();

            this.databaseService = databaseService;

            databaseService.RequirementsChanged += DatabaseService_RequirementsChanged;

            SetTCListViewFilter();
        }

        public event EventHandler SelectionChanged;
        public event EventHandler SelectTCsForSelectedReqsButtonClicked;

        public IEnumerable<int> SelectedTCs => AllTCsListBox.SelectedItems
            .Cast<int>();

        public void AddSelectedTCs(List<int> tcs)
        {
            tcs.ForEach(tc => AllTCsListBox.SelectedItems.Add(tc));
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
        public void SetSelectedTCs(List<int> tcs)
        {
            AllTCsListBox.SelectedItems.Clear();
            AddSelectedTCs(tcs);
        }

        private void SetTCListViewFilter()
        {
            var TCView = CollectionViewSource.GetDefaultView(TestCases);

            TCView.Filter = x =>
            {
                if (AllTCsListBox.SelectedItems.Contains(x))
                    return true;
                if (string.IsNullOrEmpty(TCFilter.Text))
                    return true;

                return x.ToString().StartsWith(TCFilterActual);
            };
        }

        private async void DatabaseService_RequirementsChanged(object sender, EventArgs e)
        {
            TCFilter.Text = string.Empty;

            TestCases.Clear();
            (await databaseService.GetTestCases())
                .Select(x => x.IDValue)
                .ToList()
                .ForEach(x => TestCases.Add(x));
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            AllTCsListBox.SelectedItems.Clear();
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void AllTCsListBox_LostFocus(object sender, MouseEventArgs e)
        {
            PushSelectedTCsUp();
        }

        private void TCFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            e.Handled = true;
            var inputs = TCFilter.Text
                .Select(x => char.IsNumber(x) ? x : ' ')
                .Aggregate("", (acc, x) => acc + x)
                .Trim()
                .Split(' ')
                .Where(x => x != string.Empty)
                .Select(x => int.Parse(x));
            

            List<int> tcs;

            if (AddTCsFromReqsCheckbox.IsChecked == false)
                tcs = inputs.ToList();
            else
            {
                var allReqs = databaseService.GetRequirements();
                var reqsTemp = inputs.Select(i => allReqs.Find(req => req.IDValue == i));

                tcs = databaseService.GetTestCasesFromReqList(reqsTemp)
                    .Select(tc=>tc.IDValue)
                    .ToList();

            }

            AddSelectedTCs(tcs);
            PushSelectedTCsUp();

            TCFilter.Text = "";
        }

        private void TCFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var delimeter = Regex.Match(TCFilter.Text.Reverse().Aggregate("", (acc, y) => acc + y), "[^0-9]");
            if (!delimeter.Success)
                TCFilterActual = TCFilter.Text;

            TCFilterActual = TCFilter.Text.Split(delimeter.Value.ToCharArray()).Last();

            CollectionViewSource.GetDefaultView(TestCases).Refresh();
        }

        private void PushSelectedTCsUp()
        {
            var selectedTcs = SelectedTCs
                .Reverse()
                .ToList();

            if (selectedTcs.Count == 1 && TestCases.IndexOf(selectedTcs[0]) == 0)
                return;

            var tempFilter = TCFilter.Text;
            TCFilter.Text = "";

            selectedTcs
                .ForEach(x => TestCases.Move(TestCases.IndexOf(x), 0));

            TCFilter.Text = tempFilter;
            AllTCsListBox.UpdateLayout();

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool isUserInteraction = false;
        private void AllTCsListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isUserInteraction = true;
        }

        private void AllTCsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isUserInteraction)
            {
                SelectionChanged?.Invoke(this, EventArgs.Empty);
                isUserInteraction = false;
            }
        }

        private void SelectTCForReqsButton_Click(object sender, RoutedEventArgs e)
        {
            SelectTCsForSelectedReqsButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
