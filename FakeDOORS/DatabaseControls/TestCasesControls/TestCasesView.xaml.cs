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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FakeDOORS.DatabaseControls.TestCasesControls
{
    /// <summary>
    /// Interaction logic for TestCasesView.xaml
    /// </summary>
    public partial class TestCasesView : UserControl
    {
        public ObservableCollection<int> TestCases { get; set; } = new ObservableCollection<int>();

        private IDatabaseService databaseService;
        public TestCasesView(IDatabaseService databaseService)
        {
            InitializeComponent();

            this.databaseService = databaseService;

            databaseService.RequirementsChanged += DatabaseService_RequirementsChanged;

            SetTCListViewFilter();
        }

        public event EventHandler SelectionChanged;

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

                var delimeter = Regex.Match(TCFilter.Text, "[^0-9]");
                if (!delimeter.Success)
                    return x.ToString().StartsWith(TCFilter.Text);

                return x.ToString().StartsWith(TCFilter.Text.Split(delimeter.Value.ToCharArray()).Last());
            };
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
            var tcs = TCFilter.Text
                .Trim()
                .Split(' ')
                .Select(x => int.Parse(x))
                .ToList();

            AddSelectedTCs(tcs);
            PushSelectedTCsUp();

            TCFilter.Text = "";
        }

        private void TCFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
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
    }
}
