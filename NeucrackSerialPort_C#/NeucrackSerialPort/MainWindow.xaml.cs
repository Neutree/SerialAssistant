using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Management;

using NeucrackSerialPort.model;
using System.IO.Ports;

namespace NeucrackSerialPort
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPortModel mSerialPort;
        private bool mIsSerialOpen=false;//界面上标志的串口连接状态
        long mReceivedCount = 0, mSendCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            SerialInit();
            this.Closing += delegate(object sender, System.ComponentModel.CancelEventArgs e)
            {
                Properties.Settings.Default.BaudRate = SerialBaud.SelectedIndex;//保存当前设置的波特率
                Properties.Settings.Default.Save();
            };
        }
        
        
        ///// <summary>
        ///// 隐藏配置面变按钮单击事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Button_HideSettingPanel(object sender, RoutedEventArgs e)
        //{
        //    //if(SettingPanel.Width!=0)
        //    //{
        //    //    new Thread(() =>
        //    //    {
        //    //        this.Dispatcher.Invoke(new Action(() =>
        //    //        {
        //    //            HideSettingPanel();
        //    //        }));
        //    //    }).Start();
        //    //}
        //    //else
        //    //{
        //    //    new Thread(() =>
        //    //    {
        //    //        this.Dispatcher.Invoke(new Action(() =>
        //    //        {
        //    //            ShowSettingPanel();
        //    //        }));
        //    //    }).Start();
        //    //}
        //}

        //private void HideSettingPanel()
        //{
        //    //mSetingPanelWidth = SettingPanel.Width;
        //    //mSetingPanelMinWidth = SettingPanel.MinWidth;
        //    //SettingPanel.MinWidth = 0;
        //    //SettingPanel.Width = 0;
        //    //thickMargin = DisplayPanel.Margin;
        //    //DisplayPanel.Margin = new Thickness(10,0,10,0);
        //    //ButtonHideSettingpanel.Content = ">>";
        //}
        //private void ShowSettingPanel()
        //{
        //    //SettingPanel.Width = mSetingPanelWidth;
        //    //SettingPanel.MinWidth = mSetingPanelMinWidth;
        //    //DisplayPanel.Margin = thickMargin;
        //    //ButtonHideSettingpanel.Content = "<<";
        //}


        private void SerialInit()
        {
            //串口相关操作对象定义
            mSerialPort=new SerialPortModel();

            //更新界面串口选项
            string[] strArr = mSerialPort.GetAllSerialPortInfo();
            foreach (string s in strArr)
            {
                SerialPort.Items.Add(s);
            }
            if (SerialPort.Items.Count > 0)
                SerialPort.SelectedIndex = 0;
            SerialBaud.SelectedIndex = Properties.Settings.Default.BaudRate;//加载上一次使用的波特率
            //添加串口相关的事件函数
            mSerialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(SerialDataReceivedHandler);

        }

        public delegate void UpdateBytesDelegate(byte[] data);  
        void SerialDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int length = mSerialPort.BytesToRead;
            byte[] data = new byte[length];
            mReceivedCount += length;
            new Thread(() =>
            {
                try
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        SetReceivedDataCount(mReceivedCount);
                    }));
                }catch(Exception)
                {

                }
            }).Start();
            try
            {
                for (int i = 0; i < length; i++)
                {
                    data[i] = (byte)mSerialPort.ReadByte();
                }
            }catch(Exception)
            {
                return;
            }
            Dispatcher.Invoke(new UpdateBytesDelegate(UpdateBytesbox), data);
        }

        
        private void UpdateBytesbox(byte[] data)
        {
            if (IsStop)
            {
               // mSerialPort.ReceiveDataUpdate(data);
            }
            else
            {
                //txtRecData.AppendText(Port.ReceiveDataUpdate(data) + Environment.NewLine);
              //  Paragraph p = (Paragraph)InputReceivedDataArea.Document.Blocks.FirstBlock;
               // Run r = new Run(mSerialPort.ReceiveDataUpdate(data) + Environment.NewLine);
                //r.Foreground = Brushes.Black;
                //p.Inlines.Add(r);
                InputReceivedDataArea.AppendText(mSerialPort.ReceiveDataUpdate(data));
                InputReceivedDataArea.ScrollToEnd();
            }
        }

        /// <summary>
        /// 打开按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            if (!mIsSerialOpen)//界面显示没有打开
            {
                System.IO.Ports.Parity parity;
                System.IO.Ports.StopBits stopBits;
                int flowControl;
                switch(SerialParity.Text)
                {
                    case "Even":
                        parity=Parity.Even;
                        break;
                    case "Odd":
                        parity=Parity.Odd;
                        break;
                    case "Mark":
                        parity=Parity.Mark;
                        break;
                    case "Space":
                        parity=Parity.Space;
                        break;
                    default:
                        parity=Parity.None;
                        break;
                }
                switch(SerialStop.Text)
                {
                    case "1.5":
                        stopBits=System.IO.Ports.StopBits.OnePointFive;
                        break;
                    case "1":
                        stopBits=System.IO.Ports.StopBits.One;
                        break;
                    case "2":
                        stopBits=System.IO.Ports.StopBits.Two;
                        break;
                    default:
                        stopBits=System.IO.Ports.StopBits.None;
                        break;
                }
                switch(SerialFlowControl.Text)
                {
                    case "RTS/CTS":
                        flowControl = 1;
                        break;
                    case "XON/XOFF":
                        flowControl = 2;
                        break;
                    default:
                        flowControl = 0;
                        break;

                }
                //设置参数
                try{
                    int indexComStart = SerialPort.Text.IndexOf('(')+1;
                    int indexComEnd = SerialPort.Text.IndexOf(')');
                    mSerialPort.Setting(SerialPort.Text.Substring(indexComStart,indexComEnd-indexComStart), int.Parse(SerialBaud.Text), int.Parse(SerialDataNum.Text), parity, stopBits, flowControl, 2048, 2048, 50, 50, "UTF-8");
                }catch(Exception )
                {
                    MessageBox.Show("参数有误！！","错误啦");
                    return;
                }
                switch (mSerialPort.Open())
                {
                    case 0://打开成功
                        ButtonOpen.Content = "关闭";
                        mIsSerialOpen = true;
                        break;
                    case 1:
                        ButtonOpen.Content = "关闭";
                        mIsSerialOpen = true;
                        break;
                    case -1:
                        MessageBox.Show("操作系统因 I/O 错误或指定类型的安全错误而拒绝访问", "打开错误");
                        break;
                    case -2:
                        MessageBox.Show("参数值范围错误", "打开错误");
                        break;
                    case -3:
                        MessageBox.Show("串口参数中含有无效参数", "打开错误");
                        break;
                    case -4:
                        MessageBox.Show("I/O错误", "打开错误");
                        break;
                    case -5:
                        MessageBox.Show("当前状态无效错误", "打开错误");
                        break;
                }
            }
            else//界面显示已经打开了
            {
                switch(mSerialPort.Close())
                {
                    case 0:
                        
                        break;
                    case 1:
                        break;
                    case -1:
                        break;
                }
                ButtonOpen.Content = "打开";
                mIsSerialOpen = false;
            }

        }

        public bool IsStop { get; set; }

        /// <summary>
        /// 重置发送、接收计数按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonResetSerialDataCount_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        SetReceivedDataCount(0);
                        SetsendDataCount(0);
                        mReceivedCount = 0;
                        mSendCount = 0;
                    }));
                }
                catch (Exception)
                {

                }
            }).Start();
        }
        /// <summary>
        /// 设置发送计数
        /// </summary>
        /// <param name="p"></param>
        private void SetsendDataCount(long p)
        {
            SerialSendDataCount.Content = p.ToString();
        }
        /// <summary>
        /// 设置接收计数
        /// </summary>
        /// <param name="mReceivedCount"></param>
        private void SetReceivedDataCount(long mReceivedCount)
        {

            SerialReceivedDataCount.Content = mReceivedCount.ToString();

        }

        /// <summary>
        /// 清除接收显示按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClearReceivedArea_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        InputReceivedDataArea.Document.Blocks.Clear();
                    }));
                }
                catch (Exception)
                {

                }
            }).Start();
        }
    }
}
