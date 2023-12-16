using Gw2Sharp.WebApi.Exceptions;
using Gw2Sharp.WebApi.V2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Blish_HUD.GameService;

namespace Ideka.BHUDCommon;

public class SkinData : ApiCache<int, Skin>
{
    protected override async Task<IEnumerable<Skin>> ApiGetter(CancellationToken ct)
    {
        var items = new List<Skin>();
        for (int i = 0;; i++)
        {
            try
            {
                var page = await Gw2WebApi.AnonymousConnection.Client.V2.Skins.PageAsync(i, ct);
                if (!page.Any())
                    break;
                items.AddRange(page);
            }
            catch (PageOutOfRangeException)
            {
                break;
            }
        }
        return items;
    }
}
