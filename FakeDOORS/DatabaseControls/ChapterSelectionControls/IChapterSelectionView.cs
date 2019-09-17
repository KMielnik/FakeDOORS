using System;

namespace FakeDOORS.DatabaseControls.ChapterSelectionControls
{
    public interface IChapterSelectionView
    {
        bool ClearAllTCs { get; }
        bool SelectChaptersTCs { get; }
        (string chapter, int id) SelectedChapter { get; }

        event EventHandler SelectionChanged;
        void ResetView();

        void ClearChapterSelection();
    }
}