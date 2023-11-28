var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HouseDataDb>();

var app = builder.Build();

// Set the API port that it listen on
app.Urls.Add("http://*:15000");

app.MapPost("/updateTemp",
    (HouseDataDb db, HttpContext httpContext) => Esp.UpdateTemp(db, httpContext));

app.MapPost("/updateHumidity", 
    (HouseDataDb db, HttpContext httpContext) => Esp.UpdateHumidity(db, httpContext));

app.MapPost("/link", 
    (HouseDataDb db, HttpContext httpContext) => Mirror.LinkMirror(db, httpContext));

app.Run();
