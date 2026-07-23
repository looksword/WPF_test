using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WPF_test.Helpers;
using WPF_test.Models;
using WPF_test.Services.Interfaces;

namespace WPF_test.ViewModels
{
    /// <summary>
    /// IO页面ViewModel - 负责UI交互和数据显示
    /// 不再自行启动后台线程，改为订阅 IIODataPollingService 的数据更新事件
    /// </summary>
    public class IOPageViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IIODataPollingService _pollingService;
        private readonly IOPageModel _model;
        private bool _isSubscribed;

        // 暴露给View的集合和属性
        public ObservableCollection<IOItem> IOItems => _model.IOItems;
        public string CurrentPage => _model.CurrentPage;
        public int CurrentPageIndex => _model.CurrentPageIndex;
        public bool IsUpdating => _model.IsUpdating;

        // 翻页命令
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 构造函数，注入轮询服务
        /// </summary>
        public IOPageViewModel(IIODataPollingService pollingService)
        {
            _pollingService = pollingService ?? throw new ArgumentNullException(nameof(pollingService));
            _model = new IOPageModel();

            PrevPageCommand = new RelayCommand(PrevPage, () => !IsUpdating);
            NextPageCommand = new RelayCommand(NextPage, () => !IsUpdating);

            // 初始化第一页
            LoadPage(0);

            // 订阅数据更新事件
            SubscribeToData();
        }

        #region 数据订阅与取消

        private void SubscribeToData()
        {
            if (_isSubscribed) return;
            _pollingService.IODataUpdated += OnIODataUpdated;
            _isSubscribed = true;
        }

        private void UnsubscribeFromData()
        {
            if (!_isSubscribed) return;
            _pollingService.IODataUpdated -= OnIODataUpdated;
            _isSubscribed = false;
        }

        /// <summary>
        /// 当轮询服务产生新数据时，在UI线程更新IOItems
        /// </summary>
        private async void OnIODataUpdated(object sender, Dictionary<string, bool> data)
        {
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher == null) return;

            await dispatcher.InvokeAsync(() =>
            {
                foreach (var kvp in data)
                {
                    var item = _model.IOItems.FirstOrDefault(x => x.Address == kvp.Key);
                    if (item != null)
                    {
                        item.Status = kvp.Value;
                    }
                }
            });
        }

        #endregion

        #region 地址生成与页面加载

        /// <summary>
        /// 将十进制地址转换为八进制字符串（三位）
        /// </summary>
        private string ToOctalString(int decimalAddress, int digits = 3)
        {
            if (decimalAddress == 0) return new string('0', digits);

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
        /// 生成完整的IO地址（例如 X001, Y077）
        /// </summary>
        private string GenerateAddress(string prefix, int decimalAddress)
        {
            string octalStr = ToOctalString(decimalAddress);
            return $"{prefix}{octalStr}";
        }

        /// <summary>
        /// 加载指定索引的页面（0~3）
        /// </summary>
        private async void LoadPage(int index)
        {
            if (index < 0 || index >= IOPageModel.PageNames.Length) return;

            // 更新当前页索引和名称
            _model.CurrentPageIndex = index;
            _model.CurrentPage = IOPageModel.PageNames[index];
            OnPropertyChanged(nameof(CurrentPage));
            OnPropertyChanged(nameof(CurrentPageIndex));

            var config = IOPageModel.PageConfigs[index];
            var items = new ObservableCollection<IOItem>();

            // 生成 64 个点（4列 × 16行），按行优先填充，但地址按列连续分配
            // 列0: 起始地址+0~15, 列1: +16~31, 列2: +32~47, 列3: +48~63
            for (int i = 0; i < 64; i++)
            {
                int row = i / 4;          // 行索引 0~15
                int col = i % 4;          // 列索引 0~3
                int decimalAddress = config.StartAddress + col * 16 + row;
                if (decimalAddress > config.EndAddress) break;

                string address = GenerateAddress(config.Prefix, decimalAddress);
                items.Add(new IOItem
                {
                    Address = address,
                    Status = false,
                    Remark = string.Empty
                });
            }

            // 在UI线程更新集合
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

        #endregion

        #region 翻页命令

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

        #endregion

        #region INotifyPropertyChanged

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            UnsubscribeFromData();
            // 注意：不释放 _pollingService，因为它由容器管理，生命周期与App一致
        }

        #endregion
    }
}