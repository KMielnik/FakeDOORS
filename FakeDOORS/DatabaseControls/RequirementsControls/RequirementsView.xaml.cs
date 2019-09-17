using FakeDOORS.DatabaseControls.RequirementsControls;
using FakeDOORS.DatabaseControls.RequirementsControls.Converters;
using ReqTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace FakeDOORS
{
    /// <summary>
    /// Interaction logic for RequirementsView.xaml
    /// </summary>
    public partial class RequirementsView : UserControl, IRequirementsView
    {
        public ObservableCollection<Requirement> Requirements { get; set; } = new ObservableCollection<Requirement>();
        private HashSet<int> SelectedTestCases = new HashSet<int>();

        public ObservableCollection<Dictionary<int, int>> ReqTopHelperData { get; set; } = new ObservableCollection<Dictionary<int, int>>
                { new Dictionary<int, int>()};
        public ObservableCollection<Dictionary<int, int>> ReqBottomHelperData { get; set; } = new ObservableCollection<Dictionary<int, int>>
                { new Dictionary<int, int>()};

        private IDatabaseService databaseService;

        private int actualBrush = 0;
        private Brush[] brushes = {
            GetBrushFromHex("#3cb44b"),
            GetBrushFromHex("#ffe119"),
            GetBrushFromHex("#4363d8"),
            GetBrushFromHex("#f58231"),
            GetBrushFromHex("#911eb4"),
            GetBrushFromHex("#42d4f4"),
            GetBrushFromHex("#f032e6"),
            GetBrushFromHex("#bfef45"),
            GetBrushFromHex("#fabebe"),
            GetBrushFromHex("#469990"),
            GetBrushFromHex("#e6beff"),
            GetBrushFromHex("#9A6324")
        };
        private static SolidColorBrush GetBrushFromHex(string hex)
            => (SolidColorBrush)new BrushConverter().ConvertFrom(hex);

        public RequirementsView(IDatabaseService databaseService)
        {
            InitializeComponent();

            this.databaseService = databaseService;
            databaseService.RequirementsChanged += DatabaseService_RequirementsChanged;
        }

        private void AddNormalColumn(string header, int width, Binding binding)
        {
            ReqHelperTop.Columns.Add(new DataGridTextColumn
            {
                Header = header,
                Width = width,
                IsReadOnly = true,
                DisplayIndex = 0
            });
            ReqHelperBottom.Columns.Add(new DataGridTextColumn
            {
                Header = header,
                Width = width,
                IsReadOnly = true,
                DisplayIndex = 0
            });
            ReqDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = header,
                Binding = binding,
                IsReadOnly = true,
                Width = width,
                DisplayIndex = 0
            });
        }

        public void SetReqView(ReqViewSettings settings)
        {
            ReqDataGrid.RowStyle = new Style();
            ReqDataGrid.Columns.Clear();
            ReqHelperTop.Columns.Clear();
            ReqHelperBottom.Columns.Clear();

            foreach (var setting in settings)
                switch (setting)
                {
                    case ReqViewSettings.SettingTypes.IDColumn:
                        {
                            AddNormalColumn("ID", 100, new Binding(nameof(Requirement.ID)));
                            break;
                        }
                    case ReqViewSettings.SettingTypes.TextColumn:
                        {
                            AddNormalColumn("Text", 500, new Binding(nameof(Requirement.TextIntended)));
                            break;
                        }
                    case ReqViewSettings.SettingTypes.FVariant:
                        {
                            AddNormalColumn("Functional Variants", 200, new Binding(nameof(Requirement.FVariants)));
                            break;
                        }

                    case ReqViewSettings.SettingTypes.BoldHeaders:
                        {
                            var dataTrigger = new DataTrigger()
                            {
                                Binding = new Binding(nameof(Requirement.Type)),
                                Value = Requirement.Types.Head
                            };

                            dataTrigger.Setters.Add(new Setter()
                            {
                                Property = FontWeightProperty,
                                Value = FontWeights.UltraBold
                            });

                            ReqDataGrid.RowStyle.Triggers.Add(dataTrigger);
                            break;
                        }
                }

            #region always_on_filters
            var visibilityTrigger = new DataTrigger()
            {
                Binding = new Binding(nameof(Requirement.IsVisible)),
                Value = false
            };

            visibilityTrigger.Setters.Add(new Setter()
            {
                Property = VisibilityProperty,
                Value = Visibility.Hidden
            });

            ReqDataGrid.RowStyle.Triggers.Add(visibilityTrigger);
            #endregion
        }

        public async Task SetSelectedTestCases(List<int> testCases)
        {
            var newTestCases = testCases
                .Except(SelectedTestCases)
                .ToList();

            var deletedTestCases = SelectedTestCases
                .Except(testCases)
                .ToList();

            var deletingTasks = new List<Task>();
            foreach (int TCSelected in deletedTestCases)
            {
                SelectedTestCases.Remove(TCSelected);
                deletingTasks.Add(DeleteTCColumn(TCSelected));
            }

            var addingTasks = new List<Task>();
            foreach (var tc in newTestCases)
            {
                SelectedTestCases.Add(tc);
                addingTasks.Add(AddTCColumn(tc));
            }

            await Task.WhenAll(deletingTasks);
            await Task.WhenAll(addingTasks);

            await RefreshHelpers();
            await Task.Delay(500);
            FixHelpersOrder();
        }

        private void FixHelpersOrder()
        {
            var firstTCColumnIndex = ReqDataGrid.Columns
                .TakeWhile(x => !(x.Header is int))
                .Count();

            for (int i = firstTCColumnIndex; i < ReqDataGrid.Columns.Count; i++)
            {
                var columnTCValue = (int)ReqDataGrid.Columns[i].Header;
                var topColumn = ReqHelperTop.Columns
                    ?.FirstOrDefault(x => x.Header is int && (int)x.Header == columnTCValue);
                var bottomColumn = ReqHelperBottom.Columns
                    ?.FirstOrDefault(x => x.Header is int && (int)x.Header == columnTCValue);

                if (topColumn != null && bottomColumn != null)
                {
                    topColumn.DisplayIndex = ReqDataGrid.Columns[i].DisplayIndex;
                    bottomColumn.DisplayIndex = ReqDataGrid.Columns[i].DisplayIndex;
                }
            }
        }

        private async Task DeleteTCColumn(int tc)
        {
            var removedReqColumn = ReqDataGrid.Columns
                       .FirstOrDefault(x => x.Header is int && (int)x.Header == tc);
            var removedTopColumn = ReqHelperTop.Columns
                       .FirstOrDefault(x => x.Header is int && (int)x.Header == tc);
            var removedBottomColumn = ReqHelperBottom.Columns
                       .FirstOrDefault(x => x.Header is int && (int)x.Header == tc);

            ReqTopHelperData[0].Remove(tc);
            ReqBottomHelperData[0].Remove(tc);


            await Task.Run(async () =>
            {
                if (removedReqColumn != null)
                    await ReqDataGrid.Dispatcher.BeginInvoke((Action)(() => ReqDataGrid.Columns.Remove(removedReqColumn)));

                if (removedTopColumn != null)
                    await ReqHelperTop.Dispatcher.BeginInvoke((Action)(() => ReqHelperTop.Columns.Remove(removedTopColumn)));

                if (removedBottomColumn != null)
                    await ReqHelperBottom.Dispatcher.BeginInvoke((Action)(() => ReqHelperBottom.Columns.Remove(removedBottomColumn)));
            });
        }

        private DataGridTextColumn GenerateTCColumn(int tc, Brush color, int width)
        {
            DataGridTextColumn TCColumn = new DataGridTextColumn
            {
                Header = tc,
                Width = width,
                IsReadOnly = true
            };

            var dataTrigger = new DataTrigger()
            {
                Binding = new Binding(nameof(Requirement.TCIDsValue)) { Converter = new TestCaseConverter(), ConverterParameter = tc },
                Value = "True"
            };

            dataTrigger.Setters.Add(new Setter()
            {
                Property = BackgroundProperty,
                Value = color
            });

            dataTrigger.Setters.Add(new Setter()
            {
                Property = ForegroundProperty,
                Value = color
            });

            dataTrigger.Setters.Add(new Setter()
            {
                Property = BorderBrushProperty,
                Value = color
            });

            TCColumn.CellStyle = new Style();
            TCColumn.CellStyle.Triggers.Add(dataTrigger);

            TCColumn.HeaderStyle = new Style();

            TCColumn.HeaderStyle.Setters.Add(new Setter()
            {
                Property = HorizontalAlignmentProperty,
                Value = HorizontalAlignment.Stretch
            });

            TCColumn.HeaderStyle.Setters.Add(new EventSetter()
            {
                Event = MouseDoubleClickEvent,
                Handler = new MouseButtonEventHandler(ColumnHeader_DoubleClickEvent)
            });

            TCColumn.HeaderStyle.Setters.Add(new EventSetter()
            {
                Event = MouseRightButtonDownEvent,
                Handler = new MouseButtonEventHandler(ColumnHeader_RightClickEvent)
            });

            TCColumn.HeaderStyle.Setters.Add(new Setter()
            {
                Property = ToolTipProperty,
                Value = databaseService.GetTestCaseText(tc)
            });

            return TCColumn;
        }
        private DataGridTextColumn GenerateTCHelperColumn(int tc, Brush color, int width)
        {
            var helperColumn = new DataGridTextColumn
            {
                Header = tc,
                Binding = new Binding($"[{tc}]"),
                Width = width,
                IsReadOnly = true
            };

            var dataTrigger = new DataTrigger()
            {
                Binding = new Binding("[" + tc + "]") { Converter = new GreaterThanConverter() },
                Value = true
            };
            dataTrigger.Setters.Add(new Setter()
            {
                Property = BorderBrushProperty,
                Value = Brushes.Black
            });
            dataTrigger.Setters.Add(new Setter()
            {
                Property = BackgroundProperty,
                Value = color
            });

            var eventTrigger = new EventSetter()
            {
                Event = PreviewMouseLeftButtonDownEvent,
                Handler = new MouseButtonEventHandler(Helper_LeftClickEvent)
            };

            helperColumn.CellStyle = new Style();
            helperColumn.CellStyle.Triggers.Add(dataTrigger);
            helperColumn.CellStyle.Setters.Add(eventTrigger);

            return helperColumn;
        }

        private void Helper_LeftClickEvent(object sender, MouseButtonEventArgs e)
        {
            var gridCell = (DataGridCell)sender;
            var tc = (int)gridCell.Column.Header;

            var senderDatagrid = VisualTreeHelper.GetParent(gridCell);
            while (senderDatagrid != null && senderDatagrid.GetType() != typeof(DataGrid))
                senderDatagrid = VisualTreeHelper.GetParent(senderDatagrid);

            var isGoingDown = ((DataGrid)senderDatagrid).Name == nameof(ReqHelperBottom);

            var selectedRows = ReqDataGrid.SelectedItems.Count;

            if (selectedRows >= 1)
            {
                var firstSelected = (Requirement)ReqDataGrid.SelectedItems[0];
                ReqDataGrid.SelectedItems.Clear();

                if (firstSelected.TCIDsValue.Contains(tc))
                    ReqDataGrid.SelectedItems.Add(firstSelected);
                else
                {
                    ReqDataGrid.SelectedItems.Add(ReqDataGrid.Items.Cast<Requirement>().First(x => x.TCIDsValue.Contains(tc) && x.IsVisible));
                    ReqDataGrid.ScrollIntoView(firstSelected);
                    return;
                }

                ReqDataGrid.ScrollIntoView(firstSelected);

                if (selectedRows > 1)
                    return;


                ReqDataGrid.SelectedItems.Clear();
                var reqs = ReqDataGrid.Items.Cast<Requirement>().AsEnumerable();
                if (isGoingDown == false)
                    reqs = reqs.Reverse();

                ReqDataGrid.SelectedItems
                    .Add(reqs
                        .SkipWhile(x => x != firstSelected)
                        .Skip(1)
                        .FirstOrDefault(x => x.TCIDsValue.Contains(tc) && x.IsVisible));

                if (ReqDataGrid.SelectedItem == null)
                    ReqDataGrid.SelectedItems.Add(firstSelected);

                ReqDataGrid.ScrollIntoView(ReqDataGrid.SelectedItem);
            }
            else
            {
                ReqDataGrid.SelectedItems.Add(ReqDataGrid.Items.Cast<Requirement>().First(x => x.TCIDsValue.Contains(tc) && x.IsVisible));
                ReqDataGrid.ScrollIntoView((Requirement)ReqDataGrid.SelectedItems[0]);
            }
        }

        private async Task AddTCColumn(int tc)
        {
            var color = brushes[actualBrush++ % brushes.Length];

            const int width = 45;

            var TCColumn = GenerateTCColumn(tc, color, width);
            var topHelperColumn = GenerateTCHelperColumn(tc, color, width);
            var bottomHelperColumn = GenerateTCHelperColumn(tc, color, width);

            ReqTopHelperData[0].Add(tc, 0);
            ReqBottomHelperData[0].Add(tc, 0);

            await Task.Run(async () =>
                    await ReqDataGrid.Dispatcher.BeginInvoke((Action)(() => ReqDataGrid.Columns.Add(TCColumn))))
                .ContinueWith(async s => await Task.Run(async () =>
                      await ReqHelperTop.Dispatcher.BeginInvoke((Action)(() => ReqHelperTop.Columns.Add(topHelperColumn)))))
                .ContinueWith(async s => await Task.Run(async () =>
                      await ReqHelperBottom.Dispatcher.BeginInvoke((Action)(() => ReqHelperBottom.Columns.Add(bottomHelperColumn)))));
        }

        private void ColumnHeader_RightClickEvent(object sender, MouseButtonEventArgs e)
        {
            var TC = (int)(sender as System.Windows.Controls.Primitives.DataGridColumnHeader).Content;
            ReqDataGrid.SelectedItems.Clear();
            ReqDataGrid.Items.Cast<Requirement>()
                .Where(x => x.TCIDsValue.Contains(TC))
                .ToList()
                .ForEach(x => ReqDataGrid.SelectedItems.Add(x));
            ReqDataGrid.Focus();
        }

        private void ColumnHeader_DoubleClickEvent(object sender, MouseButtonEventArgs e)
        {
            var header = (System.Windows.Controls.Primitives.DataGridColumnHeader)sender;

            var tcID = (int)header.Content;

            var firstOccurence = ReqDataGrid.Items.Cast<Requirement>()
                    .TakeWhile(x => x.TCIDsValue.Contains(tcID) != true)
                    .Count();

            var lastOccurence = ReqDataGrid.Items.Cast<Requirement>().Count() - ReqDataGrid.Items.Cast<Requirement>()
                   .Reverse()
                   .TakeWhile(x => x.TCIDsValue.Contains(tcID) != true)
                   .Count();

            ReqDataGrid.ScrollIntoView(ReqDataGrid.Items[lastOccurence]);

            ReqDataGrid.ScrollIntoView(ReqDataGrid.Items[firstOccurence]);
        }

        private void RefreshRequirements()
        {
            Requirements.Clear();

            databaseService.GetRequirements()
                .ForEach(Requirements.Add);
        }

        private void DatabaseService_RequirementsChanged(object sender, EventArgs e)
        {
            RefreshRequirements();
            GetScrollViewer(ReqDataGrid)
               .ScrollToVerticalOffset(ReqDataGrid.Items.IndexOf(ReqDataGrid.Items
                   .Cast<Requirement>()
                   .Skip(1)
                   .FirstOrDefault(x => x.Type == Requirement.Types.Head)));
        }

        private void RequirementsView_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshRequirements();
        }

        public static ScrollViewer GetScrollViewer(UIElement element)
        {
            if (element == null) return null;

            ScrollViewer retour = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element) && retour == null; i++)
                if (VisualTreeHelper.GetChild(element, i) is ScrollViewer)
                    retour = (ScrollViewer)(VisualTreeHelper.GetChild(element, i));
                else
                    retour = GetScrollViewer(VisualTreeHelper.GetChild(element, i) as UIElement);

            return retour;
        }

        private async Task RefreshHelpers()
        {
            await RefreshHelpers(true, true,
                GetScrollViewer(ReqDataGrid).HorizontalOffset,
                GetScrollViewer(ReqDataGrid).VerticalOffset);
        }

        private async Task RefreshHelpers(bool horizontalRefresh, bool verticalRefresh, double horizontalOffset, double verticalOffset, double verticalChange = 0)
        {
            if (horizontalRefresh)
            {
                new List<ScrollViewer>()
                {
                    GetScrollViewer(ReqHelperTop),
                    GetScrollViewer(ReqHelperBottom)
                }.ForEach(x => x.ScrollToHorizontalOffset(horizontalOffset));
            }

            if (verticalRefresh)
            {
                var datagridHeight = ReqDataGrid.ActualHeight;
                var rowHeight = ReqDataGrid.RowHeight;
                var firstRow = (int)verticalOffset;
                var lastRow = firstRow + (int)(datagridHeight / rowHeight);

                if (Requirements.AsParallel().Any(x => x.IsVisible == false))
                {
                    var firstVisible = ReqDataGrid.Items
                        .IndexOf(ReqDataGrid.Items.Cast<Requirement>()
                        .First(x => x.IsVisible == true));
                    var lastVisible = ReqDataGrid.Items
                        .IndexOf(ReqDataGrid.Items.Cast<Requirement>()
                        .Last(x => x.IsVisible == true));

                    if (lastVisible < lastRow)
                        lastRow = lastVisible + 1;

                    if (verticalChange < 0 && firstRow < firstVisible)
                    {
                        var scrollViewer = GetScrollViewer(ReqDataGrid);
                        scrollViewer.ScrollToVerticalOffset(firstVisible);
                    }
                    else if (verticalChange > 0 && lastVisible < firstRow)
                    {
                        var scrollViewer = GetScrollViewer(ReqDataGrid);
                        scrollViewer.ScrollToVerticalOffset(lastVisible);
                    }
                }

                var selectedTCs = new List<int>();
                foreach (int TC in SelectedTestCases)
                    selectedTCs.Add(TC);


                Parallel.ForEach(selectedTCs, TCSelected =>
                {
                    if (ReqTopHelperData[0].ContainsKey(TCSelected))
                    {
                        ReqTopHelperData[0][TCSelected] = ReqDataGrid.Items.Cast<Requirement>()
                            .Take(firstRow)
                            .Where(x => x.TCIDsValue.Contains(TCSelected))
                            .Count();
                    }

                    if (ReqBottomHelperData[0].ContainsKey(TCSelected))
                    {
                        ReqBottomHelperData[0][TCSelected] = ReqDataGrid.Items.Cast<Requirement>()
                            .Skip(lastRow)
                            .Where(x => x.TCIDsValue.Contains(TCSelected))
                            .Count();
                    }
                });

            }
            ReqDataGrid.UpdateLayout();
            ReqHelperTop.Items.Refresh();
            ReqHelperBottom.Items.Refresh();
        }

        private async void ReqDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            await RefreshHelpers(e.HorizontalChange != 0, e.VerticalChange != 0, e.HorizontalOffset, e.VerticalOffset, e.VerticalChange);
        }

        private void ReqDataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            for (int i = 0; i < ReqDataGrid.Columns.Count; i++)
            {
                if (i < ReqHelperTop.Columns.Count)
                    ReqHelperTop.Columns[i].Width = ReqDataGrid.Columns[i].Width;
                if(i< ReqHelperBottom.Columns.Count)
                    ReqHelperBottom.Columns[i].Width = ReqDataGrid.Columns[i].Width;
            }
        }

        private void ReqDataGrid_ColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
        {
            var topHelper = ReqHelperTop.Columns
                .FirstOrDefault(x => x.Header.ToString() == e.Column.Header.ToString());

            if (topHelper != null && e.Column.DisplayIndex < ReqHelperTop.Columns.Count)
                topHelper.DisplayIndex = e.Column.DisplayIndex;

            var bottomHelper = ReqHelperBottom.Columns
                .FirstOrDefault(x => x.Header.ToString() == e.Column.Header.ToString());

            if (bottomHelper != null && e.Column.DisplayIndex < ReqHelperBottom.Columns.Count)
                bottomHelper.DisplayIndex = e.Column.DisplayIndex;
        }

        private void Helper_GotFocus(object sender, RoutedEventArgs e)
        {
            var helper = (DataGrid)sender;
            helper.SelectedItems.Clear();

            ReqDataGrid.Focus();
        }

        public void LimitScrollingToOneChapter(int chapter)
        {
            foreach (var req in Requirements)
                req.IsVisible = true;

            if (chapter == 0)
            {
                ReqDataGrid.Items.Refresh();
                return;
            }

            var visibleReqs = databaseService.GetRequirementsFromChapter(chapter);
            Requirements
                .Except(visibleReqs)
                .ToList()
                .ForEach(x => x.IsVisible = false);

            var scrollViewer = GetScrollViewer(ReqDataGrid);
            scrollViewer.ScrollToVerticalOffset(ReqDataGrid.Items.IndexOf(visibleReqs.FirstOrDefault()));

            ReqDataGrid.Items.Refresh();
        }

        public IEnumerable<Requirement> GetSelectedRequirements()
        => ReqDataGrid.SelectedItems.Cast<Requirement>();
    }
}
