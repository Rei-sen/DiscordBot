using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services;

internal class InteractionHandler : BackgroundService
{
    private readonly DiscordSocketClient client;
    private readonly InteractionService interactionService;
    private readonly IServiceProvider services;

    public InteractionHandler(DiscordSocketClient client,
                              InteractionService interactions,
                              IServiceProvider services)
    {
        this.client = client;
        this.interactionService = interactions;
        this.services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        client.Ready += () => interactionService.RegisterCommandsGloballyAsync(true);
        client.InteractionCreated += OnInteractionAsync;

        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), services);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        interactionService.Dispose();
        return base.StopAsync(cancellationToken);
    }
    private async Task OnInteractionAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(client, interaction);
            var result = await interactionService.ExecuteCommandAsync(context, services);
            Console.WriteLine(interaction.ToString());
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ToString());
        }
        catch (Exception ex)
        {
            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(msg => msg.Result.DeleteAsync());
            }
        }
    }

}
