var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HouseDataDb>();

var app = builder.Build();

// Set the API port that it listen on
app.Urls.Add("http://*:15000");

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await WS.Echo(webSocket);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        await next(context);
    }

});

app.MapPost("/updateTemp",
    (HouseDataDb db, HttpContext httpContext) => Esp.UpdateTemp(db, httpContext));

app.MapPost("/updateHumidity",
    (HouseDataDb db, HttpContext httpContext) => Esp.UpdateHumidity(db, httpContext));

app.MapPost("/link",
    (HouseDataDb db, HttpContext httpContext) => Mirror.LinkMirror(db, httpContext));

app.Run();
