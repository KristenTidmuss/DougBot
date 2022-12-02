using System.Net.WebClient;
using System.Diagnostics;
using System.IO;
using Discord;
using Discord.Interactions;

namespace DougBot.SlashCommands;

public class FrankAttach : InteractionModuleBase
{
    [SlashCommand("imagesync", "add image to images.dougdoug.com")]
    [EnabledInDm(false)]
    [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
    public async Task frankattach([Summary(description: "Image Link")] string url,
     [Summary(description: "file name")] string name,
     [Summary(description: "What's your username")] string username,
     [Summary(description: "File Extension (txt, pdf, mp3, wav, etc.")] string ext )
    {
        Process cmd = new Process();    // open shell process
        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;
        cmd.Start();

        cmd.StandardInput.WriteLine($"~/dougbot-image-hosting/imageSync.sh {url} {name} {username}");   // call shell script that gets the file and pushes it to repo
        cmd.StandardInput.Flush();
        cmd.StandardInput.Close();
        cmd.WaitForExit();
        Console.WriteLine(cmd.StandardOutput.ReadToEnd());

    }
}