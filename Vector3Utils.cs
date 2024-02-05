using Ideka.NetCommon;
using Microsoft.Xna.Framework;
using System;

namespace Ideka.BHUDCommon;

public static class Vector3Utils
{
    public static int DistanceSign(Vector3 a, Vector3 b, float distance)
        => Math.Sign((a - b).LengthSquared() - MathUtils.Squared(distance));

    public static bool DistanceUnder(Vector3 a, Vector3 b, float distance)
        => DistanceSign(a, b, distance) < 0;

    public static bool DistanceOver(Vector3 a, Vector3 b, float distance)
        => DistanceSign(a, b, distance) > 0;

    public static bool DistanceEqual(Vector3 a, Vector3 b, float distance)
        => DistanceSign(a, b, distance) == 0;
}
