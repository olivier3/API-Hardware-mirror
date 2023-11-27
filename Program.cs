var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HouseDataDb>();

var app = builder.Build();

// Set the API port that it listen on
app.Urls.Add("http://*:15000");

app.MapGet("/", async (HouseDataDb db) =>
{
    

    var test = new HouseData();

    db.HouseData.Add(test);

    await db.SaveChangesAsync();
});



app.Run();
