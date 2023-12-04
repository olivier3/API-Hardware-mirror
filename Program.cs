using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HouseDataDb>();

var app = builder.Build();

// Set the API port that it listen on
app.Urls.Add("http://*:15000");

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

// Set the options for the WebSocket
app.UseWebSockets(webSocketOptions);

// API route to connect to a WebSocket.
app.MapGet("/ws",
    async (HouseDataDb db, HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        if (context.Request.Query.Count() == 1 && context.Request.Query["userId"].Count == 1)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            int.TryParse(context.Request.Query["userId"], out int userId);

            WS.webSocketsDict.TryGetValue(userId.ToString(), out List<WebSocket> wsList);
            if (wsList is null)
            {
                wsList = new List<WebSocket>();
            }
            wsList.Add(webSocket);
            WS.webSocketsDict.TryAdd(userId.ToString(), wsList);
            await WS.HandleConnection(webSocket, db, userId);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

// API route to notify the system for updated values.
app.MapPost("/notify",
    (HouseDataDb db, HttpContext httpContext) =>
        WS.Notify(db, httpContext));

// API route to update temperature
app.MapPost("/updateTemp",
    (HouseDataDb db, HttpContext httpContext) => Esp.UpdateTemp(db, httpContext));

// API route to update humidity data
app.MapPost("/updateHumidity",
    (HouseDataDb db, HttpContext httpContext) => Esp.UpdateHumidity(db, httpContext));

// API route to link user to esp32
app.MapPost("/link",
    (HouseDataDb db, HttpContext httpContext) => Mirror.LinkMirror(db, httpContext));

app.Run();
