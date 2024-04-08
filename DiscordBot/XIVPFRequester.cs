// TEMPORARY

namespace DiscordBot
{
    internal class XIVPFRequester
    {
        private static HttpClient HttpClient = new()
        {
            BaseAddress = new Uri("https://xivpf.com"),
        };

        public XIVPFRequester() { }

        public async Task<string> Request()
        {
            using HttpResponseMessage response = await HttpClient.GetAsync("listings");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
