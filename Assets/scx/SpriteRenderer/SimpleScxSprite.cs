using UnityEngine;

namespace scx.SpriteRenderer {
    /// 简单 SCX 图块元数据实现
    ///
    /// - 像素坐标 采用左下原点坐标系: 原点在左下角, x 向右, y 向上.
    /// - 归一化坐标 采用左下原点坐标系: 原点在左下角, (0,0) = 左下角, (1,1) = 右上角.
    /// - 不支持图集打包阶段的旋转: 图块在图集中的方向必须与原图方向一致.
    public sealed class SimpleScxSprite : ScxSprite {
        /// 图块名称
        public readonly string _name;

        /// 图块在图集中的区域 (像素坐标)
        public readonly RectInt atlasRect;

        /// 图块在原图中的有效区域 (像素坐标)
        public readonly RectInt sourceRect;

        /// 原图尺寸/裁边前尺寸 (像素单位)
        public readonly Vector2Int sourceSize;

        /// 基于原图尺寸 sourceSize 的锚点 (归一化坐标)
        public readonly Vector2 pivot;

        public SimpleScxSprite(
            string name,
            RectInt atlasRect,
            RectInt sourceRect,
            Vector2Int sourceSize,
            Vector2 pivot
        ) {
            this._name = name;
            this.atlasRect = atlasRect;
            this.sourceRect = sourceRect;
            this.sourceSize = sourceSize;
            this.pivot = pivot;
        }

        public string name() {
            return this._name;
        }

        public ScxSpriteRenderData createSpriteRenderData(int textureWidth, int textureHeight, float pixelsPerUnit) {
            // 1, 计算 UV
            //
            // atlasRect 基于图集左下角为原点，因此可以直接映射到 0~1 UV.
            var uMin = (float)atlasRect.x / textureWidth;
            var vMin = (float)atlasRect.y / textureHeight;
            var uMax = (float)(atlasRect.x + atlasRect.width) / textureWidth;
            var vMax = (float)(atlasRect.y + atlasRect.height) / textureHeight;

            var uv0 = new Vector2(uMin, vMin); // 左下
            var uv1 = new Vector2(uMax, vMin); // 右下
            var uv2 = new Vector2(uMin, vMax); // 左上
            var uv3 = new Vector2(uMax, vMax); // 右上

            // 2, 计算 pivot 在原图中的像素位置
            //
            // pivot 是基于原图尺寸 sourceSize 的归一化锚点:
            // (0,0) = 原图左下角, (1,1) = 原图右上角
            var pivotPixelX = pivot.x * sourceSize.x;
            var pivotPixelY = pivot.y * sourceSize.y;

            // 3, 计算裁边后有效区域，相对 pivot 的局部边界 (像素)
            //
            // sourceRect 表示 "有效图像区域在原图中的位置", 其坐标原点也是左下角.
            //
            // left/right/bottom/top 的含义:
            // - left   : 有效区域左边界, 相对 pivot 的偏移
            // - right  : 有效区域右边界, 相对 pivot 的偏移
            // - bottom : 有效区域下边界, 相对 pivot 的偏移
            // - top    : 有效区域上边界, 相对 pivot 的偏移
            //
            // 然后再除以 pixelsPerUnit, 转换到世界单位.
            var left = (sourceRect.x - pivotPixelX) / pixelsPerUnit;
            var right = (sourceRect.x + sourceRect.width - pivotPixelX) / pixelsPerUnit;
            var bottom = (sourceRect.y - pivotPixelY) / pixelsPerUnit;
            var top = (sourceRect.y + sourceRect.height - pivotPixelY) / pixelsPerUnit;

            // 4, 生成局部四边形顶点
            //
            // 顶点顺序固定为:
            // p0 左下, p1 右下, p2 左上, p3 右上
            var p0x = left;
            var p0y = bottom;
            var p0z = 0f;

            var p1x = right;
            var p1y = bottom;
            var p1z = 0f;

            var p2x = left;
            var p2y = top;
            var p2z = 0f;

            var p3x = right;
            var p3y = top;
            var p3z = 0f;

            return new ScxSpriteRenderData(
                uv0, uv1, uv2, uv3, p0x, p0y, p0z, p1x, p1y, p1z, p2x, p2y, p2z, p3x, p3y, p3z
            );
        }
    }
}