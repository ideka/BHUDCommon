using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using static Blish_HUD.GameService;

namespace Ideka.BHUDCommon;

public static class PrimitiveExtensions
{
    private static bool _requested = false;
    private static BasicEffect? _effect;
    private static BasicEffect? Effect
    {
        get
        {
            if (_effect == null && !_requested)
            {
                _requested = true;
                Graphics.QueueMainThreadRender(graphicsDevice =>
                {
                    if (_requested && _effect == null)
                    {
                        _effect = new BasicEffect(graphicsDevice);
                        _requested = false;
                    }
                });
            }

            return _effect;
        }
    }

    public static void DrawPrimitive(this SpriteBatch spriteBatch, Primitive primitive, Color color)
    {
        if (Effect is not BasicEffect effect)
            return;

        var vertices = primitive.Points.Select(p => new VertexPositionColor(p, color)).ToArray();

        effect.Projection = Gw2Mumble.PlayerCamera.WorldViewProjection;
        effect.CurrentTechnique.Passes[0].Apply();
        spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1);
    }
}

public class Primitive(IEnumerable<Vector3> points)
{
    public List<Vector3> Points { get; } = points.ToList();

    public class ScreenPrimitive
    {
        public List<List<Vector2>> Points { get; } = [];
        public float Depth { get; }
        private float MaxDepth => Gw2Mumble.PlayerCamera.FarPlaneRenderDistance;

        public ScreenPrimitive(IEnumerable<Vector3> points)
        {
            float sum = 0;
            int count = 0;
            List<Vector2>? list = null;
            foreach (var point in points)
            {
                if (list == null)
                {
                    list = [];
                    Points.Add(list);
                }

                if (point.Z < 0)
                {
                    list = null;
                    continue;
                }

                list.Add(Flatten(point));
                sum += point.Z;
                count++;
            }

            Depth = sum / count / MaxDepth;
        }

        public void Draw(SpriteBatch spriteBatch, Color color, float thickness)
        {
            foreach (var list in Points)
                spriteBatch.DrawPolygon(Vector2.Zero, list, color, thickness, Depth, true);
        }

        public static Vector2 Flatten(Vector3 v)
            => new Vector2(
                (v.X / v.Z + 1) / 2 * Graphics.SpriteScreen.Width,
                (1 - v.Y / v.Z) / 2 * Graphics.SpriteScreen.Height);
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

    public IEnumerable<Vector2> Flat()
        => Points.Select(p => p.ToVector2());

    public ScreenPrimitive ToScreen()
        => new ScreenPrimitive(Points.Select(ToScreen));

    public static Primitive operator +(Primitive a, Primitive b)
        => new Primitive(a.Points.Concat(b.Points));

    public static Vector3 ToScreen(Vector3 point)
        => Vector3.Transform(point, Gw2Mumble.PlayerCamera.WorldViewProjection);

    public static Vector3[] To3D(Vector2[] points2d, float xx, float xy, float xz, float yx, float yy, float yz)
    {
        var points = new Vector3[points2d.Length];
        for (var i = 0; i < points2d.Length; i++)
        {
            var src = points2d[i];
            points[i] = new Vector3(
                src.X * xx + src.Y * yx,
                src.X * xy + src.Y * yy,
                src.X * xz + src.Y * yz);
        }

        return points;
    }

    public static Primitive HorizontalArc(float rx, float ry, float start, float extents, int sides)
        => new Primitive(To3D(SpriteBatchExtensions.CreateArc(rx, ry, start, extents, sides), 1, 0, 0, 0, 1, 0));

    public static Primitive HorizontalCircle(float radius, int sides)
        => HorizontalArc(radius, radius, 0, 2, sides);

    public static Primitive VerticalArc(float rx, float ry, float start, float extents, int sides)
        => new Primitive(To3D(SpriteBatchExtensions.CreateArc(rx, ry, start, extents, sides), 1, 0, 0, 0, 0, 1));

    public static Primitive VerticalCircle(float radius, int sides)
        => VerticalArc(radius, radius, 0, 2, sides);
}
