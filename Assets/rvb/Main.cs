using System.Collections.Generic;
using rvb.scripts;
using rvb.utils;
using scx.SpriteRenderer;
using UnityEngine;

namespace rvb {
    /// <summary>
    /// SheepMgr + ScxSpriteRenderer 最小可视化接入示例。
    ///
    /// 第一阶段目标：
    /// 1. 看见双方单位生成、移动、攻击和死亡后的增删。
    /// 2. 角色暂时使用可配置的连续预览帧。
    /// 3. 后续再把 skinId + animType + animFrame 精确映射到图集帧。
    /// </summary>
    public class Main : MonoBehaviour {
        [Header("Atlas")] public Texture2D texture;
        public TextAsset json;
        [Header("Atlas")] public Texture2D texture1;
        public TextAsset json1;
        [Header("Atlas")] public Texture2D texture2;
        public TextAsset json2;
        public Material mainMaterial;
        public Material highlightMaterial;

        [Header("Logic")] [SerializeField] private float logicFPS = 30f;
        [SerializeField] private int maxLogicStepsPerUnityFrame = 4;
        [SerializeField] private float logicToWorldScale = 0.01f;
        [SerializeField] private float logicHeightToWorldScale = 0.01f;
        [SerializeField] private int fallbackLogicalAnimationFrames = 30;

        [Header("Test Army")] [SerializeField] private int redRoleId = 22;
        [SerializeField] private int blueRoleId = 22;
        [SerializeField] private int initialCountPerCamp = 300;
        [SerializeField] private int hotkeySpawnCount = 10;
        [SerializeField] private bool enableAutomaticTroops = false;


        private int bulletPreviewFrameCount = 1;

        private ScxSpriteRenderer scxSpriteRenderer;
        private ScxSpriteRenderer scxSpriteRenderer1;
        private ScxSpriteRenderer scxSpriteRenderer2;
        private string[] spriteNames;
        private SheepMgr sheepMgr;
        private SheepCtl sheepCtl = new SheepCtl();

        // SheepMgr 的 PetView.index 是稳定的对象池槽位，适合绑定渲染单元。
        private readonly Dictionary<string, ScxSpriteRenderUnit> roleRenderers =
            new Dictionary<string, ScxSpriteRenderUnit>();

        private readonly HashSet<string> seenRoleSlots = new HashSet<string>();
        private readonly List<string> staleRoleSlots = new List<string>();

        // BulletView 没有公开池下标，测试阶段使用唯一 bullet.id。
        private readonly Dictionary<int, ScxSpriteRenderUnit> bulletRenderers =
            new Dictionary<int, ScxSpriteRenderUnit>();

        private readonly HashSet<int> seenBulletIds = new HashSet<int>();
        private readonly List<int> staleBulletIds = new List<int>();

        private float logicAccumulator;
        private LoadRoleResult loadRoleResult;

        private void Start() {
            if (texture == null || json == null || mainMaterial == null) {
                Debug.LogError("RVB: texture、json、mainMaterial 必须在 Inspector 中赋值。");
                enabled = false;
                return;
            }

            this.loadRoleResult = SheepSpriteAtlasLoader.loadRole(texture, json.text);
            scxSpriteRenderer = new ScxSpriteRenderer(
                loadRoleResult.spriteAtlas,
                100,
                mainMaterial,
                Mathf.Max(5000, initialCountPerCamp * 4)
            );

            var loadRoleResult1 = SheepSpriteAtlasLoader.loadRole(texture1, json1.text);
            scxSpriteRenderer1 = new ScxSpriteRenderer(
                loadRoleResult1.spriteAtlas,
                100,
                mainMaterial,
                Mathf.Max(5000, initialCountPerCamp * 4)
            );

            var loadRoleResult2 = SheepSpriteAtlasLoader.loadBullet(texture2, json2.text);
            scxSpriteRenderer2 = new ScxSpriteRenderer(
                loadRoleResult2,
                100,
                mainMaterial,
                Mathf.Max(5000, initialCountPerCamp * 4)
            );

            bulletPreviewFrameCount = scxSpriteRenderer2.getSpriteNames().Length;

            spriteNames = scxSpriteRenderer2.getSpriteNames();
            scxSpriteRenderer.setParent(gameObject);
            scxSpriteRenderer1.setParent(gameObject);
            scxSpriteRenderer2.setParent(gameObject);

            sheepCtl.comImages.roles_framess[0] = new Dictionary<int, Dictionary<int, int[]>>();
            sheepCtl.comImages.roles_framess[1] = new Dictionary<int, Dictionary<int, int[]>>();
            sheepCtl.comImages.roles_framess[0][106] = new Dictionary<int, int[]>();
            sheepCtl.comImages.roles_framess[1][106] = new Dictionary<int, int[]>();

            foreach (var keyValuePair in loadRoleResult.animFrame) {
                var k = keyValuePair.Key;
                var v = keyValuePair.Value;
                sheepCtl.comImages.roles_framess[0][106][k] = new int[v];
            }

            foreach (var keyValuePair in loadRoleResult1.animFrame) {
                var k = keyValuePair.Key;
                var v = keyValuePair.Value;
                sheepCtl.comImages.roles_framess[1][106][k] = new int[v];
            }


            BindSheepMgr();

            if (sheepMgr == null) {
                return;
            }

            ClearAllRenderUnits();
            logicAccumulator = 0f;

            sheepMgr.gameIndex++;
            sheepMgr.setState(SheepRoomState.Start);
            sheepMgr.onGameStart();
            sheepMgr.game_clear();
        }

