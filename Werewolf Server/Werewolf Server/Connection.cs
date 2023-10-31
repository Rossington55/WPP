using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

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


        public async void Broadcast(Message message)
        {
            Log(message);

            //Convert to json string
            string jsonString = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(jsonString);

            //Only send if socket is still open
            if (socket.State == WebSocketState.Open)
            {
                //socket black magic witchcraftery
                var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                await socket.SendAsync(arraySegment,
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private void Log(Message message)
        {
            string output = $"Sending: {message.commandClient} to {message.player}";
            Console.WriteLine(output);
        }
    }
}
