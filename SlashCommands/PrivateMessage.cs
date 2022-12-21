using System.Net.WebClient;
using System.IO;
using Discord;
using Discord.Interactions;

namespace DougBot.SlashCommands;

public class PrivateMessage : InteractionModuleBase
{
    [SlashCommand("PrivateMessage", "send a dm to a specified user")]
    [EnabledInDm(false)]
    [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
    public async Task frankattach([Summary(description: "user id")] ulong user,
     [Summary(description: "Msg to user here:")] string msg)
    {
       await user.SendMessageAsync(msg);
    }
}