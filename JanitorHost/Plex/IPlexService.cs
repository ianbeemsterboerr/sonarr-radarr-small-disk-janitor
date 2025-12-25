namespace JanitorHost.Plex;

public interface IPlexService
{
    public Task<IEnumerable<int>> GetWatchedShowsTvdbIds();
}
