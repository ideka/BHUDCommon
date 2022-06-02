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

        public static Vector2 ToScreen(Matrix world)
            => Flatten(Vector3.Transform(Vector3.Zero, world * Gw2Mumble.PlayerCamera.WorldViewProjection));

        public static Vector2 ToScreen(Vector3 point)
            => ToScreen(Matrix.CreateTranslation(point));

        public static Primitive HorizontalCircle(float radius, int sides)
        {
            const double max = 2.0 * Math.PI;
            var points = new Vector3[sides];
            var step = max / sides;
            var theta = 0.0;

            for (var i = 0; i < sides; i++)
            {
                points[i] = new Vector3((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)), 0);
                theta += step;
            }

            return new Primitive(points);
        }
    }
}
