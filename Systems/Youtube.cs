using System.Timers;
using Discord;
using Discord.WebSocket;
using DougBot.Models;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace DougBot.Systems;

public class Youtube
{
    private readonly DiscordSocketClient _Client;

    public Youtube(DiscordSocketClient client)
    {
        _Client = client;
        var timer = new System.Timers.Timer
        {
            Interval = 600000,
            Enabled = true
        };
        timer.Elapsed += CheckUploads;
        Console.WriteLine("Youtube System Initialized");
        CheckUploads(null,null);
    }

    private async void CheckUploads(object? sender, ElapsedEventArgs elapsedEventArgs)
    {
        try
        {
            await _Client.SetStatusAsync(UserStatus.Idle);
            var settings = Setting.GetSettings();
            var channels = settings.YoutubeChannels.Split(Environment.NewLine);
            var youtube = new YoutubeClient();
            var pinged = false;
            foreach (var channel in channels)
            {
                var channelMention = channel.Split(';')[0];
                var pingRole = channel.Split(';')[1];
                var ytChannel = await youtube.Channels.GetByHandleAsync("https://youtube.com/" + channelMention);
                var uploads = await youtube.Channels.GetUploadsAsync(ytChannel.Id);
                var lastUpload = uploads.FirstOrDefault();
                var video = await youtube.Videos.GetAsync(lastUpload.Id);
                if (video.UploadDate.UtcDateTime > settings.YoutubeLastCheck)
                {
                    var embed = new EmbedBuilder()
                        .WithAuthor(ytChannel.Title, ytChannel.Thumbnails[0].Url, ytChannel.Url)
                        .WithTitle(video.Title)
                        .WithImageUrl(video.Thumbnails.OrderBy(t => t.Resolution.Area).Last().Url)
                        .WithUrl(video.Url)
                        .Build();
                    var guild = _Client.GetGuild(Convert.ToUInt64(settings.guildID));
                    var discChannel = guild.GetTextChannel(Convert.ToUInt64(settings.YoutubePostChannel));
                    await discChannel.SendMessageAsync($"<@&{pingRole}>", embeds: new[] { embed },
                        allowedMentions: AllowedMentions.All);
                    pinged = true;
                }
            }
            await _Client.SetStatusAsync(UserStatus.DoNotDisturb);
            if (pinged)
            {
                Setting.UpdateLastChecked(DateTime.UtcNow);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}