        private void BindSheepMgr() {
            sheepMgr = new SheepMgr(sheepCtl);

            // 防止关闭 Domain Reload 后重复订阅。
            sheepMgr.OnRoleRender -= HandleRoleRender;
            sheepMgr.OnRoleRender += HandleRoleRender;

            sheepMgr.OnBulletRender -= HandleBulletRender;
            sheepMgr.OnBulletRender += HandleBulletRender;

            // 没有原 SheepCtl.roles_framess 时，必须给逻辑一个动画长度。
            // 否则默认值为 1，In/Dead 等状态可能一帧就结束。
            sheepMgr.AnimationFrameCountResolver = ResolveLogicalAnimationFrameCount;

            // sheepMgr.advanceGameClockInGameUpdate = false;
            sheepMgr.isAutoCall = enableAutomaticTroops;
            sheepMgr.loongHp = SheepConfig.loongHps[0];
        }

        public void StartBattle() {
            // 这里先跳过倒计时，直接进入战斗状态。
            sheepMgr.setState(SheepRoomState.Run);
            sheepMgr.onGameRun();
        }

        private void Update() {
            UpdateHotkeys();

            if (sheepMgr == null || scxSpriteRenderer == null) {
                return;
            }

            if (sheepMgr.state == SheepRoomState.Start ||
                sheepMgr.state == SheepRoomState.Run) {
                RunLogicFrames();
            }

            // Unity 每个显示帧提交一次渲染。
            scxSpriteRenderer.update();
            scxSpriteRenderer1.update();
            scxSpriteRenderer2.update();
        }

        private void RunLogicFrames() {
            float safeFps = Mathf.Max(1f, logicFPS);
            float logicStepSeconds = 1f / safeFps;
            float logicStepMilliseconds = logicStepSeconds * 1000f;

            logicAccumulator += Time.deltaTime;

            // 本 Unity 帧内所有 SheepMgr 逻辑步共用一次 seen 集合。
            seenRoleSlots.Clear();
            seenBulletIds.Clear();

            int stepCount = 0;
            while (logicAccumulator >= logicStepSeconds &&
                   stepCount < maxLogicStepsPerUnityFrame) {
                var gameUpdate = sheepMgr.game_update(sheepCtl, logicStepMilliseconds);


                SyncBossMarker(0);
                SyncBossMarker(1);


                logicAccumulator -= logicStepSeconds;
                stepCount++;
            }

            // 这一显示帧没有产生逻辑步时，不清理，否则画面会闪烁。
            if (stepCount == 0) {
                return;
            }

            RecycleMissingRoleRenderers();
            RecycleMissingBulletRenderers();
        }

        private void HandleRoleRender(PetView view) {
            if (view == null || !view.isActive) {
                return;
            }

            SyncRoleView(view);
        }

        private void SyncBossMarker(int poolIndex) {
            PetView bossView = sheepMgr.boss[poolIndex];
            if (bossView == null || !bossView.isActive) {
                return;
            }

            SyncRoleView(bossView);
        }

