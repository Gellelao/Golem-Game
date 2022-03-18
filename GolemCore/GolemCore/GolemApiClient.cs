using System.Net.Http.Json;
using GolemCore.Models;

namespace GolemCore;

public class GolemApiClient : IGolemApiClient
{
    private readonly HttpClient _httpClient;

    public GolemApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<Golem>> GetGolemSelection(CancellationToken cancellationToken)
    {
        var golems = await _httpClient.GetFromJsonAsync<GolemFetchResponse>(Constants.GetGolemEndpoint, cancellationToken);

        if (golems is null or { Items.Count: 0 } or { Count: 0 })
        {
            throw new InvalidOperationException("Failed to fetch any golems");
        }

        return golems.Items;
    }

    public async Task CreateGolem(CreateGolemRequest golem, CancellationToken cancellationToken)
    {
        var result = await _httpClient.PostAsJsonAsync(Constants.PostGolemEndpoint, golem, cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to post golem");
        }
    }

    public Task GetBattleResults(Golem myGolem, Golem opponent, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}