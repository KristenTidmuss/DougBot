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
        //Get chat to send
        var messages = await Context.Channel.GetMessagesAsync(20).FlattenAsync();
        var queryString = "This is a discord chat conversation\\n";
        foreach (var message in messages.OrderBy(m => m.Timestamp))
            queryString += $"{message.Author.Username}: {message.Content}\\n";
        //clean string and trim
        var blockedChars = "/#?&=;+!@£$%^*(){}[]|<>,~`\"'";
        foreach (var c in blockedChars) queryString = queryString.Replace(c.ToString(), "");
        if (queryString.Length > 2000) queryString = queryString.Substring(0, 2000);
        //remove any text after the last instance of :
        var lastColon = queryString.LastIndexOf(':');
        queryString = queryString.Substring(0, lastColon);
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
        await RespondAsync("Responding");
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