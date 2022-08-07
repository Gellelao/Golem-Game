using System.Net.Http.Json;
using System.Text.Json;
using GolemCore.Models;
using GolemCore.Models.Golem;
using GolemCore.Models.Part;

namespace GolemCore.Api;

public class GolemApiClient : IGolemApiClient
{
    private readonly HttpClient _httpClient;
    public readonly JsonSerializerOptions Options;

    public GolemApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        var options = new JsonSerializerOptions();
        options.Converters.Add(new TriggerConverter());
        Options = options;
    }

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
        var result = await _httpClient.PostAsJsonAsync(Constants.PostEndpoint, golem, Options, cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to post golem");
        }
    }

    public async Task UpdateGolem(UpdateRequest update, CancellationToken cancellationToken)
    {
        var result = await _httpClient.PutAsJsonAsync(Constants.PostEndpoint, update, Options, cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to update golem");
        }
    }

    public async Task<List<Part>> GetParts(CancellationToken cancellationToken)
    {
        var parts = await _httpClient.GetFromJsonAsync<PartFetchResponse>(Constants.GetPartEndpoint, Options, cancellationToken);

        if (parts is null or { Items.Count: 0 } or { Count: 0 })
        {
            throw new InvalidOperationException("Failed to fetch any parts");
        }

        return parts.Items;
    }

    public async Task PostPart(CreatePartRequest part, CancellationToken cancellationToken)
    {
        var result = await _httpClient.PostAsJsonAsync(Constants.PostEndpoint, part, Options, cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to post part");
        }
    }
}