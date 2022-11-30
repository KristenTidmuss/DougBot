using System.Net.Http.Headers;
using Discord;
using Discord.Interactions;
using Newtonsoft.Json.Linq;

namespace DougBot.SlashCommands;

public class AIChatCmd : InteractionModuleBase
{
    [SlashCommand("aichat", "Send an AI message into chat")]
    [EnabledInDm(false)]
    [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
    public async Task AIChat()
    {
        await RespondAsync("Processing Chat", ephemeral: true);
        var blockedChars = "/#?&=;+!@£$%^*(){}[]|<>,~`\"'";
        //Get chat to send
        var messages = await Context.Channel.GetMessagesAsync(10).FlattenAsync();
        var queryString = "This is a discord chat conversation\\n";
        //Ignore embeds and media
        foreach (var message in messages.Where(m => m.Embeds.Count == 0 && m.Attachments.Count == 0).OrderBy(m => m.Timestamp))
        {
            //replace any instance of blockedChars in message.content
            var content = blockedChars.Aggregate(message.Content, (current, c) => current.Replace(c, ' '));
            //If hits the char limit then stop
            if(queryString.Length + content.Length > 2000) {break;}
            queryString += content + "\\n";
        }
        //Query API
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.novelai.net/ai/generate");
        request.Headers.TryAddWithoutValidation("accept", "application/json");
        request.Content = new StringContent("{\n  \"input\": \"" + queryString +
                                            "\",\n  \"model\": \"euterpe-v2\",\n  \"parameters\": {\n    \"use_string\": true,\n    \"temperature\": 1,\n    \"min_length\": 10,\n    \"max_length\": 30\n  }\n}");
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        var response = await httpClient.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        dynamic json = JToken.Parse(responseString);
        string output = json.output;
        //Get the first line and if it contains an : then get the second half
        if (output != null)
        {
            var firstLine = output.Split("\\n")[0];
            output = firstLine.Contains(":") ? firstLine.Split(":")[1] : firstLine;
            await ReplyAsync(output);
        }
        else
        {
            throw new Exception("No output from API");
        }
    }
}