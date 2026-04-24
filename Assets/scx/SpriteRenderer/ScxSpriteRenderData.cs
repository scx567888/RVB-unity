

using UnityEngine;

public class ScxSpriteRenderData {
    public readonly Vector2 uv0;
    public readonly Vector2 uv1;
    public readonly Vector2 uv2;
    public readonly Vector2 uv3;
    
    
    public readonly float left;
    public readonly float right;
    public readonly float bottom;
    public readonly float top;

    public ScxSpriteRenderData(ScxSprite sprite, int textureWidth, int textureHeight, float pixelsPerUnit) {
        var rect = sprite.atlasRect;

        var uMin = (float)rect.x / textureWidth;
        var vMin = (float)rect.y / textureHeight;
        var uMax = (float)(rect.x + rect.width) / textureWidth;
        var vMax = (float)(rect.y + rect.height) / textureHeight;


        uv0 = new Vector2(uMin, vMin); // 0
        uv1 = new Vector2(uMax, vMin); // 1
        uv2 = new Vector2(uMin, vMax); // 2
        uv3 = new Vector2(uMax, vMax); // 3
        
        

        // 1. pivot 在原图中的像素位置
        var pivotPixelX = sprite.pivot.x * sprite.sourceSize.x;
        var pivotPixelY = sprite.pivot.y * sprite.sourceSize.y;

        // 2. 裁边后矩形在“以 pivot 为原点”的局部像素空间中的范围
        var _left = sprite.sourceRect.x - pivotPixelX;
        var _right = sprite.sourceRect.x + sprite.sourceRect.width - pivotPixelX;
        var _bottom = sprite.sourceRect.y - pivotPixelY;
        var _top = sprite.sourceRect.y + sprite.sourceRect.height - pivotPixelY;

        // 3. 像素转单位
        this.left = _left/pixelsPerUnit;
        this.right = _right/pixelsPerUnit;
        this.bottom = _bottom/pixelsPerUnit;
        this.top = _top/pixelsPerUnit;
        
        
    }
    
}