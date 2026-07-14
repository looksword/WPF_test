using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF_test.Models
{
    /// <summary>
    /// IO数据模型
    /// </summary>
    public class IOItem : INotifyPropertyChanged
    {
        private bool _status;
        private string _remark = string.Empty;
        private string _address = string.Empty;

        public string Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    _address = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        public bool Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Remark
        {
            get => _remark;
            set
            {
                if (_remark != value)
                {
                    _remark = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }
}