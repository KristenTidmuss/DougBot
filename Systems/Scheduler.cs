using System.Timers;
using Discord;
using Discord.WebSocket;
using DougBot.Models;
using Timer = System.Timers.Timer;

namespace DougBot.Systems;

public class Scheduler
{
    private readonly DiscordSocketClient _Client;

    public Scheduler(DiscordSocketClient client)
    {
        _Client = client;
        var timerMinute = new Timer
        {
            Interval = 60000,
            Enabled = true
        };
        timerMinute.Elapsed += Minute;
        var timerHour = new Timer
        {
            Interval = 3600000,
            Enabled = true
        };
        timerHour.Elapsed += Hour;
        Console.WriteLine("Scheduler System Initialized");
    }

    private async void Hour(object? sender, ElapsedEventArgs e)
    {
        await CleanForums(_Client);
    }

    private async void Minute(object? sender, ElapsedEventArgs e)
    {
        try
        {
            var settings = Setting.GetSettings();
            await _Client.SetGameAsync(settings.statusMessage);
            var pendingQueues = Queue.GetDue();
            foreach (var queue in pendingQueues)
            {
                switch (queue.Type)
                {
                    case "RemoveRole":
                        var para = queue.Data.Split(';');
                        await RemoveRole(_Client, ulong.Parse(para[0]), ulong.Parse(para[1]), ulong.Parse(para[2]));
                        break;
                }
                Queue.Remove(queue);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private async Task RemoveRole(DiscordSocketClient client, ulong guidID, ulong userID, ulong roleID)
    {
        var guild = client.GetGuild(guidID);
        var user = guild.GetUser(userID);
        var role = guild.GetRole(roleID);
        if(user == null){return;}
        await user.RemoveRoleAsync(role);
    }

    private async Task CleanForums(DiscordSocketClient client)
    {
        var settings = Setting.GetSettings();
        //Get forums from client
        var guild = client.Guilds.FirstOrDefault(g => g.Id.ToString() == settings.guildID);
        var forums = guild.Channels.Where(c => c.GetType().Name == "SocketForumChannel");
        //Loop the forums checking for posts inactive for 2 days or more
        foreach (SocketForumChannel forum in forums)
        {
            var threads = await forum.GetActiveThreadsAsync();
            var forumThreads = threads.Where(t => t.ParentChannelId == forum.Id);
            foreach (var thread in forumThreads)
            {
                var message = await thread.GetMessagesAsync(1).FlattenAsync();
                if(message.First().Timestamp.UtcDateTime < DateTime.UtcNow.AddDays(-2))
                {
                    await thread.ModifyAsync(t => t.Archived = true);
                }
            }
        }
    }
}