using Discord;
using Discord.Interactions;

namespace DougBot.SlashCommands;

public class TypingCmd : InteractionModuleBase
{
    [SlashCommand("typing", "Type in chat for a given amount of seconds")]
    [EnabledInDm(false)]
    [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
    public async Task Typing([Summary(description: "Channel to type in")] ITextChannel channel,
        [Summary(description: "Amount of seconds to type (max 1000 s)")]
        int seconds)
    {
        if(seconds < 0 or seconds > 1000)   // edge cases
        {
            throw new Exception("Invalid length");
        }
        await RespondAsync($"Beginning typing for {seconds} seconds in {channel.Mention} ", ephemeral: true);
        var type = channel.EnterTypingState();
        await Task.Delay(seconds * 1000);
        type.Dispose();
    }
}