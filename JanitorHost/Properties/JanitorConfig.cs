namespace JanitorHost.Properties;

public class JanitorConfig
{
    public string? SonarrUrl { get; set; }
    public string? SonarrApiKey { get; set; }
    public required string PlexToken { get; set; }
    public int ScanFrequencyMinutes { get; set; } = 5;
}
