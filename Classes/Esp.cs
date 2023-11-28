using System.Text.Json;
using Microsoft.EntityFrameworkCore;

static class Esp
{
    async public static Task<IResult> UpdateTemp(HouseDataDb db, HttpContext httpContext)
    {
        WeatherJSON? content = await JsonSerializer.DeserializeAsync<WeatherJSON>(httpContext.Request.Body);

        var item = await db.HouseData.FirstOrDefaultAsync(x => x.EspId == content.espId);

        if (item == null || content.espId <= 0)
        {
            var error = JsonSerializer.Deserialize<MessageJSON>("{\"message\":\"Incorrect value\"}");

            return Results.BadRequest(error);
        }

        if (item != null)
        {
            item.Temperature = content.temperature;

            await db.SaveChangesAsync();
        }
        else
        {
            HouseData data = new HouseData
            {
                EspId = content.espId,
                Temperature = content.temperature
            };

            db.HouseData.Add(data);

            await db.SaveChangesAsync();
        }

        var res = JsonSerializer.Deserialize<MessageJSON>("{\"message\":\"Temperature updated\"}");

        return Results.Ok(res);
    }
}