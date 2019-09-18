using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReqTools;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FakeDOORS.DatabaseControls.ChapterSelectionControls
{
    /// <summary>
    /// Interaction logic for ChapterSelectionWindow.xaml
    /// </summary>
    public partial class ChapterSelectionWindow : MetroWindow
    {
        public ObservableCollection<(string chapter, int id)> Chapters { get; set; }
        public (string chapter, int id) Answer { get; private set; }
        public bool SelectTCs { get; set; }
        public bool ClearPreviousTCs { get; set; }
        public ChapterSelectionWindow()
        {
            InitializeComponent();
            DataContext = this;

            Chapters = new ObservableCollection<(string chapter, int id)>();

            var database = App.ServiceProvider.GetRequiredService<IDatabaseService>();

            database.GetChapters()
                .ToList()
                .ForEach(x => Chapters.Add(x));

            var ChapterView = CollectionViewSource.GetDefaultView(Chapters);
            ChapterView.Filter = x =>
            {
                if (string.IsNullOrWhiteSpace(ChapterFilterTextBox.Text))
                    return true;

                return (x as (string chapter, int id)?).Value.chapter.StartsWith(ChapterFilterTextBox.Text);
            };

            SelectTCs = true;
            ClearPreviousTCs = true;
        }

        private void ReqsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ReqsListView.SelectedItem is null)
                return;

            Answer = (ValueTuple<string, int>)ReqsListView.SelectedItem;
            DialogResult = true;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (Chapters.Any(x => x.chapter == ChapterFilterTextBox.Text))
                ReqsListView.SelectedItem = Chapters.FirstOrDefault(x => x.chapter == ChapterFilterTextBox.Text);

            if (ReqsListView.SelectedItem == null && ReqsListView.Items.Count != 1)
            {
                MessageBox.Show("Please select chapter.");
                return;
            }

            Answer = (ValueTuple<string, int>)(ReqsListView.SelectedItem ?? ReqsListView.Items[0]);
            DialogResult = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(Chapters).Refresh();
            if (ReqsListView.Items.Count != 0)
                ReqsListView.ScrollIntoView(ReqsListView.Items[0]);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ChapterFilterTextBox.Focus();
        }
    }
}
