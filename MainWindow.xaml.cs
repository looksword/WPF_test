using System.Windows;
using System.Windows.Controls;

namespace WPF_test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 关闭按钮点击
        /// </summary>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Tab 页面切换（RadioButton 点击事件）
        /// </summary>
        private void TabPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton radio)
                return;

            string? tag = radio.Tag?.ToString();
            if (string.IsNullOrEmpty(tag))
                return;

            // 隐藏所有页面
            CollapseAllPages();

            // 根据 Tag 显示对应页面
            switch (tag)
            {
                case "Main":
                    Page_Main.Visibility = Visibility.Visible;
                    break;
                case "Manual":
                    Page_Manual.Visibility = Visibility.Visible;
                    break;
                case "Registered":
                    Page_Registered.Visibility = Visibility.Visible;
                    break;
                case "Production":
                    Page_Production.Visibility = Visibility.Visible;
                    break;
                case "AlarmRecord":
                    Page_AlarmRecord.Visibility = Visibility.Visible;
                    break;
                case "Other":
                    Page_Other.Visibility = Visibility.Visible;
                    break;
                case "Alm":
                    Page_Alm.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// 故障解除按钮点击（不切换页面，仅供预留）
        /// </summary>
        private void Tab_FaultReset_Click(object sender, RoutedEventArgs e)
        {
            // 故障解除操作 — 预留，不切换页面
            MessageBox.Show("故障解除指令已发送", "故障解除",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 隐藏所有子页面
        /// </summary>
        private void CollapseAllPages()
        {
            Page_Main.Visibility = Visibility.Collapsed;
            Page_Manual.Visibility = Visibility.Collapsed;
            Page_Registered.Visibility = Visibility.Collapsed;
            Page_Production.Visibility = Visibility.Collapsed;
            Page_AlarmRecord.Visibility = Visibility.Collapsed;
            Page_Other.Visibility = Visibility.Collapsed;
            Page_Alm.Visibility = Visibility.Collapsed;
        }
    }
}
