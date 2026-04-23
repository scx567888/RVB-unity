using UnityEngine;

/// <summary>
/// SCX 图
/// </summary>
public class ScxSprite { 
    
    /// 图块名称
    private string name;
    
    /// 该图块在图集中的矩形区域
    private RectInt atlasRect;
    
    /// 该图块在原图中的矩形区域
    private RectInt sourceRect;  
    
    /// 原图尺寸 (裁边前)
    private Vector2Int sourceSize;

    /// 归一化锚点, 通常 0~1
    private Vector2 pivot;
    
}
