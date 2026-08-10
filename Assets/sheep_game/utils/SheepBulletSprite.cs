using System;
using scx.SpriteRenderer;
using UnityEngine;

namespace sheep_game.utils {

    /// <summary>
    /// 子弹图块元数据。
    ///
    /// 这里只负责生成：
    /// 1. 图集 UV；
    /// 2. 以局部原点为中心的四边形。
    ///
    /// 子弹的位置、方向、旋转和整体缩放由更上层负责。
    /// 主要对应原版 rotType == 3 / rotType == 5 的渲染方式。
    /// </summary>
    [Serializable]
    public class SheepBulletSprite : ScxSprite {

        public string _name;

        /// <summary>
        /// 原始数据中的帧编号或索引。
        /// 当前渲染数据计算不使用。
        /// </summary>
        public int i;

        public int w;
        public int h;

        /// <summary>
        /// 原始帧偏移。
        /// rotType 3/5 的原版计算没有使用这些值，
        /// 因此本实现保留数据但不参与局部顶点计算。
        /// </summary>
        public float x;
        public float y;
        public float z;

        public float sx;
        public float sy;

        public float[] uv;

        public string name() {
            return _name;
        }

        public ScxSpriteRenderData createSpriteRenderData(
            int textureWidth,
            int textureHeight,
            float pixelsPerUnit
        ) {
            if (string.IsNullOrEmpty(_name)) {
                throw new InvalidOperationException(
                    "SheepBulletSprite name 不能为空."
                );
            }

            if (uv == null || uv.Length != 8) {
                throw new InvalidOperationException(
                    $"SheepBulletSprite uv 错误: {_name}, " +
                    $"预期长度为 8，实际长度为 {uv?.Length ?? 0}."
                );
            }

            

            if (w < 0 || h < 0) {
                throw new InvalidOperationException(
                    $"SheepBulletSprite 尺寸错误: {_name}, w={w}, h={h}."
                );
            }

            /*
             * 原始 Cocos UV 的 V 方向与 Unity 相反，
             * 因此需要执行：
             *
             * UnityV = 1 - CocosV
             */
            Vector2 cocosUvToUnityUv(int index) {
                return new Vector2(
                    uv[index],
                    1f - uv[index + 1]
                );
            }

            /*
             * 原始 uv 数组顶点含义：
             *
             * uv[0], uv[1] -> 左下
             * uv[2], uv[3] -> 右下
             * uv[4], uv[5] -> 左上
             * uv[6], uv[7] -> 右上
             *
             * ScxSpriteRenderData 顶点顺序：
             *
             * p0 -> 左下
             * p1 -> 右下
             * p2 -> 左上
             * p3 -> 右上
             */
            var uv0 = cocosUvToUnityUv(0);
            var uv1 = cocosUvToUnityUv(2);
            var uv2 = cocosUvToUnityUv(4);
            var uv3 = cocosUvToUnityUv(6);

            /*
             * 原版 rotType 3/5 使用：
             *
             * width  = frame.w * frame.sx * bulletScale
             * height = frame.h * frame.sy * bulletScale
             *
             * bulletScale 由 ScxSpriteRenderUnit.setScale() 处理，
             * 因此这里仅计算帧自身的 sx/sy。
             */
            var width = w * sx / pixelsPerUnit;
            var height = h * sy / pixelsPerUnit;

            var halfWidth = width * 0.5f;
            var halfHeight = height * 0.5f;

            /*
             * 四边形以局部原点为中心。
             *
             * 上层只需要设置：
             * - position：子弹世界位置
             * - rotation：根据 rotType 和 direction 计算
             * - scale：子弹 conf.scale
             */
            var p0x = -halfWidth;
            var p0y = -halfHeight;
            var p0z = 0f;

            var p1x = halfWidth;
            var p1y = -halfHeight;
            var p1z = 0f;

            var p2x = -halfWidth;
            var p2y = halfHeight;
            var p2z = 0f;

            var p3x = halfWidth;
            var p3y = halfHeight;
            var p3z = 0f;

            return new ScxSpriteRenderData(
                uv0,
                uv1,
                uv2,
                uv3,

                p0x,
                p0y,
                p0z,

                p1x,
                p1y,
                p1z,

                p2x,
                p2y,
                p2z,

                p3x,
                p3y,
                p3z
            );
        }
    }
}