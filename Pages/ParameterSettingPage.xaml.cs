using System.Windows;
using System.Windows.Controls;

namespace WPF_test.Pages
{
    public partial class ParameterSettingPage : UserControl
    {
        public ParameterSettingPage()
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