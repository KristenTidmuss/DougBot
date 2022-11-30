using System.Timers;
using Discord;
using Discord.WebSocket;
using DougBot.Models;

namespace DougBot.Systems;

public class ReactionFilter
{
    private readonly DiscordSocketClient _Client;

    public ReactionFilter(DiscordSocketClient client)
    {
        _Client = client;
        var recentTimer = new System.Timers.Timer
        {
            Interval = 10000,
            Enabled = true
        };
        recentTimer.Elapsed += HandleRecent;
        
        var olderTimer = new System.Timers.Timer
        {
            Interval = 1800000,
            Enabled = true
        };
        olderTimer.Elapsed += HandleOld;
        Console.WriteLine("Youtube System Initialized");
    }

    private async void HandleRecent(object? sender, ElapsedEventArgs elapsedEventArgs)
    {
        await FilterReactions(5);
    }
    private async void HandleOld(object? sender, ElapsedEventArgs elapsedEventArgs)
    {
        await FilterReactions(50);
    }

    private async Task FilterReactions(int messageCount)
    {
        try
        {
            var settings = Setting.GetSettings();
            var guild = _Client.GetGuild(Convert.ToUInt64(settings.guildID));
            var guildEmotes = await guild.GetEmotesAsync();
            var guildEmoteNames = guildEmotes.Select(e => e.Name);
            var filterEmotes = settings.reactionFilterEmotes.Split(',');
            foreach (var id in settings.reactionFilterChannels.Split(','))
            {
                //Get the last messageCount messages from the channel
                var channel = (SocketTextChannel)await _Client.GetChannelAsync(ulong.Parse(id));
                var messages = await channel.GetMessagesAsync(messageCount).FlattenAsync();
                foreach (var message in messages)
                {
                    //Get reactions on the message contained in guildEmotes or filterEmotes
                    var reactions = message.Reactions
                        .Where(r => !guildEmoteNames.Contains(r.Key.Name) && !filterEmotes.Contains(r.Key.Name));
                    //remove those reactions from the message
                    foreach (var react in reactions)
                    {
                        await message.RemoveAllReactionsForEmoteAsync(react.Key);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}