        private void SyncRoleView(PetView view) {
            int slot = view.id;
            seenRoleSlots.Add(((int)view.camp) + "_" + slot);

            if (!roleRenderers.TryGetValue(((int)view.camp) + "_" + slot, out ScxSpriteRenderUnit renderPet)) {
                ScxSpriteRenderUnit unit = null;
                if (view.camp == SheepCamp.Red) {
                    unit = scxSpriteRenderer.createUnit();
                    unit.setVisible(true);
                }
                else {
                    unit = scxSpriteRenderer1.createUnit();
                    unit.setVisible(true);
                }

                unit.setScale(view.conf.scale, view.conf.scale, 1f);
                unit.setRotationFromEuler(45, 0, 0);

                var initialFrame = ResolveRoleSpriteFrame(view);
                unit.setFrame(initialFrame);

                renderPet = unit;
                roleRenderers.Add(((int)view.camp) + "_" + slot, renderPet);
            }

            float worldX = view.animX * logicToWorldScale;
            float worldY = view.animZ * logicHeightToWorldScale;
            float worldZ = view.animY * logicToWorldScale;

            renderPet.setVisible(true);
            renderPet.setPosition(worldX, worldY, worldZ);

            var frameIndex = ResolveRoleSpriteFrame(view);
            renderPet.setFrame(frameIndex);
        }

