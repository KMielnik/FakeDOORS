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
using System.Threading.Tasks;
using FakeDOORS.DatabaseControls.RequirementsControls;
using System.Linq;

namespace FakeDOORS
{
    /// <summary>
    /// Interaction logic for RequirementsView.xaml
    /// </summary>
    public partial class RequirementsView : UserControl
    {
        public ObservableCollection<Requirement> Requirements { get; set; } = new ObservableCollection<Requirement>();
        private HashSet<TestCase> SelectedTestCases = new HashSet<TestCase>();
        private IDatabaseService databaseService;

        private int actualBrush = 0;
        private Brush[] brushes = {
            GetBrushFromHex("#e6194B"),
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

        public RequirementsView()
        {
            InitializeComponent();

            databaseService = App.ServiceProvider.GetService<IDatabaseService>();
            databaseService.RequirementsChanged += DatabaseService_RequirementsChanged;
        }

        public void SetReqView(ReqViewSettings settings)
        {
            ReqDataGrid.RowStyle = new Style();
            ReqDataGrid.Columns.Clear();

            foreach (var setting in settings)
                switch (setting)
                {
                    case ReqViewSettings.SettingTypes.IDColumn:
                        {
                            ReqDataGrid.Columns.Add(new DataGridTextColumn
                            {
                                Header = "ID",
                                Binding = new Binding(nameof(Requirement.ID)),
                                IsReadOnly = true
                            });
                            break;
                        }
                    case ReqViewSettings.SettingTypes.TextColumn:
                        {
                            ReqDataGrid.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Text",
                                Binding = new Binding(nameof(Requirement.TextIntended)),
                                IsReadOnly = true,
                                Width = 500
                            });
                            break;
                        }
                    case ReqViewSettings.SettingTypes.FVariant:
                        {
                            ReqDataGrid.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Functional Variants",
                                Binding = new Binding(nameof(Requirement.FVariants)),
                                IsReadOnly = true,
                                Width = 200
                            });
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
        }

        public async Task AddTestCasesToView(List<TestCase> testCases)
        {
            var newTestCases = testCases
                .Where(x => !SelectedTestCases.Contains(x))
                .Select(x => x.IDValue);

            foreach(var tc in newTestCases)
            {
                var TCColumn = GenerateTCColumn(tc);

                await Task.Run(async () =>
                    await ReqDataGrid.Dispatcher.BeginInvoke((Action)(() => ReqDataGrid.Columns.Add(TCColumn))));
            }
        }

        private DataGridTextColumn GenerateTCColumn(int tc)
        {
            DataGridTextColumn TCColumn = new DataGridTextColumn
            {
                Header = tc,
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
                Value = brushes[actualBrush % brushes.Length]
            });

            dataTrigger.Setters.Add(new Setter()
            {
                Property = ForegroundProperty,
                Value = brushes[actualBrush % brushes.Length]
            });

            dataTrigger.Setters.Add(new Setter()
            {
                Property = BorderBrushProperty,
                Value = brushes[actualBrush++ % brushes.Length]
            });

            TCColumn.CellStyle = new Style();
            TCColumn.CellStyle.Triggers.Add(dataTrigger);

            TCColumn.HeaderStyle = new Style();

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
                Value = SelectedTestCases.First(x => x.IDValue == tc).Text
            });

            return TCColumn;
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
        }

        private void RequirementsView_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshRequirements();
        }

        private void ReqDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void ReqDataGrid_LayoutUpdated(object sender, EventArgs e)
        {

        }

        private void ReqDataGrid_ColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
        {

        }

        private void Helper_GotFocus(object sender, RoutedEventArgs e)
        {

        }

    }
}
