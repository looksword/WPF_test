using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_test.Services.Interfaces;
using WPF_test.Helpers;

namespace WPF_test.ViewModels
{
    /// <summary>
    /// PLC ↔ Robot 通讯页面的 ViewModel
    /// </summary>
    public class PLCToRobotViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IModbusService _modbusService;
        private bool _isConnected;
        private string _statusText = "未连接";
        private string _ip = "127.0.0.1";
        private string _port = "502";

        // 读取参数
        private string _readSlaveId = "1";
        private string _readAddress = "0";
        private string _readCount = "10";
        private string _readResult = string.Empty;

        // 写入参数
        private string _writeSlaveId = "1";
        private string _writeAddress = "0";
        private string _writeValues = "1,2,3,4,5";
        private string _writeResult = string.Empty;

        private bool _isBusy;

        public PLCToRobotViewModel(IModbusService modbusService)
        {
            _modbusService = modbusService ?? throw new ArgumentNullException(nameof(modbusService));
            ConnectCommand = new AsyncRelayCommand(ConnectAsync, () => CanConnect);
            DisconnectCommand = new RelayCommand(Disconnect, () => IsConnected);
            ReadCommand = new AsyncRelayCommand(ReadAsync, () => CanReadWrite);
            WriteCommand = new AsyncRelayCommand(WriteAsync, () => CanReadWrite);
        }

        #region 属性

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    OnPropertyChanged(nameof(CanConnect));
                    OnPropertyChanged(nameof(CanDisconnect));
                    OnPropertyChanged(nameof(CanReadWrite));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public string Ip
        {
            get => _ip;
            set => SetProperty(ref _ip, value);
        }

        public string Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string ReadSlaveId
        {
            get => _readSlaveId;
            set => SetProperty(ref _readSlaveId, value);
        }

        public string ReadAddress
        {
            get => _readAddress;
            set => SetProperty(ref _readAddress, value);
        }

        public string ReadCount
        {
            get => _readCount;
            set => SetProperty(ref _readCount, value);
        }

        public string ReadResult
        {
            get => _readResult;
            set => SetProperty(ref _readResult, value);
        }

        public string WriteSlaveId
        {
            get => _writeSlaveId;
            set => SetProperty(ref _writeSlaveId, value);
        }

        public string WriteAddress
        {
            get => _writeAddress;
            set => SetProperty(ref _writeAddress, value);
        }

        public string WriteValues
        {
            get => _writeValues;
            set => SetProperty(ref _writeValues, value);
        }

        public string WriteResult
        {
            get => _writeResult;
            set => SetProperty(ref _writeResult, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        // 命令可用性
        public bool CanConnect => !IsConnected && !IsBusy;
        public bool CanDisconnect => IsConnected && !IsBusy;
        public bool CanReadWrite => IsConnected && !IsBusy;

        #endregion

        #region 命令

        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand ReadCommand { get; }
        public ICommand WriteCommand { get; }

        #endregion

        #region 命令实现

        private async Task ConnectAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                StatusText = "正在连接...";
                if (!int.TryParse(Port, out int port))
                {
                    StatusText = "端口号无效";
                    return;
                }
                await _modbusService.ConnectAsync(Ip, port);
                IsConnected = true;
                StatusText = "已连接";
            }
            catch (Exception ex)
            {
                StatusText = $"连接失败: {ex.Message}";
                IsConnected = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Disconnect()
        {
            if (IsBusy) return;
            try
            {
                _modbusService.Disconnect();
                IsConnected = false;
                StatusText = "已断开";
            }
            catch (Exception ex)
            {
                StatusText = $"断开失败: {ex.Message}";
            }
        }

        private async Task ReadAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                if (!byte.TryParse(ReadSlaveId, out byte slaveId))
                    throw new ArgumentException("站号无效（1~247）");
                if (!ushort.TryParse(ReadAddress, out ushort address))
                    throw new ArgumentException("地址无效");
                if (!ushort.TryParse(ReadCount, out ushort count) || count == 0)
                    throw new ArgumentException("数量无效（>0）");

                ReadResult = "读取中...";
                var values = await _modbusService.ReadHoldingRegistersAsync(slaveId, address, count);
                ReadResult = string.Join(", ", values.Select(v => v.ToString()));
            }
            catch (Exception ex)
            {
                ReadResult = $"错误: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task WriteAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                if (!byte.TryParse(WriteSlaveId, out byte slaveId))
                    throw new ArgumentException("站号无效（1~247）");
                if (!ushort.TryParse(WriteAddress, out ushort address))
                    throw new ArgumentException("地址无效");

                var parts = WriteValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var values = parts.Select(p => ushort.Parse(p.Trim())).ToArray();
                if (values.Length == 0)
                    throw new ArgumentException("请输入至少一个数值");

                WriteResult = "写入中...";
                await _modbusService.WriteMultipleRegistersAsync(slaveId, address, values);
                WriteResult = $"写入成功，共 {values.Length} 个寄存器";
            }
            catch (Exception ex)
            {
                WriteResult = $"写入失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _modbusService?.Dispose();
        }

        #endregion
    }
}