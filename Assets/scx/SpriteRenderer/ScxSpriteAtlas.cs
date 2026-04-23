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
    }

}