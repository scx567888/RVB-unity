using rvb.utils;
using scx.SpriteRenderer;
using UnityEngine;

namespace sheep {
    public class SheepGame : MonoBehaviour {
        // ********************* 逻辑帧相关 *********************

        // 逻辑帧率
        [Range(1, 240)] 
        public int tickRate = 30;

        // 逻辑帧率累加器
        private double tickAccumulator = 0;

        // ********************* 资源相关 *********************

        // 渲染器资源
        public Texture2D texture;
        public TextAsset json;
        public Material mainMaterial;

        // ********************* 渲染器相关 *********************

        private ScxSpriteRenderer scxSpriteRenderer;
        private ScxSpriteRenderUnit scxSpriteRenderUnit;
        private ScxSpriteRenderUnit scxSpriteRenderUnit1;

        // ********************* 渲染插值相关 *********************

        public bool useLerp = true;
        private Vector3 previousPosition;
        private Vector3 currentPosition;

        // ********************* 主逻辑相关 *********************

        private SheepWorld sheepWorld;

        void Start() {
            // 创建渲染器
            var loadRoleResult = SheepSpriteAtlasLoader.loadRole(texture, json.text);
            this.scxSpriteRenderer = new ScxSpriteRenderer(
                loadRoleResult.spriteAtlas,
                100,
                mainMaterial,
                100
            );
            this.scxSpriteRenderUnit = scxSpriteRenderer.createUnit();
            this.scxSpriteRenderUnit.setFrame(0);
            this.scxSpriteRenderUnit1 = scxSpriteRenderer.createUnit();
            this.scxSpriteRenderUnit1.setFrame(0);

            // 初始化主逻辑
            sheepWorld = new SheepWorld();
        }

        void Update() {
            // 计算 tick 帧 时长
            double tickInterval = 1.0 / tickRate;

            // 累加 tick 帧 计数
            tickAccumulator += Time.deltaTime;

            // 调度逻辑帧
            while (tickAccumulator >= tickInterval) {
                tick();
                tickAccumulator -= tickInterval;
            }

            // 计算插值
            float alpha = (float)(tickAccumulator / tickInterval);

            // 渲染
            render(alpha);
        }

        void tick() {
            previousPosition = currentPosition;

            sheepWorld.tick();

            currentPosition = sheepWorld.position;
        }

        void render(float alpha) {
            Vector3 renderPosition;
            // 判断是否启用线性插值
            if (useLerp) {
                renderPosition = Vector3.Lerp(
                    previousPosition,
                    currentPosition,
                    alpha
                );
            }
            else {
                renderPosition = currentPosition;
            }

            scxSpriteRenderUnit.setPosition(renderPosition.x, renderPosition.y, renderPosition.z);
            scxSpriteRenderUnit1.setPosition(currentPosition.x, currentPosition.y + 1, currentPosition.z);
            scxSpriteRenderer.update();
        }
    }
}