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

namespace NeucrackSerialPort
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private double mSetingPanelWidth=0;
        private Thickness thickMargin=new Thickness(0,0,0,0);
        /// <summary>
        /// 隐藏配置面变按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_HideSettingPanel(object sender, RoutedEventArgs e)
        {
            
            if(SettingPanel.Width!=0)
            {
                mSetingPanelWidth = SettingPanel.Width;
                SettingPanel.Width = 0;
                thickMargin = DisplayPanel.Margin;
                DisplayPanel.Margin = new Thickness(0,0,0,0);
                DisplayPanel.Width += mSetingPanelWidth;
                ButtonHideSettingpanel.Content = ">>";
                
            }
            else
            {
                SettingPanel.Width = mSetingPanelWidth;
                DisplayPanel.Width -= mSetingPanelWidth;
                DisplayPanel.Margin = thickMargin;
                ButtonHideSettingpanel.Content = "<<";
            }
        }

    }
}
