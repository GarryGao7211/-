using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false

        }



        bool isOpened = false;//串口状态标志
        private void button1_Click(object sender, EventArgs e)
        {
            if (!isOpened)
            {
                serialPort.PortName = cmbPort.Text;
                serialPort.BaudRate = Convert.ToInt32(cmbBaud.Text, 10);
                try
                {
                    serialPort.Open();     //打开串口
                    button1.Text = "关闭串口";
                    cmbPort.Enabled = false;//关闭使能
                    cmbBaud.Enabled = false;
                    isOpened = true;
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(post_DataReceived);//串口接收处理函数
                }
                catch
                {
                    MessageBox.Show("串口打开失败！");
                }
            }
            else
            {
                try
                {
                    serialPort.Close();     //关闭串口
                    button1.Text = "打开串口";
                    cmbPort.Enabled = true;//打开使能
                    cmbBaud.Enabled = true;
                    isOpened = false;
                }
                catch
                {
                    MessageBox.Show("串口关闭失败！");
                }
            }
        }



        private void post_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {





        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //我们在form载入时，扫描识别存在的串口。
            RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
            if (keyCom != null)
            {
                string[] sSubKeys = keyCom.GetValueNames();
                cmbPort.Items.Clear();
                foreach (string sName in sSubKeys)
                {
                    string sValue = (string)keyCom.GetValue(sName);
                    cmbPort.Items.Add(sValue);
                }
                if (cmbPort.Items.Count > 0)
                    cmbPort.SelectedIndex = 0;
            }
            cmbBaud.Text = "115200";
        }




    }
}
