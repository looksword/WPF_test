using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_test.Services.Interfaces
{
    /// <summary>
    /// IO数据轮询服务，负责周期性从Modbus读取数据并广播
    /// </summary>
    public interface IIODataPollingService : IDisposable
    {
        /// <summary>
        /// 当IO数据更新时触发，参数为地址和布尔值的字典
        /// </summary>
        event EventHandler<Dictionary<string, bool>>? IODataUpdated;

        /// <summary>
        /// 启动轮询
        /// </summary>
        void Start();

        /// <summary>
        /// 停止轮询
        /// </summary>
        void Stop();
    }
}
