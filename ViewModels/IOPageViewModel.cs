using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_test.Models;
using WPF_test.Helpers;

namespace WPF_test.ViewModels
{
    /// <summary>
    /// IO页面ViewModel - 负责UI交互和数据显示
    /// </summary>
    public class IOPageViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IOPageModel _model;
        private readonly IDataService? _dataService;
        private readonly SemaphoreSlim _dataLock = new SemaphoreSlim(1, 1);
        private CancellationTokenSource? _cts;
        private bool _isDisposed;

        public ObservableCollection<IOItem> IOItems => _model.IOItems;
        public string CurrentPage => _model.CurrentPage;
        public int CurrentPageIndex => _model.CurrentPageIndex;
        public bool IsUpdating => _model.IsUpdating;

        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public IOPageViewModel(IDataService? dataService = null)
        {
            _model = new IOPageModel();
            _dataService = dataService ?? new MockDataService();

            PrevPageCommand = new RelayCommand(PrevPage, () => !IsUpdating);
            NextPageCommand = new RelayCommand(NextPage, () => !IsUpdating);

            // 初始化第一页
            LoadPage(0);

            // 启动数据采集
            StartDataCollection();
        }

        /// <summary>
        /// 将十进制地址转换为八进制字符串
        /// </summary>
        private string ToOctalString(int decimalAddress, int digits = 3)
        {
            if (decimalAddress == 0) return "000";

            int octal = 0;
            int place = 1;
            int temp = decimalAddress;

            while (temp > 0)
            {
                int remainder = temp % 8;
                octal += remainder * place;
                place *= 10;
                temp /= 8;
            }

            return octal.ToString($"D{digits}");
        }

        /// <summary>
        /// 生成IO地址（8进制）
        /// </summary>
        private string GenerateAddress(string prefix, int decimalAddress)
        {
            string octalStr = ToOctalString(decimalAddress);
            return $"{prefix}{octalStr}";
        }

