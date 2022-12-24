using Discord;
using Discord.WebSocket;
using DougBot.Models;
using Fernandezja.ColorHashSharp;
using Newtonsoft.Json;

namespace DougBot.Systems;

public class Events
{
    private DiscordSocketClient _Client;
    public Events(DiscordSocketClient client)
    {
        _Client = client;
        client.MessageReceived += MessageReceivedHandler;
        Console.WriteLine("EventHandler Initialized");
    }

    private async Task MessageReceivedHandler(SocketMessage message)
    {
        var settings = Setting.GetSettings();
        if (message.Channel is SocketDMChannel && message.Author.Id != _Client.CurrentUser.Id)
        {
            var embeds = new List<EmbedBuilder>();
            //Main embed
            var colorHash = new ColorHash();
            var color = colorHash.BuildToColor(message.Author.Id.ToString());
            embeds.Add(new EmbedBuilder()
                .WithDescription(message.Content)
                .WithColor((Color)color)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName($"{message.Author.Username}#{message.Author.Discriminator} ({message.Author.Id})")
                    .WithIconUrl(message.Author.GetAvatarUrl()))
                .WithCurrentTimestamp());
            //Attachment embeds
            embeds.AddRange(message.Attachments.Select(attachment => new EmbedBuilder().WithTitle(attachment.Filename).WithImageUrl(attachment.Url).WithUrl(attachment.Url)));
            var dict = new Dictionary<string, string>
            {
                { "guildId", settings.guildID },
                { "channelId", settings.dmReceiptChannel },
                { "message", "" },
                { "embedBuilders", JsonConvert.SerializeObject(embeds) }
            };
            var json = JsonConvert.SerializeObject(dict);
            Queue.Create("SendMessage", null, json, DateTime.UtcNow);
        }
    }
}