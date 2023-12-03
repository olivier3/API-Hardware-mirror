using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Primitives;

class WS
{
    public static async Task HandleConnection(WebSocket webSocket, HouseDataDb db, StringValues userId)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult? receiveResult;

        var item = db.HouseData.FirstOrDefault(x => x.UserId.ToString() == userId.ToString());

        string jsonMessage =
                "{" +
                    "\"temperature\":" + item.Temperature + "," +
                    "\"humidity\":" + item.Humidity +
                "}";
                
        byte[] bytes = Encoding.UTF8.GetBytes(jsonMessage);
        ArraySegment<byte> bufferSend = new ArraySegment<byte>(bytes);
        await webSocket.SendAsync(bufferSend, WebSocketMessageType.Text, true, CancellationToken.None);

        do
        {
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        while (!receiveResult.CloseStatus.HasValue);

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }

    public static async Task<IResult> Notify(ConcurrentDictionary<string, WebSocket> webSocketsDict,
        HouseDataDb db, HttpContext httpContext)
    {
        Console.WriteLine("Notify called");
        WebSocketReceiveResult? receiveResult = null;
        IQueryable<HouseData> items = null;

        EspIdJSON espId = await JsonSerializer.DeserializeAsync<EspIdJSON>(httpContext.Request.Body);
        items = db.HouseData.Where(x => x.EspId.ToString() == espId.espId);

        if (items != null)
        {
            foreach (var item in items)
            {
                if (webSocketsDict.TryGetValue(item.UserId.ToString(), out WebSocket ws))
                {
                    string jsonMessage =
                    "{" +
                        "\"temperature\":" + item.Temperature + "," +
                        "\"humidity\":" + item.Humidity +
                    "}";
                    byte[] bytes = Encoding.UTF8.GetBytes(jsonMessage);
                    ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                    await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }

            return Results.Ok();
        }

        var noConnection = JsonSerializer.Deserialize<MessageJSON>("{\"message\":\"No user conection found\"}");
        return Results.BadRequest(noConnection);
    }
}