        /// <summary>
        /// 加载指定页面
        /// </summary>
        private async void LoadPage(int index)
        {
            if (index < 0 || index >= IOPageModel.PageNames.Length) return;

            try
            {
                await _dataLock.WaitAsync();

                _model.CurrentPageIndex = index;
                _model.CurrentPage = IOPageModel.PageNames[index];
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(CurrentPageIndex));

                var config = IOPageModel.PageConfigs[index];
                var items = new ObservableCollection<IOItem>();

                // 生成 64 个点（4列×16行），按 UniformGrid 的行优先填充顺序
                // 但地址按列连续分配：列0: 0~15, 列1:16~31, 列2:32~47, 列3:48~63
                for (int i = 0; i < 64; i++)
                {
                    int row = i / 4;          // 行索引 0~15
                    int col = i % 4;          // 列索引 0~3
                    int decimalAddress = config.StartAddress + col * 16 + row;
                    if (decimalAddress > config.EndAddress) break; // 容错

                    string address = GenerateAddress(config.Prefix, decimalAddress);
                    items.Add(new IOItem
                    {
                        Address = address,
                        Status = false,
                        Remark = string.Empty
                    });
                }

                var dispatcher = System.Windows.Application.Current?.Dispatcher;
                if (dispatcher != null)
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        _model.IOItems = items;
                        OnPropertyChanged(nameof(IOItems));
                    });
                }
            }
            finally
            {
                _dataLock.Release();
            }
        }

        /// <summary>
        /// 更新IO状态（由采集线程调用）
        /// </summary>
        public async Task UpdateIOStatusAsync(string address, bool status)
        {
            if (_isDisposed || string.IsNullOrEmpty(address)) return;

            try
            {
                await _dataLock.WaitAsync();

                // 在UI线程更新
                var dispatcher = System.Windows.Application.Current?.Dispatcher;
                if (dispatcher != null)
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        var item = _model.IOItems?.FirstOrDefault(x => x.Address == address);
                        if (item != null)
                        {
                            item.Status = status;
                        }
                    });
                }
            }
            finally
            {
                _dataLock.Release();
            }
        }

        /// <summary>
        /// 批量更新IO状态
        /// </summary>
        public async Task UpdateIOStatusBatchAsync(Dictionary<string, bool>? statusDict)
        {
            if (_isDisposed || statusDict == null || statusDict.Count == 0) return;

            try
            {
                await _dataLock.WaitAsync();

                var dispatcher = System.Windows.Application.Current?.Dispatcher;
                if (dispatcher != null)
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        foreach (var kvp in statusDict)
                        {
                            var item = _model.IOItems?.FirstOrDefault(x => x.Address == kvp.Key);
                            if (item != null)
                            {
                                item.Status = kvp.Value;
                            }
                        }
                    });
                }
            }
            finally
            {
                _dataLock.Release();
            }
        }

        private void PrevPage()
        {
            int newIndex = (_model.CurrentPageIndex - 1 + IOPageModel.PageNames.Length) % IOPageModel.PageNames.Length;
            LoadPage(newIndex);
        }

        private void NextPage()
        {
            int newIndex = (_model.CurrentPageIndex + 1) % IOPageModel.PageNames.Length;
            LoadPage(newIndex);
        }

        /// <summary>
        /// 启动数据采集
        /// </summary>
        private void StartDataCollection()
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => DataCollectionLoop(_cts.Token));
        }

        /// <summary>
        /// 数据采集循环（在后台线程运行）
        /// </summary>
        private async Task DataCollectionLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && !_isDisposed)
            {
                try
                {
                    if (_dataService != null)
                    {
                        // 模拟采集数据（实际使用时替换为真实采集）
                        var data = await _dataService.CollectDataAsync();

                        // 更新UI
                        await UpdateIOStatusBatchAsync(data);
                    }

                    // 采集间隔
                    await Task.Delay(100, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // 记录错误日志
                    System.Diagnostics.Debug.WriteLine($"数据采集错误: {ex.Message}");
                    await Task.Delay(1000, token);
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _cts?.Cancel();
            _cts?.Dispose();
            _dataLock?.Dispose();
        }
    }

    /// <summary>
    /// 数据服务接口
    /// </summary>
    public interface IDataService
    {
        Task<Dictionary<string, bool>> CollectDataAsync();
    }

    /// <summary>
    /// 模拟数据服务（用于测试）
    /// </summary>
    public class MockDataService : IDataService
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<string, bool> _lastData = new Dictionary<string, bool>();

        public Task<Dictionary<string, bool>> CollectDataAsync()
        {
            var data = new Dictionary<string, bool>();

            // 模拟所有IO地址的数据（X000-X077, X100-X177, Y000-Y077, Y100-Y177）
            var prefixes = new[] { "X", "Y" };
            var startAddresses = new[] { 0, 100 };

            foreach (var prefix in prefixes)
            {
                foreach (var start in startAddresses)
                {
                    int end = start + 77;
                    for (int addr = start; addr <= end; addr++)
                    {
                        // 只处理8进制有效的地址（不包含8和9）
                        string addrStr = addr.ToString();
                        if (!addrStr.Contains('8') && !addrStr.Contains('9'))
                        {
                            string octalAddr = ConvertToOctal(addr);
                            string fullAddr = $"{prefix}{octalAddr}";

                            if (_lastData.TryGetValue(fullAddr, out bool lastValue))
                            {
                                data[fullAddr] = _random.NextDouble() < 0.1 ? !lastValue : lastValue;
                            }
                            else
                            {
                                data[fullAddr] = _random.NextDouble() > 0.5;
                            }
                            _lastData[fullAddr] = data[fullAddr];
                        }
                    }
                }
            }

            return Task.FromResult(data);
        }

        private string ConvertToOctal(int decimalValue)
        {
            if (decimalValue == 0) return "000";

            int octal = 0;
            int place = 1;
            int temp = decimalValue;

            while (temp > 0)
            {
                int remainder = temp % 8;
                octal += remainder * place;
                place *= 10;
                temp /= 8;
            }

            return octal.ToString("D3");
        }
    }
}