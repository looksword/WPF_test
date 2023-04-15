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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitialSelfComponent();
            textbox1.TextChanged += Textbox1_TextChanged;
        }

        private void InitialSelfComponent()
        {
            TestOption newbtn = new TestOption();
            this.stack_button.Children.Add(newbtn);
        }

        private void Textbox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            label1.Content = textbox1.Text;
        }
    }
}
