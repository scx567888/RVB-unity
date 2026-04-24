using System.Numerics;

public class ScxSpriteRenderData {
    public readonly Vector2 uv0;
    public readonly Vector2 uv1;
    public readonly Vector2 uv2;
    public readonly Vector2 uv3;

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
    }
    
}