using Discord.Interactions;
using DougBot.Models;

namespace DougBot.Modals;

public class YoutubeInteraction : InteractionModuleBase
{
    [ModalInteraction("settingsYoutubeFeed")]
    public async Task ModalResponse(YoutubeModal modal)
    {
        var settings = Setting.GetSettings();
        settings.YoutubePostChannel = modal.YoutubePostChannel;
        settings.YoutubeChannels = modal.youtubeChannels;
        settings.UpdateSettings();
        await RespondAsync("Settings Saved", ephemeral: true);
    }
}