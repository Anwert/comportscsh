using System;
using System.Threading;
using System.IO.Ports;
using System.Management;
using System.Collections.Generic;

namespace comportswinforms
{
	class ComPortsService
	{
		static SerialPort _serialPort;
		Thread _readThread;
		static bool cont { get; set; }
		static string message { get; set; }

		public delegate void AppendText(string msg);
		static AppendText appendText;

        public ComPortsService(AppendText append_text)
        {
			appendText = append_text;
			message = "";
			cont = true;

			// Создать объект класса SerialPort с настройками по умолчанию
			_serialPort = new SerialPort();

			// Указать таймауты
			_serialPort.ReadTimeout = 1;
			_serialPort.WriteTimeout = 1;

			_readThread = new Thread(Read);
			_readThread.Start();
		}

		public void SelectPortAndStart(string port_name)
		{
			if (_serialPort.IsOpen)
				_serialPort.Close();

			_serialPort.PortName = port_name;
			_serialPort.Open();
		}

		public void SendMessage(string message_to_send)
		{
			try
			{
				_serialPort.WriteLine(
					String.Format("<{0}>: {1}", _serialPort.PortName, message_to_send));
			}
			catch
			{
				throw;
			}
		}

		public static void Read()
		{
			while (cont)
			{
				try
				{
					appendText(_serialPort.ReadLine());
				}
				catch
				{
					try
					{
						if (_serialPort.IsOpen)
							_serialPort.WriteLine("");
					}
					catch { }
				}
			}
		}

		public void Close()
		{
			cont = false;
			_readThread.Join();
			_serialPort.Close();
		}

		public IEnumerable<string> GetPortsNames()
		{
			ManagementObjectSearcher searcher =
				new ManagementObjectSearcher("root\\WMI",
					"SELECT * FROM MSSerial_PortName");

			foreach (ManagementObject queryObj in searcher.Get())
				yield return queryObj["PortName"].ToString();
		}
	}
}
