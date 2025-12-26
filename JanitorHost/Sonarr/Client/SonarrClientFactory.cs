using JanitorHost.Properties;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace JanitorHost.Sonarr.Client;

public class SonarrClientFactory
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly HttpClient _httpClient;

    public SonarrClientFactory(HttpClient httpClient, IOptions<JanitorConfig> janitorConfig)
    {
        _authenticationProvider = new HttpApiKeyAuthentication(
            janitorConfig.Value.SONAR_API_KEY!,
            "X-Api-Key",
            HttpApiKeyAuthentication.KeyLocation.Header
        );
        httpClient.BaseAddress = new Uri(janitorConfig.Value.SONARR_URL!);
        _httpClient = httpClient;
    }

    public SonarrClient GetClient()
    {
        return new SonarrClient(
            new HttpClientRequestAdapter(_authenticationProvider, httpClient: _httpClient)
        );
    }
}
