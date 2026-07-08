using UnityEngine;

namespace scx.SpriteRenderer {
    /// SCX 图集
    /// 
    /// 图集由一张真实贴图和若干图块元数据组成.
    public sealed class ScxSpriteAtlas {
        /// 贴图
        public readonly Texture2D texture;

        /// 图块元数据列表
        public readonly ScxSprite[] sprites;

        public ScxSpriteAtlas(Texture2D texture, ScxSprite[] sprites) {
            this.texture = texture;
            this.sprites = sprites;
        }
    }
}