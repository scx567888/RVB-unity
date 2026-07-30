using rvb.utils;
using scx.SpriteRenderer;
using UnityEngine;

namespace sheep {
    public class SheepGame : MonoBehaviour {
        // ********************* 逻辑帧相关 *********************

        // 逻辑帧率
        [Range(1, 240)] public int tickRate = 30;

        // 逻辑帧率累加器
        private double tickAccumulator = 0;

        // ********************* 资源相关 *********************

        // 渲染器资源
        public Texture2D texture;
        public TextAsset json;
        public Material mainMaterial;

        // ********************* 渲染器相关 *********************

        private ScxSpriteRenderer scxSpriteRenderer;

        // ********************* 渲染插值相关 *********************

        public bool useLerp = true;

        // ********************* 主逻辑相关 *********************

        private SheepWorld sheepWorld;

        // *******************  测试  ****************
        public GameObject boss;

        void Start() {
            // 创建渲染器
            var loadRoleResult = SheepSpriteAtlasLoader.loadRole(texture, json.text);
            this.scxSpriteRenderer = new ScxSpriteRenderer(
                loadRoleResult.spriteAtlas,
                100,
                mainMaterial,
                1000
            );
            this.scxSpriteRenderer.setParent(this.gameObject);

            // 初始化主逻辑
            sheepWorld = new SheepWorld();
        }

        void Update() {
            UpdateTest();

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

        private void UpdateTest() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                for (int i = 0; i < 20; i++) {
                    sheepWorld.addPrePet(new Pet() {
                        moveSpeed = sheepWorld.randomFloat(0.25f, 0.5f),
                        collideR = 0.5f,
                        collideMoveScale = 1,
                        collideElasticityScale = 1.3f / 4,
                        collideNotMoveNum = 500
                    });
                }
            }

            sheepWorld.bossX = boss.transform.position.x;
            sheepWorld.bossY = boss.transform.position.z;
        }

        void OnGUI() {
            // 计算区域
            var w = Screen.width;
            var h = Screen.height;

            // 设置显示样式
            var position = new Rect(100, 100, w, h * 2f / 100);

            // 设置样式
            var style = new GUIStyle {
                alignment = TextAnchor.UpperLeft,
                fontSize = h * 2 / 50,
                normal = {
                    textColor = Color.green
                }
            };

            // 绘制在屏幕左上角
            GUI.Label(position, "pet 数量 : " + sheepWorld.pets.Count, style);
        }

        // 执行逻辑帧
        void tick() {
            // 记录状态 用于插值
            foreach (var pet in sheepWorld.pets) {
                pet.lastX = pet.x;
                pet.lastY = pet.y;
            }

            // 执行 sheepWorld.tick()
            sheepWorld.tick();
        }

        // 执行渲染
        void render(float alpha) {
            var pets = sheepWorld.pets;

            foreach (var pet in pets) {
                if (pet.scxSpriteRenderUnit == null) {
                    pet.scxSpriteRenderUnit = scxSpriteRenderer.createUnit();
                    pet.scxSpriteRenderUnit.setRotationFromEuler(45, 0, 0);
                    pet.scxSpriteRenderUnit.setVisible(true);
                }

                renderPet(pet, alpha);
            }

            // 更新渲染器
            scxSpriteRenderer.update();
        }

        public void renderPet(Pet pet, float alpha) {
            Vector3 renderPosition;
            // 判断是否启用线性插值
            if (useLerp) {
                renderPosition = Vector3.Lerp(
                    new Vector3(pet.lastX, 0, pet.lastY),
                    new Vector3(pet.x, 0, pet.y),
                    alpha
                );
            }
            else {
                renderPosition = new Vector3(pet.x, 0, pet.y);
            }

            pet.scxSpriteRenderUnit.setPosition(renderPosition.x, renderPosition.y, renderPosition.z);
            pet.scxSpriteRenderUnit.setFrame(0);
        }
    }
}