using hydash.Shared.Enums;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace websocket
{
	public class Websocket
	{
		private string publicIp;
		private static string toSend;

		public static async Task Connect(ConnectionType connectionType)
		{
			using (ClientWebSocket client = new ClientWebSocket())
			{
				Uri serverUri = new Uri("ws://localhost:5080/");
				await client.ConnectAsync(serverUri, CancellationToken.None);
				Console.WriteLine("Connected to the server");

				await SendMessages(client, connectionType);
			}
		}

		private static async Task SendMessages(ClientWebSocket client, ConnectionType connectionType)
		{
			byte[] receiveBuffer = new byte[1024];
			while (client.State == WebSocketState.Open)
			{
				string publicIp = await publicIP();
				if (connectionType == ConnectionType.Client)
				{
					toSend = "Client (" + publicIp + ") is connected!";
				}
				else
				{
					toSend = "Server (HYDASH.Server) is connected!";
				}
				byte[] bytesToSend = Encoding.UTF8.GetBytes(toSend);
				await client.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

				var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
				string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
				Console.WriteLine("[SERVER] " + receivedMessage);

				// Wait a bit between sending messages
				await Task.Delay(1000);
			}
		}

		private static async Task<string> publicIP()
		{
			using (HttpClient httpClient = new HttpClient())
			{
				try
				{
					// This service returns your public IP address in plain text
					string publicIP = await httpClient.GetStringAsync("https://api.ipify.org");
					return publicIP;
				}
				catch (HttpRequestException e)
				{
					Console.WriteLine("Error getting public IP address: " + e.Message);
				}
			}
			return "Unknown";
		}
	}
}