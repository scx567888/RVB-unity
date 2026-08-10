using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using scx.SpriteRenderer;
using UnityEngine;

namespace sheep_game.utils {
    public static class SheepSpriteAtlasLoader {
        public static LoadRoleResult loadRole(Texture2D texture, string json) {
            var data0 = JsonConvert.DeserializeObject<Dictionary<string, SheepRoleSprite[][]>>(json).First()
                .Value;

            var sprites = new List<SheepRoleSprite>();

            var animFrame = new Dictionary<int, int>();
            for (var i = 0; i < data0.Length; i++) {
                var data1 = data0[i];
                if (data1 == null) {
                    continue;
                }

                for (var j = 0; j < data1.Length; j++) {
                    var sprite = data1[j];
                    sprite._name = i + "-" + j;
                    sprites.Add(sprite);
                }

                animFrame[i] = data1.Length;
            }

            var spriteAtlas = new ScxSpriteAtlas(texture, sprites.ToArray());
            return new LoadRoleResult() {
                spriteAtlas = spriteAtlas,
                animFrame = animFrame
            };
        }

        public static ScxSpriteAtlas loadBullet(Texture2D texture, string json) {
            var data0 = JsonConvert.DeserializeObject<Dictionary<string, SheepBulletSprite[]>>(json).First()
                .Value;

            var sprites = new List<SheepBulletSprite>();

            for (var i = 0; i < data0.Length; i++) {
                var data1 = data0[i];
                if (data1 == null) {
                    continue;
                }

                var sprite = data1;
                sprite._name = i + "";
                sprites.Add(sprite);
            }

            return new ScxSpriteAtlas(texture, sprites.ToArray());
        }
    }

    public class LoadRoleResult {
        // 图集 name 为 2_0, 2_1 这种 anim + "_" + index 的拼接方法
        public ScxSpriteAtlas spriteAtlas;

        // 每个动画一共有多少帧
        public Dictionary<int, int> animFrame;
    }
}