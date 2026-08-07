using System.Collections.Generic;
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
        
        public int line_w = 24;
        public int line_h = 24;
        public float d = 0.5f;
        public Color32 color0 = new Color32(236, 248, 251, 255);
        public Color32 color1 = new Color32(21, 12, 10, 255);
        public float randomOffset = 0.02f;

        // 单元列表
        private ScxSpriteRenderUnit[,] units;

        void Start() {
            // 创建渲染器
            var scxSpriteAtlas = ScxSpriteAtlasUnitySpriteAtlasLoader.load(this.unitySpriteAtlas);
            this.scxSpriteRenderer = new ScxSpriteRenderer(scxSpriteAtlas, 100, mainMaterial, 1000);

            this.scxSpriteRenderer.setParent(this.gameObject);
            
            units=new ScxSpriteRenderUnit[line_w, line_h];
            
            var w = line_w * d;
            var h = line_h * d;

            

            for (int i = 0; i < line_w; i++) {
                for (int j = 0; j < line_h; j++) {
                    var spriteRenderUnit = this.scxSpriteRenderer.createUnit();

                    spriteRenderUnit.setVisible(true);
                    spriteRenderUnit.setFrame(0);
                    
                    var x =
                        (i + 0.5f) * d
                        - w / 2
                        + Random.Range(-randomOffset, randomOffset);

                    var y =
                        (j + 0.5f) * d
                        - h / 2
                        + Random.Range(-randomOffset, randomOffset);
                    spriteRenderUnit.setPosition(x,y,0);
                    units[i, j] = spriteRenderUnit;
                }
            }
            
            // 着色
            for (int i = 0; i < line_w; i++) {
                for (int j = 0; j < line_h; j++) {
                    var spriteRenderUnit = units[i, j];
                    
                    if (i < line_h / 2) {
                        spriteRenderUnit.setColor(color0);
                    }
                    else {
                        spriteRenderUnit.setColor(color1);
                    }
                    
                }
            }

        }

        void Update() {

            this.scxSpriteRenderer.update();
        }
    }
}