using LukeHagar.PlexAPI.SDK;
using LukeHagar.PlexAPI.SDK.Models.Components;
using LukeHagar.PlexAPI.SDK.Models.Requests;
using Metadata = LukeHagar.PlexAPI.SDK.Models.Components.Metadata;

namespace JanitorHost.Plex;

public class PlexService(IPlexAPI plexApi, ILogger<PlexService> logger) : IPlexService
{
    public async Task<IEnumerable<int>> GetWatchedShowsTvdbIds()
    {
        var response = await plexApi.Library.GetSectionsAsync();
        if (response.Object?.MediaContainer?.Directory == null)
        {
            logger.LogWarning("no libraries found on plex server");
            return new List<int>();
        }

        var topLevelSections = response.Object.MediaContainer.Directory;

        List<Metadata> episodes = new List<Metadata>();
        foreach (var section in topLevelSections)
        {
            var content = await plexApi.Content.ListContentAsync(
                new ListContentRequest
                {
                    IncludeMeta = BoolInt.True,
                    IncludeGuids = BoolInt.True,
                    SectionId = section.Key!,
                    MediaQuery = new MediaQuery { Type = MediaType.Episode },
                }
            );

            if (
                content.MediaContainerWithMetadata?.MediaContainer?.Metadata == null
                || !content.MediaContainerWithMetadata.MediaContainer.Metadata.Any()
            )
            {
                continue;
            }

            episodes.AddRange(content.MediaContainerWithMetadata.MediaContainer.Metadata);
        }

        var watchedEpisodes = episodes.Where(e => e.ViewCount > 0);

        return watchedEpisodes.Select(e =>
            int.Parse(e.Guids!.First(i => i.Id.StartsWith("tvdb://")).Id.Replace("tvdb://", ""))
        );
    }
}
