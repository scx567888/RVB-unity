using System.Collections.Generic;
using System.Threading.Tasks;
using scx.SpriteRenderer;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace scx.Test {
    public class Test : MonoBehaviour {
        // unity 图集
        public SpriteAtlas unitySpriteAtlas;

        // 主材质
        public Material mainMaterial;

        // 高亮材质
        public Material highlightMaterial;

        // 渲染器
        private ScxSpriteRenderer scxSpriteRenderer;

        // 汽车列表
        private List<Car> cars;

        private string[] spriteNames;

        void Start() {
            // 创建渲染器
            var scxSpriteAtlas = ScxSpriteAtlasUnitySpriteAtlasLoader.load(this.unitySpriteAtlas);
            this.scxSpriteRenderer = new ScxSpriteRenderer(scxSpriteAtlas, 300, mainMaterial, 5000);
            this.spriteNames = this.scxSpriteRenderer.getSpriteNames();

            this.scxSpriteRenderer.setParent(this.gameObject);

            // 排序一下 内部 帧索引, 后续用 索引 setFrame 会快很多.
            var sortedFrameName = new string[this.spriteNames.Length];

            for (int i = 0; i < sortedFrameName.Length; i++) {
                sortedFrameName[i] = "car (" + (i + 1) + ")";
            }

            this.scxSpriteRenderer.sortFrame(sortedFrameName);

            // 创建汽车
            this.cars = new List<Car>();
            for (var j = 0; j < 10000 * 5; j++) {
                var spriteRenderUnit = this.scxSpriteRenderer.createUnit();
                spriteRenderUnit.setVisible(true);
                spriteRenderUnit.setPosition(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50));

                spriteRenderUnit.setFrame(this.spriteNames[0]);
                // 给每个单元一个随机起始帧索引
                var obj = new Car(spriteRenderUnit, Random.Range(0, this.spriteNames.Length));
                this.cars.Add(obj);
            }
        }

        // 计数器
        private int time = 0;

        void Update() {
            // 绕 Y 轴旋转整个节点
            var euler = transform.eulerAngles;
            euler.y += 10f * Time.deltaTime;
            transform.eulerAngles = euler;

            // 测试更换材质
            if (time == 500) {
                this.scxSpriteRenderer.setMaterialTemplate(highlightMaterial);
            }

            if (time == 1000) {
                this.scxSpriteRenderer.setMaterialTemplate(mainMaterial);
            }

            time++;

            // 多核并行执行方式
            Parallel.For(0, cars.Count, i => {
                var car = cars[i];
                car.frameIndex++;
                car.renderUnit.setFrame(car.frameIndex % this.spriteNames.Length);
            });

            // 传统方式
            // foreach (var car in this.cars) {
            //     // 每个单元的帧索引累加
            //     car.frameIndex++;
            //     car.renderUnit.setFrame(car.frameIndex % this.spriteNames.Length);
            // }

            this.scxSpriteRenderer.update();
        }
    }
}