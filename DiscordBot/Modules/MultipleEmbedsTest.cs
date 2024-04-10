using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules;

public class MultipleEmbedsTest : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("testmultipleembeds", "Test multiple embeds feature")]
    public async Task TestMultipleEmbeds()
    {

        var embeds = new List<Embed>
        {
            new EmbedBuilder().WithTitle("Embed 1").WithDescription("This is the first embed").Build(),
            new EmbedBuilder().WithTitle("Embed 2").WithDescription("This is the second embed").Build(),
            new EmbedBuilder().WithTitle("Embed 3").WithDescription("This is the third embed").Build()
        };
        await Context.Channel.SendMessageAsync(embeds: embeds.ToArray());

        await RespondAsync("Sent multiple embeds!", ephemeral: true);
    }

}
