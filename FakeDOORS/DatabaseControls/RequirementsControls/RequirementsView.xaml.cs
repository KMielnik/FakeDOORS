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

namespace FakeDOORS
{
    /// <summary>
    /// Interaction logic for RequirementsView.xaml
    /// </summary>
    public partial class RequirementsView : UserControl
    {
        public ObservableCollection<Requirement> Requirements { get; set; } = new ObservableCollection<Requirement>();
        private IDatabaseService databaseService;
        public RequirementsView()
        {
            InitializeComponent();

            databaseService = App.ServiceProvider.GetService<IDatabaseService>();
            databaseService.RequirementsChanged += DatabaseService_RequirementsChanged;
        }

        public void SetReqView()
        {
            ReqDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding(nameof(Requirement.ID)),
                IsReadOnly = true
            });

            ReqDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Text",
                Binding = new Binding(nameof(Requirement.TextIntended)),
                IsReadOnly = true,
                Width = 500
            });

            ReqDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Functional Variants",
                Binding = new Binding(nameof(Requirement.FVariants)),
                IsReadOnly = true,
                Width = 200
            });
        }

        private async Task RefreshRequirements()
        {
            Requirements.Clear();

            (await databaseService.GetRequirements())
                .ForEach(Requirements.Add);
        }

        private async void DatabaseService_RequirementsChanged(object sender, EventArgs e)
        {
            await RefreshRequirements();
        }

        private async void RequirementsView_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshRequirements();
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
