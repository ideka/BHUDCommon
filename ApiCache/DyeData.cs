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
    public IEnumerable<(Dye dye, float match)> BestMatches(Color color)
    {
        static float max(float a, float b, float c, float d)
            => Math.Max(a, Math.Max(b, Math.Max(c, d)));

        static float colorMatch(Color color, IReadOnlyList<int> other)
            => other == null || other.Count < 3
                ? 0
                : 1 - (
                    Math.Abs(color.R - other[0]) / 255f +
                    Math.Abs(color.G - other[1]) / 255f +
                    Math.Abs(color.B - other[2]) / 255f) / 3;

        return Items.Values
            .Select(x => (
                dye: x,
                match:
                    max(
                        colorMatch(color, x.Cloth.Rgb),
                        colorMatch(color, x.Metal.Rgb),
                        colorMatch(color, x.Leather.Rgb),
                        colorMatch(color, x.Fur.Rgb))))
            .OrderByDescending(x => x.match);
    }

    protected override async Task<IEnumerable<Dye>> ApiGetter(CancellationToken ct)
        => await Gw2WebApi.AnonymousConnection.Client.V2.Colors.AllAsync(ct);
}
