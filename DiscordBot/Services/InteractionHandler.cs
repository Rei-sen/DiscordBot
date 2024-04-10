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

/// <summary>
/// Represents a class that handles interactions with the Discord API.
/// </summary>
internal class InteractionHandler : BackgroundService
{
    private readonly DiscordSocketClient client;
    private readonly InteractionService interactionService;
    private readonly IServiceProvider services;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionHandler"/> class.
    /// </summary>
    /// <param name="client">The Discord socket client.</param>
    /// <param name="interactions">The interaction service.</param>
    /// <param name="services">The service provider.</param>
    public InteractionHandler(DiscordSocketClient client,
                              InteractionService interactions,
                              IServiceProvider services)
    {
        this.client = client;
        this.interactionService = interactions;
        this.services = services;
    }

    /// <summary>
    /// Executes the interaction handler asynchronously.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        client.Ready += () => interactionService.RegisterCommandsGloballyAsync(true);
        client.InteractionCreated += OnInteractionAsync;

        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), services);
    }

    /// <summary>
    /// Stops the interaction handler asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        interactionService.Dispose();
        return base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Handles the interaction asynchronously.
    /// </summary>
    /// <param name="interaction">The socket interaction.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
