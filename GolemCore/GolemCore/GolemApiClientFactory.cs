namespace GolemCore;

public static class GolemApiClientFactory
{
    public static IGolemApiClient Create(string host, string apiKey)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(host)
        };
        ConfigureHttpClient(httpClient, host, apiKey);

        return new GolemApiClient(httpClient);
    }

    private static void ConfigureHttpClient(HttpClient httpClient, string host, string apiKey)
    {
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
        httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
    }
}