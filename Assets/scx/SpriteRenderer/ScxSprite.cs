namespace scx.SpriteRenderer {
    /// SCX 图块元数据
    public interface ScxSprite {
        /// 图块名称
        public string name();

        /// 创建 ScxSpriteRenderData
        public ScxSpriteRenderData createSpriteRenderData(int textureWidth, int textureHeight, float pixelsPerUnit);
    }
}