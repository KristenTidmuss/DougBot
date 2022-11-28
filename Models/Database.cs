using System.Diagnostics;

namespace DougBot.Models;

public class Database
{
    public static string GetPath()
    {
        var path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        return Path.Join(path, "database.db");
    }
}