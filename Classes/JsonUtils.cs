/// <summary>
/// JSON class model for the temperature.
/// </summary>
class TemperatureJSON
{
    public required string espId { get; set; }
    public double temperature { get; set; }
}

/// <summary>
/// JSON class model for the QR code link.
/// </summary>
class LinkQRJSON
{
    public required string userId { get; set; }
    public required string espId { get; set; }
}

/// <summary>
/// JSON class model for the esp data sent to client.
/// </summary>
class EspData
{
    public double temperature { get; set; }
    public double humidity { get; set; }
}
/// JSON class model for the humidity.
/// </summary>
class HumidityJSON
{
    public required string espId { get; set; }
    public double humidity { get; set; }
}

/// <summary>
/// JSON class model for a message.
/// </summary>
class MessageJSON
{
    public required string message { get; set; }
}

/// <summary>
/// JSON class model for espId .
/// </summary>
class EspIdJSON
{
    public required string espId { get; set; }
}