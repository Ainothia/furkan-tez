using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("Ardunio WebSocket connected.");
        
        await HandleWebSocketConnection(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

async Task HandleWebSocketConnection(WebSocket webSocket)
{
    var buffer = new byte[1024]; // Buffer to store received data

    while (webSocket.State == WebSocketState.Open)
    {
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            Console.WriteLine("WebSocket bağlantısı client tarafından kapatıldı");
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
        }
        else
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received message: {message}");

            await webSocket.SendAsync(
                Encoding.UTF8.GetBytes($"Server received: {message}"),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );

            await ProcessReceivedData(message);
        }
    }
}

async Task ProcessReceivedData(string data)
{
    Console.WriteLine($"Processing received data: {data}");
    // Front'a data gönderilecek
}

app.Run();