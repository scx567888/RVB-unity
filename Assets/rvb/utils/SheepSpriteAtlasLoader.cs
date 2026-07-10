using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using scx.SpriteRenderer;
using UnityEngine;

namespace rvb.utils {
    public static class SheepSpriteAtlasLoader {
        public static ScxSpriteAtlas loadRole(Texture2D texture, string json) {
            var data0 = JsonConvert.DeserializeObject<Dictionary<string, SheepRoleSprite[][]>>(json).First()
                .Value;

            var sprites = new List<SheepRoleSprite>();

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
            }

            return new ScxSpriteAtlas(texture, sprites.ToArray());
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
}