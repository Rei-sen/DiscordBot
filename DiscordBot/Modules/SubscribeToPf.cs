using Discord;
using Discord.Interactions;
using DiscordBot.Model;
using DiscordBot.Model.Storage;

namespace DiscordBot.Modules;

public class SubscribeToPF(
    IInMemoryRepository<PFListing> _pfRepository,
    IRepository<PFSubscription> _subscriptionRepository
    ) : InteractionModuleBase<SocketInteractionContext>
{

    [SlashCommand("subscribetopf", "Subscribe this channel to PF listings")]
    public async Task SubscribeChannelToListings(DataCenter dc, string dutyName, string color)
    {
        var message = await Context.Channel.SendMessageAsync(embed: new EmbedBuilder().AddField("...", "...").Build());
        await RespondAsync("Subscribed to PF listings! The listings will get updated soon!", ephemeral: true);

        var subscription = new PFSubscription(Context.Channel.Id, message.Id, dc, dutyName, color);
        await _subscriptionRepository.Insert(subscription);

        var listings = GetEmptyEmbedAsync(subscription);

        await message.ModifyAsync(message => message.Embed = listings);
    }


    public Embed GetEmptyEmbedAsync(PFSubscription subscription)
    {
        return subscription.getSubscriptionEmbedBuilder().Build();
    }
}
