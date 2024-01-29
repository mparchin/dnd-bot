using bot;
using Microsoft.EntityFrameworkCore;

using var github = new Github(Environment.GetEnvironmentVariable("Github_Owner") ?? "",
    Environment.GetEnvironmentVariable("Github_Repo") ?? "",
    Environment.GetEnvironmentVariable("Github_Branch") ?? "");

using var telegram = new Telegram(Environment.GetEnvironmentVariable("Telegram_Bot_Token") ?? "");

using var db = new Db();
await db.Database.MigrateAsync();

var commit = await github.GetLatestCommitMessageAsync();

var newChats = (await telegram.GetAllChatsAsync())
    .Where(chat => db.TelegramChats.All(dbChat => dbChat.ChatId != chat.Id))
    .ToList();

db.TelegramChats.AddRange(newChats.Select(chat => new TelegramChat(chat)));
await db.SaveChangesAsync();

var chats = db.TelegramChats
    .ToList()
    .Select(chat => chat.ToChat())
    .ToArray();

Console.WriteLine($"Writing to {chats.Length} chats");

await telegram
    .SendAsync(
        $"Update 🎉:\n{commit}\n\nAs always we appreciate ANY comments (mostly positive thou 😜)\n@Eldrin @mparchin @ParsaRashidi",
        chats);