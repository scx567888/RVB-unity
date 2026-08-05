using System;
using Newtonsoft.Json;
using UnityEngine;

namespace scx.SpriteRenderer {
    /// TexturePacker JSON Array 格式加载器.
    ///
    /// 坐标约定:
    /// - TexturePacker frame: 图集左上角原点.
    /// - TexturePacker spriteSourceSize: 原图左上角原点.
    /// - SimpleScxSprite: 左下角原点.
    ///
    /// 限制:
    /// - 不支持旋转打包.
    /// - 不支持多张 texture page.
    /// - 不支持 polygon / tight mesh.
    public static class ScxSpriteAtlasTexturePackerJsonLoader {
        /// 使用原图中心作为默认 pivot
        public static ScxSpriteAtlas load(Texture2D texture, string json) {
            return load(texture, json, new Vector2(0.5f, 0.5f));
        }

        /// 加载 TexturePacker JSON Array。
        ///
        /// defaultPivot 是基于 sourceSize 的左下原点归一化坐标：
        /// (0,0) = 左下角
        /// (1,1) = 右上角
        public static ScxSpriteAtlas load(Texture2D texture, string json, Vector2 defaultPivot) {
            if (texture == null) {
                throw new ArgumentNullException(nameof(texture));
            }

            if (string.IsNullOrWhiteSpace(json)) {
                throw new ArgumentException("TexturePacker json 不能为空.", nameof(json));
            }

            TpRoot root;

            try {
                root = JsonConvert.DeserializeObject<TpRoot>(json);
            }
            catch (JsonException exception) {
                throw new ArgumentException("TexturePacker json 解析失败.", nameof(json), exception);
            }

            if (root == null) {
                throw new ArgumentException("TexturePacker json 根对象为空.", nameof(json));
            }

            if (root.frames == null || root.frames.Length == 0) {
                throw new ArgumentException("TexturePacker json 的 frames 为空.", nameof(json));
            }

            // 应当以实际参与渲染的 Texture2D 尺寸为准。
            var atlasWidth = texture.width;
            var atlasHeight = texture.height;

            var sprites = new ScxSprite[root.frames.Length];

            for (var i = 0; i < root.frames.Length; i += 1) {
                var frameData = root.frames[i];

                // 检查
                checkFrame(frameData, i, defaultPivot);

                // 1) atlasRect: TexturePacker 是左上原点 -> Scx 需要左下原点
                RectInt atlasRect = new RectInt(
                    frameData.frame.x,
                    atlasHeight - frameData.frame.y - frameData.frame.h,
                    frameData.frame.w,
                    frameData.frame.h
                );

                // 2) sourceRect: spriteSourceSize 也是基于原图左上原点 -> 转成原图左下原点
                RectInt sourceRect = new RectInt(
                    frameData.spriteSourceSize.x,
                    frameData.sourceSize.h - frameData.spriteSourceSize.y - frameData.spriteSourceSize.h,
                    frameData.spriteSourceSize.w,
                    frameData.spriteSourceSize.h
                );

                // 3) 原图尺寸直接抄
                Vector2Int sourceSize = new Vector2Int(
                    frameData.sourceSize.w,
                    frameData.sourceSize.h
                );

                // 4) pivot
                Vector2 pivot = new Vector2(
                    frameData.pivot.x,
                    1f - frameData.pivot.y
                );

                string name = frameData.filename;

                sprites[i] = new SimpleScxSprite(
                    name,
                    atlasRect,
                    sourceRect,
                    sourceSize,
                    pivot
                );
            }

            return new ScxSpriteAtlas(texture, sprites);
        }

        private static void checkFrame(TpFrame frameData, int i, Vector2 defaultPivot) {
            if (frameData == null) {
                throw new ArgumentException($"TexturePacker frame[{i}] 数据非法: frame 对象为空.");
            }

            if (string.IsNullOrWhiteSpace(frameData.filename)) {
                throw new ArgumentException($"TexturePacker frame[{i}] 数据非法: filename 为空.");
            }

            if (frameData.frame == null) {
                throw new ArgumentException($"TexturePacker frame[{i}] 数据非法: 缺少 frame.");
            }

            if (frameData.sourceSize == null) {
                throw new ArgumentException($"TexturePacker frame[{i}] 数据非法: 缺少 sourceSize.");
            }

            if (frameData.rotated) {
                throw new ArgumentException($"TexturePacker frame[{i}] 数据非法: 不支持旋转打包.");
            }

            if (frameData.spriteSourceSize == null) {
                throw new ArgumentException($"TexturePacker frame[{i}] 数据非法: spriteSourceSize 不能为空.");
            }

            if (frameData.pivot == null) {
                // 没有我们回退到 默认的 
                frameData.pivot = new TpPivot() {
                    x = defaultPivot.x,
                    y = defaultPivot.y,
                };
            }
        }

        private sealed class TpRoot {
            public TpFrame[] frames;
        }

        private sealed class TpFrame {
            public string filename;

            public TpRect frame;

            public bool rotated;

            public bool trimmed;

            public TpRect spriteSourceSize;

            public TpRect sourceSize;

            public TpPivot pivot;
        }

        private sealed class TpRect {
            public int x;

            public int y;

            public int w;

            public int h;
        }

        private sealed class TpPivot {
            public float x;

            public float y;
        }
    }
}