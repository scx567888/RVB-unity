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
        [Header("Atlas")]
        public Texture2D texture;
        public TextAsset json;
        [Header("Atlas")]
        public Texture2D texture1;
        public TextAsset json1;
        public Material mainMaterial;
        public Material highlightMaterial;

        [Header("Logic")]
        [SerializeField] private float logicFPS = 30f;
        [SerializeField] private int maxLogicStepsPerUnityFrame = 4;
        [SerializeField] private float logicToWorldScale = 0.01f;
        [SerializeField] private float logicHeightToWorldScale = 0.01f;
        [SerializeField] private int fallbackLogicalAnimationFrames = 30;

        [Header("Test Army")]
        [SerializeField] private int redRoleId = 22;
        [SerializeField] private int blueRoleId = 22;
        [SerializeField] private int initialCountPerCamp = 300;
        [SerializeField] private int hotkeySpawnCount = 10;
        [SerializeField] private bool enableAutomaticTroops = false;


        [SerializeField] private bool renderBossMarkers = true;
        [SerializeField] private bool renderBulletMarkers = false;
        [SerializeField] private int bulletPreviewStartFrame = 0;
        [SerializeField] private int bulletPreviewFrameCount = 1;

        private ScxSpriteRenderer scxSpriteRenderer;
        private ScxSpriteRenderer scxSpriteRenderer1;
        private string[] spriteNames;
        private SheepMgr sheepMgr;

        // SheepMgr 的 PetView.index 是稳定的对象池槽位，适合绑定渲染单元。
        private readonly Dictionary<string, ScxSpriteRenderUnit> roleRenderers = new Dictionary<string, ScxSpriteRenderUnit>();
        private readonly HashSet<string> seenRoleSlots = new HashSet<string>();
        private readonly List<string> staleRoleSlots = new List<string>();

        // BulletView 没有公开池下标，测试阶段使用唯一 bullet.id。
        private readonly Dictionary<int, Pet> bulletRenderers = new Dictionary<int, Pet>();
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
                40,
                mainMaterial,
                Mathf.Max(5000, initialCountPerCamp * 4)
            );
            
            var loadRoleResult1 = SheepSpriteAtlasLoader.loadRole(texture1, json1.text);
            scxSpriteRenderer1 = new ScxSpriteRenderer(
                loadRoleResult1.spriteAtlas,
                40,
                mainMaterial,
                Mathf.Max(5000, initialCountPerCamp * 4)
            );

            spriteNames = scxSpriteRenderer.getSpriteNames();
            scxSpriteRenderer.setParent(gameObject);

         

            BindSheepMgr();
            StartBattle();
        }

        private void BindSheepMgr() {
            sheepMgr = SheepMgr.sheepMgr;

            // 防止关闭 Domain Reload 后重复订阅。
            sheepMgr.OnRoleRender -= HandleRoleRender;
            sheepMgr.OnRoleRender += HandleRoleRender;

            sheepMgr.OnBulletRender -= HandleBulletRender;
            sheepMgr.OnBulletRender += HandleBulletRender;

            // 没有原 SheepCtl.roles_framess 时，必须给逻辑一个动画长度。
            // 否则默认值为 1，In/Dead 等状态可能一帧就结束。
            sheepMgr.AnimationFrameCountResolver = ResolveLogicalAnimationFrameCount;

            sheepMgr.advanceGameClockInGameUpdate = true;
            sheepMgr.isAutoCall = enableAutomaticTroops;
            sheepMgr.loongHp = SheepConfig.loongHps[0];
        }

        public void StartBattle() {
            if (sheepMgr == null) {
                return;
            }

            ClearAllRenderUnits();
            logicAccumulator = 0f;

            sheepMgr.gameIndex++;
            sheepMgr.setState(SheepRoomState.Start);
            sheepMgr.onGameStart();
            sheepMgr.game_clear();

            // 这里先跳过倒计时，直接进入战斗状态。
            sheepMgr.setState(SheepRoomState.Run);
            sheepMgr.onGameRun();

            SpawnArmy(SheepCamp.Red, redRoleId, initialCountPerCamp);
            SpawnArmy(SheepCamp.Blue, blueRoleId, initialCountPerCamp);
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
                sheepMgr.game_update(sheepMgr, null, logicStepMilliseconds);

                if (renderBossMarkers) {
                    SyncBossMarker(0);
                    SyncBossMarker(1);
                }

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
            PetView bossView = sheepMgr.getPetView(poolIndex);
            if (bossView == null || !bossView.isActive) {
                return;
            }

            SyncRoleView(bossView);
        }

        private void SyncRoleView(PetView view) {
            int slot = view.index;
            seenRoleSlots.Add(((int)view.camp)+"_"+slot);

            if (!roleRenderers.TryGetValue(((int)view.camp)+"_"+slot, out ScxSpriteRenderUnit renderPet)) {
                ScxSpriteRenderUnit unit = null;
                if (view.camp==SheepCamp.Red) {
                    unit = scxSpriteRenderer.createUnit();
                    unit.setVisible(true);    
                }
                else {
                    unit = scxSpriteRenderer1.createUnit();
                    unit.setVisible(true);    
                }
                unit.setRotationFromEuler(45,0,0);

                var initialFrame = ResolveRoleSpriteFrame(view);
                unit.setFrame(initialFrame);

                renderPet = unit;
                roleRenderers.Add(((int)view.camp)+"_"+slot, renderPet);
            }

            float worldX = view.animX * logicToWorldScale;
            float worldY = view.animZ * logicHeightToWorldScale;
            float worldZ = view.animY * logicToWorldScale;

            renderPet.setVisible(true);
            renderPet.setPosition(worldX, worldY, worldZ);

            var frameIndex = ResolveRoleSpriteFrame(view);
            renderPet.setFrame(frameIndex);
        }

        private void HandleBulletRender(BulletView bullet) {
            if (!renderBulletMarkers || bullet == null || bullet.id == 0 || bullet.isDie) {
                return;
            }

            int id = bullet.id;
            seenBulletIds.Add(id);

            if (!bulletRenderers.TryGetValue(id, out Pet renderPet)) {
                var unit = scxSpriteRenderer.createUnit();
                unit.setVisible(true);

                int initialFrame = ResolveBulletSpriteFrame(bullet);
                unit.setFrame(initialFrame);

                renderPet = new Pet(unit, initialFrame);
                bulletRenderers.Add(id, renderPet);
            }

            renderPet.renderUnit.setVisible(true);

            int frameIndex = ResolveBulletSpriteFrame(bullet);
            renderPet.frameIndex = frameIndex;
            renderPet.renderUnit.setFrame(frameIndex);
        }

        private string ResolveRoleSpriteFrame(PetView view) {
            
            var i = loadRoleResult.animFrame[(int)view.animType];
            
            int localFrame = PositiveModulo(view.animFrame, i);
            return ((int)(view.animType))+"-" +localFrame;
        }

        private int ResolveBulletSpriteFrame(BulletView bullet) {
            int count = Mathf.Max(1, bulletPreviewFrameCount);
            int localFrame = PositiveModulo(bullet.frame, count);
            return ClampSpriteIndex(bulletPreviewStartFrame + localFrame);
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

            foreach (KeyValuePair<int, Pet> pair in bulletRenderers) {
                if (!seenBulletIds.Contains(pair.Key)) {
                    staleBulletIds.Add(pair.Key);
                }
            }

            foreach (int id in staleBulletIds) {
                Pet renderPet = bulletRenderers[id];
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

            foreach (Pet renderPet in bulletRenderers.Values) {
                renderPet.destroy();
            }
            bulletRenderers.Clear();
            seenBulletIds.Clear();
            staleBulletIds.Clear();
        }

        [ContextMenu("Print First 100 Sprite Names")]
        private void PrintFirstSpriteNames() {
            if (spriteNames == null) {
                Debug.LogWarning("RVB: spriteNames 尚未初始化，请先运行场景。");
                return;
            }

            int count = Mathf.Min(100, spriteNames.Length);
            for (int i = 0; i < count; i++) {
                Debug.Log($"Sprite[{i}] = {spriteNames[i]}");
            }
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
