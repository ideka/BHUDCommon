using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    }
}
