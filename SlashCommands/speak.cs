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
     [Summary(description: "Reply msg ID")] string replyID,
     [Summary(description: "Delay (in minutes, enter 0 if no delay)")] int length)
    {
        if(length > 0)
        {
            await Task.Delay(length * 60000); 
        }
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
        
        await RespondAsync($"Sent msg in {channel.Mention} after {length} minutes"); //update user
    }

}


public class Reminder : InteractionModuleBase
{
    [SlashCommand("Reminder", "Send a reminder in a specified channel")]
    [EnabledInDm(false)]
    [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
    public async Task WAHAHA([Summary(description: "Channel to type in")] ITextChannel channel,
     [Summary(description: "Message to include")] string msg,
     [Summary(description: "Delay (minutes)")] int length)
    {
        if(length < 0)   // edge cases
        {
            throw new Exception("Invalid length");
        }
        await Task.Delay(length * 60000);  
        var messageId = channel.SendMessageAsync(msg, true);
        await RespondAsync($"Sent msg in {channel.Mention} after {length} minutes"); //update user
    }
}