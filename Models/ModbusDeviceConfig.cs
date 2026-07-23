using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_test.Models
{
    public class ModbusDeviceConfig
    {
        public string Name { get; set; } = string.Empty;
        public string Ip { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 502;
        public byte SlaveId { get; set; } = 1;
        public int PollIntervalMs { get; set; } = 100;
        public int ReconnectIntervalMs { get; set; } = 2000;
    }
}
