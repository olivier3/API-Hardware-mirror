using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HouseDataDb>();

var app = builder.Build();

var webSocketsDict = new ConcurrentDictionary<string, WebSocket>();

// Set the API port that it listen on
app.Urls.Add("http://*:15000");

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

app.MapGet("/ws",
    async (HouseDataDb db, HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        if (context.Request.Query.Count() == 1 && context.Request.Query["userId"].Count == 1)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            int.TryParse(context.Request.Query["userId"], out int userId);
            webSocketsDict.TryAdd(userId.ToString(), webSocket);
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

app.MapPost("/notify",
    (HouseDataDb db, HttpContext httpContext) =>
        WS.Notify(db, httpContext, webSocketsDict));

app.MapPost("/updateTemp",
    (HouseDataDb db, HttpContext httpContext) => Esp.UpdateTemp(db, httpContext));

app.MapPost("/updateHumidity",
    (HouseDataDb db, HttpContext httpContext) => Esp.UpdateHumidity(db, httpContext));

app.MapPost("/link",
    (HouseDataDb db, HttpContext httpContext) => Mirror.LinkMirror(db, httpContext));

app.Run();
