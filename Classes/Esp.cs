using Microsoft.EntityFrameworkCore.Query;

static class Esp
{
    async public static Task<IResult> UpdateTemp(HouseDataDb db, HttpContext httpContext)
    {
        Console.WriteLine(httpContext.Request.Body.ToString());
        return Results.NoContent();
    }
}