        // 直接替换 Main/RVB 中原来的 HandleBulletRender。
// 前提：
// 1. loadBullet 生成的帧名是 "animId-frame"，例如 "3-12"。
// 2. ScxSpriteRenderer 构造时 pixelsPerUnit = 100。
// 3. logicToWorldScale 与 logicHeightToWorldScale 最好相同；相同时与原版完全等价。
        private void HandleBulletRender(BulletView bullet) {
            if (
                bullet == null ||
                bullet.id == 0 ||
                bullet.isDie ||
                bullet.conf == null) {
                return;
            }

            int id = bullet.id;
            seenBulletIds.Add(id);

            if (!bulletRenderers.TryGetValue(id, out ScxSpriteRenderUnit renderPet)) {
                renderPet = scxSpriteRenderer2.createUnit();
                renderPet.setVisible(true);
                bulletRenderers.Add(id, renderPet);
            }

            int animId = bullet.conf.animId;

            // 原 bullets_framess 中各动画的真实帧数。
            // 未列出的动画退回到 endFrame + 1。
            int frameCount;
            switch (animId) {
                case 1:
                    frameCount = 1;
                    break;
                case 2:
                    frameCount = 9;
                    break;
                case 3:
                case 18:
                    frameCount = 38;
                    break;
                case 7:
                case 9:
                case 12:
                case 13:
                    frameCount = 61;
                    break;
                case 8:
                case 19:
                    frameCount = 24;
                    break;
                case 10:
                    frameCount = 8;
                    break;
                case 11:
                    frameCount = 16;
                    break;
                default:
                    frameCount = Mathf.Max(1, bullet.conf.endFrame + 1);
                    break;
            }

            int localFrame = PositiveModulo(bullet.frame, frameCount);

            // loadBullet 的标准命名规则。
            renderPet.setFrame(localFrame);

            int rotType = 0;
            if (SheepBulletAnimInfo.TryGetById(animId, out SheepBulletAnimInfo animInfo)) {
                rotType = animInfo.rotType;
            }

            const float spritePixelsPerUnit = 100f;
            const float sin45 = 0.7071067811865475f;
            const float epsilon = 0.000001f;

            // Cocos 逻辑坐标 (x, y, z) -> Unity 世界坐标 (x, z, y)。
            Vector3 ToWorldPoint(float x, float y, float z) {
                return new Vector3(
                    x * logicToWorldScale,
                    z * logicHeightToWorldScale,
                    y * logicToWorldScale
                );
            }

            Vector3 ToWorldVector(Vector3 value) {
                return new Vector3(
                    value.x * logicToWorldScale,
                    value.z * logicHeightToWorldScale,
                    value.y * logicToWorldScale
                );
            }

            // rotType 0 的 Sprite 原点在原版中相当于“底边中心”，
            // 标准 Sprite 通常以图片中心为 Transform 原点，因此需要抬高半个图片高度。
            float GetFixedFrameHeight(int currentAnimId, int currentFrame) {
                switch (currentAnimId) {
                    case 7:
                    case 9:
                    case 12:
                    case 13:
                        return 452f;

                    case 8:
                    case 19:
                        // 原图集中这些帧高度在 42/43 像素之间变化。
                        if ((currentFrame >= 4 && currentFrame <= 7) ||
                            (currentFrame >= 16 && currentFrame <= 19)) {
                            return 42f;
                        }

                        return 43f;

                    case 10:
                    case 11:
                    case 16:
                    case 17:
                        return 128f;

                    default:
                        // 未知固定特效的保守值。
                        return 128f;
                }
            }

            Vector3 centerOriginal;
            Vector3 rightOriginal;
            Vector3 upOriginal;

            if (rotType == 3) {
                // 原版：right = normalize(dirX, dirY, dirZ)
                //       up    = normalize(cross((0,-1,0), right))
                rightOriginal = new Vector3(
                    bullet.dirX,
                    bullet.dirY,
                    bullet.dirZ
                );

                if (rightOriginal.sqrMagnitude <= epsilon) {
                    rightOriginal = bullet.camp == SheepCamp.Red
                        ? Vector3.right
                        : Vector3.left;
                }

                rightOriginal.Normalize();

                upOriginal = Vector3.Cross(Vector3.down, rightOriginal);
                if (upOriginal.sqrMagnitude <= epsilon) {
                    upOriginal = Vector3.forward;
                }

                upOriginal.Normalize();

                centerOriginal = new Vector3(
                    bullet.x,
                    bullet.y,
                    bullet.z
                );
            }
            else if (rotType == 5) {
                // 原版：right = normalize(dirX, dirY, 0)
                //       up    = normalize(cross((0,0,1), right))
                rightOriginal = new Vector3(
                    bullet.dirX,
                    bullet.dirY,
                    0f
                );

                if (rightOriginal.sqrMagnitude <= epsilon) {
                    rightOriginal = bullet.camp == SheepCamp.Red
                        ? Vector3.right
                        : Vector3.left;
                }

                rightOriginal.Normalize();

                upOriginal = Vector3.Cross(Vector3.forward, rightOriginal);
                if (upOriginal.sqrMagnitude <= epsilon) {
                    upOriginal = Vector3.up;
                }

                upOriginal.Normalize();

                centerOriginal = new Vector3(
                    bullet.x,
                    bullet.y,
                    bullet.z
                );
            }
            else {
                // rotType == 0：爆炸、手掌、金箍棒等固定特效。
                rightOriginal = Vector3.right;
                upOriginal = new Vector3(0f, sin45, sin45);

                centerOriginal = new Vector3(
                    bullet.x,
                    bullet.y + bullet.startY,
                    bullet.z
                );
            }

            float logicalScale = bullet.conf.scale;

            // 每个 Sprite 像素在世界中的局部 X/Y 方向。
            Vector3 rightPerPixelWorld =
                ToWorldVector(rightOriginal * logicalScale);

            Vector3 upPerPixelWorld =
                ToWorldVector(upOriginal * logicalScale);

            Vector3 worldCenter = ToWorldPoint(
                centerOriginal.x,
                centerOriginal.y,
                centerOriginal.z
            );

            if (rotType == 0) {
                float frameHeight = GetFixedFrameHeight(animId, localFrame);

                // 从底边中心换算到标准 Sprite 的图片中心。
                worldCenter += upPerPixelWorld * (frameHeight * 0.5f);
            }

            float scaleX = rightPerPixelWorld.magnitude * spritePixelsPerUnit;

            if (scaleX <= epsilon) {
                renderPet.setVisible(false);
                return;
            }

            Vector3 rightAxis = rightPerPixelWorld / rightPerPixelWorld.magnitude;

            // 当 logicToWorldScale != logicHeightToWorldScale 时可能产生轻微剪切。
            // 标准 Transform 没有 shear，这里用 Gram-Schmidt 去掉剪切分量。
            Vector3 upOrthogonal = upPerPixelWorld -
                                   rightAxis * Vector3.Dot(upPerPixelWorld, rightAxis);

            float upLength = upOrthogonal.magnitude;
            if (upLength <= epsilon) {
                renderPet.setVisible(false);
                return;
            }

            Vector3 upAxis = upOrthogonal / upLength;
            Vector3 forwardAxis = Vector3.Cross(rightAxis, upAxis).normalized;

            Quaternion rotation = Quaternion.LookRotation(forwardAxis, upAxis);
            Vector3 euler = rotation.eulerAngles;

            float scaleY = upLength * spritePixelsPerUnit;

            renderPet.setVisible(true);
            renderPet.setPosition(worldCenter.x, worldCenter.y, worldCenter.z);
            renderPet.setRotationFromEuler(euler.x, euler.y, euler.z);
            renderPet.setScale(scaleX, scaleY, 1f);
        }

