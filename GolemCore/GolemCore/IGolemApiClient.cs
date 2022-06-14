using GolemCore.Models;
using GolemCore.Models.Golem;
using GolemCore.Models.Part;

namespace GolemCore;

public interface IGolemApiClient
{
    public Task<List<Golem>> GetGolemSelection(CancellationToken cancellationToken);
    public Task CreateGolem(CreateGolemRequest golem, CancellationToken cancellationToken);
    public Task UpdateGolem(UpdateRequest update, CancellationToken cancellationToken);
    public Task<List<Part>> GetParts(CancellationToken cancellationToken);
}