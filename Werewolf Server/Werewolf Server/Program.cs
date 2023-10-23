using System.Net;
using System.Text;
using System.Net.WebSockets;
using Werewolf_Server;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:8080/");
var app = builder.Build();

app.UseWebSockets();
GameManager mrBob = new GameManager();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var ws = await context.WebSockets.AcceptWebSocketAsync();

        //Let this user know they're connected
        Connection connection = new Connection("",ws);
        connection.Broadcast("Connected");

        //Decode then...
        await RecieveMessage(ws,
            async (result, buffer) =>
            {
                //Normal message
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string[] message = Encoding.UTF8.GetString(buffer, 0, result.Count).Split(";");
                    mrBob.RecieveMessage(ws, message);
                    LogMessage(message);
                }
                //Is closing
                else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
                {
                    await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            });
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
    }
});

//Decode internet magic
async Task RecieveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
{
    var buffer = new byte[1024 * 4];
    while (socket.State == WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        handleMessage(result, buffer);
    }
}
await app.RunAsync();

void LogMessage(string[] message)
{
    string output = "Recieved: ";
    foreach (string msg in message)
    {
        output += msg;
    }
    Console.WriteLine(output);
}
async Task BroadcastMany(string message, List<WebSocket> recievers)
{
    Console.WriteLine($"Sending {message}");
    var bytes = Encoding.UTF8.GetBytes(message);
    foreach (var socket in recievers)
    {
        if (socket.State == WebSocketState.Open)
        {
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await socket.SendAsync(arraySegment,
                WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
