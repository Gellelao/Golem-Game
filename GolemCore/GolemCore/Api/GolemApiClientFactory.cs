namespace GolemCore.Api;

public static class GolemApiClientFactory
{
    private static readonly string Host = "";
    private static readonly string ApiKey = "";

    public static IGolemApiClient Create()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(Host)
        };
        ConfigureHttpClient(httpClient, Host, ApiKey);

        return new GolemApiClient(httpClient);
    }

    private static void ConfigureHttpClient(HttpClient httpClient, string host, string apiKey)
    {
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
        httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
    }
}