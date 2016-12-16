using System;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace wifi_led_strip
{
	class MainClass
	{
		static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
		{
			// handle message received
			Console.WriteLine(e.Topic + " - " + Encoding.UTF8.GetString(e.Message));
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
		}
	}
}
