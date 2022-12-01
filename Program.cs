﻿using System.Reflection;
using Discord;
using Discord.Commands;
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
    private bool _FirstStart = true;


    private static Task Main()
    {
        return new Program().MainAsync();
    }

    private async Task MainAsync()
    {
        //Load Settings
        var settings = Setting.GetSettings();
        //Start discord bot
        var config = new DiscordSocketConfig();
        config.GatewayIntents = GatewayIntents.All;
        _Client = new DiscordSocketClient(config);
        _Client.Log += Log;
        _Client.Ready += Ready;
        await _Client.LoginAsync(TokenType.Bot, settings.Token);
        await _Client.StartAsync();
        //Block Task
        await Task.Delay(-1);
    }

    private async Task Ready()
    {
        if (_FirstStart)
        {
            _FirstStart = false;
            //Register Plugins
            var reactionFilter = new ReactionFilter(_Client);
            var youtube = new Youtube(_Client);
            var scheduler = new Scheduler(_Client);
            //Register Commands
            _Service = new InteractionService(_Client.Rest);
            _Service.Log += Log;
            await _Service.AddModulesAsync(Assembly.GetEntryAssembly(), _ServiceProvider);
            await _Service.RegisterCommandsGloballyAsync();
            _Client.InteractionCreated += async interaction =>
            {
                var ctx = new SocketInteractionContext(_Client, interaction);
                await _Service.ExecuteCommandAsync(ctx, _ServiceProvider);
                //Log command ran
                var cmd = (SocketSlashCommand)interaction;
                await Log(new LogMessage(LogSeverity.Info, "Command", 
                    $"Command {cmd.Data.Name} ran by {cmd.User.Username}#{cmd.User.Discriminator} in {cmd.Channel.Name}"));
            };
            _Service.SlashCommandExecuted += async (command, context, result) =>
            {
                if (!result.IsSuccess)
                {
                    if (result.ErrorReason.Contains("was not present in the dictionary.") && command.Name == "timestamp")
                    {
                        await context.Interaction.RespondAsync("Invalid time format. Please use the format `12:00 GMT ` or `01 Jan 2022 12:00 GMT`", ephemeral: true);
                    }
                    else
                    {
                        if (!context.Interaction.HasResponded)
                        {
                            await context.Interaction.RespondAsync($"Error: {result.ErrorReason}", ephemeral: true);
                        }
                        else
                        {
                            await context.Interaction.ModifyOriginalResponseAsync(m => m.Content = $"Error: {result.ErrorReason}");
                        }
                    }
                }
            };
        }
        //Status
        await _Client.SetStatusAsync(UserStatus.DoNotDisturb);
    }

    private Task Log(LogMessage msg)
    {
        if (msg.Exception is CommandException cmdException)
        {
            Console.WriteLine($"[Command/{msg.Severity}] {cmdException.Command.Name}"
                              + $" failed to execute in {cmdException.Context.Channel} by user {cmdException.Context.User.Username}.");
            Console.WriteLine(cmdException);
        }
        else if (msg.Source == "Command")
        {
            Console.WriteLine($"[Command/{msg.Severity}] {DateTime.UtcNow.ToString("h:mm:ss")} {msg.Message}");
        }
        else
        {
            Console.WriteLine($"[General/{msg.Severity}] {msg}");

        }
        return Task.CompletedTask;
    }
}