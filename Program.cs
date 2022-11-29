using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DougBot.Models;
using DougBot.Systems;

namespace DougBot;

public class Program
{
    //Main Variables
    private static InteractionService _Service;
    private static IServiceProvider _ServiceProvider;
    private static DiscordSocketClient _Client;

    private static Task Main()
    {
        return new Program().MainAsync();
    }

    private async Task MainAsync()
    {
        //Load Settings
        Setting.UpdateLastChecked(DateTime.UtcNow);
        var settings = Setting.GetSettings();
        //Start discord bot
        _Client = new DiscordSocketClient();
        _Client.Log += Log;
        _Client.Ready += Ready;
        await _Client.LoginAsync(TokenType.Bot, settings.Token);
        await _Client.StartAsync();
        //Block Task
        await Task.Delay(-1);
    }

    private async Task Ready()
    {
        //Register Commands
        _Service = new InteractionService(_Client.Rest);
        await _Service.AddModulesAsync(Assembly.GetEntryAssembly(), _ServiceProvider);
        await _Service.RegisterCommandsGloballyAsync();
        _Client.InteractionCreated += async interaction =>
        {
            var ctx = new SocketInteractionContext(_Client, interaction);
            await _Service.ExecuteCommandAsync(ctx, _ServiceProvider);
        };
        _Service.SlashCommandExecuted += async (command, context, result) =>
        {
            if (!result.IsSuccess)
            {
                if (result.ErrorReason.Contains("was not present in the dictionary.") && command.Name == "timestamp")
                {
                    await context.Interaction.RespondAsync("Invalid time format. Please use the format `12:00 GMT ` or `01 Jan 2022 12:00 GMT`", ephemeral: true);
                }
                await context.Interaction.RespondAsync(result.ErrorReason, ephemeral: true);
            }
        };
        var reactionFilter = new ReactionFilter(_Client);
        var youtube = new Youtube(_Client);
        var scheduler = new Scheduler(_Client);
        //Status
        await _Client.SetStatusAsync(UserStatus.DoNotDisturb);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}