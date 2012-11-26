/*
 * Created by SharpDevelop.
 * User: Tecan
 * Date: 15/11/2012
 * Time: 08:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO.Ports;
using System.Threading;

namespace SetTPTemp
{
	class Program
	{
		static bool _continue;
		static SerialPort SP ;

		public static void Main(string[] args)
		{
			Thread readThread = new Thread(Read);
			string portName;
			int Temperature;

			if (args.Length<1)
			{
				Console.WriteLine("Usege: SetTPTemp.exe <Port name> <Temperature>\nTo get all avalbale ports use: SetTPTemp.exe -p \nwriten by Ofer Fridman ofer.fridman@mail.huji.ac.il\nSource avalbe at https://github.com/oferfrid/SetTPTemp");
			}
			else if(args.Length<2)
			{
				// Get a list of serial port names.
				string[] ports = SerialPort.GetPortNames();

				Console.WriteLine("The following serial ports names were found:");

				// Display each port name to the console.
				foreach(string port in ports)
				{
					Console.WriteLine(port);
				}
			}
			else
			{
				try
				{
					portName = args[0];
					Temperature = System.Convert.ToInt32(args[1]);
				}
				catch
				{
					Console.WriteLine("Temperature arameter sould be numeric \n Usege: SetTPTemp.exe <Port name> <Temperature>");
					return;
				}
				
				int baudRate = 9600;
				Parity parity = Parity.None;
				int dataBits =8;
				StopBits stopBits = StopBits.One;
				SP = new SerialPort(portName,baudRate,parity,dataBits,stopBits);
				SP.Open();
				SP.ReadTimeout = 500;
				SP.WriteTimeout = 500;

				_continue = true;
				readThread.Start();

				SP.Write("n" + Temperature.ToString() + "\r");
				System.Threading.Thread.Sleep(500);
				_continue = false;
				readThread.Join();
				SP.Close();
			}
			

		}

		public static void Read()
		{
			while (_continue)
			{
				try
				{
					string message = SP.ReadExisting();
					Console.Write(message);
					message = string.Empty;
				}
				catch (TimeoutException) { }
			}
		}


	}
}