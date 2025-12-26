namespace JanitorHost.Properties;

public class JanitorConfig
{
    public string? SONARR_URL { get; set; }
    public string? SONAR_API_KEY { get; set; }
    public required string PLEX_TOKEN { get; set; }
    public int SCAN_FREQUENCY_MINUTES { get; set; } = 5;
}
