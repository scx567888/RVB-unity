using UnityEngine;

namespace scx.SpriteRenderer {
    /// SCX 图块元数据
    ///
    /// - 像素坐标采用左下原点坐标系: 原点在左下角, x 向右, y 向上.
    /// - 归一化坐标采用左下原点坐标系: 原点在左下角, (0,0) = 左下角, (1,1) = 右上角.
    /// - 不支持图集打包阶段的旋转.
    public sealed class ScxSprite {
        /// 图块名称
        public readonly string name;

        /// 图块在图集中的区域 (像素坐标)
        public readonly RectInt atlasRect;

        /// 图块在原图中的有效区域 (像素坐标)
        public readonly RectInt sourceRect;

        /// 原图尺寸/裁边前尺寸 (像素单位)
        public readonly Vector2Int sourceSize;

        /// 基于原图尺寸 sourceSize 的锚点 (归一化坐标)
        public readonly Vector2 pivot;

        public ScxSprite(
            string name,
            RectInt atlasRect,
            RectInt sourceRect,
            Vector2Int sourceSize,
            Vector2 pivot
        ) {
            this.name = name;
            this.atlasRect = atlasRect;
            this.sourceRect = sourceRect;
            this.sourceSize = sourceSize;
            this.pivot = pivot;
        }
    }
}