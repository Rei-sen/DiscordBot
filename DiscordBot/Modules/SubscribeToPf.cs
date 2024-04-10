using Discord;
using Discord.Interactions;
using DiscordBot.Model;
using DiscordBot.Model.DB;
using DiscordBot.Model.Storage;

namespace DiscordBot.Modules;

public class SubscribeToPF(
    IInMemoryRepository<PFListing> _pfRepository,
    //IRepository<PFSubscription> _subscriptionRepository,
    PFSubscriptionsContext _subscriptionContext
    ) : InteractionModuleBase<SocketInteractionContext>
{

    [SlashCommand("subscribetopf", "Subscribe this channel to PF listings")]
    public async Task SubscribeChannelToListings(DataCenter dc, string dutyName, string color)
    {
        var message = await Context.Channel.SendMessageAsync(embed: new EmbedBuilder().AddField("...", "...").Build());
        await RespondAsync("Subscribed to PF listings! The listings will get updated soon!", ephemeral: true);

        PFSubscription subscription = new PFSubscription(Context.Channel.Id, message.Id, dc, dutyName, color);


        _subscriptionContext.Add(subscription);
        _subscriptionContext.SaveChanges();
        //        await _subscriptionContext.SaveChangesAsync();

        var listings = GetEmptyEmbedAsync(subscription);
        await message.ModifyAsync(message => message.Embed = listings);
    }


    private Embed GetEmptyEmbedAsync(PFSubscription subscription)
    {
        return subscription.GetSubscriptionEmbedBuilder().Build();
    }
}
