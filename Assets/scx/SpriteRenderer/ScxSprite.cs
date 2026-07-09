namespace scx.SpriteRenderer {
    /// SCX 图块元数据
    ///
    /// - 像素坐标 采用左下原点坐标系: 原点在左下角, x 向右, y 向上.
    /// - 归一化坐标 采用左下原点坐标系: 原点在左下角, (0,0) = 左下角, (1,1) = 右上角.
    /// - 不支持图集打包阶段的旋转: 图块在图集中的方向必须与原图方向一致.
    public interface ScxSprite {
        /// 图块名称
        public string name();

        /// 创建 ScxSpriteRenderData
        public ScxSpriteRenderData createSpriteRenderData(int textureWidth, int textureHeight, float pixelsPerUnit);
    }
}