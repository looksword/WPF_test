using System.ComponentModel;
using System.Runtime.CompilerServices;
using WPF_test.Models;

namespace WPF_test.ViewModels
{
    public class DelayParameterViewModel : INotifyPropertyChanged
    {
        private DelayParameterModel _model;

        public DelayParameterViewModel()
        {
            _model = new DelayParameterModel();
        }

        // 左侧参数
        public double CylinderAlarmDelay
        {
            get => _model.CylinderAlarmDelay;
            set { _model.CylinderAlarmDelay = value; OnPropertyChanged(); }
        }

        public double JawCylinderInPlaceDelay
        {
            get => _model.JawCylinderInPlaceDelay;
            set { _model.JawCylinderInPlaceDelay = value; OnPropertyChanged(); }
        }

        public double JawCylinderResetDelay
        {
            get => _model.JawCylinderResetDelay;
            set { _model.JawCylinderResetDelay = value; OnPropertyChanged(); }
        }

        public double ARightDoorInPlaceDelay
        {
            get => _model.ARightDoorInPlaceDelay;
            set { _model.ARightDoorInPlaceDelay = value; OnPropertyChanged(); }
        }

        public double ARightDoorResetDelay
        {
            get => _model.ARightDoorResetDelay;
            set { _model.ARightDoorResetDelay = value; OnPropertyChanged(); }
        }

        public double BLeftDoorInPlaceDelay
        {
            get => _model.BLeftDoorInPlaceDelay;
            set { _model.BLeftDoorInPlaceDelay = value; OnPropertyChanged(); }
        }

        public double BLeftDoorResetDelay
        {
            get => _model.BLeftDoorResetDelay;
            set { _model.BLeftDoorResetDelay = value; OnPropertyChanged(); }
        }

        public double ARightChuckInPlaceDelay
        {
            get => _model.ARightChuckInPlaceDelay;
            set { _model.ARightChuckInPlaceDelay = value; OnPropertyChanged(); }
        }

        public double ARightChuckResetDelay
        {
            get => _model.ARightChuckResetDelay;
            set { _model.ARightChuckResetDelay = value; OnPropertyChanged(); }
        }

        public double BLeftChuckInPlaceDelay
        {
            get => _model.BLeftChuckInPlaceDelay;
            set { _model.BLeftChuckInPlaceDelay = value; OnPropertyChanged(); }
        }

        public double BLeftChuckResetDelay
        {
            get => _model.BLeftChuckResetDelay;
            set { _model.BLeftChuckResetDelay = value; OnPropertyChanged(); }
        }

        // 右侧参数
        public int DownloadRetryCount
        {
            get => _model.DownloadRetryCount;
            set { _model.DownloadRetryCount = value; OnPropertyChanged(); }
        }

        public double CncStartSignalHold
        {
            get => _model.CncStartSignalHold;
            set { _model.CncStartSignalHold = value; OnPropertyChanged(); }
        }

        public double CngVirtualMachiningTime
        {
            get => _model.CngVirtualMachiningTime;
            set { _model.CngVirtualMachiningTime = value; OnPropertyChanged(); }
        }

        public double ACncCompleteBlowTime
        {
            get => _model.ACncCompleteBlowTime;
            set { _model.ACncCompleteBlowTime = value; OnPropertyChanged(); }
        }

        public double BCncCompleteBlowTime
        {
            get => _model.BCncCompleteBlowTime;
            set { _model.BCncCompleteBlowTime = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 恢复默认值
        public void ResetToDefault()
        {
            var defaultModel = new DelayParameterModel();
            CylinderAlarmDelay = defaultModel.CylinderAlarmDelay;
            JawCylinderInPlaceDelay = defaultModel.JawCylinderInPlaceDelay;
            JawCylinderResetDelay = defaultModel.JawCylinderResetDelay;
            ARightDoorInPlaceDelay = defaultModel.ARightDoorInPlaceDelay;
            ARightDoorResetDelay = defaultModel.ARightDoorResetDelay;
            BLeftDoorInPlaceDelay = defaultModel.BLeftDoorInPlaceDelay;
            BLeftDoorResetDelay = defaultModel.BLeftDoorResetDelay;
            ARightChuckInPlaceDelay = defaultModel.ARightChuckInPlaceDelay;
            ARightChuckResetDelay = defaultModel.ARightChuckResetDelay;
            BLeftChuckInPlaceDelay = defaultModel.BLeftChuckInPlaceDelay;
            BLeftChuckResetDelay = defaultModel.BLeftChuckResetDelay;
            DownloadRetryCount = defaultModel.DownloadRetryCount;
            CncStartSignalHold = defaultModel.CncStartSignalHold;
            CngVirtualMachiningTime = defaultModel.CngVirtualMachiningTime;
            ACncCompleteBlowTime = defaultModel.ACncCompleteBlowTime;
            BCncCompleteBlowTime = defaultModel.BCncCompleteBlowTime;
        }

        // 保存参数（可扩展为实际保存逻辑）
        public void Save()
        {
            // 这里可以添加保存到配置文件或数据库的逻辑
            System.Windows.MessageBox.Show("参数已保存！", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}