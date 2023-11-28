/// <summary>
/// JSON class model for the temperature.
/// </summary>
class TemperatureJSON
{
    public int espId { get; set; }
    public double temperature { get; set; }
}

/// <summary>
/// JSON class model for the humidity.
/// </summary>
class HumidityJSON
{
    public int espId { get; set; }
    public double humidity { get; set; }
}

/// <summary>
/// JSON class model for a message.
/// </summary>
class MessageJSON
{
    public required string message { get; set; }
}