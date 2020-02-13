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

namespace FakeDOORS.DatabaseControls.ScrollToRequirementControls
{
    /// <summary>
    /// Interaction logic for ScrollToRequirementView.xaml
    /// </summary>
    public partial class ScrollToRequirementView : UserControl, IScrollToRequirementView
    {
        public ScrollToRequirementView()
        {
            InitializeComponent();
        }

        public event EventHandler<ReqScrollEventArgsr> ReqScrollRequested;

        private void ReqTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                ReqScrollRequested?.Invoke(this, new ReqScrollEventArgsr(ReqTextBox.Text));
        }

        private void ScrollButton_Click(object sender, RoutedEventArgs e)
        {
            ReqScrollRequested?.Invoke(this, new ReqScrollEventArgsr(ReqTextBox.Text));
        }
    }
}
