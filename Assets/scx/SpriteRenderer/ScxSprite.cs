using UnityEngine;

/// <summary>
/// SCX 图块 (不支持旋转)
/// </summary>
public interface ScxSprite { 
    
    /// 图块名称
    string name();
    
    /// 该图块在图集中的矩形区域
    RectInt atlasRect();
    
    /// 该图块在原图中的矩形区域
    RectInt sourceRect();  
    
    /// 原图尺寸 (裁边前)
    Vector2Int sourceSize();

    /// 归一化锚点, 通常 0~1
    Vector2 pivot();
    
}
