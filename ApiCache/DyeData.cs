using Blish_HUD;
using Ideka.NetCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Blish_HUD.GameService;
using Color = Microsoft.Xna.Framework.Color;
using Dye = Gw2Sharp.WebApi.V2.Models.Color;

namespace Ideka.BHUDCommon;

public class DyeData : ApiCache<int, Dye>
{
    private static readonly Logger _logger = Logger.GetLogger<DyeData>();
    protected override Logger Logger => _logger;

    public DyeData(string cacheFilePath) : base(cacheFilePath)
    {
    }

    public IEnumerable<Dye>? BestMatch(Color color)
    {
        lock (_lock)
        {
            if (!Data.Any())
                return null;

            return Data.Values
                .Select(x => (
                    diff:
                        Math.Abs(color.R - x.Cloth.Rgb[0]) +
                        Math.Abs(color.G - x.Cloth.Rgb[1]) +
                        Math.Abs(color.B - x.Cloth.Rgb[2]),
                    dye: x))
                .GroupBy(x => x.diff)
                .OrderBy(x => x.First().diff)
                .First()
                .Select(x => x.dye);
        }
    }

    protected override async Task<IEnumerable<Dye>> ApiGetter(CancellationToken ct)
        => await Gw2WebApi.AnonymousConnection.Client.V2.Colors.AllAsync(ct);
}
