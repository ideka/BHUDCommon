using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Ideka.BHUDCommon.AnchoredRect;

public class Anchoring
{
    public virtual Vector2 AnchorMin { get; set; } = new Vector2(0, 0);
    public virtual Vector2 AnchorMax { get; set; } = new Vector2(1, 1);

    [JsonIgnore]
    public virtual float AnchorMinX { get => AnchorMin.X; set { AnchorMin = new Vector2(value, AnchorMin.Y); } }
    [JsonIgnore]
    public virtual float AnchorMinY { get => AnchorMin.Y; set { AnchorMin = new Vector2(AnchorMin.X, value); } }
    [JsonIgnore]
    public virtual float AnchorMaxX { get => AnchorMax.X; set { AnchorMax = new Vector2(value, AnchorMax.Y); } }
    [JsonIgnore]
    public virtual float AnchorMaxY { get => AnchorMax.Y; set { AnchorMax = new Vector2(AnchorMax.X, value); } }
    [JsonIgnore]
    public virtual float AnchorX { set { AnchorMinX = AnchorMaxX = value; } }
    [JsonIgnore]
    public virtual float AnchorY { set { AnchorMinY = AnchorMaxY = value; } }

    public virtual Vector2 Pivot { get; set; } = new Vector2(.5f, .5f);

    [JsonIgnore]
    public virtual float PivotX { get => Pivot.X; set { Pivot = new Vector2(value, Pivot.Y); } }
    [JsonIgnore]
    public virtual float PivotY { get => Pivot.Y; set { Pivot = new Vector2(Pivot.X, value); } }

    public virtual Vector2 Position { get; set; } = new Vector2(0, 0);

    [JsonIgnore]
    public virtual float PositionX { get => Position.Y; set { Position = new Vector2(value, Position.Y); } }
    [JsonIgnore]
    public virtual float PositionY { get => Position.Y; set { Position = new Vector2(Position.X, value); } }

    public virtual Vector2 SizeDelta { get; set; } = new Vector2(0, 0);

    [JsonIgnore]
    public virtual float SizeDeltaX { get => SizeDelta.X; set { SizeDelta = new Vector2(value, SizeDelta.Y); } }
    [JsonIgnore]
    public virtual float SizeDeltaY { get => SizeDelta.Y; set { SizeDelta = new Vector2(SizeDelta.X, value); } }
}
