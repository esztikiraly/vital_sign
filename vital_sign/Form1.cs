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
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;

namespace vital_sign
{
    public partial class Form1 : Form
    {
        SerialPort serial_port = new SerialPort();
        Timer t = new Timer();
        Timer t2 = new Timer();
        double rt = 0;
        bool i = false;
        List<string> list = new List<string>();


        public Form1()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
            serial_port = new SerialPort
            {
                Parity = Parity.None,
                Handshake = Handshake.None
            };
            label1.Text = "Port is closed";
            t.Interval = 100;
            t.Enabled = false;
            t2.Interval = 100;
            t2.Enabled = true;
            t2.Tick += T2_Tick;
        }

        private void T2_Tick(object sender, EventArgs e)
        {
            rt = rt + 0.1;
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
                    

                    // GetData();
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



        //Methods


        private void GetData()
        {
            string data = serial_port.ReadLine().ToString();
            richTextBox1.Text += data;
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

        private void Form1_FormClosed(object sender, EventArgs e)
        {
            serial_port.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

     
    }
}
