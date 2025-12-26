using JanitorHost.Plex;
using JanitorHost.Properties;
using JanitorHost.Sonarr;
using JanitorHost.Sonarr.Client;
using LukeHagar.PlexAPI.SDK;
using LukeHagar.PlexAPI.SDK.Utils;
using Microsoft.Extensions.Options;

namespace JanitorHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.Configure<JanitorConfig>(builder.Configuration);

        builder.Services.AddSingleton<IPlexAPI, PlexAPI>(s =>
        {
            var client = new ApplicationJsonAcceptSetter(new SpeakeasyHttpClient());
            PlexAPI.SDKBuilder plexSdkBuilder = new PlexAPI.SDKBuilder();
            var config = s.GetRequiredService<IOptions<JanitorConfig>>().Value;
            plexSdkBuilder
                .WithToken(config.PLEX_TOKEN)
                .WithProtocol("http")
                .WithHost("mediaserver")
                .WithPort("32400")
                .WithServerIndex(1)
                .WithAccepts(LukeHagar.PlexAPI.SDK.Models.Components.Accepts.ApplicationJson)
                .WithClient(client);

            return plexSdkBuilder.Build();
        });

        builder.Services.AddKiotaHandlers();
        builder.Services.AddHttpClient<SonarrClientFactory>().AttachKiotaHandlers();

        builder.Services.AddTransient(sp =>
            sp.GetRequiredService<SonarrClientFactory>().GetClient()
        );

        builder.Services.AddTransient<IPlexService, PlexService>();
        builder.Services.AddTransient<ISonarrService, SonarrService>();

        builder.Services.AddHostedService<BackgroundPoller>();

        var app = builder.Build();
        app.Run();
    }
}
