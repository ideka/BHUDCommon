using Blish_HUD;
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

    public IEnumerable<(int diff, Dye dye)> BestMatches(Color color)
    {
        static int min(int a, int b, int c, int d)
            => Math.Min(a, Math.Min(b, Math.Min(c, d)));

        static int colorDiff(Color color, IReadOnlyList<int> other)
            => Math.Abs(color.R - other[0]) + Math.Abs(color.G - other[1]) + Math.Abs(color.B - other[2]);

        return Items.Values
            .Select(x => (
                diff:
                    min(
                        colorDiff(color, x.Cloth.Rgb),
                        colorDiff(color, x.Metal.Rgb),
                        colorDiff(color, x.Leather.Rgb),
                        colorDiff(color, x.Fur.Rgb)),
                dye: x))
            .OrderBy(x => x.diff);
    }

    protected override async Task<IEnumerable<Dye>> ApiGetter(CancellationToken ct)
        => await Gw2WebApi.AnonymousConnection.Client.V2.Colors.AllAsync(ct);
}
