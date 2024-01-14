using bot;

using var github = new Github(Environment.GetEnvironmentVariable("Github_Owner") ?? "",
    Environment.GetEnvironmentVariable("Github_Repo") ?? "",
    Environment.GetEnvironmentVariable("Github_Branch") ?? "");

using var telegram = new Telegram(Environment.GetEnvironmentVariable("Telegram_Bot_Token") ?? "");

var commit = await github.GetLatestCommitMessageAsync();

var chats = (await telegram.GetAllChatsAsync()).ToList();

await telegram
    .SendAsync(
        $"Update 🎉:\n{commit}\n\nAs always we appreciate ANY comments (mostly positive thou 😜)\n@Eldrin @mparchin @ParsaRashidi",
         [.. chats]);