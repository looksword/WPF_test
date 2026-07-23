using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using WPF_test.Helpers;
using WPF_test.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WPF_test.ViewModels
{
    public class FunctionSelectionViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FunctionSwitchModel> _allItems;
        private ObservableCollection<ObservableCollection<FunctionSwitchModel>> _columnGroups;

        public ICommand ToggleCommand { get; }

        public ObservableCollection<ObservableCollection<FunctionSwitchModel>> ColumnGroups
        {
            get => _columnGroups;
            set
            {
                if (_columnGroups != value)
                {
                    _columnGroups = value;
                    OnPropertyChanged(nameof(ColumnGroups));
                }
            }
        }

        public FunctionSelectionViewModel()
        {
            InitializeData();
            ToggleCommand = new RelayCommand<FunctionSwitchModel>(ToggleFunction);
        }

        private void InitializeData()
        {
            _allItems = new ObservableCollection<FunctionSwitchModel>
            {
                // 第一列 (7项)
                new FunctionSwitchModel { Name = "夹爪有无感应", IsEnabled = true },
                new FunctionSwitchModel { Name = "夹爪气缸感应", IsEnabled = true },
                new FunctionSwitchModel { Name = "A-右卡盘感应", IsEnabled = true },
                new FunctionSwitchModel { Name = "B-左卡盘感应", IsEnabled = true },
                new FunctionSwitchModel { Name = "A-右门感应", IsEnabled = true },
                new FunctionSwitchModel { Name = "B-左门感应", IsEnabled = true },
                new FunctionSwitchModel { Name = "蜂鸣器", IsEnabled = true },

                // 第二列 (6项)
                new FunctionSwitchModel { Name = "进气报警", IsEnabled = true },
                new FunctionSwitchModel { Name = "上位机链接报警", IsEnabled = true },
                new FunctionSwitchModel { Name = "A-右CNC链接检测", IsEnabled = true },
                new FunctionSwitchModel { Name = "B-左CNC链接检测", IsEnabled = true },
                new FunctionSwitchModel { Name = "B-右CNC急停关联", IsEnabled = true },
                new FunctionSwitchModel { Name = "B-左CNC急停关联", IsEnabled = true },

                // 第三列 (1项)
                new FunctionSwitchModel { Name = "循环跑测试", IsEnabled = false },

                // 第四列 (4项)
                new FunctionSwitchModel { Name = "料架感应", IsEnabled = true },
                new FunctionSwitchModel { Name = "刀架感应", IsEnabled = true },
                new FunctionSwitchModel { Name = "A-右CNC虚拟", IsEnabled = false },
                new FunctionSwitchModel { Name = "B-左CNC虚拟", IsEnabled = false }
            };

            // 按列分组
            var column1 = new ObservableCollection<FunctionSwitchModel>(_allItems.Take(7));
            var column2 = new ObservableCollection<FunctionSwitchModel>(_allItems.Skip(7).Take(6));
            var column3 = new ObservableCollection<FunctionSwitchModel>(_allItems.Skip(13).Take(1));
            var column4 = new ObservableCollection<FunctionSwitchModel>(_allItems.Skip(14).Take(4));

            ColumnGroups = new ObservableCollection<ObservableCollection<FunctionSwitchModel>>
            {
                column1,
                column2,
                column3,
                column4
            };
        }

        private void ToggleFunction(FunctionSwitchModel function)
        {
            if (function != null)
            {
                function.IsEnabled = !function.IsEnabled;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}