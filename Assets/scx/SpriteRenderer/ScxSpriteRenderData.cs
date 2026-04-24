

using UnityEngine;

public class ScxSpriteRenderData {
    public readonly Vector2 uv0;
    public readonly Vector2 uv1;
    public readonly Vector2 uv2;
    public readonly Vector2 uv3;
    
    
    public readonly Vector3 p0;
    public readonly Vector3 p1;
    public readonly Vector3 p2;
    public readonly Vector3 p3;

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
        
        

        var pivotPixelX = sprite.pivot.x * sprite.sourceSize.x;
        var pivotPixelY = sprite.pivot.y * sprite.sourceSize.y;

        var left   = (sprite.sourceRect.x - pivotPixelX) / pixelsPerUnit;
        var right  = (sprite.sourceRect.x + sprite.sourceRect.width - pivotPixelX) / pixelsPerUnit;
        var bottom = (sprite.sourceRect.y - pivotPixelY) / pixelsPerUnit;
        var top    = (sprite.sourceRect.y + sprite.sourceRect.height - pivotPixelY) / pixelsPerUnit;

        p0 = new Vector3(left,  bottom, 0);
        p1 = new Vector3(right, bottom, 0);
        p2 = new Vector3(left,  top,    0);
        p3 = new Vector3(right, top,    0);
        
    }
    
}