var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Urls.Add("http://*:15000");

app.MapGet("/", () => "Hello World!");

app.Run();
