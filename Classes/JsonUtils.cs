/// <summary>
/// JSON class model for the weather.
/// </summary>
class WeatherJSON
{
    public int espId { get; set; }
    public double temperature { get; set; }
}

/// <summary>
/// JSON class model for a message.
/// </summary>
class MessageJSON
{
    public required string message { get; set; }
}