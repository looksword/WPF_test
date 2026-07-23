using System.Windows;
using System.Windows.Controls;
using WPF_test.ViewModels;

namespace WPF_test.Pages
{
    public partial class PLCToRobotPage : UserControl
    {
        public PLCToRobotPage(PLCToRobotViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            OnCloseRequested();
        }

        public event RoutedEventHandler CloseRequested;

        protected virtual void OnCloseRequested()
        {
            CloseRequested?.Invoke(this, new RoutedEventArgs());
        }
    }
}