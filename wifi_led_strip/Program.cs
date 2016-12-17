using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using JsonConfig;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Globalization;
using System.Drawing;

namespace wifi_led_strip
{
	class MainClass
	{
		private static dynamic Configuration;
		private static List<byte> msg;
		private static IPEndPoint endpoint;
		private static Socket _socket;
		private static Dictionary<String, bool> LEDStatus;

		public static void Main(string[] args)
		{

			if (!File.Exists("wifi-led.json"))
			{
				Console.WriteLine("Error: Could not find wifi-led.json configuration file.");
				return;
			}

			var ConfigReader = new StreamReader("wifi-led.json");
			Configuration = Config.ApplyJson(ConfigReader.ReadToEnd(), new ConfigObject());

			LEDStatus = new Dictionary<string, bool>();

			// create client instance
			MqttClient client = new MqttClient(IPAddress.Parse((String)Configuration.wifi_led_mqtt.mqtt_broker_ip));

			// register to message received
			client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

			string clientId = Guid.NewGuid().ToString();
			client.Connect(clientId);

			// subscribe to the topic "/home/temperature" with QoS 2
			client.Subscribe(new string[] { (String)Configuration.wifi_led_mqtt.mqtt_command_topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

			/*
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
			*/
			Console.WriteLine("Start-Up successful! Waiting for MQTT messages...");
		}

		#region RGB Wifi Helpers

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
				Console.WriteLine("Exception: " + ex.Message+" - "+ex.StackTrace);
			}
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
		#endregion

		#region Color Helper
		private static Color FromHex(string hex)
		{
			if (hex.StartsWith("#"))
				hex = hex.Substring(1);

			if (hex.Length != 6) throw new Exception("Color not valid");

			return Color.FromArgb(
				int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
				int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
				int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
		}
		#endregion

		/// <summary>
		/// Handle MQTT Messages being passed in...
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
		{
			String Message = Encoding.UTF8.GetString(e.Message);

			// handle message received
			//Console.WriteLine(e.Topic + " - " + Encoding.UTF8.GetString(e.Message));

			// we have received a message

			// Step 1: filter out topic and get IP adress of the led strip
			String IP = e.Topic.Remove(0, ((String)Configuration.wifi_led_mqtt.mqtt_filter_topic).Length);

			//Console.WriteLine("IP: " + IP);

			endpoint = new IPEndPoint(IPAddress.Parse(IP), 5577);
			_socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_socket.Connect(endpoint);

			Color cc1 = FromHex(Message);

			//if (cc1.Name)

			//Console.WriteLine("R: " + cc1.R+ " G:"+cc1.G+ " B:"+cc1.B);

			if (Message == "#000000")
			{
				if (LEDStatus.ContainsKey(IP))
					LEDStatus[IP] = false;
				else
					LEDStatus.Add(IP, false);
				
				// turn off
				msg = new List<byte>();
				msg.AddRange(new byte[] { 0x71, 0x24, 0x0f });
				SendPacket(msg);

				//Console.WriteLine("Turning off");
			}
			else
			{
				if (LEDStatus.ContainsKey(IP))
				{
					if (!LEDStatus[IP])
					{
						// it's off...

						// turn it on
						msg = new List<byte>();
						msg.AddRange(new byte[] { 0x71, 0x23, 0x0f });
						//Console.WriteLine("Turning on");
						SendPacket(msg);

						Thread.Sleep(250);
					}
					LEDStatus[IP] = true;
				}
				else
				{
					// turn it on
					msg = new List<byte>();
					msg.AddRange(new byte[] { 0x71, 0x23, 0x0f });
					//Console.WriteLine("Turning on");
					SendPacket(msg);

					Thread.Sleep(250);

					LEDStatus.Add(IP, true);
				}

				//Console.WriteLine("R: " + cc1.R + " G:" + cc1.G + " B:" + cc1.B);
				SetRgb(cc1.R, cc1.G, cc1.B, 255, false);

			}

			_socket.Disconnect(true);

		}

	}
}
