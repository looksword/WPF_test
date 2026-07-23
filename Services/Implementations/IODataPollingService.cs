// Services/Implementations/IODataPollingService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WPF_test.Models;
using WPF_test.Services.Interfaces;

namespace WPF_test.Services.Implementations
{
    public class IODataPollingService : IIODataPollingService
    {
        private readonly IModbusService _modbusService;
        private readonly ModbusDeviceConfig _config;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private CancellationTokenSource? _cts;
        private bool _isRunning;
        private bool _disposed;

        private readonly List<string> _ioAddresses = new List<string>();

        public event EventHandler<Dictionary<string, bool>>? IODataUpdated;

        /// <summary>
        /// 构造函数：注入工厂和配置，指定使用哪个设备（通过设备名称）
        /// </summary>
        public IODataPollingService(IModbusClientFactory factory, IConfiguration configuration)
        {
            // 从配置中读取 "MainPLC" 设备配置
            var devices = configuration.GetSection("Modbus:Devices").Get<List<ModbusDeviceConfig>>();
            var deviceConfig = devices?.FirstOrDefault(d => d.Name == "MainPLC");
            if (deviceConfig == null)
                throw new Exception("未找到 MainPLC 的配置");

            _config = deviceConfig;

            // 通过工厂创建独立的 Modbus 客户端
            _modbusService = factory.CreateClient();

            BuildAddressList();
        }

        private void BuildAddressList()
        {
            // 生成 IO 地址列表（同之前）
            var prefixes = new[] { "X", "Y" };
            for (int addr = 0; addr <= 127; addr++)
            {
                string octal = ConvertToOctal(addr);
                _ioAddresses.Add($"X{octal}");
                _ioAddresses.Add($"Y{octal}");
            }
        }

        private string ConvertToOctal(int decimalValue)
        {
            if (decimalValue == 0) return "000";
            int octal = 0, place = 1, temp = decimalValue;
            while (temp > 0)
            {
                int rem = temp % 8;
                octal += rem * place;
                place *= 10;
                temp /= 8;
            }
            return octal.ToString("D3");
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => PollingLoop(_cts.Token));
        }

        public void Stop()
        {
            if (!_isRunning) return;
            _isRunning = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async Task PollingLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && !_disposed)
            {
                try
                {
                    // 确保连接
                    if (!_modbusService.IsConnected)
                    {
                        await TryReconnect(token);
                        continue;
                    }

                    // 读取数据（示例）
                    var data = new Dictionary<string, bool>();
                    ushort[] registers = await _modbusService.ReadHoldingRegistersAsync(_config.SlaveId, 0, 8);
                    for (int i = 0; i < registers.Length; i++)
                    {
                        ushort reg = registers[i];
                        for (int bit = 0; bit < 16; bit++)
                        {
                            string address = $"{(i * 16 + bit)}";
                            bool status = (reg & (1 << bit)) != 0;
                            data[address] = status;
                        }
                    }

                    OnIODataUpdated(data);

                    await Task.Delay(_config.PollIntervalMs, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"IO轮询异常: {ex.Message}");
                    await Task.Delay(_config.ReconnectIntervalMs, token);
                }
            }
        }

        private async Task TryReconnect(CancellationToken token)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"尝试连接 {_config.Ip}:{_config.Port}");
                await _modbusService.ConnectAsync(_config.Ip, _config.Port);
                System.Diagnostics.Debug.WriteLine("连接成功");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"重连失败: {ex.Message}");
                await Task.Delay(_config.ReconnectIntervalMs, token);
            }
        }

        private void OnIODataUpdated(Dictionary<string, bool> data)
        {
            var handler = IODataUpdated;
            if (handler != null)
            {
                Task.Run(() => handler.Invoke(this, data));
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Stop();
            _modbusService?.Dispose();
            _lock.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}