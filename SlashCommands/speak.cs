using System.Net.Http.Headers;
using Discord;
using Discord.Interactions;

namespace DougBot.SlashCommands;

public class WAHAHA : InteractionModuleBase
{
    [SlashCommand("WAHAHA", "Send a message into chat as the bot")]
    [EnabledInDm(false)]
    [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
    public async Task WAHAHA([Summary(description: "Channel to type in")] ITextChannel channel,
     [Summary(description: "Message to include")] string msg,
     [Summary(description: "TTS? (T/F)")] string tts,
     [Summary(description: "Reply msg ID")] string replyID)
    {
        if(replyID == null)
        {
            if(tts.ToLower() = "t") // check whether to tts msg
            {
                var messageId = channel.SendMessageAsync(msg, true);
            }
            else
            {
                var messageId = channel.SendMessageAsync(msg);
            }
        }
        else
        {
            if(tts.ToLower() = "t") // check whether to tts msg
            {
                var messageId = channel.SendMessageAsync(msg, true, messageReference=replyID);
            }
            else
            {
                var messageId = channel.SendMessageAsync(msg,messageReference=replyID);
            }
        }
        
        await RespondAsync($"Sent msg in {channel.Mention} "); //update user

    }
}