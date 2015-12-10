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
using System.ComponentModel;

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
        //读取数据线程 
        Thread _readThread;
        bool _keepReading;


        /// <summary>
        /// 窗口加载
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SerialInit();

            //添加事件响应
            this.Closing += delegate(object sender, System.ComponentModel.CancelEventArgs e)
            {
                Properties.Settings.Default.BaudRate = SerialBaud.SelectedIndex;//保存当前设置的波特率
                Properties.Settings.Default.Save();
            };
            ButtonSendData.Click += delegate(object sender, RoutedEventArgs e)
            {
                SendData(new TextRange(InputSendDataArea.Document.ContentStart, InputSendDataArea.Document.ContentEnd).Text);
            };
            ButtonClearSendArea.Click += delegate(object sender, RoutedEventArgs e)
            {
                ClearSendDataArea();
            };
        }

        
        
      
        /// <summary>
        /// 串口初始化函数
        /// </summary>
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
           // mSerialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(SerialDataReceivedHandler);

        }

        /// <summary>
        /// 串口接收事件函数//未使用，使用新的线程一直监视串口接收区
        /// </summary>
        /// <param name="data"></param>
        public delegate void UpdateBytesDelegate(byte[] data);
        public delegate void UpdateReceivedCount(long count);
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
                }
                catch (Exception)
                {

                }
            }).Start();
          //  Dispatcher.Invoke(new UpdateReceivedCount(SetReceivedDataCount), mReceivedCount);
            try
            {
                for (int i = 0; i < length; i++)
                {
                    data[i] = (byte)mSerialPort.ReadByte();
                }
            }
            catch (Exception)
            {
                return;
            }
            Dispatcher.Invoke(new UpdateBytesDelegate(UpdateBytesbox), data);
        }
        public void RecievedFunc()
        {
 
        }
        
        /// <summary>
        /// 更新UI（richtextbox接收区域）
        /// </summary>
        /// <param name="data"></param>
        private void UpdateBytesbox(byte[] data)
        {
            if (IsStop)
            {
                mSerialPort.ReceiveDataUpdate(data);
            }
            else
            {
                try
                {
                    //InputReceivedDataArea.AppendText(mSerialPort.ReceiveDataUpdate(data));
                    //InputReceivedDataArea.ScrollToEnd();
                    Paragraph p = (Paragraph)InputReceivedDataArea.Document.Blocks.FirstBlock;
                    Run r = new Run(mSerialPort.ReceiveDataUpdate(data));
                    //r.Foreground = Brushes.DarkRed;
                    p.Inlines.Add(r);
                    InputReceivedDataArea.ScrollToEnd();
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// 打开串口按钮单击事件
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
                    SerialStatus.Content = "已连接";
                    _keepReading = true;
                    _readThread = new Thread(ReadSerialPort);
                    _readThread.Start();
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
                SerialStatus.Content = "未连接-就绪";
                _keepReading = false;//结束线程
                mIsSerialOpen = false;
            }

        }

        /// <summary>
        /// 读取串口数据线程函数
        /// </summary>
        /// <param name="obj"></param>
        private void ReadSerialPort(object obj)
        {
            while (_keepReading)
            {
                if (mSerialPort.IsOpen)
                {
                    //byte[] readBuffer = new byte[mSerialPort.ReadBufferSize + 1];
                    //try
                    //{
                    //    // If there are bytes available on the serial port,   
                    //    // Read returns up to "count" bytes, but will not block (wait)   
                    //    // for the remaining bytes. If there are no bytes available   
                    //    // on the serial port, Read will block until at least one byte   
                    //    // is available on the port, up until the ReadTimeout milliseconds   
                    //    // have elapsed, at which time a TimeoutException will be thrown.   
                    //    int count = mSerialPort.Read(readBuffer, 0, mSerialPort.ReadBufferSize);
                    //    String SerialIn = System.Text.Encoding.ASCII.GetString(readBuffer, 0, count);
                    //    //if (count != 0)
                    //        //byteToHexStr(readBuffer);   
                    //        //Thread(byteToHexStr(readBuffer, count));
                        int length = mSerialPort.BytesToRead;
                        byte[] data = new byte[length];
                        mReceivedCount += length;
                        //new Thread(() =>
                        //{
                        //try
                        //{
                        //    this.Dispatcher.BeginInvoke(new Action(() =>
                        //    {
                        //        SetReceivedDataCount(mReceivedCount);
                        //    }));
                        //}
                        //catch (Exception)
                        //{

                        //}
                        //}).Start();
                        //    Dispatcher.Invoke(new UpdateReceivedCount(SetReceivedDataCount),mReceivedCount);
                        try
                        {
                            for (int i = 0; i < length; i++)
                            {
                                data[i] = (byte)mSerialPort.ReadByte();
                            }
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    //BackgroundWorker _backgroundWorker = new BackgroundWorker();
                    //_backgroundWorker.DoWork += UpdateBytesbox__;
                    //_backgroundWorker.RunWorkerAsync(5000);
                    //更新UI
                    Dispatcher.Invoke(new UpdateReceivedCount(SetReceivedDataCount), mReceivedCount);
                    Dispatcher.Invoke(new UpdateBytesDelegate(UpdateBytesbox), data);
                    
                }
                else
                {
                    TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, 50);
                    Thread.Sleep(waitTime);
                }
            }   
        }

        /// <summary>
        /// 变量，与控件绑定，用户选择是否停止显示接收到的数据
        /// </summary>
        public bool IsStop { get; set; }

        /// <summary>
        /// 重置发送、接收 计数 按钮单击事件
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
        public delegate void ClearReceivedArea();
        /// <summary>
        /// 清除接收显示按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClearReceivedArea_Click(object sender, RoutedEventArgs e)
        {
            //new Thread(() =>
            //{
            //    try
            //    {
            //        this.Dispatcher.Invoke(new Action(() =>
            //        {
            //            InputReceivedDataArea.Document.Blocks.Clear();
            //        }));
            //    }
            //    catch (Exception)
            //    {

            //    }
            //}).Start();
            Dispatcher.Invoke(new ClearReceivedArea(clearTextBlock));

        }

        /// <summary>
        /// 清除接收区域的所有数据
        /// </summary>
        private void clearTextBlock()
        {

            InputReceivedDataArea.Document.Blocks.Clear() ;//清除所有的block
            InputReceivedDataArea.Document.Blocks.Add(new Paragraph());//创建新的block
        }

        /// <summary>
        /// 发送按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendData(String str)
        {
            if (ButtonOpen.Content.Equals("打开"))//未打开
            {
                
            }
            if (mSerialPort.SendDataUpdate(str))
            {
                mSendCount+=str.Length;
                SerialSendDataCount.Content=mSendCount.ToString();
            }
        }
        /// <summary>
        /// 单击清除发送区域按钮
        /// </summary>
        private void ClearSendDataArea()
        {
            InputSendDataArea.Document.Blocks.Clear();//清除发送区域
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            if(IsStop)
            {
                _keepReading = false;
            }
            else
            {
                _keepReading = true;
                _readThread = new Thread(ReadSerialPort);
                _readThread.Start();
            }
        }
    }
}
