using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules;

public class EchoModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("echo", "Echo the message back and log it")]
    public async Task SubscribeChannelToListings(string s)
    {
        
        Console.WriteLine(s);
        await RespondAsync(s, ephemeral: true);
        await Task.CompletedTask;
    }

    [SlashCommand("getemotes", "aaa")]
    public async Task GetEmotes()
    {
        var guild = Context.Guild;
        var emotes = guild.Emotes;
        var sb = new StringBuilder();
        foreach (var emote in emotes)
        {
            sb.AppendLine(emote.ToString());
        }
        Console.WriteLine(sb.ToString());
        await RespondAsync(sb.ToString(), ephemeral: true);
        await Task.CompletedTask;
    }
}
