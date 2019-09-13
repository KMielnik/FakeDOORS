using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FakeDOORS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IDatabaseView databaseView;
        public MainWindow()
        {
            InitializeComponent();

            databaseView = App.ServiceProvider.GetRequiredService<IDatabaseView>();

            DatabaseViewControl.Content = databaseView;
        }
    }
}
