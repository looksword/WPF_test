using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_test
{
    /// <summary>
    /// TestOption.xaml 的交互逻辑
    /// </summary>
    public partial class TestOption : UserControl
    {
        public static int currentIndex;
        public static int newIndex;

        public TestOption()
        {
            InitializeComponent();
        }

        private void add_MouseEnter(object sender, MouseEventArgs e)
        {
            Border tempborder = (Border)sender;
            tempborder.Opacity = 100;
        }

        private void add_MouseLeave(object sender, MouseEventArgs e)
        {
            Border tempborder = (Border)sender;
            tempborder.Opacity = 0;
        }

        private void close_add_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.optionBoxPanel.Children.Remove(this);
        }

        private void left_add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentIndex = MainWindow.optionBoxPanel.Children.IndexOf(this);
            newIndex = currentIndex;
            add_MouseDown(sender, e);
        }

        private void right_add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentIndex = MainWindow.optionBoxPanel.Children.IndexOf(this);
            newIndex = currentIndex;
            add_MouseDown(sender, e);
        }

        private void add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TestOption newTestOption = new TestOption();
            MainWindow.optionBoxPanel.Children.Insert(newIndex, newTestOption);
        }
    }
}
