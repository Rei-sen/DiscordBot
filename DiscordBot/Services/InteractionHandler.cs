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
internal class InteractionHandler(
    DiscordSocketClient _client,
    InteractionService _interactionService,
    IServiceProvider _services
    ) : BackgroundService
{
    /// <summary>
    /// Executes the interaction handler asynchronously.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.Ready += () => _interactionService.RegisterCommandsGloballyAsync(true);
        _client.InteractionCreated += OnInteractionAsync;

        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    /// <summary>
    /// Stops the interaction handler asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _interactionService.Dispose();
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
            var context = new SocketInteractionContext(_client, interaction);
            var result = await _interactionService.ExecuteCommandAsync(context, _services);

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
