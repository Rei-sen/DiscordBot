using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;


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
                    services.AddSingleton<DiscordSocketClient>();
                    //services.AddSingleton<CommandService>();
                    services.AddSingleton<InteractionService>();

                    services.AddHostedService<InteractionHandler>();
                    services.AddHostedService<DiscordStartup>();
                }).Build();

            await host.RunAsync();
        }
    }
}