using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_test.Services.Interfaces
{
    public interface IModbusService : IDisposable
    {
        Task ConnectAsync(string ip, int port = 502);
        void Disconnect();
        bool IsConnected { get; }
        Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId, ushort address, ushort num);
        Task WriteMultipleRegistersAsync(byte slaveId, ushort address, ushort[] values);
    }
}
