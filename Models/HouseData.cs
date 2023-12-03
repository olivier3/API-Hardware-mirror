/// <summary>
/// Database class model for the ESP 32 data.
/// </summary>
public class HouseData
{
    public int Id { get; set; }
    public string EspId { get; set; }
    public string UserId { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }

    /// <summary>
    /// HouseData contructor.
    /// </summary>
    public HouseData()
    {
        EspId = "0";
        UserId = "0";
        Temperature = 0;
        Humidity = 0;
    }
}