namespace VaultLoginAPI.Services.Utility
{
    public class VaultRequestUtility
    {
        public static HttpResponseMessage SendRequestToVault(HttpMethod method, string url, HttpContent? content, string? authToken)
        {
            HttpClient client = new();
            HttpRequestMessage message = new(method, url);
            if (content != null)
                message.Content = content;
            message.Headers.Add("X-Vault-Token", authToken);
            return client.Send(message);
        }

        public static List<string> BreakDownListResponse(string? response)
        {
            List<string> respList = new();
            if (response != null)
                respList = new(response.Split(","));
            return respList;
        }
    }

}
