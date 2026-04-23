using UnityEngine;

/// SCX 图集
public sealed class ScxSpriteAtlas {
    /// 贴图
    public readonly Texture2D texture;

    /// 精灵列表
    public readonly ScxSprite[] sprites;

    public ScxSpriteAtlas(Texture2D texture, ScxSprite[] sprites) {
        this.texture = texture;
        this.sprites = sprites;
        for (var i = 0; i < this.sprites.Length; i++) {
            var sprite = this.sprites[i];
            sprite.uv = computeUV(texture.width, texture.height, sprite);
        }
    }

    private static Vector2[] computeUV(int textureWidth, int textureHeight, ScxSprite sprite) {
        var rect = sprite.atlasRect;

        var uMin = (float)rect.x / textureWidth;
        var vMin = (float)rect.y / textureHeight;
        var uMax = (float)(rect.x + rect.width) / textureWidth;
        var vMax = (float)(rect.y + rect.height) / textureHeight;

        return new[] {
            new Vector2(uMin, vMin), // 0
            new Vector2(uMax, vMin), // 1
            new Vector2(uMin, vMax), // 2
            new Vector2(uMax, vMax), // 3
        };
    }
    
}