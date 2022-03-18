using GolemCore.Models;

namespace GolemCore;

public interface IGolemApiClient
{
    public Task<List<Golem>> GetGolemSelection(CancellationToken cancellationToken);
    public Task CreateGolem(CreateGolemRequest golem, CancellationToken cancellationToken);
    public Task GetBattleResults(Golem myGolem, Golem opponent, CancellationToken cancellationToken);
}