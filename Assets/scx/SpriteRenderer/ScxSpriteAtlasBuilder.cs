using System;
using System.IO;
using scx.SpriteRenderer;
using UnityEngine;
using UnityEngine.U2D;

public class ScxSpriteAtlasBuilder {
    public static ScxSpriteAtlas FromUnitySpriteAtlas(SpriteAtlas unityAtlas) {
        if (unityAtlas == null) {
            throw new ArgumentNullException(nameof(unityAtlas));
        }

        var count = unityAtlas.spriteCount;
        if (count <= 0) {
            throw new InvalidOperationException("SpriteAtlas is empty.");
        }

        var unitySprites = new Sprite[count];
        var actualCount = unityAtlas.GetSprites(unitySprites);
        if (actualCount <= 0) {
            throw new InvalidOperationException("SpriteAtlas.GetSprites returned no sprites.");
        }

        // GetSprites 返回的是 atlas 里的 Sprite 克隆；packed 时 texture 指向 atlas 贴图
        var atlasTexture = unitySprites[0].texture as Texture2D;
        if (atlasTexture == null) {
            throw new InvalidOperationException("Atlas texture is null or not a Texture2D.");
        }

        var scxSprites = new ScxSprite[actualCount];

        for (var i = 0; i < actualCount; i++) {
            var unitySprite = unitySprites[i];
            if (unitySprite == null) {
                throw new InvalidOperationException($"Sprite at index {i} is null.");
            }

            // 你的系统目前不支持旋转打包
            if (unitySprite.packed && unitySprite.packingRotation != SpritePackingRotation.None) {
                throw new NotSupportedException(
                    $"Sprite '{unitySprite.name}' uses packing rotation '{unitySprite.packingRotation}', " +
                    "but ScxSprite currently does not support rotated sprites.");
            }

            // 你的系统目前是 quad，不支持 tight packed polygon
            if (unitySprite.packed && unitySprite.packingMode != SpritePackingMode.Rectangle) {
                // throw new NotSupportedException(
                // $"Sprite '{unitySprite.name}' uses packing mode '{unitySprite.packingMode}', " +
                // "but ScxSprite currently only supports rectangle packing.");
            }

            // 当前 atlas 贴图中的矩形
            // 注意：tight packed 时 textureRect 会抛异常，所以前面先拒绝了非 Rectangle
            var textureRect = unitySprite.textureRect;

            // 原始 Sprite 在原始贴图上的矩形
            var originalRect = unitySprite.rect;

            // 当前使用矩形相对原始 bounds 的偏移
            var offset = unitySprite.textureRectOffset;

            // pivot 像素 -> 归一化 pivot
            var pivot = new Vector2(
                unitySprite.pivot.x / originalRect.width,
                unitySprite.pivot.y / originalRect.height
            );

            var atlasRect = new RectInt(
                Mathf.RoundToInt(textureRect.x),
                Mathf.RoundToInt(textureRect.y),
                Mathf.RoundToInt(textureRect.width),
                Mathf.RoundToInt(textureRect.height)
            );

            var sourceRect = new RectInt(
                Mathf.RoundToInt(offset.x),
                Mathf.RoundToInt(offset.y),
                Mathf.RoundToInt(textureRect.width),
                Mathf.RoundToInt(textureRect.height)
            );

            var sourceSize = new Vector2Int(
                Mathf.RoundToInt(originalRect.width),
                Mathf.RoundToInt(originalRect.height)
            );

            scxSprites[i] = new ScxSprite(
                unitySprite.name,
                atlasRect,
                sourceRect,
                sourceSize,
                pivot
            );
        }

        return new ScxSpriteAtlas(atlasTexture, scxSprites);
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

    /// <summary>
    /// 把 TexturePacker 导出的 JSON（frames 为数组格式）转换成 ScxSpriteAtlas
    /// </summary>
    public static ScxSpriteAtlas FromTexturePackerJson(Texture2D texture, string json) {
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
                f.pivot.y
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
}