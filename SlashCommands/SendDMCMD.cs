using Discord;
using Discord.Interactions;
using DougBot.Models;
using Newtonsoft.Json;

namespace DougBot.SlashCommands;

public class SendDMCMD : InteractionModuleBase
{
    [SlashCommand("senddm", "Send a DM to the specified user")]
    [EnabledInDm(false)]
    [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
    public async Task SendDM([Summary(description: "User to DM")] IGuildUser user,
        [Summary(description: "Message to send")]
        string message)
    {
        var settings = Setting.GetSettings();
        var embeds = new List<EmbedBuilder>
        {
            new EmbedBuilder()
                .WithDescription(message)
                .WithColor(Color.Orange)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName(Context.Guild.Name + " Mods")
                    .WithIconUrl(Context.Guild.IconUrl))
                .WithCurrentTimestamp()
        };
        var dict = new Dictionary<string, string>
        {
            { "userId", user.Id.ToString() },
            { "embedBuilders", JsonConvert.SerializeObject(embeds) }
        };
        var json = JsonConvert.SerializeObject(dict);
        Queue.Create("SendDM", null, json, DateTime.UtcNow);
        await RespondAsync($"DM queued, check <#{settings.dmReceiptChannel}>", ephemeral: true);
    }
}