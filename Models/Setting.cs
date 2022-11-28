using LiteDB;

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

    public static void Initialise()
    {
        using var db = new LiteDatabase(Database.GetPath());
        var col = db.GetCollection<Setting>("settings");
        //Check if collection is empty or more than one
        if (col.Count() != 1)
        {
            col.DeleteAll();
            Console.WriteLine("Please enter your bot token:");
            var token = Console.ReadLine();
            col.Insert(new Setting
            {
                Token = token,
                //Defaults for DougDoug discord
                guildID = "567141138021089308",
                statusMessage = "^_^",
                reactionFilterEmotes = "👍,👎",
                reactionFilterChannels = "731407385624838197,567144073857859609,880127379119415306",
                reactionFilterRole = "988901586669551687",
                YoutubePostChannel = "567144073857859609",
                YoutubeChannels =
                    "@DougDougW;720717475901341726\n@dougdougdoug;720717475901341726\n@DougDougVODChannel;731411366799343707\n@@friendstildeath3427;934873771406413885",
                YoutubeLastCheck = DateTime.Now
            });
        }
    }

    public static Setting GetSettings()
    {
        using var db = new LiteDatabase(Database.GetPath());
        var col = db.GetCollection<Setting>("settings");
        return col.FindById(1);
    }

    public void UpdateSettings()
    {
        using var db = new LiteDatabase(Database.GetPath());
        var col = db.GetCollection<Setting>("settings");
        col.Update(this);
    }
}