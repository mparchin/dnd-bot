using System.Net;
using System.Net.Http.Json;

namespace bot
{
    public class Github : IDisposable
    {
        private class CommitResponse
        {
            public Commit Commit { get; set; } = new();
        }
        private class Commit
        {
            public string Message { get; set; } = "";
        }

        private readonly HttpClient _client = new();
        private readonly string _owner;
        private readonly string _repo;
        private readonly string _branch;
        private string Path => $"https://api.github.com/repos/{_owner}/{_repo}/commits?sha={_branch}&per_page=1";
        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }

        public Github(string owner, string repo, string branch)
        {
            _owner = owner;
            _repo = repo;
            _branch = branch;
            _client.DefaultRequestHeaders.Add("User-Agent", "mparchin");
        }

        public async Task<string> GetLatestCommitMessageAsync() =>
            (await _client.GetFromJsonAsync<CommitResponse[]>(Path))?.FirstOrDefault()?
                .Commit.Message ?? "";
    }
}