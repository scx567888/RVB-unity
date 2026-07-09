using UnityEngine;

namespace scx.SpriteRenderer {
    /// 运行时渲染缓存数据.
    ///
    /// 该类会预计算两类数据:
    /// 1. UV (图集采样范围)
    /// 2. 以 pivot 为局部原点的四边形顶点坐标.
    public sealed class ScxSpriteRenderData {
        // UV 对应关系:
        // uv0 -> 左下
        // uv1 -> 右下
        // uv2 -> 左上
        // uv3 -> 右上
        public readonly Vector2 uv0;
        public readonly Vector2 uv1;
        public readonly Vector2 uv2;
        public readonly Vector2 uv3;

        // 局部顶点 (单位: 世界单位)
        // 顶点顺序:
        // p0 -> 左下
        // p1 -> 右下
        // p2 -> 左上
        // p3 -> 右上
        //
        // 坐标均以 sprite 的 pivot 为局部原点.
        public readonly float p0x;
        public readonly float p0y;
        public readonly float p0z;

        public readonly float p1x;
        public readonly float p1y;
        public readonly float p1z;

        public readonly float p2x;
        public readonly float p2y;
        public readonly float p2z;

        public readonly float p3x;
        public readonly float p3y;
        public readonly float p3z;

        public ScxSpriteRenderData(
            Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3,
            float p0x, float p0y, float p0z,
            float p1x, float p1y, float p1z,
            float p2x, float p2y, float p2z,
            float p3x, float p3y, float p3z
        ) {
            this.uv0 = uv0;
            this.uv1 = uv1;
            this.uv2 = uv2;
            this.uv3 = uv3;
            this.p0x = p0x;
            this.p0y = p0y;
            this.p0z = p0z;
            this.p1x = p1x;
            this.p1y = p1y;
            this.p1z = p1z;
            this.p2x = p2x;
            this.p2y = p2y;
            this.p2z = p2z;
            this.p3x = p3x;
            this.p3y = p3y;
            this.p3z = p3z;
        }
    }
}