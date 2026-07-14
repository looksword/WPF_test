using System.Windows;
using System.Windows.Controls;

namespace WPF_test.Pages
{
    public partial class ProcessMonitorPage : UserControl
    {
        public ProcessMonitorPage()
        {
            InitializeComponent();
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