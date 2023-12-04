using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Primitives;

/// <summary>
/// Class for the WebSocket.
/// </summary>
class WS
{
    /// <summary>
    /// Function to handle the connection to a WebSocket.
    /// </summary>
    /// <param name="webSocket">The connected WebSocket</param>
    /// <param name="db">The database context</param>
    /// <param name="userId">The userId of the of the connected WebSocket</param>
    /// <returns>A Task that handle the WebSocket connections</returns>
    public static async Task HandleConnection(WebSocket webSocket, HouseDataDb db, int userId)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult? receiveResult;
        HouseData? item = db.HouseData.FirstOrDefault(x => x.UserId == userId);
        string jsonMessage = "";

        if (item != null)
        {
            jsonMessage =
                "{" +
                    "\"temperature\":" + item.Temperature + "," +
                    "\"humidity\":" + item.Humidity +
               "}";
        }

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

    /// <summary>
    /// Notify function to call when values in the ESP 32 are updated.
    /// </summary>
    /// <param name="db">Database context</param>
    /// <param name="httpContext">Http request context</param>
    /// <param name="webSocketsDict">Data structure containing all active WebSocket connections</param>
    /// <returns>The result if the request is successfull or not</returns>
    internal async static Task<IResult> Notify(HouseDataDb db, HttpContext httpContext, ConcurrentDictionary<string, WebSocket> webSocketsDict)
    {
        IQueryable<HouseData> items = null;
        EspIdJSON? espId = await JsonSerializer.DeserializeAsync<EspIdJSON>(httpContext.Request.Body);
        items = db.HouseData.Where(x => x.EspId == espId.espId);

        if (items != null && items.Count() > 0)
        {
            foreach (var item in items)
            {
                if (webSocketsDict.TryGetValue(item.UserId.ToString(), out WebSocket ws))
                {
                    string jsonMessage =
                    "{" +
                        "\"notify\":\"Bonjour\"" +
                    "}";

                    byte[] bytes = Encoding.UTF8.GetBytes(jsonMessage);
                    ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                    await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }

            var succesMessage = JsonSerializer.Deserialize<MessageJSON>("{\"message\":\"System notified\"}");
            return Results.Ok(succesMessage);
        }

        var noConnection = JsonSerializer.Deserialize<MessageJSON>("{\"message\":\"No user connection found\"}");
        return Results.BadRequest(noConnection);
    }

    /// <summary>
    /// Function to send temperature data to WebSocket.
    /// </summary>
    /// <param name="db">Database context</param>
    /// <param name="webSocketsDict">Data structure with all active WebSocket connections</param>
    /// <param name="espId">Esp 32 ID</param>
    /// <returns></returns>
    internal async static Task SendWebSocketData(HouseDataDb db, ConcurrentDictionary<string, WebSocket> webSocketsDict, string espId)
    {
        IQueryable<HouseData>? items = null;
        items = db.HouseData.Where(x => x.EspId == espId);

        if (items != null && items.Count() > 0)
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
        }
    }
}