        private string ResolveRoleSpriteFrame(PetView view) {
            var i = loadRoleResult.animFrame[(int)view.animType];

            int localFrame = PositiveModulo(view.animFrame, i);
            return ((int)(view.animType)) + "-" + localFrame;
        }

        private int ResolveLogicalAnimationFrameCount(PetView view) {
            // 这是逻辑状态完成所使用的动画长度，不是 Scx 图集总帧数。
            return Mathf.Max(1, fallbackLogicalAnimationFrames);
        }

        private int ClampSpriteIndex(int index) {
            if (spriteNames == null || spriteNames.Length == 0) {
                return 0;
            }

            return PositiveModulo(index, spriteNames.Length);
        }

        private static int PositiveModulo(int value, int modulo) {
            if (modulo <= 0) {
                return 0;
            }

            int result = value % modulo;
            return result < 0 ? result + modulo : result;
        }

        private void RecycleMissingRoleRenderers() {
            staleRoleSlots.Clear();

            foreach (KeyValuePair<string, ScxSpriteRenderUnit> pair in roleRenderers) {
                if (!seenRoleSlots.Contains(pair.Key)) {
                    staleRoleSlots.Add(pair.Key);
                }
            }

            foreach (var slot in staleRoleSlots) {
                var renderPet = roleRenderers[slot];
                roleRenderers.Remove(slot);
                renderPet.destroy();
            }
        }

        private void RecycleMissingBulletRenderers() {
            staleBulletIds.Clear();

            foreach (KeyValuePair<int, ScxSpriteRenderUnit> pair in bulletRenderers) {
                if (!seenBulletIds.Contains(pair.Key)) {
                    staleBulletIds.Add(pair.Key);
                }
            }

            foreach (int id in staleBulletIds) {
                var renderPet = bulletRenderers[id];
                bulletRenderers.Remove(id);
                renderPet.destroy();
            }
        }

        private void UpdateHotkeys() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                StartBattle();
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                SpawnArmy(SheepCamp.Red, redRoleId, hotkeySpawnCount);
            }

            if (Input.GetKeyDown(KeyCode.B)) {
                SpawnArmy(SheepCamp.Blue, blueRoleId, hotkeySpawnCount);
            }

            if (Input.GetKeyDown(KeyCode.H) && highlightMaterial != null) {
                scxSpriteRenderer.setMaterialTemplate(highlightMaterial);
            }

            if (Input.GetKeyDown(KeyCode.M) && mainMaterial != null) {
                scxSpriteRenderer.setMaterialTemplate(mainMaterial);
            }
        }

        private void SpawnArmy(SheepCamp camp, int roleId, int count) {
            if (sheepMgr == null || count <= 0) {
                return;
            }

            sheepMgr.produce_pets(roleId, count, camp);
        }

        private void ClearAllRenderUnits() {
            foreach (var renderPet in roleRenderers.Values) {
                renderPet.destroy();
            }

            roleRenderers.Clear();
            seenRoleSlots.Clear();
            staleRoleSlots.Clear();

            foreach (var renderPet in bulletRenderers.Values) {
                renderPet.destroy();
            }

            bulletRenderers.Clear();
            seenBulletIds.Clear();
            staleBulletIds.Clear();
        }

        private void OnDestroy() {
            if (sheepMgr != null) {
                sheepMgr.OnRoleRender -= HandleRoleRender;
                sheepMgr.OnBulletRender -= HandleBulletRender;

                if (sheepMgr.AnimationFrameCountResolver == ResolveLogicalAnimationFrameCount) {
                    sheepMgr.AnimationFrameCountResolver = null;
                }
            }

            ClearAllRenderUnits();
        }
    }
}