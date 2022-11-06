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
        List<double> data = new List<double>();
        double R1 = 0;
        double R2 = 0;

        public Form1()
        {
            InitializeComponent();
            series1 = new EKG_data(DateTime.Now, DateTime.Now, DateTime.Now.AddMinutes(3));
            series1.DateTimeFormat = DateTimeFormat.LongTime;
            series1.LabelInterval = 50;
            series1.MinValue = 0;
            series1.MaxValue = 120;
            series1.Title = "ECG";
            series1.SupportedLabels = LabelKinds.XAxisLabel;

            lineChart1.Series.Add(series1);
            lineChart1.Title = "ECG Data";
            lineChart1.ShowXCoordinates = false;
            lineChart1.ShowLegendTitle = false;
            lineChart1.XAxis.Title = "Time in milliseconds";
            lineChart1.YAxis.Title = "mm";

            lineChart1.XAxis.MinValue = 0;
            lineChart1.XAxis.MaxValue = 2;
            lineChart1.XAxis.Interval = 0.004;

            lineChart1.YAxis.MinValue = 0;
            lineChart1.YAxis.MaxValue = 270;
            lineChart1.YAxis.Interval = 20;

            List<MindFusion.Drawing.Brush> brushes = new List<MindFusion.Drawing.Brush>()
            {
                new MindFusion.Drawing.SolidBrush(Color.BlueViolet),
                new MindFusion.Drawing.SolidBrush(Color.Brown),
                new MindFusion.Drawing.SolidBrush(Color.Coral)
            };

            List<double> thicknesses = new List<double>() { 2 };

            //chart styles
            PerSeriesStyle style = new PerSeriesStyle(brushes, brushes, thicknesses, null);
            lineChart1.Plot.SeriesStyle = style;
            lineChart1.Theme.PlotBackground = new MindFusion.Drawing.SolidBrush(Color.White);
            lineChart1.Theme.GridLineColor = Color.LightGray;
            lineChart1.Theme.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            lineChart1.TitleMargin = new MindFusion.Charting.Margins(10);
            lineChart1.GridType = GridType.Horizontal;

            //timer
            t.Tick += T_Tick;
            t.Interval = 4;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
            btn_closed.Enabled = false;
            btn_open.Enabled = true;
            btn_start.Enabled = false;
            btn_stop.Enabled = false;
        }


        //Opening the port
        private void btn_start_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = 9600;
                serialPort1.Open();
                btn_open.Enabled = false;
                btn_closed.Enabled = true;
                label1.Text = "Port is opened.";
                btn_start.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
       

        //closing the port
        private void btn_closed_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();
                btn_closed.Enabled = false;
                btn_open.Enabled=true;
                label1.Text = "Port is closed.";
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
       

        //start the visualisation
        private void btn_start_Click_1(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
               // if (ujra) lineChart1.Series.Remove(series1);
                t.Start();
                btn_stop.Enabled = true;
                btn_start.Enabled = false;
              btn_closed.Enabled = false;
            }
        }

        //Stop(freeze) the visualisation
        private void btnSop_Click(object sender, EventArgs e)
        {
            t.Stop();
            btn_start.Enabled=true;
            btn_stop.Enabled = false;
            btn_closed.Enabled = true;
        }


        double sum =0;
        int counter = 0;
        double avg = 0;

        private void T_Tick(object sender, EventArgs e)
        {

            //Get the data from the port 
            byte incoming_data = (byte)serialPort1.ReadByte();


            //Filter data
            

            //calculating avg and sum
            sum += incoming_data;
            counter++;
            avg = sum / counter;

            //show data on the chart
            double val = incoming_data / 1.0;
                series1.addValue(val);
            Console.WriteLine(val);

            

            //heart rate calculation
            if (incoming_data > 10)
            {
                if (R1 == 0)
                {
                    R1 = counter;
                }
                else
                {
                    if (R2 == 0)
                    {
                        R2 = counter;
                        double RR = R2 - R1;
                       // lbl_hr.Text = (1500/RR).ToString();
                        R1 = 0;
                        R2 = 0;
                    }
                   
                }

               

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

       















    //Other
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


        private void Form1_Close(object sender, EventArgs e)
        {
            serialPort1.Close();
            Application.Exit();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}

