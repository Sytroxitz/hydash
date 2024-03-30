using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hydash.WebsocketServer
{
	class Program
	{
		// Thread-safe collection to keep track of all connected clients
		private static ConcurrentBag<WebSocket> clients = new ConcurrentBag<WebSocket>();

		public static async Task Main(string[] args)
		{
			Console.Title = "HYDASH - Websocket Server";
			HttpListener httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://localhost:5080/");
			httpListener.Start();
			Console.WriteLine("Listening on port 5080...");
			await Task.Delay(50);
			Console.WriteLine("WebSocket connection established");

			while (true)
			{
				HttpListenerContext listenerContext = await httpListener.GetContextAsync();
				if (listenerContext.Request.IsWebSocketRequest)
				{
					HttpListenerWebSocketContext webSocketContext = await listenerContext.AcceptWebSocketAsync(null);
					WebSocket webSocket = webSocketContext.WebSocket;

					// Add the new WebSocket connection to the collection of clients
					clients.Add(webSocket);

					// Handle each client in a separate task
					Task.Run(() => Echo(webSocket));
				}
				else
				{
					listenerContext.Response.StatusCode = 400;
					listenerContext.Response.Close();
					Console.WriteLine("HTTP connection closed with status code 400");
				}
			}
		}

		static async Task Echo(WebSocket webSocket)
		{
			byte[] buffer = new byte[1024];
			while (webSocket.State == WebSocketState.Open)
			{
				var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				if (result.MessageType == WebSocketMessageType.Close)
				{
					await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
					// Optionally, remove the closed WebSocket from the collection of clients
				}
				else
				{
					Console.WriteLine("Received: " + Encoding.UTF8.GetString(buffer, 0, result.Count));
					// Echo the message back to the client
					//await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
				}
			}
		}
	}
}