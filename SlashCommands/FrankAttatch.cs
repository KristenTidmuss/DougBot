using System.Net.WebClient;
using System.IO;
using Discord;
using Discord.Interactions;

namespace DougBot.SlashCommands;

public class FrankAttach : InteractionModuleBase
{
    [SlashCommand("frankattach", "send an attachment into current channel")]
    [EnabledInDm(false)]
    [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
    public async Task frankattach([Summary(description: "Existing name of file")] string existName,
     [Summary(description: "New file name")] string name,
     [Summary(description: "File Extension (txt, pdf, mp3, wav, etc.")] string ext,
     [Summary(description: "Channel to type in")] ITextChannel channel,
     [Summary(description: "Message to include")] string msg,
     [Summary(description: "TTS? (T/F)")] string tts,
     [Summary(description: "Link to attachment ()")] string file )
    {
        string fileName = "";
        //file doesn't already exists
        if (existName == null)
        {
            using (WebClient myWebClient = new WebClient())
                {
                    // Download the Web resource and save it into the current filesystem folder.
                    myWebClient.DownloadFile(file, Path.combine("./FileStorage/", fileName));        
                }
            fileName = name + ext;
        }
        else //file exists, pull from FileStorage
        {
            fileName = existName + ext;
        }

        if(tts.ToLower() = "t") // check whether to tts msg
        {
            var messageId = channel.SendFileAsync(fileName, msg, true);
        }
        else
        {
            var messageId = channel.SendFileAsync(fileName, msg);
        }
        await RespondAsync($"Sent file in {channel.Mention} "); //update user

    }
}