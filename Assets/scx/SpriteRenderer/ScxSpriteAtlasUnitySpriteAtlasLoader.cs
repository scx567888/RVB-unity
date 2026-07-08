using System;
using UnityEngine;
using UnityEngine.U2D;

namespace scx.SpriteRenderer {
    /// ScxSpriteAtlas 加载器 
    /// 适用于 Unity 自带的 SpriteAtlas 格式
    /// 
    /// 限制:
    /// - 不支持图集打包阶段的旋转.
    /// - 不支持 Tight Packing / 非矩形 Sprite Mesh.
    /// - 不支持多张 atlas texture, 也就是一个 Unity SpriteAtlas 被拆成多个 texture page.
    public static class ScxSpriteAtlasUnitySpriteAtlasLoader {
        /// 将 Unity 自带的 SpriteAtlas 格式 加载为 ScxSpriteAtlas
        public static ScxSpriteAtlas load(SpriteAtlas unitySpriteAtlas) {
            if (unitySpriteAtlas == null) {
                throw new ArgumentNullException(nameof(unitySpriteAtlas));
            }

            // 1, 读取 unitySpriteAtlas 中所有的 Sprites
            var unitySprites = new Sprite[unitySpriteAtlas.spriteCount];
            var actualCount = unitySpriteAtlas.GetSprites(unitySprites);

            if (actualCount <= 0) {
                throw new ArgumentException("unitySpriteAtlas 至少要有一个 sprite.", nameof(unitySpriteAtlas));
            }

            // 2. 使用第一个 Sprite 的贴图作为 atlas 贴图
            // 这也意味着 我们不支持 unitySpriteAtlas 存在多个 pack
            var atlasTexture = unitySprites[0].texture;

            // 3, 转换为 ScxSprite
            var scxSprites = new ScxSprite[actualCount];

            for (var i = 0; i < actualCount; i += 1) {
                var unitySprite = unitySprites[i];

                // 不支持多张 atlas texture
                if (unitySprite.texture != atlasTexture) {
                    throw new ArgumentException("不支持多张 atlas texture.", nameof(unitySpriteAtlas));
                }

                // 不支持图集打包阶段的旋转
                if (unitySprite.packingRotation != SpritePackingRotation.None) {
                    throw new ArgumentException("不支持图集打包阶段的旋转.", nameof(unitySpriteAtlas));
                }
                
                // 不支持 Tight Packing / 非矩形 Sprite Mesh.
                // 这里我们不用 unitySprite.packingMode != SpritePackingMode.Rectangle 判断, 这样不准确
                // 我们通过 uv 数量进行判断
                if (unitySprite.uv.Length != 4) {
                    throw new ArgumentException("不支持 Tight Packing / 非矩形 Sprite Mesh.", nameof(unitySpriteAtlas));
                }

                // 当前 atlas 贴图中的矩形
                // 注意: tight packed 时 textureRect 会抛异常, 所以前面先拒绝了非 Rectangle
                var textureRect = unitySprite.textureRect;

                // 原始 Sprite 在原始贴图上的矩形
                var originalRect = unitySprite.rect;

                // 当前使用矩形相对原始 bounds 的偏移
                var offset = unitySprite.textureRectOffset;

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

                // pivot 像素 -> 归一化 pivot
                var pivot = new Vector2(
                    unitySprite.pivot.x / originalRect.width,
                    unitySprite.pivot.y / originalRect.height
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
    }
}