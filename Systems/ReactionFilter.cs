using System.Timers;
using Discord;
using Discord.WebSocket;
using DougBot.Models;

namespace DougBot.Systems;

public class ReactionFilter
{

    public ReactionFilter(DiscordSocketClient client)
    {
        client.ReactionAdded += HandleReactionAdded;
        Console.WriteLine("Reaction Filter System Initialized");
    }

    private async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        Task.Run(() => Filter(message, channel, reaction));
    }


    private async Task Filter(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        try
        {
            //Get emotes whitelist
            var settings = Setting.GetSettings();
            var guild = (reaction.Channel as SocketGuildChannel).Guild;
            var guildEmotes = guild.Emotes;
            var emoteWhitelist = guildEmotes.Select(e => e.Name).ToList();
            emoteWhitelist.AddRange(settings.reactionFilterEmotes.Split(','));
            //get guild and user
            if (!emoteWhitelist.Contains(reaction.Emote.Name) && settings.reactionFilterChannels.Split(',').Contains(channel.Value.Id.ToString()))
            {
                var messageObj = await message.GetOrDownloadAsync();
                var user = (SocketGuildUser)reaction.User.Value;
                //Only remove if it still exists
                if(messageObj.Reactions.Keys.Contains(reaction.Emote))
                {
                    await messageObj.RemoveReactionAsync(reaction.Emote, user);
                }
                //Assign role
                await user.AddRoleAsync(Convert.ToUInt64(settings.reactionFilterRole),
                    new RequestOptions { AuditLogReason = "Reaction Filter" });
                //Schedule role removal
                Queue.Create("RemoveRole", settings.guildID + ";" + user.Id + ";" + settings.reactionFilterRole,
                    DateTime.UtcNow.AddDays(3));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}