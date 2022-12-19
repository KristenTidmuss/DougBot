using System.Timers;
using Discord;
using Discord.WebSocket;
using DougBot.Models;
using Newtonsoft.Json;

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
            //Get values and emotes whitelist
            var settings = Setting.GetSettings();
            var guild = (reaction.Channel as SocketGuildChannel).Guild;
            var guildEmotes = guild.Emotes;
            var emoteWhitelist = guildEmotes.Select(e => e.Name).ToList();
            emoteWhitelist.AddRange(settings.reactionFilterEmotes.Split(','));
            //Check if emote is in whitelist
            if (!emoteWhitelist.Contains(reaction.Emote.Name) && settings.reactionFilterChannels.Split(',').Contains(channel.Value.Id.ToString()))
            {
                var messageObj = await message.GetOrDownloadAsync();
                var user = (SocketGuildUser)reaction.User.Value;
                //Only remove if it still exists
                Dictionary<string, string>? dict;
                string? json;
                if(messageObj.Reactions.Keys.Contains(reaction.Emote))
                {
                    //Check if already scheduled for removal
                    if(!Queue.GetAll().Any(q => q.Type == "RemoveReaction" && q.Keys.Contains(message.Id.ToString()) && q.Keys.Contains(reaction.Emote.Name)))
                    {
                        dict = new Dictionary<string,string>{
                            {"guildId", guild.Id.ToString()},
                            {"channelId", channel.Id.ToString()},
                            {"messageId", message.Id.ToString()},
                            {"emoteName", reaction.Emote.Name}
                        };
                        json = JsonConvert.SerializeObject(dict);
                        Queue.Create("RemoveReaction", null, json, DateTime.UtcNow);
                    }
                }
                //Assign role
                dict = new Dictionary<string,string>{
                    {"guildId", guild.Id.ToString()},
                    {"userId", user.Id.ToString()},
                    {"roleId", settings.reactionFilterRole}
                };
                json = JsonConvert.SerializeObject(dict);
                Queue.Create("AddRole", null, json, DateTime.UtcNow);
                //Schedule role removal
                dict = new Dictionary<string,string>{
                    {"guildId", guild.Id.ToString()},
                    {"userId", user.Id.ToString()},
                    {"roleId", settings.reactionFilterRole}
                };
                json = JsonConvert.SerializeObject(dict);
                Queue.Create("RemoveRole", null, json, DateTime.UtcNow.AddMinutes(10));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}