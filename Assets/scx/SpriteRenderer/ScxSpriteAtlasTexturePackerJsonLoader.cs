using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace scx.SpriteRenderer {
    /// <summary>
    /// TexturePacker JSON Array 格式加载器。
    ///
    /// 坐标约定：
    /// - TexturePacker frame：图集左上角原点。
    /// - TexturePacker spriteSourceSize：原图左上角原点。
    /// - SimpleScxSprite：左下角原点。
    ///
    /// 限制：
    /// - 不支持旋转打包。
    /// - 不支持多张 texture page。
    /// - 不支持 polygon / tight mesh。
    /// </summary>
    public static class ScxSpriteAtlasTexturePackerJsonLoader {
        /// <summary>
        /// 使用原图中心作为默认 pivot。
        /// </summary>
        public static ScxSpriteAtlas load(Texture2D texture, string json) {
            return load(texture, json, new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// 加载 TexturePacker JSON Array。
        ///
        /// defaultPivot 是基于 sourceSize 的左下原点归一化坐标：
        /// (0,0) = 左下角
        /// (1,1) = 右上角
        /// </summary>
        public static ScxSpriteAtlas load(
            Texture2D texture,
            string json,
            Vector2 defaultPivot
        ) {
            if (texture == null) {
                throw new ArgumentNullException(nameof(texture));
            }

            if (string.IsNullOrWhiteSpace(json)) {
                throw new ArgumentException(
                    "TexturePacker json 不能为空.",
                    nameof(json)
                );
            }

            validatePivot(defaultPivot, nameof(defaultPivot));

            TpRoot root;

            try {
                root = JsonConvert.DeserializeObject<TpRoot>(json);
            }
            catch (JsonException exception) {
                throw new ArgumentException(
                    "TexturePacker json 解析失败.",
                    nameof(json),
                    exception
                );
            }

            if (root == null) {
                throw new ArgumentException(
                    "TexturePacker json 根对象为空.",
                    nameof(json)
                );
            }

            if (root.frames == null || root.frames.Length == 0) {
                throw new ArgumentException(
                    "TexturePacker json 的 frames 为空.",
                    nameof(json)
                );
            }

            // 应当以实际参与渲染的 Texture2D 尺寸为准。
            var atlasWidth = texture.width;
            var atlasHeight = texture.height;

            if (atlasWidth <= 0 || atlasHeight <= 0) {
                throw new ArgumentException(
                    "Texture2D 尺寸必须大于 0.",
                    nameof(texture)
                );
            }

            // meta.size 一旦存在，就应当与实际 Texture2D 一致。
            // 不一致时继续解析会造成 Y 翻转和 UV 全部错误。
            if (root.meta?.size != null) {
                var metaWidth = root.meta.size.w;
                var metaHeight = root.meta.size.h;

                if (metaWidth > 0 &&
                    metaHeight > 0 &&
                    (metaWidth != atlasWidth || metaHeight != atlasHeight)) {
                    throw new ArgumentException(
                        $"TexturePacker meta.size 为 {metaWidth}x{metaHeight}，" +
                        $"但 Texture2D 实际尺寸为 {atlasWidth}x{atlasHeight}.",
                        nameof(texture)
                    );
                }
            }

            var sprites = new ScxSprite[root.frames.Length];
            var spriteNames = new HashSet<string>(StringComparer.Ordinal);

            for (var i = 0; i < root.frames.Length; i += 1) {
                var frameData = root.frames[i];

                if (frameData == null) {
                    throw frameError(i, "frame 对象为空.");
                }

                if (string.IsNullOrWhiteSpace(frameData.filename)) {
                    throw frameError(i, "filename 为空.");
                }

                if (frameData.frame == null) {
                    throw frameError(i, "缺少 frame.");
                }

                if (frameData.sourceSize == null) {
                    throw frameError(i, "缺少 sourceSize.");
                }

                if (frameData.rotated) {
                    throw frameError(
                        i,
                        $"不支持旋转打包: {frameData.filename}"
                    );
                }

                var sourceWidth = frameData.sourceSize.w;
                var sourceHeight = frameData.sourceSize.h;

                if (sourceWidth <= 0 || sourceHeight <= 0) {
                    throw frameError(
                        i,
                        $"sourceSize 非法: {sourceWidth}x{sourceHeight}"
                    );
                }

                validateRect(
                    frameData.frame,
                    atlasWidth,
                    atlasHeight,
                    i,
                    "frame"
                );

                var spriteSourceSize = frameData.spriteSourceSize;

                // 正常的 TexturePacker JSON 都应当包含 spriteSourceSize。
                // 对未裁边且字段缺失的情况做一个合理回退。
                if (spriteSourceSize == null) {
                    if (frameData.trimmed) {
                        throw frameError(
                            i,
                            "trimmed=true，但缺少 spriteSourceSize."
                        );
                    }

                    spriteSourceSize = new TpRect {
                        x = 0,
                        y = 0,
                        w = sourceWidth,
                        h = sourceHeight
                    };
                }

                validateRect(
                    spriteSourceSize,
                    sourceWidth,
                    sourceHeight,
                    i,
                    "spriteSourceSize"
                );

                // 未旋转的矩形图块，这两个尺寸应当一致。
                if (frameData.frame.w != spriteSourceSize.w ||
                    frameData.frame.h != spriteSourceSize.h) {
                    throw frameError(
                        i,
                        "frame 尺寸与 spriteSourceSize 尺寸不一致: " +
                        $"frame={frameData.frame.w}x{frameData.frame.h}, " +
                        $"spriteSourceSize=" +
                        $"{spriteSourceSize.w}x{spriteSourceSize.h}"
                    );
                }

                // TexturePacker 图集坐标：
                // 左上原点 -> 左下原点。
                var atlasRect = new RectInt(
                    frameData.frame.x,
                    atlasHeight - frameData.frame.y - frameData.frame.h,
                    frameData.frame.w,
                    frameData.frame.h
                );

                // TexturePacker 原图裁剪坐标：
                // 左上原点 -> 左下原点。
                var sourceRect = new RectInt(
                    spriteSourceSize.x,
                    sourceHeight - spriteSourceSize.y - spriteSourceSize.h,
                    spriteSourceSize.w,
                    spriteSourceSize.h
                );

                var sourceSize = new Vector2Int(
                    sourceWidth,
                    sourceHeight
                );

                // 保留目录结构，仅去掉扩展名。
                //
                // 例如：
                // attack/skeleton-gongji_00.png
                // ->
                // attack/skeleton-gongji_00
                //
                // 保留目录可以避免不同目录下出现同名 sprite 时发生冲突。
                var name = removeExtension(frameData.filename);

                if (!spriteNames.Add(name)) {
                    throw frameError(
                        i,
                        $"存在重复的 sprite 名称: {name}"
                    );
                }

                sprites[i] = new SimpleScxSprite(
                    name,
                    atlasRect,
                    sourceRect,
                    sourceSize,
                    defaultPivot
                );
            }

            return new ScxSpriteAtlas(texture, sprites);
        }

        private static void validateRect(
            TpRect rect,
            int containerWidth,
            int containerHeight,
            int frameIndex,
            string fieldName
        ) {
            if (rect.w <= 0 || rect.h <= 0) {
                throw frameError(
                    frameIndex,
                    $"{fieldName} 尺寸必须大于 0: {rect.w}x{rect.h}"
                );
            }

            if (rect.x < 0 || rect.y < 0) {
                throw frameError(
                    frameIndex,
                    $"{fieldName} 坐标不能小于 0: ({rect.x}, {rect.y})"
                );
            }

            var right = (long)rect.x + rect.w;
            var bottom = (long)rect.y + rect.h;

            if (right > containerWidth || bottom > containerHeight) {
                throw frameError(
                    frameIndex,
                    $"{fieldName} 超出范围: " +
                    $"rect=({rect.x},{rect.y},{rect.w},{rect.h}), " +
                    $"container={containerWidth}x{containerHeight}"
                );
            }
        }

        private static void validatePivot(
            Vector2 pivot,
            string parameterName
        ) {
            if (float.IsNaN(pivot.x) ||
                float.IsInfinity(pivot.x) ||
                float.IsNaN(pivot.y) ||
                float.IsInfinity(pivot.y)) {
                throw new ArgumentException(
                    "pivot 不能包含 NaN 或 Infinity.",
                    parameterName
                );
            }
        }

        private static Exception frameError(
            int frameIndex,
            string message
        ) {
            return new ArgumentException(
                $"TexturePacker frame[{frameIndex}] 数据非法: {message}"
            );
        }

        /// <summary>
        /// 保留目录，仅移除最后一段扩展名。
        /// </summary>
        private static string removeExtension(string filename) {
            var normalized = filename.Replace('\\', '/');

            var slashIndex = normalized.LastIndexOf('/');
            var dotIndex = normalized.LastIndexOf('.');

            if (dotIndex > slashIndex) {
                return normalized.Substring(0, dotIndex);
            }

            return normalized;
        }

        private sealed class TpRoot {
            [JsonProperty("frames")]
            public TpFrame[] frames;

            [JsonProperty("meta")]
            public TpMeta meta;
        }

        private sealed class TpMeta {
            [JsonProperty("size")]
            public TpSize size;
        }

        private sealed class TpFrame {
            [JsonProperty("filename")]
            public string filename;

            [JsonProperty("frame")]
            public TpRect frame;

            [JsonProperty("rotated")]
            public bool rotated;

            [JsonProperty("trimmed")]
            public bool trimmed;

            [JsonProperty("spriteSourceSize")]
            public TpRect spriteSourceSize;

            [JsonProperty("sourceSize")]
            public TpSize sourceSize;
        }

        private sealed class TpRect {
            [JsonProperty("x")]
            public int x;

            [JsonProperty("y")]
            public int y;

            [JsonProperty("w")]
            public int w;

            [JsonProperty("h")]
            public int h;
        }

        private sealed class TpSize {
            [JsonProperty("w")]
            public int w;

            [JsonProperty("h")]
            public int h;
        }
    }
}