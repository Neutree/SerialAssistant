﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Management;
using System.IO.Ports;


namespace NeucrackSerialPort.model
{
    class SerialPortModel : SerialPort, INotifyPropertyChanged
    {

        public int Setting(String port,int baud,int dataBitNum,Parity parityBitNum,StopBits stopBits,int flowControl,int readBufferSize,int writeBufferSize, int readTimeOut,int writeTimeOut, String Encode)
        {
            try
            {
                base.PortName = port;
                base.BaudRate = baud;
                base.DataBits = dataBitNum;
                base.Parity = parityBitNum;
                base.StopBits = stopBits;
                //流控，暂时未使用该功能
                if (flowControl == 1)
                {
                    //  base.RtsEnable = true;
                }
                base.Encoding = System.Text.Encoding.GetEncoding(Encode);
                base.ReadBufferSize = readBufferSize;
                base.ReadTimeout = readTimeOut;
                base.WriteBufferSize = writeBufferSize;
                base.WriteTimeout = writeTimeOut;
            }
            catch (Exception)
            {
                return -1;
            }
            return 0;//设置成功
        }
        
        public new int Open()
        {
            if (!base.IsOpen)//串口还未打开
            {
                try
                {
                    base.Open();
                }
                catch (System.UnauthorizedAccessException)
                {
                    return -1;
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    return -2;
                }
                catch (System.ArgumentException)
                {
                    return -3;
                }
                catch (System.IO.IOException)
                {
                    return -4;
                }
                catch (System.InvalidOperationException)
                {
                    return -5;
                }
            }
            else//串口已经打开了
            {
                return 1;
            }
            return 0;//打开正常
        }
        public new int Close()
        {
            
            try
            {
                if (base.IsOpen)//已经打开了串口
                {
                    base.Close();
                    base.Dispose();
                }
                else
                    return 1;
            }
            catch (Exception)
            {
                return -1;
            }
            return 0;
        }

        public String[] GetAllSerialPortInfo()
        {
            return GetHarewareInfo(HardwareEnum.Win32_PnPEntity, "Name");
        }


        /// <summary>
        /// Get the system devices information with windows api.
        /// </summary>
        /// <param name="hardType">Device type.</param>
        /// <param name="propKey">the property of the device.</param>
        /// <returns></returns>
        private static string[] GetHarewareInfo(HardwareEnum hardType, string propKey)
        {

            List<string> strs = new List<string>();
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType))
                {
                    var hardInfos = searcher.Get();
                    foreach (var hardInfo in hardInfos)
                    {
                        if (hardInfo.Properties[propKey].Value != null)
                        {
                            String str = hardInfo.Properties[propKey].Value.ToString();
                            if(str.Contains("COM"))
                            strs.Add(str);
                        }

                    }
                }
                return strs.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            {
                strs = null;
            }
        }//end of func GetHarewareInfo().

        /// <summary>
        /// 枚举win32 api
        /// </summary>
        private enum HardwareEnum
        {
            // 硬件
            Win32_Processor, // CPU 处理器
            Win32_PhysicalMemory, // 物理内存条
            Win32_Keyboard, // 键盘
            Win32_PointingDevice, // 点输入设备，包括鼠标。
            Win32_FloppyDrive, // 软盘驱动器
            Win32_DiskDrive, // 硬盘驱动器
            Win32_CDROMDrive, // 光盘驱动器
            Win32_BaseBoard, // 主板
            Win32_BIOS, // BIOS 芯片
            Win32_ParallelPort, // 并口
            Win32_SerialPort, // 串口
            Win32_SerialPortConfiguration, // 串口配置
            Win32_SoundDevice, // 多媒体设置，一般指声卡。
            Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP)
            Win32_USBController, // USB 控制器
            Win32_NetworkAdapter, // 网络适配器
            Win32_NetworkAdapterConfiguration, // 网络适配器设置
            Win32_Printer, // 打印机
            Win32_PrinterConfiguration, // 打印机设置
            Win32_PrintJob, // 打印机任务
            Win32_TCPIPPrinterPort, // 打印机端口
            Win32_POTSModem, // MODEM
            Win32_POTSModemToSerialPort, // MODEM 端口
            Win32_DesktopMonitor, // 显示器
            Win32_DisplayConfiguration, // 显卡
            Win32_DisplayControllerConfiguration, // 显卡设置
            Win32_VideoController, // 显卡细节。
            Win32_VideoSettings, // 显卡支持的显示模式。

            // 操作系统
            Win32_TimeZone, // 时区
            Win32_SystemDriver, // 驱动程序
            Win32_DiskPartition, // 磁盘分区
            Win32_LogicalDisk, // 逻辑磁盘
            Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。
            Win32_LogicalMemoryConfiguration, // 逻辑内存配置
            Win32_PageFile, // 系统页文件信息
            Win32_PageFileSetting, // 页文件设置
            Win32_BootConfiguration, // 系统启动配置
            Win32_ComputerSystem, // 计算机信息简要
            Win32_OperatingSystem, // 操作系统信息
            Win32_StartupCommand, // 系统自动启动程序
            Win32_Service, // 系统安装的服务
            Win32_Group, // 系统管理组
            Win32_GroupUser, // 系统组帐号
            Win32_UserAccount, // 用户帐号
            Win32_Process, // 系统进程
            Win32_Thread, // 系统线程
            Win32_Share, // 共享
            Win32_NetworkClient, // 已安装的网络客户端
            Win32_NetworkProtocol, // 已安装的网络协议
            Win32_PnPEntity,//all device
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
