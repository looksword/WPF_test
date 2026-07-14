using System.Collections.ObjectModel;
using WPF_test.Models;

namespace WPF_test.ViewModels
{
    /// <summary>
    /// 刀架刀具维护页面的 ViewModel
    /// </summary>
    public class ToolMaintenanceViewModel
    {
        /// <summary>
        /// 第一组刀架（Q01~Q20）
        /// </summary>
        public ObservableCollection<ToolPostItem> ToolsFirstGroup { get; }

        /// <summary>
        /// 第二组刀架（Q21~Q40）
        /// </summary>
        public ObservableCollection<ToolPostItem> ToolsSecondGroup { get; }

        public ToolMaintenanceViewModel()
        {
            ToolsFirstGroup = new ObservableCollection<ToolPostItem>();
            ToolsSecondGroup = new ObservableCollection<ToolPostItem>();

            // 初始化第一组：Q01 ~ Q20
            for (int i = 1; i <= 20; i++)
            {
                ToolsFirstGroup.Add(new ToolPostItem
                {
                    ToolPostId = $"Q{i:D2}",      // 格式化为两位数字，如 Q01
                    ToolInfo = string.Empty       // 初始为空，等待用户输入
                });
            }

            // 初始化第二组：Q21 ~ Q40
            for (int i = 21; i <= 40; i++)
            {
                ToolsSecondGroup.Add(new ToolPostItem
                {
                    ToolPostId = $"Q{i:D2}",
                    ToolInfo = string.Empty
                });
            }
        }

        // 后续可添加保存、加载等命令方法
        // 例如：
        // public ICommand SaveCommand { get; }
        // private void Save() { ... }
    }
}