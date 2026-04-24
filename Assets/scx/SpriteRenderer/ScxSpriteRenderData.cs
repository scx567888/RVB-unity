using UnityEngine;

public class ScxSpriteRenderData {
    public readonly Vector2 uv0;
    public readonly Vector2 uv1;
    public readonly Vector2 uv2;
    public readonly Vector2 uv3;
    
    
    public readonly float p0x;
    public readonly float p0y;
    public readonly float p0z;
    
    public readonly float p1x;
    public readonly float p1y;
    public readonly float p1z;
    
    public readonly float p2x;
    public readonly float p2y;
    public readonly float p2z;
    
    public readonly float p3x;
    public readonly float p3y;
    public readonly float p3z;

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

        p0x = left;
        p0y = bottom;
        p0z = 0;
        
        p1x = right;
        p1y = bottom;
        p1z = 0;
        
        p2x = left;
        p2y = top;
        p2z = 0;
        
        p3x = right;
        p3y = top;
        p3z = 0;
        
    }
    
}