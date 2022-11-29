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
        using var db = new Database.DougBotContext();
        db.Queues.Add(new Queue
        {
            Type = type,
            Data = data,
            CreatedAt = DateTime.UtcNow,
            DueAt = dueAt
        });
        db.SaveChanges();
    }

    public static List<Queue> GetAll()
    {
        using var db = new Database.DougBotContext();
        return db.Queues.ToList();
    }

    public static List<Queue> GetDue()
    {
        using var db = new Database.DougBotContext();
        return db.Queues.Where(c => c.DueAt < DateTime.UtcNow).ToList();
    }

    public static void Remove(Queue queue)
    {
        using var db = new Database.DougBotContext();
        db.Queues.Remove(queue);
        db.SaveChanges();
    }
}