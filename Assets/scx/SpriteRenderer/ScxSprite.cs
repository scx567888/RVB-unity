using UnityEngine;

namespace scx.SpriteRenderer {
    /// SCX 图块元数据 (不支持旋转)
    ///
    /// 坐标规范（非常重要）:
    /// 1. atlasRect 使用图集像素坐标，原点在图集左下角，x 向右，y 向上。
    /// 2. sourceRect 使用原图像素坐标，原点在原图左下角，x 向右，y 向上。
    /// 3. sourceSize 是原图尺寸（裁边前尺寸）。
    /// 4. pivot 是基于原图尺寸 sourceSize 的归一化锚点：
    ///    (0, 0) = 原图左下角，(1, 1) = 原图右上角。
    ///
    /// 约定:
    /// - 所有进入运行时渲染层的数据，都必须先转换到上述统一坐标系。
    /// - 如果外部工具导出的 Rect 是左上原点，应在导入阶段完成一次性转换，
    ///   渲染阶段不再做任何 Y 翻转。
    public sealed class ScxSprite {
        /// 图块名称
        public readonly string name;

        /// 图块在图集中的区域（像素坐标，左下原点）
        public readonly RectInt atlasRect;

        /// 图块在原图中的有效区域（像素坐标，左下原点）
        public readonly RectInt sourceRect;

        /// 原图尺寸（裁边前尺寸，单位：像素）
        public readonly Vector2Int sourceSize;

        /// 基于原图尺寸 sourceSize 的归一化锚点
        /// (0,0)=左下角, (1,1)=右上角
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