using System.Timers;
using Discord;
using Discord.WebSocket;
using DougBot.Models;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace DougBot.Scheduler;

public class Schedule
{
    private readonly DiscordSocketClient _Client;
    private bool _IsRunning;
    private Timer _TimerMain = new Timer
    {
        Interval = 5000,
        Enabled = true
    };
    private Timer _TimerLong = new Timer
    {
        Interval = 1800000,
        Enabled = true
    };

    public Schedule(DiscordSocketClient client)
    {
        _Client = client;
        //Main Queue
        _TimerMain.Elapsed += MainQueue;
        //Infrequent Tasks
        _TimerLong.Elapsed += Hour;
        Console.WriteLine("Scheduler System Initialized");
    }

    private static void Hour(object? sender, ElapsedEventArgs e)
    {
        Queue.Create("Youtube", null,null, DateTime.UtcNow);
        Queue.Create("Forum", null,null, DateTime.UtcNow);
        Queue.Create("Freshmen", null,null, DateTime.UtcNow);
    }

    private async void MainQueue(object? sender, ElapsedEventArgs e)
    {
        _TimerMain.Enabled = false;
        try
        {
            var settings = Setting.GetSettings();
            var pendingQueues = Queue.GetDue();
            //Run items 
            Parallel.ForEach(pendingQueues, queue =>
            {
                var param = new Dictionary<string, string>();
                if (queue.Keys != null)
                {
                    param = JsonConvert.DeserializeObject<Dictionary<string, string>>(queue.Keys);
                }

                switch (queue.Type)
                {
                    case "Forum":
                        Forums.Clean(_Client);
                        break;
                    case "Youtube":
                        Youtube.CheckYoutube(_Client);
                        break;
                    case "Freshmen":
                        Freshmen.CheckFreshmen(_Client);
                        break;
                    case "RemoveRole":
                        Role.Remove(_Client,
                            ulong.Parse(param["guildId"]),
                            ulong.Parse(param["userId"]),
                            ulong.Parse(param["roleId"]));
                        break;
                    case "AddRole":
                        Role.Add(_Client,
                            ulong.Parse(param["guildId"]),
                            ulong.Parse(param["userId"]),
                            ulong.Parse(param["roleId"]));
                        break;
                    case "RemoveReaction":
                        Reaction.Remove(_Client, 
                            ulong.Parse(param["guildId"]),
                            ulong.Parse(param["channelId"]),
                            ulong.Parse(param["messageId"]),
                            param["emoteName"]);
                        break;
                    case "SetStatus":
                        _Client.SetGameAsync(queue.Data);
                        break;
                }
                Queue.Remove(queue);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        _TimerMain.Enabled = true;
    }
}