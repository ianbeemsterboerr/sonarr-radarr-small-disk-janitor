using JanitorHost.Sonarr.Client;
using JanitorHost.Sonarr.Client.Api.V3.Episode;
using JanitorHost.Sonarr.Client.Models;

namespace JanitorHost.Sonarr;

public class SonarrService(SonarrClient sonarrClient, ILogger<SonarrService> logger)
    : ISonarrService
{
    public async Task DeleteWatchedEpisodes(IEnumerable<int> episodeTvdbIds)
    {
        var series = await sonarrClient.Api.V3.Series.GetAsync();

        List<EpisodeResource> episodeIdsToDelete = new List<EpisodeResource>();

        if (series == null)
        {
            logger.LogWarning("series in sonarr returns null, something is probably wrong!");
            return;
        }

        if (!series.Any())
        {
            return;
        }

        foreach (var show in series)
        {
            var episodes = await sonarrClient.Api.V3.Episode.GetAsync(e =>
            {
                e.QueryParameters =
                    new EpisodeRequestBuilder.EpisodeRequestBuilderGetQueryParameters
                    {
                        SeriesId = show.Id,
                        IncludeEpisodeFile = true,
                    };
            });
            episodeIdsToDelete.AddRange(
                episodes!.Where(e => episodeTvdbIds.Contains(e.TvdbId!.Value))
            );
        }

        foreach (var epiToDelete in episodeIdsToDelete)
        {
            if (epiToDelete.EpisodeFile == null || !epiToDelete.EpisodeFile.Id.HasValue)
            {
                logger.LogWarning(
                    "sonarr episode file is either null or the id has no value, indicating something is wrong. skipping.."
                );
                continue;
            }
            await sonarrClient
                .Api.V3.Episodefile[epiToDelete.EpisodeFile.Id.Value.ToString()]
                .DeleteAsync();
        }
    }
}
