using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace CommunicateWithArduino
{
	public partial class Form1 : Form
	{
		public static System.IO.Ports.SerialPort port;
		delegate void SetTextCallback(string text);

		// This BackgroundWorker is used to demonstrate the 
		// preferred way of performing asynchronous operations.
		private BackgroundWorker hardWorker;

		private Thread readThread = null;

        public string lineVal { get; set; }

        public delegate void ChangeChart();
        public ChangeChart changeNow;

		public Form1()
		{
			InitializeComponent();

			hardWorker = new BackgroundWorker();
			sendBtn.Enabled = false;

  //          chart1.Series[0].Name = "Volts";
  //          chart1.Series[0].Points.AddXY(1, 1);
  //          chart1.ChartAreas[0].AxisY.Minimum = 0;
  //          chart1.ChartAreas[0].AxisY.Maximum = 5.0;

  //          changeNow = new ChangeChart(ChangeChartValue);
		}

        //private void ChangeChartValue()
        //{
        //    chart1.Series[0].Points.Clear();
        //    try
        //    {
        //        chart1.Series[0].Points.AddXY(1, Convert.ToDouble(lineVal) / 1000);
        //    }
        //    catch
        //    {
        //        chart1.Series[0].Points.AddXY(1, 5);
        //    }
        //}

		private void btnConnect_Click(object sender, EventArgs e)
		{
			System.ComponentModel.IContainer components = 
				new System.ComponentModel.Container();
			port = new System.IO.Ports.SerialPort(components);
			port.PortName = comPort.SelectedItem.ToString();
			port.BaudRate = Int32.Parse(baudRate.SelectedItem.ToString());
			port.DtrEnable = true;
			port.ReadTimeout = 5000;
			port.WriteTimeout = 500;
			port.Open();

			readThread = new Thread(new ThreadStart(this.Read));
			readThread.Start();
			this.hardWorker.RunWorkerAsync();

			btnConnect.Text = "<Connected>";

			btnConnect.Enabled = false;
			comPort.Enabled = false;
			sendBtn.Enabled = true;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			foreach (string s in SerialPort.GetPortNames())
			{
				comPort.Items.Add(s);
			}
			comPort.SelectedIndex = 0;

			baudRate.Items.Add("9600");
			baudRate.Items.Add("115200");

			baudRate.SelectedIndex = 0;
		}

		private void sendBtn_Click(object sender, EventArgs e)
		{
			if (port.IsOpen)
			{
				port.Write(sendText.Text);
			}
		}

		private void SetText(string text)
		{
            if (this.tmpTxtBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.tmpTxtBox.Text = text;
                //chart1.Invoke(changeNow);
            }
		}

		public void Read()
		{
			while (port.IsOpen)
			{
				try
				{
					string message = port.ReadLine();
                    this.lineVal = message;
                    this.SetText(lineVal);
				}
				catch (TimeoutException) { }
			}
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			readThread.Abort();

			port.Close();
		}

		private void clrBtn_Click(object sender, EventArgs e)
		{
			if (port.IsOpen)
			{
				byte[] stuff = {0x7C, 0x00};
				port.Write(stuff, 0, 2);
			}
		}

		private void demoBtn_Click(object sender, EventArgs e)
		{
			if (port.IsOpen)
			{
				byte[] stuff = { 0x7C, 0x04 };
				port.Write(stuff, 0, 2);
			}
		}

		private void stopBtn_Click(object sender, EventArgs e)
		{
			if (port.IsOpen)
			{
                Application.Exit();
			}
		}

	}
}
