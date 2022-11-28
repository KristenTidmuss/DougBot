using LiteDB;

namespace DougBot.Models;

public class Queue
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DueAt { get; set; }

    public static void Create(string type, string data, DateTime dueAt)
    {
        using var db = new LiteDatabase(Database.GetPath());
        var col = db.GetCollection<Queue>("customers");
        col.Insert(new Queue
        {
            Type = type,
            Data = data,
            CreatedAt = DateTime.UtcNow,
            DueAt = dueAt
        });
    }

    public static List<Queue> GetAll()
    {
        using var db = new LiteDatabase(Database.GetPath());
        var col = db.GetCollection<Queue>("customers");
        return col.FindAll().ToList();
    }

    public static List<Queue> GetDue()
    {
        using var db = new LiteDatabase(Database.GetPath());
        var col = db.GetCollection<Queue>("customers");
        return col.Find(c => c.DueAt < DateTime.UtcNow).ToList();
    }

    public static void Remove(Queue queue)
    {
        using var db = new LiteDatabase(Database.GetPath());
        var col = db.GetCollection<Queue>("customers");
        col.Delete(queue.Id);
    }
}