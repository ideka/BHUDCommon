using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static Blish_HUD.GameService;

namespace Ideka.BHUDCommon
{
    public class Primitive
    {
        public List<Vector3> Points { get; }

        public Primitive(IEnumerable<Vector3> points)
        {
            Points = points.ToList();
        }

        public Primitive(params Vector3[] points) : this(points.AsEnumerable())
        {
        }

        public IEnumerable<Vector3> Transform(Matrix matrix)
        {
            foreach (var p in Points)
                yield return Vector3.Transform(p, matrix);
        }

        public Primitive Transformed(Matrix matrix)
            => new Primitive(Transform(matrix));

        public IEnumerable<Vector2> ToScreen()
        {
            foreach (var p in Points)
                yield return ToScreen(p);
        }

        public static Primitive operator +(Primitive a, Primitive b)
            => new Primitive(a.Points.Concat(b.Points));

        public static Vector2 Flatten(Vector3 v)
            => v.Z <= 0
                ? new Vector2(float.NaN, float.NaN)  // TODO: Figure out a more graceful way of handling this edge case
                : new Vector2(
                    (v.X / v.Z + 1) / 2 * Graphics.SpriteScreen.Width,
                    (1 - v.Y / v.Z) / 2 * Graphics.SpriteScreen.Height);

        public static Vector2 ToScreen(Vector3 point)
            => Flatten(Vector3.Transform(point, Gw2Mumble.PlayerCamera.WorldViewProjection));

        public static Primitive HorizontalCircle(float radius, int sides)
        {
            var points2d = SpriteBatchExtensions.CreateCircle(radius, sides);
            var points = new Vector3[sides];
            for (var i = 0; i < sides; i++)
            {
                var src = points2d[i];
                points[i] = new Vector3(src.X, src.Y, 0);
            }

            return new Primitive(points);
        }

        public static Primitive VerticalArc(float rx, float ry, float start, float extents, int sides)
        {
            var points2d = SpriteBatchExtensions.CreateArc(rx, ry, start, extents, sides);
            var points = new Vector3[sides];
            for (var i = 0; i < sides; i++)
            {
                var src = points2d[i];
                points[i] = new Vector3(src.X, 0, src.Y);
            }

            return new Primitive(points);
        }
    }
}
