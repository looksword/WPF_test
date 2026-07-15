using System.Windows;
using System.Windows.Controls;
using WPF_test.ViewModels;

namespace WPF_test.Pages
{
    public partial class FunctionSelectionPage : UserControl
    {
        public FunctionSelectionPage()
        {
            InitializeComponent();
            this.DataContext = new FunctionSelectionViewModel();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
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