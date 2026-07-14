using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF_test.Models
{
    /// <summary>
    /// IO页面数据模型
    /// </summary>
    public class IOPageModel : INotifyPropertyChanged
    {
        private ObservableCollection<IOItem> _ioItems = new ObservableCollection<IOItem>();
        private string _currentPage = string.Empty;
        private int _currentPageIndex;
        private bool _isUpdating;

        public ObservableCollection<IOItem> IOItems
        {
            get => _ioItems;
            set
            {
                _ioItems = value ?? new ObservableCollection<IOItem>();
                OnPropertyChanged();
            }
        }

        public string CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public int CurrentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                _currentPageIndex = value;
                OnPropertyChanged();
            }
        }

        public bool IsUpdating
        {
            get => _isUpdating;
            set
            {
                _isUpdating = value;
                OnPropertyChanged();
            }
        }

        // 页面配置
        public static readonly string[] PageNames = { "输入01", "输入02", "输出01", "输出02" };

        // 每个页面的地址范围配置（8进制）
        public static readonly PageConfig[] PageConfigs = new[]
        {
            new PageConfig { Prefix = "X", StartAddress = 0, EndAddress = 63, IsInput = true },
            new PageConfig { Prefix = "X", StartAddress = 64, EndAddress = 127, IsInput = true },
            new PageConfig { Prefix = "Y", StartAddress = 0, EndAddress = 63, IsInput = false },
            new PageConfig { Prefix = "Y", StartAddress = 64, EndAddress = 127, IsInput = false }
        };

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }

    /// <summary>
    /// 页面配置
    /// </summary>
    public class PageConfig
    {
        public string Prefix { get; set; } = string.Empty;
        public int StartAddress { get; set; }
        public int EndAddress { get; set; }
        public bool IsInput { get; set; }
        public bool IsOctal { get; set; }
    }
}