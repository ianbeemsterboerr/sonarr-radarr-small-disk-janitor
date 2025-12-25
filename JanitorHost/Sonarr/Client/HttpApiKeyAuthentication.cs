using System.Diagnostics;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace JanitorHost.Sonarr.Client;

/// <summary>
/// This authentication provider authenticates requests using an API key using http.
/// Kiota has the wrong opinion of not allowing http to be used for any requests, this is their suggested workaraound.
/// One day they will get smart.
/// </summary>
public class HttpApiKeyAuthentication : IAuthenticationProvider
{
    private readonly string _apiKey;
    private readonly string _parameterName;
    private readonly KeyLocation _keyLoc;
    private readonly AllowedHostsValidator _allowedHostsValidator;

    /// <summary>
    /// Instantiates a new <see cref="HttpApiKeyAuthentication"/> using the provided parameters.
    /// </summary>
    /// <param name="apiKey">The API key to use for authentication.</param>
    /// <param name="parameterName">The name of the query parameter or header to use for authentication.</param>
    /// <param name="keyLocation">The location of the API key.</param>
    /// <param name="allowedHosts">The hosts that are allowed to use the provided API key.</param>
    public HttpApiKeyAuthentication(
        string apiKey,
        string parameterName,
        KeyLocation keyLocation,
        params string[] allowedHosts
    )
    {
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentNullException(nameof(apiKey));
        if (string.IsNullOrEmpty(parameterName))
            throw new ArgumentNullException(nameof(parameterName));
        if (allowedHosts == null)
            throw new ArgumentNullException(nameof(allowedHosts));
        _apiKey = apiKey;
        _parameterName = parameterName;
        _keyLoc = keyLocation;
        _allowedHostsValidator = new AllowedHostsValidator(allowedHosts);
    }

    private static ActivitySource _activitySource = new(typeof(RequestInformation).Namespace!);

    /// <inheritdoc />
    public Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = default,
        CancellationToken cancellationToken = default
    )
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        using var span = _activitySource.StartActivity();
        if (!_allowedHostsValidator.IsUrlHostValid(request.URI))
        {
            span?.SetTag("com.microsoft.kiota.authentication.is_url_valid", false);
            return Task.CompletedTask;
        }

        var uri = request.URI;

        switch (_keyLoc)
        {
            case KeyLocation.QueryParameter:
                var uriString =
                    uri.OriginalString
                    + (uri.Query != string.Empty ? "&" : "?")
                    + $"{_parameterName}={_apiKey}";
                request.URI = new Uri(uriString);
                break;
            case KeyLocation.Header:
                request.Headers.Add(_parameterName, _apiKey);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_keyLoc));
        }
        span?.SetTag("com.microsoft.kiota.authentication.is_url_valid", true);
        return Task.CompletedTask;
    }

    /// <summary>
    /// The location of the API key parameter.
    /// </summary>
    public enum KeyLocation
    {
        /// <summary>
        /// The API key is passed as a query parameter.
        /// </summary>
        QueryParameter,

        /// <summary>
        /// The API key is passed as a header.
        /// </summary>
        Header,
    }
}
