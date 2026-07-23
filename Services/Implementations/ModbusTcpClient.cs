using NModbus;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WPF_test.Services.Interfaces;

namespace WPF_test.Services.Implementations
{
    /// <summary>
    /// Modbus TCP 客户端（同步+异步），线程安全，支持自动释放资源。
    /// 使用工厂模式创建 IModbusMaster 实例。
    /// </summary>
    public class ModbusTcpClient : IModbusService
    {
        private TcpClient _tcpClient;
        private IModbusMaster _master;          // 改为接口类型
        private readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1);
        private bool _disposed;

        public bool IsConnected { get; private set; }

        /// <summary>
        /// 异步连接 Modbus TCP 服务器
        /// </summary>
        /// <param name="ip">IP 地址</param>
        /// <param name="port">端口号，默认 502</param>
        public async Task ConnectAsync(string ip, int port = 502)
        {
            if (IsConnected)
                Disconnect();

            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(ip, port);

            // 使用工厂创建主站
            var factory = new ModbusFactory();
            _master = factory.CreateMaster(_tcpClient);

            // 可调整超时与重试策略
            _master.Transport.Retries = 3;
            _master.Transport.ReadTimeout = 2000;
            _master.Transport.WriteTimeout = 2000;
            IsConnected = true;
        }

        /// <summary>
        /// 同步连接（WPF 中建议在后台线程调用）
        /// </summary>
        public void Connect(string ip, int port = 502)
        {
            ConnectAsync(ip, port).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 断开连接，释放底层资源
        /// </summary>
        public void Disconnect()
        {
            if (!IsConnected) return;

            _master?.Dispose();
            _tcpClient?.Close();
            _tcpClient?.Dispose();
            _master = null;
            _tcpClient = null;
            IsConnected = false;
        }

        /// <summary>
        /// 异步读取保持寄存器（最常用的读取方式）
        /// </summary>
        /// <param name="slaveId">从站 ID（1~247）</param>
        /// <param name="startAddress">起始地址（0-based）</param>
        /// <param name="numberOfRegisters">寄存器数量</param>
        /// <returns>ushort 数组</returns>
        public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId, ushort startAddress, ushort numberOfRegisters)
        {
            EnsureConnected();
            await _asyncLock.WaitAsync();
            try
            {
                return await _master.ReadHoldingRegistersAsync(slaveId, startAddress, numberOfRegisters);
            }
            finally
            {
                _asyncLock.Release();
            }
        }

        /// <summary>
        /// 同步读取保持寄存器
        /// </summary>
        public ushort[] ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort numberOfRegisters)
        {
            return ReadHoldingRegistersAsync(slaveId, startAddress, numberOfRegisters).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 异步写入多个保持寄存器
        /// </summary>
        /// <param name="slaveId">从站 ID</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="values">要写入的值数组</param>
        public async Task WriteMultipleRegistersAsync(byte slaveId, ushort startAddress, ushort[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("写入数组不能为空", nameof(values));

            EnsureConnected();
            await _asyncLock.WaitAsync();
            try
            {
                await _master.WriteMultipleRegistersAsync(slaveId, startAddress, values);
            }
            finally
            {
                _asyncLock.Release();
            }
        }

        /// <summary>
        /// 同步写入多个保持寄存器
        /// </summary>
        public void WriteMultipleRegisters(byte slaveId, ushort startAddress, ushort[] values)
        {
            WriteMultipleRegistersAsync(slaveId, startAddress, values).GetAwaiter().GetResult();
        }

        // 可选：读取输入寄存器、线圈等，根据需求扩展
        // public async Task<bool[]> ReadCoilsAsync(...) { ... }

        private void EnsureConnected()
        {
            if (!IsConnected || _master == null)
                throw new InvalidOperationException("Modbus 客户端未连接或已断开。");
        }

        #region IDisposable 实现
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _asyncLock?.Wait();
                try
                {
                    Disconnect();
                }
                finally
                {
                    _asyncLock?.Release();
                    _asyncLock?.Dispose();
                }
            }

            _disposed = true;
        }
        #endregion
    }
}