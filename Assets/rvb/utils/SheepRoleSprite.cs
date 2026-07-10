using System;
using scx.SpriteRenderer;
using UnityEngine;

namespace rvb.utils {
    
    [Serializable]
    public class SheepRoleSprite : ScxSprite {

        public string _name;

        public float x;
        public float y;
        public float z;

        public int w;
        public int h;

        public float sx;
        public float sy;

        public float ax;
        public float ay;

        public float rz;

        public float[] uv;
        
        public string name() {
            return _name;
        }

        public ScxSpriteRenderData createSpriteRenderData(
            int textureWidth,
            int textureHeight,
            float pixelsPerUnit
        ) {
            if (uv == null || uv.Length != 8) {
                throw new Exception($"SheepSprite uv 错误: {_name}");
            }

            // 关键：roles_framess 的 v 方向和 Unity 反了
            Vector2 CocosUvToUnityUv(int index) {
                return new Vector2(
                    uv[index],
                    1f - uv[index + 1]
                );
            }

            // 原 JS 正常朝向下：
            // Pos0 左上 -> m[4], m[5]
            // Pos1 左下 -> m[0], m[1]
            // Pos2 右下 -> m[2], m[3]
            // Pos3 右上 -> m[6], m[7]
            //
            // 你的 renderer 顶点顺序：
            // p0 左下
            // p1 右下
            // p2 左上
            // p3 右上
            //
            // 所以映射为：
            // p0 左下 -> m[0], m[1]
            // p1 右下 -> m[2], m[3]
            // p2 左上 -> m[4], m[5]
            // p3 右上 -> m[6], m[7]
            var uv0 = CocosUvToUnityUv(0); // 左下
            var uv1 = CocosUvToUnityUv(2); // 右下
            var uv2 = CocosUvToUnityUv(4); // 左上
            var uv3 = CocosUvToUnityUv(6); // 右上

            var width = w * sx;
            var height = h * sy;

            var left = (-ax * width + x) / pixelsPerUnit;
            var right = ((1f - ax) * width + x) / pixelsPerUnit;
            var bottom = (-ay * height + y) / pixelsPerUnit;
            var top = ((1f - ay) * height + y) / pixelsPerUnit;

            var zz = z / pixelsPerUnit;

            return new ScxSpriteRenderData(
                uv0,
                uv1,
                uv2,
                uv3,
                left, bottom, zz,  // p0 左下
                right, bottom, zz, // p1 右下
                left, top, zz,     // p2 左上
                right, top, zz     // p3 右上
            );
        }
    }
}