using Microsoft.EntityFrameworkCore;

public class HouseData
{
    public int Id { get; set; }
    public int EspId { get; set; }
    public int UserId { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
}