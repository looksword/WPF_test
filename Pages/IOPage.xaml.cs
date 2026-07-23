using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using WPF_test.Services.Interfaces;
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
            //_viewModel = new IOPageViewModel(new MockDataService());

            // 从应用程序容器中获取 IIODataPollingService 实例
            if (Application.Current is App app && app.ServiceProvider != null)
            {
                var pollingService = app.ServiceProvider.GetRequiredService<IIODataPollingService>();
                _viewModel = new IOPageViewModel(pollingService);
            }
            else
            {
                // 后备方案：如果容器不可用，则直接构造（通常不会发生）
                // 注意：需要引用具体的实现，最好避免这种用法
                // _viewModel = new IOPageViewModel(new IODataPollingService(new ModbusTcpClient()));
                throw new System.InvalidOperationException("无法获取服务提供者");
            }

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