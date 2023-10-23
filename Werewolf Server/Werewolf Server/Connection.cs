using System.Net.WebSockets;
using System.Text;

namespace Werewolf_Server
{
    public class Connection
    {
        public string connectionName;
        public WebSocket socket;
        public Player player;


        public Connection(string connectionName, WebSocket socket)
        {
            this.connectionName = connectionName;
            this.socket = socket;
        }


        public async void Broadcast(string message)
        {
            Console.WriteLine($"Sending {message}");
            var bytes = Encoding.UTF8.GetBytes(message);
            if (socket.State == WebSocketState.Open)
            {
                var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                await socket.SendAsync(arraySegment,
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
