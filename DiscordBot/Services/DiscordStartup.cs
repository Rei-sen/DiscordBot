using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.Services;

internal class DiscordStartup(IConfiguration config, DiscordSocketClient client) : IHostedService
{
    private readonly DiscordSocketClient client = client;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // right click on the project then go to Manage User Secrets, it should show a json you can put your key into done
        var discordKey = config.GetValue<string>("DiscordKey");

        await client.StartAsync();
        await client.LoginAsync(Discord.TokenType.Bot, discordKey);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.LogoutAsync();
        await client.StopAsync();
    }
}