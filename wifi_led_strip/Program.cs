using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace wifi_led_strip
{
	class MainClass
	{
		private static Socket _socket;

		static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
		{
			// handle message received
			Console.WriteLine(e.Topic + " - " + Encoding.UTF8.GetString(e.Message));
		}

		public static byte ComputeAdditionChecksum(byte[] data)
		{
			byte sum = 0;
			unchecked // Let overflow occur without exceptions
			{
				foreach (byte b in data)
				{
					sum += b;
				}
			}
			return sum;
		}

		static void SendPacket(List<byte> data)
		{
			try
			{
				// add the checksum to the package as last byte
				var checksum = ComputeAdditionChecksum(data.ToArray());
				data.Add(checksum);

				byte[] buffer = data.ToArray();
				int result = _socket.Send(buffer, buffer.Length, SocketFlags.None);

				Console.WriteLine($"SendPacket returned: {result}");
			}
			catch (Exception ex)
			{

			}
		}

		public static void Main(string[] args)
		{
			// create client instance
			MqttClient client = new MqttClient(IPAddress.Parse("192.168.178.92"));

			// register to message received
			client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

			string clientId = Guid.NewGuid().ToString();
			client.Connect(clientId);

			// subscribe to the topic "/home/temperature" with QoS 2
			client.Subscribe(new string[] { "#" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });


			// connect
			IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("192.168.178.79"), 5577);
			_socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_socket.Connect(endpoint);

			// turn on


			// turn on
			List<byte> msg = new List<byte>();
			msg.AddRange(new byte[] { 0x71, 0x23, 0x0f });
			SendPacket(msg);

			Thread.Sleep(1000);

			// turn off
			msg = new List<byte>();
			msg.AddRange(new byte[] { 0x71, 0x24, 0x0f });
			SendPacket(msg);

			Thread.Sleep(1000);

			// turn on
			msg = new List<byte>();
			msg.AddRange(new byte[] { 0x71, 0x23, 0x0f });
			SendPacket(msg);

			SetRgb(20, 150, 3, 255, false);

			Thread.Sleep(1000);

			for (byte i = 0;i<255;i++)
			{
				Thread.Sleep(10);
				Console.WriteLine(i);
				SetRgb(i, 255, 255, 255, false);	
			}

			for (byte i = 0; i < 255; i++)
			{
				Thread.Sleep(10);
				Console.WriteLine(i);
				SetRgb(255, i, 255, 255, false);
			}

			for (byte i = 0; i < 255; i++)
			{
				Thread.Sleep(10);
				Console.WriteLine(i);
				SetRgb(255, 255, i, 255, false);
			}

			for (byte i = 0; i < 255; i++)
			{
				Thread.Sleep(10);
				Console.WriteLine(i);
				SetRgb(i, 255, i, 255, false);
			}


			// turn off
			msg = new List<byte>();
			msg.AddRange(new byte[] { 0x71, 0x24, 0x0f });
			SendPacket(msg);

		}

		static void SetRgb(byte r, byte g, byte b, byte w, bool persist = true)
		{
			List<byte> msg = new List<byte>();

			/*if (persist)
				msg.Add(0x31);
			else*/

			msg.Add(0x41);
			msg.Add(r);         // Red
			msg.Add(g);         // Green
			msg.Add(b);         // Blue
			//msg.Add(w);         // White

			msg.Add(0x00);
			msg.Add(0xf0);
			msg.Add(0x0f);

			SendPacket(msg);
		}

	}
}
