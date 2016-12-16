/* Originally code was ported to C# by Jasper Siegmund (http://blog.repsaj.nl/index.php/2016/09/iot-aquarium-monitor-controlling-led-from-c/)
 * and adapted by Daniel Kirstenpfad into a C# library to be used
 * 
 * http://github.com/jsiegmund/
 * 
 * */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WifiFluxLedStripLibrary
{
	public class FluxLED
	{
		public FluxLED()
		{
		}
		/*
		async Task DiscoverDevices()
		{
			IPEndPoint broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, 48899);

			// the broadcast message is a fixed one, do not change
			string msg = "HF-A11ASSISTHREAD";
			byte[] msgBytes = Encoding.ASCII.GetBytes(msg);

			UdpClient udp = new UdpClient();
			await udp.SendAsync(msgBytes, msgBytes.Length, broadcastEndPoint);

			do
			{
				// all active devices will reply, select the one we need 
				UdpReceiveResult receiveResult = await udp.ReceiveAsync();
				string returnData = Encoding.ASCII.GetString(receiveResult.Buffer);

				if (returnData.Contains(this._config.Device))
					_deviceAddress = returnData.Substring(0, returnData.IndexOf(','));
			} while (_deviceAddress == null);
		}
		*/

		/*
		// connect the socket to the endpoint
		IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(this._deviceAddress), 5577);
		_socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		_socket.Connect(endpoint);
		*/

		/*
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

		void SendPacket(List<byte> data)
		{
			try
			{
				// add the checksum to the package as last byte
				var checksum = ComputeAdditionChecksum(data.ToArray());
				data.Add(checksum);

				byte[] buffer = data.ToArray();
				int result = _socket.Send(buffer, buffer.Length, SocketFlags.None);

				Debug.WriteLine($"SendPacket returned: {result}");
			}
			catch (Exception ex)
			{

			}
		}
		*/

		/*
		byte[] ReadRaw(int byte_count = 1024)
		{
			byte[] buffer = new byte[byte_count];
			_socket.Receive(buffer);
			return buffer;
		}

		byte[] ReadResponse(int expected)
		{
			var remaining = expected;
			var rx = new List<byte>();

			while (remaining > 0)
			{
				var chunk = ReadRaw(remaining);
				remaining -= chunk.Length;
				rx.AddRange(chunk);
			}

			return rx.ToArray();
		}
		*/

		/*
		void TurnOn(bool on = true)
		{
			List<byte> msg = new List<byte>();
			if (on)
			{
				msg.AddRange(new byte[] { 0x71, 0x23, 0x0f });
			}
			else
			{
				msg.AddRange(new byte[] { 0x71, 0x24, 0x0f });
			}

			SendPacket(msg);
			this.isOn = on;
		}

		void TurnOff()
		{
			TurnOn(false);
		}

		void SetRgb(byte r, byte g, byte b, byte w, bool persist = true)
		{
			List<byte> msg = new List<byte>();

			if (persist)
				msg.Add(0x31);
			else
				msg.Add(0x41);

			msg.Add(r);         // Red
			msg.Add(g);         // Green
			msg.Add(b);         // Blue
			msg.Add(w);         // White

			msg.Add(0x00);
			msg.Add(0x0f);

			SendPacket(msg);
		}
		*/
	}
}
