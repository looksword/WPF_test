using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WPF_test.ViewModels;

namespace WPF_test.Pages
{
    public partial class OtherPage : UserControl
    {
        private UserControl currentSubPage;

        public OtherPage()
        {
            InitializeComponent();
        }

        private void ShowSubPage(UserControl subPage)
        {
            // 清除当前子页面
            SubPageContainer.Children.Clear();

            // 添加新的子页面
            SubPageContainer.Children.Add(subPage);

            // 订阅关闭事件
            if (subPage is ProcessMonitorPage processPage)
            {
                processPage.CloseRequested += SubPage_CloseRequested;
            }
            else if (subPage is IOPage ioPage)
            {
                ioPage.CloseRequested += SubPage_CloseRequested;
            }
            else if (subPage is ToolMaintenancePage toolmaintencePage)
            {
                toolmaintencePage.CloseRequested += SubPage_CloseRequested;
            }
            else if (subPage is PLCToCNCPage plctocncPage)
            {
                plctocncPage.CloseRequested += SubPage_CloseRequested;
            }
            else if (subPage is PLCToRobotPage plctorobotPage)
            {
                plctorobotPage.CloseRequested += SubPage_CloseRequested;
            }
            else if (subPage is PLCToPCPage plctopcPage)
            {
                plctopcPage.CloseRequested += SubPage_CloseRequested;
            }
            else if (subPage is ParameterSettingPage parametersettingPage)
            {
                parametersettingPage.CloseRequested += SubPage_CloseRequested;
            }
            else if (subPage is FunctionSelectionPage functionselectionPage)
            {
                functionselectionPage.CloseRequested += SubPage_CloseRequested;
            }

            // 隐藏主内容
            MainContentGrid.Visibility = Visibility.Collapsed;

            // 显示子页面并播放淡入动画
            SubPageContainer.Visibility = Visibility.Visible;
            SubPageContainer.Opacity = 0;

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            SubPageContainer.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            currentSubPage = subPage;
        }

        // 隐藏子页面
        private void HideSubPage()
        {
            if (currentSubPage == null) return;

            // 取消事件订阅
            if (currentSubPage is ProcessMonitorPage processPage)
            {
                processPage.CloseRequested -= SubPage_CloseRequested;
            }
            else if (currentSubPage is IOPage ioPage)
            {
                ioPage.CloseRequested -= SubPage_CloseRequested;
            }
            else if (currentSubPage is ToolMaintenancePage toolmaintencePage)
            {
                toolmaintencePage.CloseRequested -= SubPage_CloseRequested;
            }
            else if (currentSubPage is PLCToCNCPage plctocncPage)
            {
                plctocncPage.CloseRequested -= SubPage_CloseRequested;
            }
            else if (currentSubPage is PLCToRobotPage plctorobotPage)
            {
                plctorobotPage.CloseRequested -= SubPage_CloseRequested;
            }
            else if (currentSubPage is PLCToPCPage plctopcPage)
            {
                plctopcPage.CloseRequested -= SubPage_CloseRequested;
            }
            else if (currentSubPage is ParameterSettingPage parametersettingPage)
            {
                parametersettingPage.CloseRequested -= SubPage_CloseRequested;
            }
            else if (currentSubPage is FunctionSelectionPage functionselectionPage)
            {
                functionselectionPage.CloseRequested -= SubPage_CloseRequested;
            }

            // 播放淡出动画
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.2)
            };
            fadeOut.Completed += (s, e) =>
            {
                SubPageContainer.Visibility = Visibility.Collapsed;
                SubPageContainer.Children.Clear();
                MainContentGrid.Visibility = Visibility.Visible;
            };
            SubPageContainer.BeginAnimation(UIElement.OpacityProperty, fadeOut);

            currentSubPage = null;
        }

        // 子页面关闭事件处理
        private void SubPage_CloseRequested(object sender, RoutedEventArgs e)
        {
            HideSubPage();
        }

        // ===== 按钮点击事件 =====
        private void BtnProcessMonitor_Click(object sender, RoutedEventArgs e)
        {
            var page = new ProcessMonitorPage();
            ShowSubPage(page);
        }

        private void BtnIOPage_Click(object sender, RoutedEventArgs e)
        {
            var page = new IOPage();
            ShowSubPage(page);
        }

        // IO强制测试 - 不弹出子页面（仅提示）
        private void BtnIOForceTest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("IO强制测试功能", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 刀架刀具规格维护 - 弹出子页面
        private void BtnToolMaintenance_Click(object sender, RoutedEventArgs e)
        {
            var page = new ToolMaintenancePage();
            ShowSubPage(page);
        }

        // PLC<==>PC - 弹出子页面
        private void BtnPLCToPC_Click(object sender, RoutedEventArgs e)
        {
            var page = new PLCToPCPage();
            ShowSubPage(page);
        }

        // PLC<==>ROBOT - 弹出子页面
        private void BtnPLCToRobot_Click(object sender, RoutedEventArgs e)
        {
            var serviceProvider = (App.Current as App)?.ServiceProvider;
            var viewModel = serviceProvider.GetRequiredService<PLCToRobotViewModel>();
            var page = new PLCToRobotPage(viewModel);
            ShowSubPage(page);
        }

        // PLC<==>CNC - 弹出子页面
        private void BtnPLCToCNC_Click(object sender, RoutedEventArgs e)
        {
            var page = new PLCToCNCPage();
            ShowSubPage(page);
        }

        // PC - 不弹出子页面（仅提示）
        private void BtnPC_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("PC功能", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 参数设定 - 弹出子页面
        private void BtnParameterSetting_Click(object sender, RoutedEventArgs e)
        {
            var page = new ParameterSettingPage();
            ShowSubPage(page);
        }

        // 功能旋转 - 弹出子页面
        private void BtnFunctionSelection_Click(object sender, RoutedEventArgs e)
        {
            var page = new FunctionSelectionPage();
            ShowSubPage(page);
        }
    }
}
