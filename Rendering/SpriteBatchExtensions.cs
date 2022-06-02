using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ideka.BHUDCommon
{
    internal static class SpriteBatchExtensions
    {
        private static Texture2D _whitePixelTexture;

        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (_whitePixelTexture == null)
            {
                _whitePixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _whitePixelTexture.SetData(new[] { Color.White });
                spriteBatch.Disposing += (sender, args) =>
                {
                    _whitePixelTexture?.Dispose();
                    _whitePixelTexture = null;
                };
            }

            return _whitePixelTexture;
        }

        public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 position, Color color, float size = 1f, float layerDepth = 0)
        {
            var scale = Vector2.One * size;
            var offset = new Vector2(0.5f) - new Vector2(size * 0.5f);
            spriteBatch.Draw(GetTexture(spriteBatch), position + offset, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }

        public static void DrawPoint(this SpriteBatch spriteBatch, float x, float y, Color color, float size = 1f, float layerDepth = 0)
        {
            DrawPoint(spriteBatch, new Vector2(x, y), color, size, layerDepth);
        }

        public static void DrawPolygon(this SpriteBatch spriteBatch, Vector2 offset, IEnumerable<Vector2> points, Color color, float thickness = 1f, float layerDepth = 0, bool open = false)
        {
            if (!points.Any())
                return;

            if (!points.Skip(1).Any())
            {
                DrawPoint(spriteBatch, points.First(), color, thickness);
                return;
            }

            var texture = GetTexture(spriteBatch);

            foreach (var (point, next) in points.Zip(points.Skip(1), (a, b) => (a, b)))
                DrawPolygonEdge(spriteBatch, texture, point + offset, next + offset, color, thickness, layerDepth);

            if (!open)
                DrawPolygonEdge(spriteBatch, texture, points.Last() + offset, points.First() + offset, color, thickness, layerDepth);
        }

        private static void DrawPolygonEdge(SpriteBatch spriteBatch, Texture2D texture, Vector2 point1, Vector2 point2, Color color, float thickness, float layerDepth)
        {
            var length = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(texture, point1, null, color, angle, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }

        public static void DrawRectangleFill(this SpriteBatch spriteBatch, RectangleF rectangle, Color color, float layerDepth = 0)
        {
            float center = rectangle.Left + rectangle.Width / 2;
            spriteBatch.DrawLine(center, rectangle.Top, center, rectangle.Bottom, color, rectangle.Width, layerDepth);
        }

        const float SideLength = 3;

        public static int GetSidesFor(Vector2 radii, double amplitude)
            => GetSidesFor(Math.Max(radii.X, radii.Y), amplitude);

        // TODO: Find a better alternative to this
        public static int GetSidesFor(float radius, double amplitude)
            => Math.Max(3, (int)Math.Ceiling(Math.Abs(
                amplitude * MathUtils.AngleToRad / Math.Acos(1 - MathUtils.Squared(SideLength) / MathUtils.Squared(radius) / 2))));

        public static void DrawArc(this SpriteBatch spriteBatch, Vector2 center,
            Vector2 radii, double start, double extents, int sides,
            Color color, float thickness = 1f, float layerDepth = 0)
        {
            spriteBatch.DrawArc(center, radii.X, radii.Y, start, extents, sides, color, thickness, layerDepth);
        }

        public static void DrawArc(this SpriteBatch spriteBatch, Vector2 center,
            float rx, float ry, double start, double extents, int sides,
            Color color, float thickness = 1f, float layerDepth = 0)
        { 
            extents = double.IsNaN(extents) ? 0
                : double.IsInfinity(extents) ? 2
                : extents;

            rx += thickness / 2 * Math.Sign(extents);
            ry += thickness / 2 * Math.Sign(extents);

            spriteBatch.DrawPolygon(center, CreateArc(rx, ry, start, extents, sides), color, thickness, layerDepth, true);
        }

        public static Vector2[] CreateArc(float rx, float ry, double start, double extents, int sides)
        {
            var points = new Vector2[sides];
            var theta = start * MathUtils.AngleToRad;
            var step = extents * MathUtils.AngleToRad / (sides - 1);

            for (var i = 0; i < sides; i++)
            {
                var vector = new Vector2(-(float)(rx * Math.Cos(theta)), -(float)(ry * Math.Sin(theta)));
                points[i] = vector;
                theta += step;
            }

            return points;
        }
    }
}
