using UnityEngine;

/// <summary>
/// SCX 图块 (不支持旋转)
/// </summary>
public sealed class ScxSprite {

    /// 图块名称
    public readonly string name;

    /// 该图块在图集中的矩形区域
    public readonly RectInt atlasRect;

    /// 该图块在原图中的矩形区域
    public readonly RectInt sourceRect;

    /// 原图尺寸 (裁边前)
    public readonly Vector2Int sourceSize;

    /// 归一化锚点, 通常 0~1
    public readonly Vector2 pivot;

    public ScxSprite(string name, RectInt atlasRect, RectInt sourceRect, Vector2Int sourceSize, Vector2 pivot) {
        this.name = name;
        this.atlasRect = atlasRect;
        this.sourceRect = sourceRect;
        this.sourceSize = sourceSize;
        this.pivot = pivot;
    }
    
}