using Discord;
using Discord.Interactions;
using DougBot.Models;

namespace DougBot.SlashCommands;

public class SetStatusCmd : InteractionModuleBase
{
    [SlashCommand("setstatus", "Set the status of the bot")]
    [EnabledInDm(false)]
    [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
    public async Task SetStatus([Summary(description: "What should the status be")] string status)
    {
        //Set the bots status
        Setting.UpdateStatusMessage(status);
        await RespondAsync($"Status set to `{status}` and will update within a minute");
    }
}