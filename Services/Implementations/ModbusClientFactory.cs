using System;
using System.Collections.Generic;
using System.Text;
using WPF_test.Services.Interfaces;

namespace WPF_test.Services.Implementations
{
    public class ModbusClientFactory : IModbusClientFactory
    {
        public IModbusService CreateClient()
        {
            return new ModbusTcpClient();
        }
    }
}
