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

namespace vital_sign
{
    public partial class Form1 : Form
    {
        EKG_data series1, series2, series3;
        Timer t = new Timer();
        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            series1 = new EKG_data(DateTime.Now, DateTime.Now, DateTime.Now.AddMinutes(1));
            series1.DateTimeFormat = DateTimeFormat.LongTime;
            series1.LabelInterval = 10;
            series1.MinValue = 0;
            series1.MaxValue = 120;
            series1.Title = "Server 1";
            series1.SupportedLabels = LabelKinds.XAxisLabel;

            series2 = new EKG_data(DateTime.Now, DateTime.Now, DateTime.Now.AddMinutes(1));
            series2.DateTimeFormat = DateTimeFormat.LongTime;
            series2.LabelInterval = 10;
            series2.MinValue = 0;
            series2.MaxValue = 120;
            series2.Title = "Server 2";
            series2.SupportedLabels = LabelKinds.None;

            series3 = new EKG_data(DateTime.Now, DateTime.Now, DateTime.Now.AddMinutes(1));
            series3.DateTimeFormat = DateTimeFormat.LongTime;
            series3.LabelInterval = 10;
            series3.MinValue = 0;
            series3.MaxValue = 120;
            series3.Title = "Server 3";
            series3.SupportedLabels = LabelKinds.None;

            lineChart1.Series.Add(series1);
            lineChart1.Series.Add(series2);
            lineChart1.Series.Add(series3);

            t.Tick += T_Tick;
            t.Interval = 500;
            t.Start();
        }

        private void T_Tick(object sender, EventArgs e)
        {
            double val = rnd.NextDouble()*10+10;
            series1.addValue(val);

            val = rnd.NextDouble()*10+40;
            series2.addValue(val);

            val = rnd.NextDouble() * 10 + 60;
            series3.addValue(val);
            Console.WriteLine(val);

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
    }
}

