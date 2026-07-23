using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WPF_test.Pages;
using WPF_test.Services.Implementations;
using WPF_test.Services.Interfaces;
using WPF_test.ViewModels;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WPF_test
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            // 启动后台轮询服务（它会自动连接并开始读取）
            var pollingService = ServiceProvider.GetRequiredService<IIODataPollingService>();
            pollingService.Start();

            // 启动主窗口（通过 DI 解析，以便后续可以注入其他服务）
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // 加载配置文件
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
            services.AddSingleton<IConfiguration>(configuration);

            // 注册 Modbus 服务（单例，整个应用共享一个连接）
            services.AddSingleton<IModbusService, ModbusTcpClient>();
            services.AddTransient<IModbusClientFactory, ModbusClientFactory>();

            // 注册 IO 轮询服务（单例）
            services.AddSingleton<IIODataPollingService, IODataPollingService>();

            // 注册 ViewModel（每次请求新实例，适合页面级别）
            services.AddTransient<IOPageViewModel>();
            services.AddTransient<PLCToRobotViewModel>();

            // 注册主窗口（Transient 或 Singleton 均可，这里用 Transient）
            services.AddTransient<MainWindow>();

            // 如果有其他页面需要 DI，也在这里注册
            // services.AddTransient<OtherPage>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // 停止并释放轮询服务
            var pollingService = ServiceProvider?.GetService<IIODataPollingService>();
            pollingService?.Stop();
            (pollingService as IDisposable)?.Dispose();

            base.OnExit(e);
        }
    }
}