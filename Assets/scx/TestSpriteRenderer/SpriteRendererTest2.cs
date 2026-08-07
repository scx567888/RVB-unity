using System.Collections.Generic;
using System.Threading.Tasks;
using scx.SpriteRenderer;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace scx.SpriteRendererTest {
    public class SpriteRendererTest2 : MonoBehaviour {
        // unity 图集
        public SpriteAtlas unitySpriteAtlas;

        // 主材质
        public Material mainMaterial;

        // 渲染器
        private ScxSpriteRenderer scxSpriteRenderer;

        // 汽车列表
        private List<ScxSpriteRenderUnit> units;

        void Start() {
            // 创建渲染器
            var scxSpriteAtlas = ScxSpriteAtlasUnitySpriteAtlasLoader.load(this.unitySpriteAtlas);
            this.scxSpriteRenderer = new ScxSpriteRenderer(scxSpriteAtlas, 300, mainMaterial, 1000);

            this.scxSpriteRenderer.setParent(this.gameObject);

            // 创建单元
            this.units = new List<ScxSpriteRenderUnit>();
            for (var j = 0; j < 5000; j++) {
                var spriteRenderUnit = this.scxSpriteRenderer.createUnit();
                spriteRenderUnit.setVisible(true);
                spriteRenderUnit.setPosition(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
                spriteRenderUnit.setFrame(0);
                this.units.Add(spriteRenderUnit);
            }
        }

        void Update() {
            foreach (var unit in this.units) {
                unit.setColor(Color.red);
            }

            this.scxSpriteRenderer.update();
        }
    }
}