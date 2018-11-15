using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace comportswinforms
{
	public partial class Form1 : Form
	{
		ComPortsService _comPortsService;
		string currentPort;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			_comPortsService = new ComPortsService(AppendText);
			string[] ports_names = _comPortsService.GetPortsNames().ToArray<string>();
			foreach (string port_name in ports_names)
				comboBox1.Items.Add(port_name);
		}

		public void AppendText(string msg)
		{
			if (!string.IsNullOrEmpty(msg))
				richTextBox1.AppendText(msg + Environment.NewLine);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string message_to_send = textBox1.Text;
			try
			{
				_comPortsService.SendMessage(message_to_send);
			}
			catch { }
			ShowMyMessage(message_to_send);
		}

		private void ShowMyMessage(string message_to_send)
		{
			richTextBox1.AppendText(String.Format("<Me>: {0}", message_to_send) +
				Environment.NewLine);
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				string selected_item_str = (string)comboBox1.SelectedItem;
				_comPortsService.SelectPortAndStart(selected_item_str);
				currentPort = selected_item_str;
			}
			catch
			{
				MessageBox.Show("This port is busy.");
				comboBox1.SelectedItem = currentPort;
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			_comPortsService.Close();
		}
	}
}
