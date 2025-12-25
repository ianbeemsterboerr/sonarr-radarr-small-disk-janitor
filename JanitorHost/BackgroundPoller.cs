using JanitorHost.Plex;
using JanitorHost.Properties;
using JanitorHost.Sonarr;
using Microsoft.Extensions.Options;

namespace JanitorHost;

public class BackgroundPoller(
    IPlexService plexService,
    ISonarrService sonarrService,
    IOptions<JanitorConfig> janitorConfig,
    ILogger<BackgroundPoller> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("scanning for watched shows..");
            var shows = (await plexService.GetWatchedShowsTvdbIds()).ToList();

            if (!shows.Any())
            {
                logger.LogTrace("no shows found, not deleting..");
                return;
            }
            await sonarrService.DeleteWatchedEpisodes(shows);

            await Task.Delay(
                TimeSpan.FromMinutes(janitorConfig.Value.ScanFrequencyMinutes),
                cancellationToken
            );
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
