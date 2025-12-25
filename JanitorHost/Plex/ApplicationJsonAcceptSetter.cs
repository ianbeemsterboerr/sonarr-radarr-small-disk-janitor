using LukeHagar.PlexAPI.SDK.Utils;

namespace JanitorHost.Plex;

public class ApplicationJsonAcceptSetter(ISpeakeasyHttpClient innerClient) : ISpeakeasyHttpClient
{
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        request.Headers.Add("Accept", "application/json");
        var response = await innerClient.SendAsync(request);
        return response;
    }

    public Task<HttpRequestMessage> CloneAsync(HttpRequestMessage request)
    {
        throw new NotImplementedException();
    }
}
