using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;

namespace form1
{
   

    public partial class serial : Form
    {
        public serial()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        bool isOpened = false;//串口状态标志
        private void button1_Click_1(object sender, EventArgs e)
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
        const int data_length = 13;

        List<byte> list_buffer = new List<byte>(512);


        private void post_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[serialPort.BytesToRead]; //获得缓冲区大小

            serialPort.Read(buffer, 0, buffer.Length); //存入buffer

            list_buffer.AddRange(buffer);

            while (list_buffer.Count > 4)  //自发自收跟Write一个协议
            {
                if (list_buffer[0] == 0xA1 && list_buffer[1] == 0x33 && list_buffer[3] == 0x0C)
                {
                    if (list_buffer.Count < data_length)
                    {
                        return;
                    }
                    else if (list_buffer[2] == 0xFE ) //经度判断
                    {
                        

                        byte[] real_data_lon = new byte[data_length];
                        list_buffer.CopyTo(0, real_data_lon, 0, data_length);

                        byte receive_check_lon = 0;

                        for(int i = 0;  i < real_data_lon.Length -1; i++)
                        {
                           receive_check_lon ^= real_data_lon[i];
                          //real_data_lon[data_length - 1] = real_data_lon[i];
                        }
                        
                        if (receive_check_lon != real_data_lon[data_length - 1])

                        //if(real_data_lon[data_length -1] != read_check_lon)
                        {
                            MessageBox.Show("经度数据出错");
                            return;
                        }
                       
                        list_buffer.RemoveRange(0, data_length);
                        double tmp_lon = BitConverter.ToDouble(real_data_lon, 4);

                        textBox1.Text = "";
                        textBox1.Text = tmp_lon.ToString();
                    }


                    else if (list_buffer[2] == 0xFF)//纬度判断
                    {
                        
                        byte[] real_data_lat = new byte[data_length];
                        list_buffer.CopyTo(0, real_data_lat, 0, data_length);

                       byte receive_check_lat = 0;
                        
                        for (int j =0; j< real_data_lat.Length -1 ; j++)
                        {
                            receive_check_lat ^= real_data_lat[j];
                            //real_data_lat[data_length - 1] = real_data_lat[j];
                        }

                        if(receive_check_lat != real_data_lat[data_length -1])
                       // if (real_data_lat[data_length - 1] != read_check_lat)
                        {
                            MessageBox.Show("纬度数据出错");
                            return;
                        }
                        
                        list_buffer.RemoveRange(0, data_length);
                        double tmp_lat = BitConverter.ToDouble(real_data_lat, 4);
                        textBox2.Text = "";
                        textBox2.Text = tmp_lat.ToString(); //显示纬度
                    }


                    else if (list_buffer[2] == 0xFD)//高度判断
                    {
                        
                        byte[] real_data_hei = new byte[data_length];
                        list_buffer.CopyTo(0, real_data_hei, 0, data_length);

                        byte receive_check_hei = 0;

                        for (int k = 0; k < real_data_hei.Length - 1; k++)
                        {
                            receive_check_hei ^= real_data_hei[k];
                           // real_data_hei[data_length - 1] = real_data_hei[k];

                        }
                        if (receive_check_hei != real_data_hei[data_length - 1])
                        //if (real_data_hei[data_length - 1] != read_check_hei)
                        {
                            MessageBox.Show("高度数据出错");
                            return;
                        }
                        list_buffer.RemoveRange(0, data_length);
                        double tmp_hei = BitConverter.ToDouble(real_data_hei, 4);
                        textBox3.Text = "";
                        textBox3.Text = tmp_hei.ToString(); //显示高度
                    }
                }
                else
                {
                    list_buffer.RemoveAt(0);
                }

            }        

        }

        private void ReceiveTbox_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cmbPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbBaud_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SendTbox_TextChanged(object sender, EventArgs e)
        {

        }

        byte read_check_lon = 0; //检测最后一个字节的变量
        byte read_check_lat = 0;
        byte read_check_hei = 0;

        private void 发送数据_Click(object sender, EventArgs e)
        {
            //textBox1.Text = "";
            //textBox2.Text = "";
            //textBox3.Text = "";//发送数据之前清空收到的数据

            byte[] data_three = new byte[13]; 
            data_three[0] = 0xA1;
            data_three[1] = 0x33;    //帧头
           
            data_three[3] = 0x0C;    //数据长度




            if (!serialPort.IsOpen)
            {
                MessageBox.Show("串口没有打开");
            }
            if (serialPort.IsOpen && SendTbox1.Text.Trim() == "")
            {
                MessageBox.Show("经度发送框没有数据");
                return;
            }
            if (serialPort.IsOpen && SendTbox2.Text.Trim() == "")
            {
                MessageBox.Show("纬度发送框没有数据");
                return;
            }
            if (serialPort.IsOpen && SendTbox3.Text.Trim() == "")
            {
                MessageBox.Show("高度发送框没有数据");
                return;
            }

            else
            {
                double lon = Convert.ToDouble(SendTbox1.Text);
                double lat = Convert.ToDouble(SendTbox2.Text);
                double hei = Convert.ToDouble(SendTbox3.Text);

                if (lon > 180 || lon < -180)
                {
                    MessageBox.Show("经度输入错误！");
                    SendTbox1.Text = "";
                    return;
                }
                if (lat > 180 || lat < -180)
                {
                    MessageBox.Show("纬度输入错误！");
                    SendTbox2.Text = "";
                    return;
                }
                if (hei <= 0)
                {
                    MessageBox.Show("高度输入错误！");
                    SendTbox3.Text = "";
                    return;
                }


                byte[] tmp_lon = BitConverter.GetBytes(lon);
                data_three[2] = 0xFE; //改为经度命令字

                tmp_lon.CopyTo(data_three, 4);

                for (int i = 0; i < data_three.Length - 1; i++)
                {
                    data_three[data_three.Length - 1] ^= data_three[i];
                }

                read_check_lon = data_three[data_three.Length - 1]; //存入经度校验字节

                serialPort.Write(data_three, 0, data_three.Length); //串口写入经度数据
                data_three[data_three.Length - 1] = 0; //清空校验数据



                byte[] tmp_lat = BitConverter.GetBytes(lat);

                data_three[2] = 0xFF; //改为纬度命令字

                tmp_lat.CopyTo(data_three, 4);
                for (int i = 0; i < data_three.Length - 1; i++)
                {
                    data_three[data_three.Length - 1] ^= data_three[i];
                }
                read_check_lat = data_three[data_three.Length - 1];//存入纬度校验字节
                serialPort.Write(data_three, 0, data_three.Length);//串口写入纬度数据
                data_three[data_three.Length - 1] = 0; //清空校验数据



                byte[] tmp_hei = BitConverter.GetBytes(hei);

                data_three[2] = 0xFD; //改为高度命令字

                tmp_hei.CopyTo(data_three, 4);
                for (int i = 0; i < data_three.Length - 1; i++)
                {
                    data_three[data_three.Length - 1] ^= data_three[i];
                }
                read_check_hei = data_three[data_three.Length - 1];//存入高度校验字节
                serialPort.Write(data_three, 0, data_three.Length);//串口写入高度数据
                data_three[data_three.Length - 1] = 0; //清空校验数据

            }
        }



        private void serial_Load(object sender, EventArgs e)
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
