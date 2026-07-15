using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPF_test.Pages
{
    public partial class MainPage : UserControl
    {
        private UserControl currentSubPage;
        public MainPage()
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
            if (subPage is ToolMaintenancePage toolmaintencePage)
            {
                toolmaintencePage.CloseRequested += SubPage_CloseRequested;
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
            if (currentSubPage is ToolMaintenancePage toolmaintencePage)
            {
                toolmaintencePage.CloseRequested -= SubPage_CloseRequested;
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

        // 刀架刀具规格维护 - 弹出子页面
        private void BtnToolMaintenance_Click(object sender, RoutedEventArgs e)
        {
            var page = new ToolMaintenancePage();
            ShowSubPage(page);
        }
    }
}
