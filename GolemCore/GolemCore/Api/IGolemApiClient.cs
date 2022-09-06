using GolemCore.Models;
using GolemCore.Models.Golem;
using GolemCore.Models.Part;

namespace GolemCore.Api;

public interface IGolemApiClient
{
    public Task<List<Golem>> GetAllGolems(CancellationToken cancellationToken);
    public Task CreateGolem(CreateGolemRequest golem, CancellationToken cancellationToken);
    public Task UpdateGolem(UpdateRequest update, CancellationToken cancellationToken);
    public Task<List<Part>> GetParts(CancellationToken cancellationToken);
}