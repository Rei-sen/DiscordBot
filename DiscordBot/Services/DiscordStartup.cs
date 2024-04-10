using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.Services;

/// <summary>
/// Represents the startup service for the Discord bot.
/// </summary>
internal class DiscordStartup(
    DiscordSocketClient _client,
    IConfiguration _config
    ) : IHostedService
{
    /// <summary>
    /// Starts the Discord bot asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // right click on the project then go to Manage User Secrets, it should show a json you can put your key into done
        var discordKey = _config.GetValue<string>("DiscordKey");

        await _client.StartAsync();
        await _client.LoginAsync(Discord.TokenType.Bot, discordKey);
    }

    /// <summary>
    /// Stops the Discord bot asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.LogoutAsync();
        await _client.StopAsync();
    }
}
