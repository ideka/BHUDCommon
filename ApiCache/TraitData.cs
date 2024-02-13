using Gw2Sharp.WebApi.V2.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Blish_HUD.GameService;

namespace Ideka.BHUDCommon;

public class TraitData : ApiCache<int, Trait>
{
    protected override async Task<IEnumerable<Trait>> ApiGetter(CancellationToken ct)
        => await Gw2WebApi.AnonymousConnection.Client.V2.Traits.AllAsync(ct);
}
