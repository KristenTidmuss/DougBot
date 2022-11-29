namespace DougBot.Models;

public class Setting
{
    public int Id { get; set; }
    public string Token { get; set; }
    public string guildID { get; set; }
    public string statusMessage { get; set; }
    public string reactionFilterEmotes { get; set; }
    public string reactionFilterChannels { get; set; }
    public string reactionFilterRole { get; set; }
    public string YoutubePostChannel { get; set; }
    public string YoutubeChannels { get; set; }
    public DateTime YoutubeLastCheck { get; set; }

    public static Setting GetSettings()
    {
        using var db = new Database.DougBotContext();
        return db.Settings.FirstOrDefault();
    }

    public static void UpdateReactionFilter(string reactionFilterEmotes, string reactionFilterChannels, string reactionFilterRole)
    {
        using var db = new Database.DougBotContext();
        var settings = db.Settings.FirstOrDefault();
        settings.reactionFilterEmotes = reactionFilterEmotes;
        settings.reactionFilterChannels = reactionFilterChannels;
        settings.reactionFilterRole = reactionFilterRole;
        db.SaveChanges();
    }
    public static void UpdateYoutubeFeed(string YoutubePostChannel, string YoutubeChannels)
    {
        using var db = new Database.DougBotContext();
        var settings = db.Settings.FirstOrDefault();
        settings.YoutubePostChannel = YoutubePostChannel;
        settings.YoutubeChannels = YoutubeChannels;
        db.SaveChanges();
    }

    public static void UpdateStatusMessage(string statusMessage)
    {
        using var db = new Database.DougBotContext();
        var settings = db.Settings.FirstOrDefault();
        settings.statusMessage = statusMessage;
        db.SaveChanges();
    }
    
    public static void UpdateLastChecked(DateTime lastChecked)
    {
        using var db = new Database.DougBotContext();
        var settings = db.Settings.FirstOrDefault();
        settings.YoutubeLastCheck = lastChecked;
        db.SaveChanges();
    }
}