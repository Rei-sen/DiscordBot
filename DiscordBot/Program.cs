using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Model;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using DiscordBot.Model.Storage;
using DiscordBot.Model.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

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
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IInMemoryRepository<PFListing>, InMemoryRepository<PFListing>>();
                    services.AddSingleton<DiscordSocketClient>();
                    services.AddSingleton<InteractionService>();

                    IConfiguration configuration = hostContext.Configuration;

                    string databaseLocation = configuration.GetValue<string>("DatabaseLocation") ?? ".";

                    // Ensure the directory exists
                    Directory.CreateDirectory(databaseLocation);

                    services.AddDbContext<PFSubscriptionsContext>(options =>
                    {
                        options.UseSqlite($"Data Source={databaseLocation}/pf_subscriptions.db");
                    });

                    services.AddHostedService<InteractionHandler>();
                    services.AddHostedService<DiscordStartup>();
                    services.AddHostedService<PartyFinderService>();
                }).Build();

            await host.RunAsync();
        }
    }
}