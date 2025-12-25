using JanitorHost.Plex;
using JanitorHost.Sonarr;

namespace JanitorHost;

public class BackgroundPoller(
    IPlexService plexService,
    ISonarrService sonarrService
) : IHostedService
{

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var shows = await plexService.GetWatchedShowsTvdbIds();

        await sonarrService.DeleteWatchedEpisodes(shows);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
