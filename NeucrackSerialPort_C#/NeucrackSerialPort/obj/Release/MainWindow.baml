   M S B A M L   `   `   `  ���� M  INeucrackSerialPort, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:NeucrackSerialPortNeucrackSerialPort  #    NeucrackSerialPort.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     2local clr-namespace:NeucrackSerialPort       � Title$  
MainWindow��$��623.667��$	��650��$	��472��$	��400��.���  � RowDefinitions �� �� $��25 �.+�]� $��0��.+��� -   $��SettingPanel�� �� Orientation$ Vertical=�.+��  � ColumnDefinitions �� $��60 ��� $��80 � �� �� �� �� �� �� .+��� $��0��$��0��$��2q�$��	串口号���� $��1��$��0��$��2q�$��	波特率���� $��2��$��0��$��2q�$��	数据位���� $��3��$��0��$��2q�$��	校验位���� $��4��$��0��$��2q�$��	停止位���� $��5��$��0��$��2q�$��流控���� -   $��
SerialPort��$��0��$��1��$��2q��� -   $��
SerialBaud�� �� SelectedIndex$ 0��$��1��$��1��$��2q�.@��� .��9600�� .��19200�� .��38400�� .��115200�� .��Custom�� -   $��SerialDataNum��$��2��$��1��$��2q�$ 3��.@��� .��5�� .��6�� .��7�� .��8�� .��9�� -   $��SerialParity��$ 0��$��3��$��1��$��2q�.@��� .��None�� .��Even�� .��Odd�� .��Mark�� .��Space�� -   $��
SerialStop��$ 0��$��4��$��1��$��2q�.@��� .��1�� .��1.5�� .��2�� -   $��SerialFlowControl��$ 0��$��5��$��1��$��2q�.@��� .��None�� .��	RTS/CTS�� .��
XON/XOFF�� -   $��
ButtonOpen��$��5q�$��35��$��打开��d� -	   $��ButtonPause��$��5q�$��35�� d� 	IsChecked �� *IsStop+ �� RelativeSource �� *FindAncestor+ �� AncestorType#   NeucrackSerialPort.MainWindow"  ����  �� 
TargetType" d�	 �� Triggers	 P� 
 P� Property
 �@d�	IsChecked P� Value$
 TrueY�. ���  �� Property � �� �� Value$ 继续显示����  � ��$ 	ExtraBold"�P� 
 �@d�	IsChecked$ FalseY�. ���  � ��$ 暂停显示���� $!��-------接收设置--------���� $ Vertical=�.+��� -
   $��CheckboxHexDisplay��$��十六进制显示��$��2q� �� *Port.IsHexData+�� -   $��CheckboxShowTime��$��显示接收时间��$��2q��� -   $��CheckboxPutToFile��$��接收转向文件��$��2q�� $��2q� �� �� ��  �� �� .+��� -   $��CheckboxAutoReturn��$��0��$��0��$��自动换行��$��Center=�$��Center=��� -   $��InputAutoReturnMinTime��$��1��$��0��$	��500�� �� TextAlignment$ Center=�$��Center=�$��Center=��� $��2��$��0��$��ms���� $ 
Horizontal=�.+��� -   $��ButtonSaveToFile��$��5q�$��30��$��存到文件���� -   $��ButtonClearReceivedArea��$��5q�$��30��$��清除显示���� $!��-------发送设置--------���� -   $��CheckboxUseFileSource��$��启用文件数据源��$
��Left=�$��10,2,2,2q�$��Center=��� -   $��CheckboxHexSend��$��十六进制发送��$
��Left=�$��10,2,2,2q�$��Center=��� -   $��CheckboxContinualSend��$��数据流循环发送��$
��Left=�$��10,2,2,2q�$��Center=�� $��10,2,2,2q� �� $	��34* ��� $	��15* ��� $
��Auto � �� �� .+��� $��0��$��0��$��循环时间��$
��Left=�$��Center=��� -   $��InputSendContinalTime��$��1��$��0��$	��500��$ Center=�$��Center=�$��Center=��� $��2��$��0��$��ms���� -    ��Name$ displayPanel��.+��� -    ��Name$ InputReceivedDataArea��" �� VerticalScrollBarVisibility$
 Auto=�$ �� HorizontalScrollBarVisibility$
 Auto=�$	��313��$��10q�� $��	10,3,10,0q� �� �� $��50 �.+��� -   $ InputSendDataArea��$
 Auto=�$
 Auto=�$	��100��$��0��$��3q��� $��1��$ Vertical=�.+��� -    ��Name$ ButtonSendData��$��发送��$��40��$��6,3,4,5q��� -   $ ButtonSaveSendData��$��20��$��3,3,0,5q�$��40��$��保存���� -   $��ButtonClearSendArea��$��6,3,5,5q�$��清空���  �� $��3* ��� .+��� -    ��Name$ SavedDataList��$��0��$��15,0,2,0q�$��60���� -   $ ButtonClearOneSavedData��$��1��$��     60��$��3,2,16,2q�$��清除这条���  �� $
��121* ��� $
��130* ��� $
��126* ��� $
��125* �.+��� -   $��ButtonLoadFile��$��0��$
��32,5q�$��载入文件���� -   $��TextSendFileHint��$��1��$��Center=�$��16��$
��0,12q�还未载入文件�� -   $��ButtonSendFile��$��2��$
��33,5q�$��发送文件��� $��1�� J� RenderTransformOrigin$ 
0.555,0.52&� �� $
��160* ��� $
��355* ��� $	��77* �.+��� $��0��$ 
Horizontal=�$	��200��$��2��$��0,0,3,0q�.+��� $��	状态：���� -    $��SerialStatus��$��未连接-就绪��
���  ���� $��       ���� $��1��$ 
Horizontal=�.+��� $��已发:���� -!   $��SerialSendDataCount��$��0���� $��已收:���� -"   $��SerialReceivedDataCount��$��0���� -#   $ ��ButtonResetSerialDataCount��$��2��$��Right=�$��复位计数��$��0,0,5,1q�$��52��$��23��$��Bottom=