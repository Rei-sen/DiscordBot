using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using DiscordBot.Model.Storage;

namespace DiscordBot
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => config
                    .AddEnvironmentVariables()
                    .AddUserSecrets<Program>()) 
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IRepository<ChannelSubscription>, InMemoryRepository<ChannelSubscription>>();
                    services.AddSingleton<DiscordSocketClient>();
                    services.AddSingleton<InteractionService>();

                    services.AddHostedService<InteractionHandler>();
                    services.AddHostedService<DiscordStartup>();
                    services.AddHostedService<PartyFinder>();

                }).Build();

            await host.RunAsync();
            await host.RunAsync();
        }
    }
}