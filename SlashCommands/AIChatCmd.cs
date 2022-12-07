using Discord;
using Discord.Interactions;
using DougBot.Models;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace DougBot.SlashCommands;

public class AIChatCmd : InteractionModuleBase
{
    [SlashCommand("aichat", "Send an AI message into chat")]
    [EnabledInDm(false)]
    [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
    public async Task AIChat([Summary(description: "Messages to process (Default: 5)"), MaxValue(100)] int procCount = 5)
    {
        await RespondAsync("Processing Chat", ephemeral: true);
        var settings = Setting.GetSettings();
        //Get chat to send
        var messages = await Context.Channel.GetMessagesAsync(procCount).FlattenAsync();
        var queryString = "WAHAHA is a chatbot that reluctantly takes part in chat with sarcastic responses:\n\n";
        //Ignore embeds and media
        messages = messages.Where(m =>
            m.Embeds.Count == 0 &&
            m.Attachments.Count == 0 &&
            !m.Content.StartsWith("<") && !m.Content.EndsWith(">")
        ).OrderBy(m => m.Timestamp);
        //Process all messages
        foreach (var message in messages)
        {
            queryString += $"{message.Author.Username}: {message.Content}\n";
        }
        queryString += "WAHAHA: ";
        //Query API for chat response
        var openAiService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = settings.OpenAiToken
        });
        var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest
        {
            Prompt = queryString,
            MaxTokens = 30,
            Temperature = (float)0.5,
            TopP = 1,
            PresencePenalty = (float)0.0,
            FrequencyPenalty = (float)0.5,
            Stop = "\n"
        }, OpenAI.GPT3.ObjectModels.Models.Curie);
        if (!completionResult.Successful)
        {
            throw new Exception("API Error: " + completionResult.Error);
        }
        var aiText = completionResult.Choices.FirstOrDefault().Text;
        //check the response is not offensive using the OpenAI moderation API
        var moderationResult = await openAiService.Moderation.CreateModeration(new CreateModerationRequest
        {
            Input = aiText
        });
        if (!moderationResult.Successful)
        {
            throw new Exception("API Error: " + moderationResult.Error);
        }
        //calculate cost
        var cost = completionResult.Usage.TotalTokens * 0.000002;
        //Respond
        if (moderationResult.Results.Any(r => !r.Flagged) && aiText != "")
        {
            await ReplyAsync(aiText);
            await ModifyOriginalResponseAsync(m =>
                m.Content = $"Message sent\n" +
                            $"Tokens: Input {completionResult.Usage.PromptTokens} + Output {completionResult.Usage.CompletionTokens} = {completionResult.Usage.TotalTokens}\n" +
                            $"Cost: ${cost}");
        }
        else
        {
            await ModifyOriginalResponseAsync(m =>
                m.Content = $"Message failed to pass content moderation\n" +
                            $"Tokens: Input {completionResult.Usage.PromptTokens} + Output {completionResult.Usage.CompletionTokens} = {completionResult.Usage.TotalTokens}\n" +
                            $"Cost: ${cost}\n" +
                            $"Response: {aiText}");
        }
    }
}