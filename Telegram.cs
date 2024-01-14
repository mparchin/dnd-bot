using System.Net;
using System.Net.Http.Json;

namespace bot
{
    public class Telegram : IDisposable
    {
        public class User
        {
            public long Id { get; set; }
        }

        public class Chat
        {
            public long Id { get; set; }
        }
        public class Message
        {
            public long Message_id { get; set; }
            public Chat Chat { get; set; } = new();
        }

        private class SendMessage
        {
            public long Chat_id { get; set; }
            public string Text { get; set; } = "";
        }

        public class Update
        {
            public long Update_id { get; set; }
            public Message? Message { get; set; }
        }

        private class Response<TEntity>
        {
            public bool Ok { get; set; }
            public TEntity? Result { get; set; }
        }

        private readonly string _token;
        private string GetPath(string method) => $"https://api.telegram.org/bot{_token}/{method}";

        // private readonly HttpClient _client = new(new HttpClientHandler
        // {
        //     Proxy = new WebProxy
        //     {
        //         Address = new Uri($"http://172.20.10.3:10809")
        //     }
        // });

        private readonly HttpClient _client = new();

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }

        public Telegram(string token)
        {
            _token = token;
        }

        public async Task<User> GetMeAsync()
        {
            var ret = new Response<User>();
            var res = await _client.GetAsync(GetPath("getMe"));
            if (res.IsSuccessStatusCode)
                ret = await res.Content.ReadFromJsonAsync<Response<User>>();
            return ret?.Result ?? new User();
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            var ret = new Response<Update[]>();
            var res = await _client.GetAsync(GetPath("getUpdates"));
            if (res.IsSuccessStatusCode)
                ret = await res.Content.ReadFromJsonAsync<Response<Update[]>>();
            return ret?.Result?.Select(u => u.Message?.Chat ?? new Chat())
                .Where(chat => chat.Id != 0)
                .GroupBy(chat => chat.Id)
                .Select(g => g.First())
                .ToList() ?? [];
        }

        public Task SendAsync(string text, params Chat[] chats) =>
            Task.WhenAll(chats.Select(chat => new SendMessage
            {
                Chat_id = chat.Id,
                Text = text
            }).Select(message => _client.PostAsJsonAsync(GetPath("sendMessage"), message)));

    }
}