using System.Text.Json;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Static class for the ESP 32.
/// </summary>
static class Esp
{
    /// <summary>
    /// Function to update the esp 32 temperature in the database
    /// </summary>
    /// <param name="db">Database context</param>
    /// <param name="httpContext">Context of the http request</param>
    /// <returns>Return the http status code and message</returns>
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