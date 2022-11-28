using Discord;
using Discord.WebSocket;
using DougBot.Models;

namespace DougBot.Systems;

public class ReactionFilter
{
    public ReactionFilter(DiscordSocketClient client)
    {
        client.ReactionAdded += FilterReactions;
    }

    private async Task FilterReactions(Cacheable<IUserMessage, ulong> Message,
        Cacheable<IMessageChannel, ulong> Channel, SocketReaction Reaction)
    {
        try
        {
            var settings = Setting.GetSettings();
            //get guild and user
            var guild = ((SocketGuildChannel)Channel.Value).Guild;
            var message = await Message.GetOrDownloadAsync();
            var user = (SocketGuildUser)Reaction.User.Value;
            if (!settings.reactionFilterEmotes.Split(',').Contains(Reaction.Emote.Name)
                && !guild.Emotes.ToList().Contains(Reaction.Emote)
                && settings.reactionFilterChannels.Split(',').Contains(Channel.Value.Id.ToString()))
            {
                await message.RemoveReactionAsync(
                    Reaction.Emote,
                    Reaction.User.Value,
                    RequestOptions.Default);
                //Assign role
                await user.AddRoleAsync(Convert.ToUInt64(settings.reactionFilterRole),
                    new RequestOptions { AuditLogReason = "Reaction Filter" });
                //Schedule role removal
                Queue.Create("RemoveRole", guild.Id + ";" + user.Id + ";" + settings.reactionFilterRole,
                    DateTime.UtcNow.AddDays(5));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}