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
			int Port ;
			int Temperature;

			if (args.Length<2)
			{
				Console.WriteLine("Usege: SetTPTemp.exe <Port> <Temperature>\n writen by Ofer Fridman ofer.fridman@mail.huji.ac.il");
			}
			else
			{
				try
				{
					Port = System.Convert.ToInt32(args[0]);
					Temperature = System.Convert.ToInt32(args[1]);
				}
				catch
				{
					Console.WriteLine("Parameters sould be numeric \n Usege: SetTPTemp.exe <Port> <Temperature>");
					return;
				}
				
				string portName = "COM" + Port.ToString();
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
				}
				catch (TimeoutException) { }
			}
		}


	}
}