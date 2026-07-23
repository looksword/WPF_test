using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WPF_test.Pages;
using WPF_test.Services.Implementations;
using WPF_test.Services.Interfaces;
using WPF_test.ViewModels;

namespace WPF_test
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            // 启动主窗口（通过 DI 解析，以便后续可以注入其他服务）
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // 注册 Modbus 服务（单例，整个应用共享一个连接）
            services.AddSingleton<IModbusService, ModbusTcpClient>();

            // 注册 ViewModel（每次请求新实例，适合页面级别）
            services.AddTransient<PLCToRobotViewModel>();

            // 注册主窗口（Transient 或 Singleton 均可，这里用 Transient）
            services.AddTransient<MainWindow>();

            // 如果有其他页面需要 DI，也在这里注册
            // services.AddTransient<OtherPage>();
        }
    }
}