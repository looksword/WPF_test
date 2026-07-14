using System.Windows;
using System.Windows.Controls;

namespace WPF_test.Pages
{
    public partial class PLCToCNCPage : UserControl
    {
        public PLCToCNCPage()
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