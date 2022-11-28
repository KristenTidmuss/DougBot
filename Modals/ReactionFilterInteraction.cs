using Discord.Interactions;
using DougBot.Models;

namespace DougBot.Modals;

public class ReactionFilterInteraction : InteractionModuleBase
{
    [ModalInteraction("settingsReactionFilter")]
    public async Task ModalResponse(ReactionFilterModal modal)
    {
        var settings = Setting.GetSettings();
        settings.reactionFilterEmotes = modal.reactionEmotes;
        settings.reactionFilterChannels = modal.reactionChannels;
        settings.reactionFilterRole = modal.reactionRole;
        settings.UpdateSettings();
        await RespondAsync("Settings Saved", ephemeral: true);
    }
}