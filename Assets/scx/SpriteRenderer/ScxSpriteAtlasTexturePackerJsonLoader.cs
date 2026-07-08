using System;
using System.IO;
using UnityEngine;

// todo 待优化
namespace scx.SpriteRenderer {
    /// ScxSpriteAtlas 加载器 
    /// 适用于 TexturePacker 导出的 JSON (frames 为数组格式)
    public static class ScxSpriteAtlasTexturePackerJsonLoader {
        /// 将 TexturePacker 导出的 JSON (frames 为数组格式) 加载为 ScxSpriteAtlas
        public static ScxSpriteAtlas load(Texture2D texture, string json) {
            if (texture == null) {
                throw new ArgumentNullException(nameof(texture));
            }

            if (string.IsNullOrWhiteSpace(json)) {
                throw new ArgumentException("json is null or empty", nameof(json));
            }

            var root = JsonUtility.FromJson<TpRoot>(json);
            if (root == null || root.frames == null || root.frames.Length == 0) {
                throw new Exception("TexturePacker json parse failed, or frames is empty.");
            }

            // 优先用 json meta.size，其次退回 texture 本身尺寸
            int atlasWidth = (root.meta != null && root.meta.size != null && root.meta.size.w > 0)
                ? root.meta.size.w
                : texture.width;

            int atlasHeight = (root.meta != null && root.meta.size != null && root.meta.size.h > 0)
                ? root.meta.size.h
                : texture.height;

            var sprites = new ScxSprite[root.frames.Length];

            for (int i = 0; i < root.frames.Length; i++) {
                TpFrame f = root.frames[i];

                if (f.rotated) {
                    throw new NotSupportedException(
                        $"ScxSprite does not support rotated sprite: {f.filename}"
                    );
                }

                // 1) atlasRect: TexturePacker 是左上原点 -> Scx 需要左下原点
                RectInt atlasRect = new RectInt(
                    f.frame.x,
                    atlasHeight - f.frame.y - f.frame.h,
                    f.frame.w,
                    f.frame.h
                );

                // 2) sourceRect: spriteSourceSize 也是基于原图左上原点 -> 转成原图左下原点
                RectInt sourceRect = new RectInt(
                    f.spriteSourceSize.x,
                    f.sourceSize.h - f.spriteSourceSize.y - f.spriteSourceSize.h,
                    f.spriteSourceSize.w,
                    f.spriteSourceSize.h
                );

                // 3) 原图尺寸直接抄
                Vector2Int sourceSize = new Vector2Int(
                    f.sourceSize.w,
                    f.sourceSize.h
                );

                // 4) pivot
                // 大多数情况下可以直接用。
                // 如果你导出的某个格式 pivot 实际是左上原点，再改成 new Vector2(f.pivot.x, 1f - f.pivot.y)
                Vector2 pivot = new Vector2(
                    f.pivot.x,
                    1f - f.pivot.y
                );

                // 名字你可以自己决定：
                // A. 保留完整路径: "1/yellow_attack (1).png"
                // string name = f.filename;

                // B. 去掉扩展名，仅保留文件名: "yellow_attack (1)"
                string name = Path.GetFileNameWithoutExtension(f.filename);

                sprites[i] = new ScxSprite(
                    name,
                    atlasRect,
                    sourceRect,
                    sourceSize,
                    pivot
                );
            }

            return new ScxSpriteAtlas(texture, sprites);
        }


        [Serializable]
        private class TpRoot {
            public TpFrame[] frames;
            public TpMeta meta;
        }

        [Serializable]
        private class TpMeta {
            public TpSize size;
        }

        [Serializable]
        private class TpFrame {
            public string filename;
            public TpRect frame;
            public bool rotated;
            public bool trimmed;
            public TpRect spriteSourceSize;
            public TpSize sourceSize;
            public TpPivot pivot;
        }

        [Serializable]
        private class TpRect {
            public int x;
            public int y;
            public int w;
            public int h;
        }

        [Serializable]
        private class TpSize {
            public int w;
            public int h;
        }

        [Serializable]
        private class TpPivot {
            public float x;
            public float y;
        }
    }
}