using System;
using System.Collections.Generic;
using System.Text;
using WPF_test.Services.Interfaces;

namespace WPF_test.Services.Interfaces
{
    /// <summary>
    /// 用于创建独立 Modbus 客户端实例的工厂
    /// </summary>
    public interface IModbusClientFactory
    {
        /// <summary>
        /// 创建一个新的 Modbus 客户端实例（未连接）
        /// </summary>
        IModbusService CreateClient();
    }
}
