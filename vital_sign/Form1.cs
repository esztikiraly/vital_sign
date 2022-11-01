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
using MindFusion.Charting;
using MindFusion.Drawing;
using System.IO;

namespace vital_sign
{
    public partial class Form1 : Form
    {
        EKG_data series1;
        Timer t = new Timer();
        Random rnd = new Random();
        SerialPort serialPort1 = new SerialPort();
        int i = 0;
        List<double> data = new List<double>();

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.Open();
                btn_open.Enabled = false;
                btn_closed.Enabled = true;
                label1.Text = "Port is opened.";
                progressBar1.Visible = true;
                progressBar1.Value = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            GetData();

        }

        private void GetData()
        {
            string output = ASCIIToDecimal(serialPort1.ReadLine()).ToString();

            StreamWriter sw = new StreamWriter("data.txt");
            string[] s = output.Split('1');

            for (int i = 0; i < s.Length; i++)
            {
                sw.WriteLine(s[i]);
            }

            sw.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
            btn_closed.Enabled = false;
            btn_open.Enabled = true;
            progressBar1.Visible = false;
            
        }
        private void Form1_Close(object sender, EventArgs e)
        {
            serialPort1.Close();
            Application.Exit();


        }


        private void btn_closed_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();
                btn_closed.Enabled = false;
                btn_open.Enabled=true;
                label1.Text = "Port is closed.";
                progressBar1.Value=0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        
        public Form1()
        {
            InitializeComponent();
            series1 = new EKG_data(DateTime.Now, DateTime.Now, DateTime.Now.AddMinutes(1));
            series1.DateTimeFormat = DateTimeFormat.LongTime;
            series1.LabelInterval = 10;
            series1.MinValue = 0;
            series1.MaxValue = 120;
            series1.Title = "ECG";
            series1.SupportedLabels = LabelKinds.XAxisLabel;


            lineChart1.Series.Add(series1);

            lineChart1.Title = "ECG Data";
            lineChart1.ShowXCoordinates = false;
            lineChart1.ShowLegendTitle = false;

            lineChart1.XAxis.Title = "";
            lineChart1.XAxis.MinValue = 0;
            lineChart1.XAxis.MaxValue = 120;
            lineChart1.XAxis.Interval = 10;

            lineChart1.YAxis.MinValue = 0;
            lineChart1.YAxis.MaxValue = 100;
            lineChart1.YAxis.Interval = 10;

            List<MindFusion.Drawing.Brush> brushes = new List<MindFusion.Drawing.Brush>()
            {
                new MindFusion.Drawing.SolidBrush(Color.BlueViolet),
                new MindFusion.Drawing.SolidBrush(Color.Brown),
                new MindFusion.Drawing.SolidBrush(Color.Coral)
            };

            List<double> thicknesses = new List<double>() { 2 };

            PerSeriesStyle style = new PerSeriesStyle(brushes, brushes, thicknesses, null);
            lineChart1.Plot.SeriesStyle=style;
            lineChart1.Theme.PlotBackground = new MindFusion.Drawing.SolidBrush(Color.White);
            lineChart1.Theme.GridLineColor = Color.LightGray;
            lineChart1.Theme.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            lineChart1.TitleMargin = new MindFusion.Charting.Margins(10);
            lineChart1.GridType = GridType.Horizontal;



            t.Tick += T_Tick;
            t.Interval = 100;
           
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                t.Start();

                StreamReader sr = new StreamReader("data.txt",Encoding.Default);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line!="")
                    {
                        data.Add(double.Parse(line));
                    }
                    
                }

             sr.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            t.Stop();
        }



        private void T_Tick(object sender, EventArgs e)
        {
            GetData();

            try
            {
                double val = data[i];
                i++;
                // double val = 20;
                series1.addValue(val);
                Console.WriteLine(val);
            }
            catch (Exception ex)
            {

                t.Stop();
            }

            if (series1.Size > 1)
            {
                double currVal = series1.GetValue(series1.Size - 1, 0);
                if (currVal > lineChart1.XAxis.MaxValue)
                {
                    double span = currVal - series1.GetValue(series1.Size - 2, 0);
                    lineChart1.XAxis.MinValue += span;
                    lineChart1.XAxis.MaxValue += span;
                }
                lineChart1.ChartPanel.InvalidateLayout();
            }

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

