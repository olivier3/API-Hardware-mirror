using System.Collections.Concurrent;
using System.Data.Common;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

class WS
{
    public static async Task Echo(WebSocket webSocket, ConcurrentDictionary<string, WebSocket> webSocketsDict)
    {

        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult receiveResult = null;

        do
        {
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            if (receiveResult.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                var receivedMessage = JsonConvert.DeserializeObject<TemperatureJSON>(message);

                Console.WriteLine($"Received: {receivedMessage}");
            }

            foreach (var item in webSocketsDict)
            {
                await item.Value.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);
            }
        }
        while (!receiveResult.CloseStatus.HasValue);


        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);

    }
}