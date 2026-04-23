using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// SCX 图集
public sealed class ScxSpriteAtlas {
    /// 贴图
    public readonly Texture2D texture;

    /// 精灵列表
    public readonly ScxSprite[] sprites;
    
    private readonly Dictionary<string, ScxSprite> namedSprites;
    
    private readonly string[] frameNames;
    

    public ScxSpriteAtlas(Texture2D texture, ScxSprite[] sprites) {
        this.texture = texture;
        this.sprites = sprites;
        this.namedSprites = new Dictionary<string, ScxSprite>();
        for (var i = 0; i < this.sprites.Length; i++) {
            var sprite = this.sprites[i];
            namedSprites[sprite.name] = sprite;
            sprite.uv = computeUV(texture.width, texture.height, sprite);
        }

        frameNames = namedSprites.Keys.ToArray();
    }

    public ScxSprite getByName(string name) {
        return namedSprites[name];
    }
    
    public ScxSprite getByIndex(int index) {
        return sprites[index];
    }
    
    public string[] getFrameNames() {
        return this.frameNames;
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