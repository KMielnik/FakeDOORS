using System;
using System.Windows;
using System.Windows.Controls;

namespace FakeDOORS.DatabaseControls.ChapterSelectionControls
{
    /// <summary>
    /// Interaction logic for ChapterSelectionView.xaml
    /// </summary>
    public partial class ChapterSelectionView : UserControl, IChapterSelectionView
    {
        public ChapterSelectionView()
        {
            InitializeComponent();
        }

        public event EventHandler SelectionChanged;
        public (string chapter, int id) SelectedChapter
        {
            get => selectedChapter;
            private set
            {
                selectedChapter = value;
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public bool ClearAllTCs { get; private set; }
        public bool SelectChaptersTCs { get; private set; }

        private (string chapter, int id) selectedChapter = ("-", 0);
        public void ClearChapterSelection()
        {
            SelectedChapter = ("-", 0);
            ChapterNameTextBlock.Text = "-";
        }

        private void ChapterSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var chapterSelectionWindow = new ChapterSelectionWindow();
            chapterSelectionWindow.ShowDialog();
            if (chapterSelectionWindow.DialogResult == true)
            {
                ClearAllTCs = chapterSelectionWindow.ClearPreviousTCs;
                SelectChaptersTCs = chapterSelectionWindow.SelectTCs;

                SelectedChapter = chapterSelectionWindow.Answer;

                ChapterNameTextBlock.Text = SelectedChapter.chapter;
            }
        }

        private void ChapterClear_Click(object sender, RoutedEventArgs e)
        {
            ClearChapterSelection();
        }
    }
}
