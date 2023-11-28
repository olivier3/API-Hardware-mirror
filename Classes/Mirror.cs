
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Class for the mirror request.
/// </summary>
static class Mirror
{
    /// <summary>
    /// Function to link the mirror and the esp32
    /// </summary>
    /// <param name="db">Database context</param>
    /// <param name="httpContext">Http request context</param>
    /// <returns>Http status code and message or value</returns>
    async internal static Task<IResult> LinkMirror(HouseDataDb db, HttpContext httpContext)
    {
        LinkQRJSON? content = await JsonSerializer.DeserializeAsync<LinkQRJSON>(httpContext.Request.Body);

        var item = await db.HouseData.FirstOrDefaultAsync(x => x.EspId == content.espId);

        if (item == null)
        {
            var noEspData = JsonSerializer.Deserialize<MessageJSON>("{\"message\":\"No esp data found\"}");

            return Results.BadRequest(noEspData);
        }

        if (content.espId <= 0 || content.userId <= 0)
        {
            var badValue = JsonSerializer.Deserialize<MessageJSON>("{\"message\":\"Invalid value sent\"}");

            return Results.BadRequest(badValue);
        }

        item.UserId = content.userId;

        item.EspId = content.espId;

        await db.SaveChangesAsync();

        var espValues = JsonSerializer.Deserialize<EspData>(
            "{" +
                $"\"temperature\":{item.Temperature}," + 
                $"\"humidity\":{item.Humidity}" + 
            "}");

        return Results.Ok(espValues);
    }
}