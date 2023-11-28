
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

static class Mirror
{
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