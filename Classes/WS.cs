using System.Collections.Concurrent;
using System.Data.Common;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

class WS
{
    public static async Task HandleConnection(WebSocket webSocket, ConcurrentDictionary<string, WebSocket> webSocketsDict, HouseDataDb db)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult? receiveResult = null;
        EspIdJSON espId;
        IQueryable<HouseData> items = null;

        do
        {
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            if (receiveResult.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                Console.WriteLine(message);
                espId = JsonSerializer.Deserialize<EspIdJSON>(message);
                items = db.HouseData.Where(x => x.EspId.ToString() == espId.espId);
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    if (webSocketsDict.ContainsKey(item.UserId.ToString()))
                    {
                        webSocketsDict.TryGetValue(item.UserId.ToString(),out WebSocket ws); 
                        await ws.SendAsync(
                            new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                            receiveResult.MessageType,
                            receiveResult.EndOfMessage,
                            CancellationToken.None);
                    }
                }
            }
        }
        while (!receiveResult.CloseStatus.HasValue);

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);

    }
}