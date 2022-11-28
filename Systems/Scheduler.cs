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
    }

    private async void Minute(object? sender, ElapsedEventArgs e)
    {
        try
        {
            await _Client.SetStatusAsync(UserStatus.Idle);
            var settings = Setting.GetSettings();
            await _Client.SetGameAsync(settings.statusMessage);
            var pendingQueues = Queue.GetDue();
            foreach (var queue in pendingQueues)
            {
                switch (queue.Type)
                {
                    case "RemoveRole":
                        var para = queue.Data.Split(';');
                        await RemoveRole(ulong.Parse(para[0]), ulong.Parse(para[1]), ulong.Parse(para[2]));
                        break;
                }

                Queue.Remove(queue);
            }

            await _Client.SetStatusAsync(UserStatus.DoNotDisturb);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private async Task RemoveRole(ulong guidID, ulong userID, ulong roleID)
    {
        var guild = _Client.GetGuild(guidID);
        var user = guild.GetUser(userID);
        var role = guild.GetRole(roleID);
        await user.RemoveRoleAsync(role);
    }
}