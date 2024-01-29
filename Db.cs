using Microsoft.EntityFrameworkCore;

namespace bot
{
    public class Db : DbContext
    {
        public static string GetLocalDbConnection() =>
            $"Data Source={Environment.GetEnvironmentVariable("Local_Db_Path")}" +
            $"{Environment.GetEnvironmentVariable("Local_Db_File")};";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite(GetLocalDbConnection());

        public DbSet<TelegramChat> TelegramChats { get; set; }
    }

    public class TelegramChat
    {
        public int Id { get; set; }
        public long ChatId { get; set; }

        public TelegramChat()
        {

        }
        public TelegramChat(Telegram.Chat chat)
        {
            ChatId = chat.Id;
        }

        public Telegram.Chat ToChat() => new()
        {
            Id = ChatId
        };
    }
}