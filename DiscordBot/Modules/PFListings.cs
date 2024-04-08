using Discord.Commands;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules;

public class PFListings : InteractionModuleBase<SocketInteractionContext>
{
    public enum DataCenters
    {
        Aether,
        Chaos, 
        Crystal,
        Dynamis,
        Elemental,
        Gaia,
        Light,
        Mana,
        Materia,
        Meteor,
        Primal,
    }

    [SlashCommand("subscribetopf", "Subscribe this channel to PF listings")]
    public async Task SubscribeChannelToListings(DataCenters dc)
    {
        await RespondAsync(dc.ToString());
        await Task.CompletedTask;
    }
}
