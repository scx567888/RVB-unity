using UnityEngine;

/// <summary>
/// SCX 图集
/// </summary>
public sealed class ScxSpriteAtlas {
    
    /// <summary>
    /// 贴图
    /// </summary>
    public readonly Texture2D texture;
    
    /// <summary>
    /// 精灵列表
    /// </summary>
    public readonly ScxSprite[] sprites;

    public ScxSpriteAtlas(Texture2D texture, ScxSprite[] sprites) {
        this.texture = texture;
        this.sprites = sprites;
    }

}
