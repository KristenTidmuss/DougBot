using Discord;
using Discord.WebSocket;
using DougBot.Models;
using Newtonsoft.Json;

namespace DougBot.Scheduler;

public static class Freshmen
{
    public static async void CheckFreshmen(DiscordSocketClient client)
    {
        var settings = Setting.GetSettings();
        var guild = client.Guilds.FirstOrDefault(g => g.Id.ToString() == settings.guildID);
        var role = guild.Roles.FirstOrDefault(r => r.Id == 935020318408462398);
        var users = await guild.GetUsersAsync().FlattenAsync();
        users = users.Where(u =>
            u.JoinedAt.Value.UtcDateTime < DateTime.UtcNow.AddDays(-7) &&
            u.RoleIds.Contains(role.Id));
        foreach (var user in users)
        {
            var dict = new Dictionary<string,string>{
                {"guildId", guild.Id.ToString()},
                {"userId", user.Id.ToString()},
                {"roleId", role.Id.ToString()}
            };
            var json = JsonConvert.SerializeObject(dict);
            Queue.Create("RemoveRole", null, json, DateTime.UtcNow);
        }
    }
}