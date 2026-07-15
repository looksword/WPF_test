namespace WPF_test.Models
{
    public class DelayParameterModel
    {
        // 左侧参数
        public double CylinderAlarmDelay { get; set; } = 10.0;
        public double JawCylinderInPlaceDelay { get; set; } = 1.0;
        public double JawCylinderResetDelay { get; set; } = 1.0;
        public double ARightDoorInPlaceDelay { get; set; } = 1.0;
        public double ARightDoorResetDelay { get; set; } = 1.0;
        public double BLeftDoorInPlaceDelay { get; set; } = 1.0;
        public double BLeftDoorResetDelay { get; set; } = 1.0;
        public double ARightChuckInPlaceDelay { get; set; } = 1.0;
        public double ARightChuckResetDelay { get; set; } = 1.0;
        public double BLeftChuckInPlaceDelay { get; set; } = 1.0;
        public double BLeftChuckResetDelay { get; set; } = 1.0;

        // 右侧参数
        public int DownloadRetryCount { get; set; } = 3;
        public double CncStartSignalHold { get; set; } = 0.5;
        public double CngVirtualMachiningTime { get; set; } = 20.0;
        public double ACncCompleteBlowTime { get; set; } = 10.0;
        public double BCncCompleteBlowTime { get; set; } = 10.0;
    }
}