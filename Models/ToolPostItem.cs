using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF_test.Models
{
    /// <summary>
    /// 刀架刀具项模型
    /// </summary>
    public class ToolPostItem : INotifyPropertyChanged
    {
        private string _toolPostId;
        private string _toolInfo;

        /// <summary>
        /// 刀架编号（如 Q01）
        /// </summary>
        public string ToolPostId
        {
            get => _toolPostId;
            set
            {
                if (_toolPostId != value)
                {
                    _toolPostId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 刀具信息（用户可编辑）
        /// </summary>
        public string ToolInfo
        {
            get => _toolInfo;
            set
            {
                if (_toolInfo != value)
                {
                    _toolInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}