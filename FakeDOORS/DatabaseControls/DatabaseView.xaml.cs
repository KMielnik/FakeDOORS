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

namespace FakeDOORS
{
    /// <summary>
    /// Interaction logic for DatabaseView.xaml
    /// </summary>
    public partial class DatabaseView : UserControl
    {
        private RequirementsView requirementsView;
        public DatabaseView()
        {
            InitializeComponent();

            requirementsView = new RequirementsView();
            requirementsView.Loaded += RequirementsView_Loaded;

            RequirementsViewControl.Content = requirementsView;
        }

        private void RequirementsView_Loaded(object sender, RoutedEventArgs e)
        {
            requirementsView.SetReqView();
        }
    }
}
