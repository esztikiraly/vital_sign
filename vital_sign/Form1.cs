using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace vital_sign
{
    public partial class Form1 : Form
    {
        SerialPort serial_port = new SerialPort();
        Timer t = new Timer();
        UInt16 dataSensor1, dataSensor2, dataSensor3;
        public Form1()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
            SerialPort serial_port = new SerialPort();
            label1.Text = "Port is closed";
            t.Interval = 500;
            t.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serial_port.PortName = comboBox1.Text;
                serial_port.Open();
                label1.Text = "Port is open.";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }



        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                serial_port.Close();
                label1.Text = "Port is closed.";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (serial_port.IsOpen)
                {
                    t.Enabled = true;
                    t.Tick += T_Tick;
                    GetData();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

     

        private void T_Tick(object sender, EventArgs e)
        {



            GetData();

        }

        private void button4_Click(object sender, EventArgs e)
        {
        
        }


        //Methods
        private void GetData()
        {
            string incoming_data = ASCIIToDecimal(serial_port.ReadExisting().ToString());
            richTextBox1.Text += incoming_data;
        }
        public static string ASCIIToDecimal(string str)
        {
            string dec = string.Empty;

            for (int i = 0; i < str.Length; ++i)
            {
                string cDec = ((byte)str[i]).ToString();

                if (cDec.Length < 3)
                    cDec = cDec.PadLeft(3, '0');

                dec += cDec;
            }

            return dec;
        }
    }
}
