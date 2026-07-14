using System.Windows;
using System.Windows.Controls;
using WPF_test.ViewModels;

namespace WPF_test.Pages
{
    public partial class IOPage : UserControl
    {
        private IOPageViewModel _viewModel;

        public IOPage()
        {
            InitializeComponent();

            // 使用真实数据服务
            // _viewModel = new IOPageViewModel(new RealDataService());

            // 使用模拟数据服务（测试用）
            _viewModel = new IOPageViewModel(new MockDataService());

            this.DataContext = _viewModel;
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel?.Dispose();
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