namespace JanitorHost.Sonarr;

public interface ISonarrService
{
    Task DeleteWatchedEpisodes(IEnumerable<int> episodeTvdbIds);
}
