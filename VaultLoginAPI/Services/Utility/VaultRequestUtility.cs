namespace VaultLoginAPI.Services.Utility
{
    public class VaultRequestUtility
    {
        /**
         A generic function used to make requests to Vault to reduce the amount of duplcate code.

         @param method the HTTP method that will be utilized in the request (GET, POST, PUT, DELETE...)
         
         @param url the Vault URL that the request will be made to.

         @param content the body of the HTTP request, this is often times null in the event of a GET request.

         @param authToken the authorization token that will be used in the request.

         @returns the response message from the HTTP request.
         */

        public static HttpResponseMessage SendRequestToVault(HttpMethod method, string url, HttpContent? content, string? authToken)
        {
            HttpClient client = new();
            HttpRequestMessage message = new(method, url);
            if (content != null)
                message.Content = content;
            message.Headers.Add("X-Vault-Token", authToken);
            return client.Send(message);
        }

        /**
         Breaks down a comma delimited response into a string list. Can be expanded to do any non alphanumeric character
         via Regex.

         @param response the string response to be broken down.
         
         @returns the response broken down into a list, an empty list if the response is null.
         */
        public static List<string> BreakDownListResponse(string? response)
        {
            List<string> respList = new();
            if (response != null)
                respList = new(response.Split(","));
            return respList;
        }
    }

}
