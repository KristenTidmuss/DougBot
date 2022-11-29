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
        var blockedChars = "/#?&=;+!@£$%^*(){}[]|<>,~`\"'";
        //Get chat to send
        var messages = await Context.Channel.GetMessagesAsync(10).FlattenAsync();
        var queryString = "This is a discord chat conversation\\n";
        foreach (var message in messages.Where(m => m.Embeds.Count == 0 && m.Attachments.Count == 0).OrderBy(m => m.Timestamp))
        {
            //replace any instance of blockedChars in message.content
            var content = message.Content;
            foreach (var c in blockedChars)
            {
                content = content.Replace(c, ' ');
            }
            if(queryString.Length + content.Length > 2000) {break;}
            queryString += content + "\\n";
        }
        //clean string and trim
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
        await RespondAsync("Responding: If nothing is sent to chat there was nothing legible from the AI", ephemeral: true);
        string responseMessage = "";
        //Loop every line in the output, if it contains a : then send anything after it and if not just send the message
        foreach (var line in output.Split("\n"))
            if (line.Contains(":"))
            {
                var split = line.Split(":");
                responseMessage += split[1] + Environment.NewLine;
            }
            else if (line != "")
            {
                responseMessage += line + Environment.NewLine;
            }
        await Context.Channel.SendMessageAsync(responseMessage);
    }
}