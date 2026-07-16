using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static rvb.scripts.BullteCreate;
using static rvb.scripts.SheepModes;

namespace rvb.scripts {
    /// <summary>
    /// 羊了个羊战斗逻辑管理器。
    ///
    /// 说明：原 Cocos 版本把逻辑、渲染和 SheepCtl 组件混在一起。
    /// 本版本保留战斗逻辑，渲染相关调用均为可选 dynamic 桥接或回调，
    /// 因而可以接入你自己的 SheepCtl/渲染实现。
    /// </summary>
    public class SheepMgr {
        public static SheepMgr sheepMgr = new SheepMgr(false);
        public static SheepMgr inc;

        public bool isAutoCall = true;
        public float autoTime = 0f;
        public int gameMode = 0;
        public int timeMode = 2;
        public int loongHp = 10000;

        /// <summary>可选的外部 Boss 显示/控制组件；核心逻辑不依赖它。</summary>
        public Boss[] boss = new Boss[2];

        public float plotRatio = 0.5f;
        public int plotRatioIndex;
        public SheepRoomState state = SheepRoomState.Ready;

        /// <summary>普通单位源对象。Boss 固定占用 view_pets 的 0、1 下标，不放入此集合。</summary>
        public HashSet<PetView>[] pets;

        public int gameIndex;
        public float gameStartTimerForBuff;
        public Vector3 cameraEulerAngles = Vector3.zero;
        public long endTime;

        public List<int>[] preBuffs;
        public List<Buff>[] buffs;
        public int[] countNewBuffs;
        public int[] countBuffs;
        public int[] countShowBuffs;
        public bool[] flagLongBuffs;

        public Dictionary<int, int>[] petStartCounts;
        public List<PetView> god_view_pets;
        public PerfStat perfStat;

        public PetView[] view_pets;
        public BulletView[] view_bullets;
        public BulletView[] pre_view_bullets;

        public long updateTime;

        /// <summary>
        /// 待加入对象需要提供 init(int, PetView)。普通单位 PetView 已满足；
        /// 自定义 Boss/组件也可以通过 dynamic 接入。
        /// </summary>
        public List<PetView> petsAdd;

        public Stack<int> petsDel;
        public int petCount;

        public Stack<int> bulletsDel;
        public int bulletCount;
        public int bulletId;

        public int[] logic_counts;
        public List<BullteCreate> bullte_creates;

        /// <summary>blockIndex -> camp -> collideId -> petIndex 列表。</summary>
        public Dictionary<int, Dictionary<int, Dictionary<int, List<int>>>> pre_blocks;

        public bool[][] isChangeCollsionFlags;
        public bool[] isChangeAckFlags;

        public int MaxCount;
        public IndexLen[][] attackViews;
        public int[][] attackView1s;
        public IndexLen[][][] collisionViews;
        public int[][][] collisionView1s;

        public Dictionary<int, SheepCallInfo> redCallInfos;
        public Dictionary<int, SheepCallInfo> blueCallInfos;

        /// <summary>可选原渲染桥接对象（ComSheepImages 或你自己的实现）。</summary>
        public ComImages comImages;

        public int cur_rob_role_index;
        public int cur_rob_bullet_index;
        public int cur_rob_role_mesh_index;
        public int cur_rob_bullet_mesh_index;
        public int cur_rob_star_mesh_index;
        public int roleMaxIndex;
        public int bulletMaxIndex;
        public int preBulletIndex;
        public CurIndexImages curIndexImages;
        public int redBuffCount;
        public int blueBuffCount;
        public int petId;

        /// <summary>逻辑侧 Boss 当前已结算生命。</summary>
        public float[] bossHp;

        public long[] bossBackStateTime;

        /// <summary>若 SheepCtl 已自行推进 Buff 时钟，可设为 false。</summary>
        public bool advanceGameClockInGameUpdate = true;

        // -------------------- 可选桥接回调 --------------------
        public Action<SheepRoomState> OnRoomStateChanged;
        public Action OnRoomStateEnd;
        public Action OnGameStartHook;
        public Action<PetView> OnRoleRender;
        public Action<BulletView> OnBulletRender;
        public Action OnFrameBlockStart;
        public Action<SheepMgr> OnFrameBlockEnd;
        public Action<SheepCamp, float, float> OnBossHpChanged;
        public Action<SheepCamp> OnCounterStarted;
        public Action<SheepCamp> OnCounterFinished;
        public Action<int> OnCameraShake;
        public Func<PetView, int> AnimationFrameCountResolver;
        public Func<SheepCamp, bool> BossShieldConsumer;

        public SheepMgr() : this(true) {
        }

        private SheepMgr(bool registerSingleton) {
            if (registerSingleton || sheepMgr == null) {
                sheepMgr = this;
            }

            pets = new[] {
                new HashSet<PetView>(),
                new HashSet<PetView>()
            };

            preBuffs = new[] {
                new List<int>(),
                new List<int>()
            };
            buffs = new[] {
                new List<Buff>(),
                new List<Buff>()
            };
            countNewBuffs = new int[2];
            countBuffs = new int[2];
            countShowBuffs = new int[2];
            flagLongBuffs = new bool[2];

            petStartCounts = new[] {
                new Dictionary<int, int>(),
                new Dictionary<int, int>()
            };
            god_view_pets = new List<PetView>();
            perfStat = new PerfStat {
                redNums = new int[16],
                blueNums = new int[16]
            };

            view_pets = new PetView[SheepConfig.MaxPetCount];
            view_bullets = new BulletView[SheepConfig.MaxBulletCount];
            pre_view_bullets = new BulletView[SheepConfig.MaxBulletCount];

            petsAdd = new List<PetView>();
            petsDel = new Stack<int>();
            bulletsDel = new Stack<int>();
            logic_counts = new[] { 1, 1 };
            bullte_creates = new List<BullteCreate>();
            pre_blocks = new Dictionary<int, Dictionary<int, Dictionary<int, List<int>>>>();

            MaxCount = SheepConfig.line_w * SheepConfig.line_h;
            attackViews = new[] {
                new IndexLen[MaxCount],
                new IndexLen[MaxCount]
            };
            attackView1s = new[] {
                new int[SheepConfig.MaxPetCount],
                new int[SheepConfig.MaxPetCount]
            };

            collisionViews = new IndexLen[2][][];
            collisionView1s = new int[2][][];
            for (int camp = 0; camp < 2; camp++) {
                collisionViews[camp] = new IndexLen[SheepConfig.MaxGroupCount][];
                collisionView1s[camp] = new int[SheepConfig.MaxGroupCount][];
                for (int group = 0; group < SheepConfig.MaxGroupCount; group++) {
                    collisionViews[camp][group] = new IndexLen[MaxCount];
                    collisionView1s[camp][group] = new int[SheepConfig.MaxPetCount];
                }
            }

            redCallInfos = new Dictionary<int, SheepCallInfo>();
            blueCallInfos = new Dictionary<int, SheepCallInfo>();
            bossHp = new[] { (float)loongHp, (float)loongHp };
            bossBackStateTime = new long[2];

            Util.system = this;
            UtilFind.system = this;
            UtilAck.system = this;
            inc = this;
        }

        private static long NowMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        private static void IgnoreExternal(Action action) {
            try {
                action?.Invoke();
            }
            catch {
                // 外部显示桥接是可选的，缺少成员时不应中断核心逻辑。
            }
        }

        private static T ReadExternal<T>(Func<T> getter, T fallback = default) {
            try {
                return getter != null ? getter() : fallback;
            }
            catch {
                return fallback;
            }
        }

        private static int CampIndex(SheepCamp camp) => (int)camp;

        private static int RoleTypeIndex(SheepRoleType roleType) => (int)roleType;

        private void EnsurePerfCapacity(int roleIndex) {
            if (roleIndex < 0) return;
            if (roleIndex >= perfStat.redNums.Length) {
                Array.Resize(ref perfStat.redNums, roleIndex + 4);
            }

            if (roleIndex >= perfStat.blueNums.Length) {
                Array.Resize(ref perfStat.blueNums, roleIndex + 4);
            }
        }

        private void RaiseRoomEnd() {
            OnRoomStateEnd?.Invoke();
        }

        public void onGameStart() {
            gameStartTimerForBuff = 0f;
            autoTime = 0f;
            clearPets();
            clearCallPets();
            foreach (Dictionary<int, int> counts in petStartCounts) {
                counts.Clear();
            }

            foreach (List<int> list in preBuffs) list.Clear();
            foreach (List<Buff> list in buffs) list.Clear();
            Array.Clear(countNewBuffs, 0, countNewBuffs.Length);
            Array.Clear(countBuffs, 0, countBuffs.Length);
            Array.Clear(countShowBuffs, 0, countShowBuffs.Length);
            Array.Clear(flagLongBuffs, 0, flagLongBuffs.Length);
            Array.Clear(bossBackStateTime, 0, bossBackStateTime.Length);
            bossHp[0] = loongHp;
            bossHp[1] = loongHp;
            OnGameStartHook?.Invoke();
        }

        public void onGameRun() {
            foreach (PetView pet in god_view_pets) {
                if (pet == null || pet.isDie) continue;
                pet.state = SheepRoleState.Palm;
                pet.subState = SheepRoleSubState.Palm;
                pet.animType = SheepRoleAnimType.Palm;
                pet.readySkillId = 70002;
            }
        }

        public void onGameEnd() {
            god_view_pets.Clear();
        }

        public void setState(SheepRoomState newState) {
            state = newState;
            Debug.Log("房间状态改变: " + newState);
            OnRoomStateChanged?.Invoke(newState);
        }

        public void addPet(PetView pet, SheepCamp camp) {
            if (pet == null) return;
            pets[CampIndex(camp)].Add(pet);
            int roleIndex = RoleTypeIndex(pet.conf.roleType);
            EnsurePerfCapacity(roleIndex);
            if (camp == SheepCamp.Red) perfStat.redNums[roleIndex]++;
            else perfStat.blueNums[roleIndex]++;
        }

        public void delPet(PetView pet) {
            if (pet == null) return;
            bool removed = pets[0].Remove(pet) | pets[1].Remove(pet);
            if (!removed || pet.conf == null) return;
            int roleIndex = RoleTypeIndex(pet.conf.roleType);
            EnsurePerfCapacity(roleIndex);
            if (pet.camp == SheepCamp.Red) {
                perfStat.redNums[roleIndex] = Math.Max(0, perfStat.redNums[roleIndex] - 1);
            }
            else {
                perfStat.blueNums[roleIndex] = Math.Max(0, perfStat.blueNums[roleIndex] - 1);
            }
        }

        public void clearPets() {
            pets[0].Clear();
            pets[1].Clear();
            perfStat.redNums = new int[Math.Max(16, perfStat.redNums?.Length ?? 0)];
            perfStat.blueNums = new int[Math.Max(16, perfStat.blueNums?.Length ?? 0)];
        }

        public int getBlockIndex(Vector3 position) {
            int xn = Mathf.FloorToInt(position.x / SheepConfig.d + SheepConfig.w / (float)SheepConfig.d / 2f);
            int yn = Mathf.FloorToInt(position.y / SheepConfig.d + SheepConfig.h / (float)SheepConfig.d / 2f);
            return Util.getIndexByXnYn(xn, yn);
        }

        public int getNextPetId() => ++petId;

        public int rob_role(int count) {
            int old = cur_rob_role_index;
            cur_rob_role_index += count;
            return old;
        }

        public int rob_bullet(int count) {
            int old = cur_rob_bullet_index;
            cur_rob_bullet_index += count;
            return old;
        }

        public int rob_pre_bullet(int count) {
            int old = preBulletIndex;
            preBulletIndex += count;
            return old;
        }

        public void clearPetViews() {
            for (int i = 0; i < view_pets.Length; i++) {
                view_pets[i]?.clear();
            }
        }

        public PetView getPetView(int petIndex) {
            if (petIndex < 0 || petIndex >= SheepConfig.MaxPetCount) return null;
            PetView pet = view_pets[petIndex];
            if (pet == null) {
                pet = new PetView(petIndex);
                view_pets[petIndex] = pet;
            }

            return pet;
        }

        public void clearViewBullets() {
            for (int i = 0; i < view_bullets.Length; i++) view_bullets[i]?.clear();
            for (int i = 0; i < pre_view_bullets.Length; i++) pre_view_bullets[i]?.clear();
        }

        public BulletView getBulletView(int index) {
            if (index < 0 || index >= SheepConfig.MaxBulletCount) return null;
            BulletView bullet = view_bullets[index];
            if (bullet == null) {
                bullet = new BulletView();
                view_bullets[index] = bullet;
            }

            return bullet;
        }

        public BulletView getBulletPreView(int index) {
            if (index < 0 || index >= SheepConfig.MaxBulletCount) return null;
            BulletView bullet = pre_view_bullets[index];
            if (bullet == null) {
                bullet = new BulletView();
                pre_view_bullets[index] = bullet;
            }

            return bullet;
        }

        public void copyBulletPreView(int e, int bulletId, PetView view_pet, PetView view_tar_pet, Info l = null) {
            var n = SheepBullet.getById(bulletId);
            var r = view_pet != null ? view_pet.camp == SheepCamp.Red ? n.startOffsetX : -n.startOffsetX : 0;
            var preBullet = this.getBulletPreView(e);
            preBullet.bulletId = bulletId;
            preBullet.roleUid = view_pet != null ? view_pet.id : 0;
            preBullet.roleIndex = view_pet != null ? view_pet.index : 0;


            preBullet.camp = view_pet != null ? view_pet.camp : l.camp;
            if (view_tar_pet != null && 0 == view_tar_pet.roleId) {
                preBullet.tarRoleIndex = view_tar_pet.index;
            }
            else {
                preBullet.tarRoleIndex = -1;
            }

            if (n.moveType == (int)SheepBulletMoveType.Fixed) {
                float t = l != null && l.startX != 0
                    ? l.startX
                    : view_pet != null && view_pet.posX != 0
                        ? view_pet.posX
                        : 0;
                preBullet.x = t;
                float s = l != null && l.startY != 0
                    ? l.startY
                    : view_pet != null && view_pet.posY != 0
                        ? view_pet.posY
                        : 0;
                preBullet.y = s;

                preBullet.startY = n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.dirX = 0;
                preBullet.dirY = 0;
                preBullet.dirZ = 1;
            }
            else if (n.moveType == (int)SheepBulletMoveType.LineDir) {
                preBullet.x = view_pet.posX + r;
                preBullet.y = view_pet.posY + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.dirX = view_pet.dirX;
                preBullet.dirY = view_pet.dirY;
            }
            else if (n.moveType == (int)SheepBulletMoveType.CurvePosFrame) {
                float t = view_pet != null ? view_pet.posX : l.startX;
                float s = view_pet != null ? view_pet.posY : l.startY;
                float c = view_tar_pet != null ? view_tar_pet.posX : view_pet.tarPosX;
                float f = view_tar_pet != null ? view_tar_pet.posY : view_pet.tarPosY;
                preBullet.x = t + r;
                preBullet.y = s + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.startX = t + r;
                preBullet.startY = s + n.startOffsetY;
                preBullet.startZ = 0 + n.startOffsetZ;
                preBullet.endX = c;
                preBullet.endY = f;
                preBullet.endZ = 0 + n.endOffsetZ;
                preBullet.dirX = 0;
                preBullet.dirY = 0;
                preBullet.z = 1;
            }
            else if (n.moveType == (int)SheepBulletMoveType.DirAngle) {
                preBullet.x = view_pet.posX + r;
                preBullet.y = view_pet.posY + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.dirX = l.dirX;
                preBullet.dirY = l.dirY;
                preBullet.dirZ = l.dirZ;
            }
            else if (n.moveType == (int)SheepBulletMoveType.RadiusAngle) {
                preBullet.x = view_pet.posX + r;
                preBullet.y = view_pet.posY + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.startX = view_pet.posX + r;
                preBullet.startY = view_pet.posY + n.startOffsetY;
                preBullet.startZ = 0 + n.startOffsetZ;
                preBullet.dirX = l.dirX;
                preBullet.dirY = l.dirY;
                preBullet.dirZ = l.dirZ;
                preBullet.angle = l.angle;
            }
            else if (n.moveType == (int)SheepBulletMoveType.LineDirEndPos) {
                preBullet.x = l.startX;
                preBullet.y = l.startY;
                preBullet.z = l.startZ;
                preBullet.startX = l.startX;
                preBullet.startY = l.startY;
                preBullet.startZ = l.startZ;
                preBullet.endX = l.endX;
                preBullet.endY = l.endY;
                preBullet.endZ = l.endZ;
                if (l.dirX != 0 || l.dirY != 0 || l.dirZ != 0) {
                    preBullet.dirX = l.dirX;
                    preBullet.dirY = l.dirY;
                    preBullet.dirZ = l.dirZ;
                }
                else {
                    float t = l.endX - l.startX;
                    float i = l.endY - l.startY;
                    float s = l.endZ - l.startZ;
                    float o = (float)Math.Sqrt(t * t + i * i);
                    preBullet.dirX = t / o;
                    preBullet.dirY = i / o;
                    preBullet.dirZ = s / o;
                }
            }
            else if (n.moveType == (int)SheepBulletMoveType.LinePosFrame) {
                preBullet.x = l.startX;
                preBullet.y = l.startY;
                preBullet.z = l.startZ;
                preBullet.startX = l.startX;
                preBullet.startY = l.startY;
                preBullet.startZ = l.startZ;
                preBullet.endX = l.endX;
                preBullet.endY = l.endY;
                preBullet.endZ = l.endZ;
                float t = l.endX - l.startX;
                float i = l.endY - l.startY;
                float s = l.endZ - l.startZ;
                float o = (float)Math.Sqrt(t * t + i * i);
                preBullet.dirX = t / o;
                preBullet.dirY = i / o;
                preBullet.dirZ = s / o;
            }
            else {
                preBullet.x = view_pet.posX + r;
                preBullet.y = view_pet.posY + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.dirX = 0;
                preBullet.dirY = 0;
                preBullet.dirZ = 1;
            }

            preBullet.atkVue = view_pet != null ? view_pet.conf.atk : l.atk;
            preBullet.frame = 0;
        }

        public async Task game_run(SheepCtl sheepCtl, CancellationToken cancellationToken = default) {
            game_clear();
            mainClearBlocks();

            int runGameIndex = gameIndex;
            updateTime = NowMs();

            while (!cancellationToken.IsCancellationRequested &&
                   runGameIndex == gameIndex &&
                   (state == SheepRoomState.Run || state == SheepRoomState.Start)) {
                try {
                    long lastUpdateTime = updateTime;
                    updateTime = NowMs();
                    long diff = updateTime - lastUpdateTime;

                    if (diff >= 100) {
                        Debug.LogWarning("主线程更新逻辑耗时过长: " + diff + "ms");
                    }

                    if (diff < 33) {
                        await Task.Delay((int)(33 - diff), cancellationToken);
                    }

                    bool canStep = true;
                    if (comImages != null) {
                        canStep = ReadExternal(() => (bool)comImages.isHasFreeImage(), true);
                    }

                    if (canStep) {
                        float deltaMs = Mathf.Max(1f, NowMs() - lastUpdateTime);
                        game_update(this, sheepCtl, deltaMs);
                    }
                }
                catch (OperationCanceledException) {
                    return;
                }
                catch (Exception exception) {
                    Debug.LogError("主线程更新逻辑错误: " + exception);
                    return;
                }
            }
        }

        public void game_update(SheepMgr manager, SheepCtl sheepCtl, float deltaMs) {
            if (manager == null) manager = this;
            try {
                if (advanceGameClockInGameUpdate && manager.state == SheepRoomState.Run) {
                    manager.gameStartTimerForBuff += deltaMs;
                }

                consume(sheepCtl, deltaMs);
                buff_add_pets();
                buff_add_bullets();

                int unitCount = manager.pets[0].Count + manager.pets[1].Count;
                if (unitCount <= 0 && manager.petCount <= 2) {
                    update_merge_workers(manager, sheepCtl, deltaMs);
                    return;
                }

                cur_rob_role_index = 0;
                cur_rob_bullet_index = 0;

                if (comImages != null) {
                    curIndexImages = ReadExternal<CurIndexImages>(() => comImages.startAdd(), null);
                }

                role_logic();

                if (comImages != null) {
                    IgnoreExternal(() => comImages.endAdd());
                }

                update_merge_workers(manager, sheepCtl, deltaMs);
            }
            catch (Exception exception) {
                Debug.LogError("update逻辑错误: " + exception);
                throw;
            }
        }

        private int ResolveAnimationFrameCount(PetView source, SheepCtl sheepCtl) {
            if (source?.view_pet == null) return 1;
            if (AnimationFrameCountResolver != null) {
                int count = AnimationFrameCountResolver(source.view_pet);
                if (count > 0) return count;
            }

            return ReadExternal(() => {
                int camp = CampIndex(source.camp);
                int skin = source.skinId ?? 0;
                int anim = (int)source.view_pet.animType;
                var frames = sheepCtl.comImages.roles_framess[camp][skin][anim];
                return Math.Max(1, Convert.ToInt32(frames.Length));
            }, 1);
        }

        public void update_merge_workers(SheepMgr manager, SheepCtl sheepCtl, float deltaMs) {
            if (manager == null) manager = this;
            long now = NowMs();

            mainClearBlocks();
            OnFrameBlockStart?.Invoke();
            IgnoreExternal(() => sheepCtl.comImages.mesh_block.onFrameUpdateStart());

            if (manager.endTime > 0 && manager.endTime < now) {
                manager.endTime = 0;
                RaiseRoomEnd();
                return;
            }

            Array.Clear(manager.countNewBuffs, 0, 2);
            Array.Clear(manager.countBuffs, 0, 2);
            Array.Clear(manager.countShowBuffs, 0, 2);

            for (int camp = 0; camp < 2; camp++) {
                List<Buff> activeBuffs = manager.buffs[camp];
                while (activeBuffs.Count > 0 && activeBuffs[0].time < manager.gameStartTimerForBuff) {
                    activeBuffs.RemoveAt(0);
                }

                foreach (Buff buff in activeBuffs) {
                    manager.countBuffs[camp] += buff.count != 0
                        ? buff.count
                        : SheepConfig.counterBuffNumber;
                    manager.countShowBuffs[camp] += buff.count;
                }
            }

            for (int camp = 0; camp < 2; camp++) {
                List<int> pending = manager.preBuffs[camp];
                if (pending.Count == 0) continue;

                int sum = 0;
                bool hasCounterMarker = false;
                foreach (int value in pending) {
                    if (value == 0) hasCounterMarker = true;
                    sum += value;
                }

                if (hasCounterMarker) {
                    manager.buffs[camp].Add(new Buff {
                        time = Mathf.RoundToInt(manager.gameStartTimerForBuff + 1000f * SheepConfig.counterTime),
                        count = 0
                    });
                    if (pending.Count > 1) {
                        manager.buffs[camp].Add(new Buff {
                            time = Mathf.RoundToInt(manager.gameStartTimerForBuff + 1000f * SheepConfig.buffLastTime),
                            count = sum
                        });
                    }
                }
                else {
                    manager.buffs[camp].Add(new Buff {
                        time = Mathf.RoundToInt(manager.gameStartTimerForBuff + 1000f * SheepConfig.buffLastTime),
                        count = sum
                    });
                }

                pending.Clear();
                manager.countNewBuffs[camp] += sum;
            }

            if (updateBoss(manager, sheepCtl, deltaMs, now)) return;

            List<PetView> allPets = new List<PetView>(manager.pets[0].Count + manager.pets[1].Count);
            allPets.AddRange(manager.pets[0]);
            allPets.AddRange(manager.pets[1]);

            int redActiveBuffAnimCount = 0;
            int blueActiveBuffAnimCount = 0;

            foreach (PetView source in allPets) {
                if (source == null || source.buff_index == -1 || source.view_pet == null) continue;

                source.updateSkin(sheepCtl, this, manager, deltaMs);
                PetView view = source.view_pet;
                if (view == null) continue;

                SheepRoleState roleState = view.state;
                int animationFrameCount = ResolveAnimationFrameCount(source, sheepCtl);
                int lastFrame = Math.Max(0, animationFrameCount - 1);

                if (roleState == SheepRoleState.In && view.animFrame >= lastFrame) {
                    if (SheepSkill.TryGetById(view.readySkillId, out SheepSkill skill)) {
                        if (skill.skillType == SheepSkillType.Boom) {
                            SheepSkillSubBoom boom = SheepSkillSubBoom.getById(skill.id);
                            view.state = SheepRoleState.Boom;
                            view.animType = boom.isAnim != 0 ? SheepRoleAnimType.Boom : SheepRoleAnimType.Idle;
                        }
                    }
                    else {
                        view.state = SheepRoleState.Move;
                        view.animType = SheepRoleAnimType.Idle;
                    }
                }
                else if (roleState == SheepRoleState.Dead && view.animFrame >= lastFrame) {
                    view.state = SheepRoleState.Res;
                    view.animType = SheepRoleAnimType.None;
                    source.onRes(sheepCtl, manager);
                }
                else if (roleState == SheepRoleState.Up && view.animFrame >= lastFrame) {
                    view.state = SheepRoleState.In;
                    view.animType = SheepRoleAnimType.In;
                }
                else if (roleState == SheepRoleState.Buff) {
                    SheepSkillSubBuff buff = SheepSkillSubBuff.getById(view.readySkillId);
                    if (view.animFrame > buff.buffStratFrame && view.animFrame < buff.buffEndFrame) {
                        if (source.camp == SheepCamp.Blue) blueActiveBuffAnimCount++;
                        else redActiveBuffAnimCount++;
                    }
                }
            }

            for (int index = 0; index < bulletCount; index++) {
                BulletView bullet = getBulletView(index);
                if (bullet == null || bullet.isDie || bullet.conf == null) continue;

                if (bullet.frame >= bullet.conf.endFrame) {
                    bullet.isDie = true;
                    buff_del_bullet(index);
                    continue;
                }

                PetView owner = getPetView(bullet.roleIndex);
                int split = owner?.conf?.splitN ?? 0;
                for (int x = -split; x <= split; x++) {
                    for (int y = -split; y <= split; y++) {
                        int blockIndex = Util.getIndexByXY(bullet.x + x, bullet.y + y);
                        IgnoreExternal(() => sheepCtl.comImages.mesh_block.addFrameBlockCamp(blockIndex, bullet.camp));
                    }
                }
            }

            mainSyncBlocksToWokers();
            OnFrameBlockEnd?.Invoke(manager);
            IgnoreExternal(() => sheepCtl.comImages.mesh_block.onFrameUpdateEnd(manager));

            redBuffCount = redActiveBuffAnimCount;
            blueBuffCount = blueActiveBuffAnimCount;
            roleMaxIndex = petCount;
            bulletMaxIndex = bulletCount;
        }

        private void InitializeBossView(SheepCamp camp) {
            int index = CampIndex(camp);
            PetView view = getPetView(index);
            view.clear();
            view.id = getNextPetId();
            view.isActive = true;
            view.isDie = false;
            view.camp = camp;
            view.roleId = 0;
            view.skinId = 0;
            view.conf = SheepRoleTypeInfo.getById(0);
            view.state = (SheepRoleState)(int)SheepBossState.Ready;
            view.subState = SheepRoleSubState.None;
            view.animType = SheepRoleAnimType.Idle;
            view.curHp = loongHp;
            view.posX = camp == SheepCamp.Red ? -sheepMode.loongX : sheepMode.loongX;
            view.posY = 0f;
            view.posBefX = view.posX;
            view.posBefY = view.posY;
            view.animX = view.posX;
            view.animY = view.posY;
            view.blockIndex = Util.getIndexByXY(view.posX, view.posY);
            view.befBlockIndex = view.blockIndex;
            view.dirX = camp == SheepCamp.Red ? 1f : -1f;
            view.dirY = 0f;
            bossHp[index] = ReadExternal(() => Convert.ToSingle(boss[index].curHp), (float)loongHp);
            view.curHp = bossHp[index];
        }

        private void SetBossProgress(int camp, float value) {
            bossHp[camp] = value;
            IgnoreExternal(() => boss[camp].comProgress.setVue(value));
            IgnoreExternal(() => boss[camp].curHp = value);
        }

        private bool ConsumeBossShield(SheepCamp camp) {
            if (BossShieldConsumer != null) return BossShieldConsumer(camp);
            int index = CampIndex(camp);
            return ReadExternal(() => (bool)boss[index].subShield(), false);
        }

        public bool updateBoss(SheepMgr manager, SheepCtl sheepCtl, float deltaMs, long now) {
            bool ended = false;

            for (int index = 0; index < 2; index++) {
                PetView viewPet = getPetView(index);
                if (viewPet == null || !viewPet.isActive) continue;

                SheepCamp camp = viewPet.camp;
                SheepBossState bossState = (SheepBossState)(int)viewPet.state;

                if (bossState == SheepBossState.Ready) {
                    float initialHp = ReadExternal(() => Convert.ToSingle(boss[index].curHp), (float)loongHp);
                    viewPet.curHp = initialHp;
                    SetBossProgress(index, initialHp);
                    viewPet.state = (SheepRoleState)(int)SheepBossState.NomalRun;
                    continue;
                }

                if (bossState == SheepBossState.AwakeAnim || bossState == SheepBossState.UnAwakeAnim) {
                    SetBossProgress(index, bossHp[index]);
                    continue;
                }

                if (bossState == SheepBossState.Dead) continue;

                float currentHp = Mathf.Max(0f, viewPet.curHp);
                float previousHp = bossHp[index];
                float damage = previousHp - currentHp;

                if (damage > 0f && currentHp > 0f) {
                    int attackBuffCount = manager.countBuffs[1 - index];
                    if (attackBuffCount > 0) {
                        float multiplier = 1f + SheepConfig.buffDragonDamageIncreseRate * attackBuffCount;
                        damage = Mathf.Floor(damage * multiplier);
                        currentHp = previousHp - damage;
                        viewPet.curHp = currentHp;
                    }

                    int defenceBuffCount = manager.countBuffs[index];
                    if (defenceBuffCount > 0) {
                        float multiplier = Mathf.Pow(1f - SheepConfig.buffDragonReduceRate, defenceBuffCount);
                        multiplier = Mathf.Max(multiplier, 1f - SheepConfig.buffDragonMaxReduceRate);
                        damage = Mathf.Floor(damage * multiplier);
                        currentHp = previousHp - damage;
                        viewPet.curHp = currentHp;
                    }
                }

                // 原版会先调用 subShield()，再判断本次伤害是否大于 1。
                bool consumedShield = ConsumeBossShield(camp);
                if (consumedShield && damage > 1f) {
                    damage = 1f;
                    currentHp = Mathf.Max(0f, previousHp - 1f);
                    viewPet.curHp = currentHp;
                }

                if (!Mathf.Approximately(currentHp, previousHp)) {
                    SetBossProgress(index, currentHp);
                    OnBossHpChanged?.Invoke(camp, currentHp, damage);
                    IgnoreExternal(() => boss[index].hitAnim());
                }

                int visibleBuffCount = manager.countShowBuffs[index];
                int totalBuffCount = manager.countBuffs[index];

                if (!manager.flagLongBuffs[index] &&
                    currentHp < manager.loongHp * SheepConfig.counterHpRatio) {
                    manager.flagLongBuffs[index] = true;
                    manager.bossBackStateTime[index] = now;
                    manager.preBuffs[index].Add(0);
                    OnCounterStarted?.Invoke(camp);
                    OnCameraShake?.Invoke(SheepConfig.shockBeginNumber);
                    IgnoreExternal(() => boss[index].backStateTime = now);
                    IgnoreExternal(() => sheepCtl.comMatch.showDoubleAnim(camp));
                    IgnoreExternal(() => sheepCtl.comUIAnim.backAnim(camp));
                    IgnoreExternal(() => sheepCtl.cameraCtl.onShake(SheepConfig.shockBeginNumber));
                }
                else if (manager.bossBackStateTime[index] > 0 &&
                         now - manager.bossBackStateTime[index] > 120000 &&
                         totalBuffCount - visibleBuffCount == 0) {
                    manager.bossBackStateTime[index] = 0;
                    OnCounterFinished?.Invoke(camp);
                    OnCameraShake?.Invoke(SheepConfig.shockEndNumber);
                    IgnoreExternal(() => boss[index].backStateTime = 0);
                    IgnoreExternal(() => sheepCtl.comMatch.hideDoubleAnim(camp));
                    IgnoreExternal(() => sheepCtl.comUIAnim.backSuccessAnim(camp));
                    IgnoreExternal(() => sheepCtl.cameraCtl.onShake(SheepConfig.shockEndNumber));
                }

                if (currentHp <= 0f) {
                    viewPet.state = (SheepRoleState)(int)SheepBossState.Dead;
                    viewPet.isDie = true;
                    SetBossProgress(index, 0f);
                    ended = true;
                    RaiseRoomEnd();
                    break;
                }

                int stateIndex = 0;
                for (int thresholdIndex = 0;
                     thresholdIndex < SheepConfig.loongStateSwitching.Length;
                     thresholdIndex++) {
                    if (manager.plotRatio <= SheepConfig.loongStateSwitching[thresholdIndex]) {
                        stateIndex = thresholdIndex;
                        break;
                    }
                }

                manager.plotRatioIndex = stateIndex;
                int visualState = stateIndex + 1;
                IgnoreExternal(() => boss[index].updateState(sheepCtl, manager, visualState));
                IgnoreExternal(() => boss[index].updateStateJJL(sheepCtl, manager, visualState));
            }

            return ended;
        }

        public void pre_add_pet(PetView source) {
            if (source != null) petsAdd.Add(source);
        }

        public void buff_add_pets() {
            while (petsAdd.Count > 0) {
                int index;
                if (petsDel.Count > 0) {
                    index = petsDel.Pop();
                }
                else {
                    if (petCount >= SheepConfig.MaxPetCount) {
                        Debug.LogWarning($"预加入怪物超过最大数量: {petCount}/{SheepConfig.MaxPetCount}");
                        break;
                    }

                    index = petCount++;
                }

                PetView source = petsAdd[0];
                petsAdd.RemoveAt(0);
                PetView view = getPetView(index);
                source.init(index, view);
            }
        }

        public void buff_del_pet(int index) {
            PetView pet = getPetView(index);
            if (pet == null) return;
            pet.isDie = true;
            pet.id = 0;
            petsDel.Push(index);
        }

        public void clear_pets() {
            cur_rob_role_index = 0;
            roleMaxIndex = 0;
            petCount = 0;
            petsAdd.Clear();
            petsDel.Clear();
        }

        public void buff_add_bullets() {
            int pendingCount = preBulletIndex;
            while (pendingCount > 0) {
                int index;
                if (bulletsDel.Count > 0) {
                    index = bulletsDel.Pop();
                }
                else {
                    if (bulletCount >= SheepConfig.MaxBulletCount) {
                        Debug.LogWarning($"预加入子弹超过最大数量: {bulletCount}/{SheepConfig.MaxBulletCount}");
                        break;
                    }

                    index = bulletCount++;
                }

                pendingCount--;
                BulletView preview = pre_view_bullets[pendingCount];
                if (preview != null) {
                    getBulletView(index).init(++bulletId, preview);
                }
            }

            preBulletIndex = 0;
        }

        public void buff_del_bullet(int index) {
            BulletView bullet = getBulletView(index);
            if (bullet == null || bullet.id == 0) return;
            bullet.id = 0;
            bullet.isDie = true;
            bulletsDel.Push(index);
        }

        public void clear_bullets() {
            cur_rob_bullet_index = 0;
            bulletMaxIndex = 0;
            bulletCount = 0;
            bulletsDel.Clear();
        }

        public void game_clear() {
            clearBlocks();
            clearPetViews();
            preBulletIndex = 0;
            clearViewBullets();
            clear_pets();
            clear_bullets();

            InitializeBossView(SheepCamp.Red);
            InitializeBossView(SheepCamp.Blue);
            petCount = 2;
            roleMaxIndex = 2;
        }

        public (long time, bool isEndWorker) role_logic() {
            Stopwatch stopwatch = Stopwatch.StartNew();
            logic_counts[CampIndex(SheepCamp.Red)] = redBuffCount > 0 ? 2 : 1;
            logic_counts[CampIndex(SheepCamp.Blue)] = blueBuffCount > 0 ? 2 : 1;

            int roleCount = roleMaxIndex > 0 ? rob_role_task(roleMaxIndex, curIndexImages) : 0;
            int activeBulletCount = bulletMaxIndex > 0 ? rob_bullet_task(bulletMaxIndex, curIndexImages) : 0;

            if (bullte_creates.Count > 0) {
                int previewIndex = rob_pre_bullet(bullte_creates.Count);
                foreach (BullteCreate request in bullte_creates) {
                    if (previewIndex >= SheepConfig.MaxBulletCount) break;
                    copyBulletPreView(
                        previewIndex++,
                        request.bulletId,
                        request.view_pet,
                        request.view_tar_pet,
                        request.info
                    );
                }

                bullte_creates.Clear();
            }

            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 33) {
                Debug.Log(
                    $"count_role:{roleCount} count_bullet:{activeBulletCount} 耗时:{stopwatch.ElapsedMilliseconds}ms");
            }

            return (stopwatch.ElapsedMilliseconds, true);
        }

        public int rob_role_task(int count, CurIndexImages currentImages) {
            int start = rob_role(count);
            int end = start + count;
            int updated = update_role(start, end);
            if (comImages != null) IgnoreExternal(() => comImages.update_role(currentImages));
            return updated;
        }

        public int rob_bullet_task(int count, CurIndexImages currentImages) {
            int start = rob_bullet(count);
            int end = start + count;
            int updated = update_bullet(start, end);
            if (comImages != null) IgnoreExternal(() => comImages.update_bullet(currentImages));
            return updated;
        }

        public int update_role(int start, int end) {
            int safeEnd = Math.Min(end, SheepConfig.MaxPetCount);
            for (int index = start; index < safeEnd; index++) {
                PetView viewPet = getPetView(index);
                if (viewPet == null || !viewPet.isActive) continue;

                bool wasDead = viewPet.isDie;
                if (viewPet.roleId == 0) {
                    bool logicTick = update_frame(viewPet);
                    if (!wasDead && logicTick) update_boss_state(viewPet);
                    update_role_anim(viewPet);
                    continue;
                }

                int camp = CampIndex(viewPet.camp);
                int loops = Math.Max(1, logic_counts[camp]);
                for (int loop = 0; loop < loops; loop++) {
                    bool logicTick = update_frame(viewPet);
                    if (!wasDead) update_role_state(viewPet, logicTick);
                    update_role_anim(viewPet);
                }

                OnRoleRender?.Invoke(viewPet);
                if (comImages != null) IgnoreExternal(() => comImages.addRole(viewPet));
            }

            return safeEnd - start;
        }

        public int update_bullet(int start, int end) {
            for (var i = start; i < end; i++) {
                if (i >= SheepConfig.MaxBulletCount) {
                    return i - start;
                }

                var t = this.getBulletView(i);
                if (t.isDie) {
                    continue;
                }

                if (t.id != 0 && t.conf.animId != 0) {
                    var e = t;
                    this.comImages.addBullet(e);
                }

                var xnyn = Util.getXnYn(t.x, t.y);
                var s = xnyn.xn;
                var o = xnyn.yn;
                var l = t.frame;
                var n = t.conf;
                {
                    var e = t.conf.atkFrames;
                    if (e != null) {
                        for (var i = 0; i < e.Length; i++) {
                            var n = e[i];
                            if (-1 == n || n == l) {
                                if (0 == t.tarRoleIndex || 1 == t.tarRoleIndex) {
                                    var o = this.getPetView(t.tarRoleIndex);
                                    if (UtilAck.isCanAckByBullet(t, o, i)) {
                                        UtilAck.hurtByBullet(t, o, t.atkVue);
                                    }
                                }
                                else
                                    UtilFind.forfeachBlocksByAckView(t.camp, s, o, t.conf.findR,
                                        (e => {
                                            UtilAck.isCanAckByBullet(t, e, i) && UtilAck.hurtByBullet(t, e, t.atkVue)
                                        }));

                                break;
                            }
                        }
                    }

                    switch (t.conf.moveType) {
                        case (int)SheepBulletMoveType.Fixed:
                            break;
                        case (int)SheepBulletMoveType.LineDir:
                            t.x = (float)(t.x + t.dirX * n.speed * .033);
                            t.y = (float)(t.y + t.dirY * n.speed * .033);
                            break;
                        case (int)SheepBulletMoveType.LinePosFrame:
                            t.x = t.startX + (t.endX - t.startX) * l / n.moveTimeFrame;
                            t.y = t.startY + (t.endY - t.startY) * l / n.moveTimeFrame;
                            t.z = t.startZ + (t.endZ - t.startZ) * l / n.moveTimeFrame;
                            break;
                        case (int)SheepBulletMoveType.LineTarFrame:
                            break;
                        case (int)SheepBulletMoveType.CurvePosFrame:
                            var e1 = (t.startX + t.endX) / 2;
                            var i1 = (t.startY + t.endY) / 2;
                            var s1 = n.curveHigh;
                            var o1 = t.startX + (e1 - t.startX) * l / n.moveTimeFrame;
                            var r1 = e1 + (t.endX - e1) * l / n.moveTimeFrame;
                            var a = t.startY + (i1 - t.startY) * l / n.moveTimeFrame;
                            var c = i1 + (t.endY - i1) * l / n.moveTimeFrame;
                            var f = t.startZ + (s1 - t.startZ) * l / n.moveTimeFrame;
                            var h = s1 + (t.endZ - s1) * l / n.moveTimeFrame;
                            t.x = o1 + (r1 - o1) * l / n.moveTimeFrame;
                            t.y = a + (c - a) * l / n.moveTimeFrame;
                            t.z = f + (h - f) * l / n.moveTimeFrame;
                            var p = t.endX - t.startX > 0 ? 1 : -1;
                            var u = .2 * p;
                            var d = .8;
                            var g = p;
                            var S = 0;
                            var m = -.2 * p;
                            var y = -.8;
                            var k = u + (g - u) * l / n.endFrame;
                            var B = d + (S - d) * l / n.endFrame;
                            var w = g + (m - g) * l / n.endFrame;
                            var R = S + (y - S) * l / n.endFrame;
                            t.dirX = (float)(k + (w - k) * l / n.endFrame);
                            t.dirY = 0;
                            t.dirZ = (float)(B + (R - B) * l / n.endFrame);
                            break;
                        case (int)SheepBulletMoveType.CurveTarFrame:
                            break;
                        case (int)SheepBulletMoveType.LineDirEndPos:
                            t.x = (float)(t.x + t.dirX * n.speed * .033);
                            t.y = (float)(t.y + t.dirY * n.speed * .033);
                            t.z = (float)(t.z + t.dirZ * n.speed * .033);
                            break;
                        case (int)SheepBulletMoveType.RadiusAngle:
                            t.angle += n.speed;
                            var x = t.roleUid;
                            var _ = this.getPetView(t.roleIndex);
                            if (x == _.id) {
                                t.x = (float)(_.animX + n.radius * Math.Cos(t.angle));
                                t.y = (float)(_.animY + n.radius * Math.Sin(t.angle));
                            }
                            else {
                                t.isDie = true;
                            }

                            break;
                        case (int)SheepBulletMoveType.DirAngle: {
                            t.x = (float)(t.x + t.dirX * n.speed * .033);
                            t.y = (float)(t.y + t.dirY * n.speed * .033);
                            t.z = (float)(t.z + t.dirZ * n.speed * .033);
                        }
                    }

                    t.frame = l + 1;
                }
                var r = n.createBulletID;
                if (r != 0 && n.createBulletFrame == l) {
                    var e = this.getPetView(t.roleIndex);
                    var i1 = this.getPetView(t.tarRoleIndex);
                    this.bullte_creates.Add(new BullteCreate() {
                        view_pet = e,
                        bulletId = r,
                        view_tar_pet = i1,
                        info = new Info() { startX = t.x, startY = t.y, startZ = 100 }
                    });
                }

                t = null;
            }

            return end - start;
        }

        public bool update_frame(PetView viewPet) {
            var frame = viewPet.frame;
            var loopFrame = SheepConfig.loopFrame;
            var i = frame % loopFrame == loopFrame - 1;
            var posBefX = viewPet.posBefX;
            var posBefY = viewPet.posBefY;
            var posX = viewPet.posX;
            var posY = viewPet.posY;
            if (!viewPet.isDie) {
                viewPet.animX = posBefX + (posX - posBefX) * (frame % loopFrame) / loopFrame;
                viewPet.animY = posBefY + (posY - posBefY) * (frame % loopFrame) / loopFrame;
            }

            frame += 1;
            viewPet.frame = frame;
            if (!viewPet.isDie && i) {
                viewPet.logicMove(posX, posY);
            }

            return i;
        }

        public void update_boss_state(PetView bossView) {
            SheepBossState bossState = (SheepBossState)(int)bossView.state;
            switch (bossState) {
                case SheepBossState.NomalRun:
                case SheepBossState.AwakeRun:
                case SheepBossState.BackRun: {
                    SheepRoleTypeInfo config = bossView.conf;
                    int attackFrame = bossView.curAckFrame;
                    if (attackFrame == 0) {
                        (int xn, int yn) block = Util.getXnYn(bossView.posX, bossView.posY);
                        bool hasTarget = false;
                        UtilFind.findNearBlocksByAckView(
                            bossView,
                            block.xn,
                            block.yn,
                            Mathf.FloorToInt(config.findR * SheepConfig.loongExaminationRangeBet),
                            target => {
                                if (!hasTarget && Util.isCanAckByRole(bossView, target)) {
                                    hasTarget = true;
                                    return true;
                                }

                                return false;
                            }
                        );
                        if (!hasTarget) break;
                    }

                    attackFrame++;
                    bossView.curAckFrame = attackFrame;
                    int readyFrame = config.readyAtks != null && config.readyAtks.Length > 0
                        ? Mathf.FloorToInt(config.readyAtks[0] / 3f)
                        : 0;
                    if (attackFrame == readyFrame) {
                        (int xn, int yn) block = Util.getXnYn(bossView.posX, bossView.posY);
                        UtilFind.forfeachBlocksByAckView(
                            bossView.camp,
                            block.xn,
                            block.yn,
                            config.findR,
                            target => {
                                if (Util.isCanAckByRole(bossView, target)) {
                                    UtilAck.hurtByRole(bossView, target, config.atk);
                                }
                            }
                        );
                    }

                    if (attackFrame >= Mathf.FloorToInt(1000f * config.atkCd / 100f)) {
                        bossView.curAckFrame = 0;
                    }

                    break;
                }
            }
        }

        public void update_role_state_in(PetView petSkin) {
            if (petSkin.conf.skillIn == 0) return;

            SheepSkill skill = SheepSkill.getById(petSkin.conf.skillIn);
            if (skill.skillType != SheepSkillType.Boom) return;

            SheepSkillSubBoom boom = SheepSkillSubBoom.getById(skill.id);
            if (petSkin.animFrame != 1) return;

            float fallbackX = petSkin.camp == SheepCamp.Red ? -1200f : 1200f;
            (int xn, int yn) block = Util.getXnYn(fallbackX, 0f);
            PetView target = null;
            UtilFind.findNearBlocksByAckView(petSkin, block.xn, block.yn, 100, candidate => {
                target = candidate;
                return true;
            });

            if (target != null) {
                petSkin.posBefX = petSkin.posX;
                petSkin.posBefY = petSkin.posY;
                petSkin.posX = target.posX;
                petSkin.posY = target.posY;
                petSkin.animX = target.posX;
                petSkin.animY = target.posY;
            }
            else {
                petSkin.posBefX = fallbackX;
                petSkin.posBefY = 0f;
                petSkin.posX = fallbackX;
                petSkin.posY = 0f;
                petSkin.animX = fallbackX;
                petSkin.animY = 0f;
            }

            petSkin.readySkillId = boom.id;
            petSkin.isLock = true;
        }

        public void update_role_state_move(PetView petSkin, bool logicTick, float deltaSeconds) {
            if (petSkin.isLock) return;

            FindTarResult result = UtilFind.findTar(petSkin);
            if (result.atkTar != null) {
                petSkin.state = SheepRoleState.Attack;
                petSkin.subState = SheepRoleSubState.AttackAwait;
                return;
            }

            if (result.moveTar != null) {
                petSkin.subState = SheepRoleSubState.MoveTar;
                Util.moveTar(petSkin, result.moveTar, deltaSeconds, logicTick);
                return;
            }

            if (result.moveBoss != null) {
                petSkin.subState = SheepRoleSubState.MoveBoss;
                Util.moveTar(petSkin, result.moveBoss, deltaSeconds, logicTick);
                return;
            }

            Debug.LogWarning("移动状态没有目标: pet=" + petSkin.index);
        }

        public void update_role_state_attack(PetView petSkin, bool logicTick, float deltaSeconds) {
            SheepRoleAtkMoveType attackMoveType = (SheepRoleAtkMoveType)petSkin.conf.atkMoveType;
            if (petSkin.conf.isLoongStopDistance != 0) {
                float bossX = petSkin.camp == SheepCamp.Red ? sheepMode.loongX : -sheepMode.loongX;
                if (Util.dis(petSkin.posX, petSkin.posY, bossX, 0f) <= petSkin.conf.loongStopDistanceR) {
                    attackMoveType = SheepRoleAtkMoveType.None;
                }
            }

            if (petSkin.subState == SheepRoleSubState.AttackAwait) {
                if (!Util.isAtkCd(petSkin)) {
                    petSkin.subState = SheepRoleSubState.AttackAnim;
                    petSkin.animType = SheepRoleAnimType.Attack;
                }
            }
            else if (petSkin.subState == SheepRoleSubState.AttackAnim) {
                SheepRoleTypeInfo config = petSkin.conf;
                int finishFrame = config.finishAtk;
                int animationFrame = petSkin.animFrame;
                int[] readyFrames = config.readyAtks ?? Array.Empty<int>();

                foreach (int readyFrame in readyFrames) {
                    if (animationFrame != readyFrame) continue;

                    PetView target;
                    if (config.atkType == SheepRoleAtkType.Throw) {
                        target = UtilFind.findSortAck(petSkin, config.findR);
                        if (config.roleType == SheepRoleType.pao_che) {
                            PetView backBoss = Util.getBackBoss(petSkin.camp);
                            if (backBoss != null && Util.isCanAckByRole(petSkin, backBoss)) {
                                target = backBoss;
                            }
                        }
                    }
                    else {
                        target = UtilFind.findNearAck(petSkin);
                    }

                    if (config.bullet != null && config.bullet.Length > 0) {
                        int bulletConfigIndex = petSkin.camp == SheepCamp.Red ? 0 : 1;
                        bulletConfigIndex = Math.Min(bulletConfigIndex, config.bullet.Length - 1);
                        bullte_creates.Add(new BullteCreate {
                            view_pet = petSkin,
                            bulletId = config.bullet[bulletConfigIndex],
                            view_tar_pet = target
                        });
                    }
                    else if (target != null) {
                        UtilAck.ackTar(petSkin, target);
                    }

                    break;
                }

                if (animationFrame >= finishFrame) {
                    Util.resetAtkCd(petSkin, config.atkCd);
                    FindTarResult result = UtilFind.findTar(petSkin);
                    if (result.atkTar != null) {
                        petSkin.subState = SheepRoleSubState.AttackAwait;
                        petSkin.animType = SheepRoleAnimType.Idle;
                        return;
                    }

                    if (result.moveTar != null) {
                        petSkin.state = SheepRoleState.Move;
                        petSkin.subState = SheepRoleSubState.MoveTar;
                        petSkin.animType = SheepRoleAnimType.Idle;
                        return;
                    }

                    if (result.moveBoss != null) {
                        petSkin.state = SheepRoleState.Move;
                        petSkin.subState = SheepRoleSubState.MoveBoss;
                        petSkin.animType = SheepRoleAnimType.Idle;
                        return;
                    }
                }
            }

            bool canMoveDuringAttack = attackMoveType == SheepRoleAtkMoveType.Move ||
                                       (attackMoveType == SheepRoleAtkMoveType.CdMove &&
                                        petSkin.subState == SheepRoleSubState.AttackAwait);
            if (logicTick && canMoveDuringAttack) {
                PetView target = UtilFind.findNearAck(petSkin);
                if (target != null &&
                    Util.disByRole(petSkin, target) > petSkin.conf.atkMinMoveR + target.conf.collideR) {
                    Util.moveTar(petSkin, target, deltaSeconds, true);
                }
            }
        }

        public void update_role_state_killer(PetView petSkin) {
            SheepSkillSubKiller killer = SheepSkillSubKiller.getById(petSkin.readySkillId);
            int animationFrame = petSkin.animFrame;

            if (animationFrame == killer.findMoveFrame) {
                bool interruptedByShield = false;
                if (petSkin.conf.roleType == SheepRoleType.ci_ke) {
                    UtilFind.foreachFront(
                        petSkin,
                        target => {
                            if (target.conf.roleType == SheepRoleType.dun_bing) {
                                interruptedByShield = true;
                            }
                        },
                        petSkin.conf.findR,
                        60f
                    );
                }

                if (interruptedByShield) {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return;
                }

                PetView target = UtilFind.findFarAck(petSkin, killer.findR);
                if (target != null) {
                    petSkin.logicMove(target.posX, target.posY);
                }
                else {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                }
            }

            if (animationFrame == killer.atkFrame) {
                UtilAck.ackMe(petSkin, killer.spiltRadiusBet, killer.atkBet, killer.atkFindR);
            }

            if (animationFrame >= killer.endFrame) {
                SheepRoleSubState current = petSkin.subState;
                if (current == SheepRoleSubState.KillerEnd ||
                    (int)current - (int)SheepRoleSubState.KillerStart >= killer.cnt) {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return;
                }

                petSkin.subState = (SheepRoleSubState)((int)current + 1);
                petSkin.animType = SheepRoleAnimType.Killer;
            }
        }

        public void update_role_state_boom(PetView petSkin) {
            SheepSkill skill = SheepSkill.getById(petSkin.readySkillId);
            SheepSkillSubBoom boom = SheepSkillSubBoom.getById(skill.id);
            int animationFrame = petSkin.animFrame;

            if (animationFrame == boom.atkFrame) {
                List<SheepRoleType> excluded = new List<SheepRoleType>();
                if (petSkin.conf.roleType == SheepRoleType.chong_feng_bing ||
                    petSkin.conf.roleType == SheepRoleType.qi_lin) {
                    excluded.Add(SheepRoleType.qi_lin);
                }

                UtilAck.ackMe(
                    petSkin,
                    boom.spiltRadiusBet,
                    boom.atkBet,
                    boom.atkFindR,
                    boom.hitBackDistance,
                    excluded
                );
            }

            if (animationFrame < boom.endFrame) return;

            petSkin.isLock = false;
            SheepRoleState endState = (SheepRoleState)boom.endState;
            switch (endState) {
                case SheepRoleState.Move:
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    break;
                case SheepRoleState.Rigidity:
                    petSkin.state = SheepRoleState.Rigidity;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    petSkin.readySkillId = boom.endSkill;
                    break;
                case SheepRoleState.Dead:
                    petSkin.isDie = true;
                    petSkin.state = SheepRoleState.Dead;
                    break;
                case SheepRoleState.Palm:
                    petSkin.state = SheepRoleState.Palm;
                    petSkin.subState = SheepRoleSubState.Palm;
                    petSkin.animType = SheepRoleAnimType.Palm;
                    petSkin.readySkillId = boom.endSkill;
                    break;
                default:
                    Debug.LogError("Boom.endState 错误: " + boom.endState);
                    break;
            }
        }

        public void update_role_state_invincible(PetView petSkin) {
            int animationFrame = petSkin.animFrame;
            SheepSkill skill = SheepSkill.getById(petSkin.readySkillId);
            SheepSkillSubInvincible config = SheepSkillSubInvincible.getById(skill.id);

            foreach (int healFrame in config.healFrames ?? Array.Empty<int>()) {
                if (animationFrame != healFrame) continue;
                int heal = Mathf.FloorToInt((petSkin.conf.hp - petSkin.curHp) * (config.healHealthPercent / 100f));
                UtilAck.hurtByRole(petSkin, petSkin, -heal);
                break;
            }

            foreach (int attackFrame in config.atkFrames ?? Array.Empty<int>()) {
                if (animationFrame != attackFrame) continue;
                UtilAck.ackMe(petSkin, config.spiltRadiusBet, config.atkBet, config.atkFindR);
                break;
            }

            if (animationFrame >= config.endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
        }

        public void update_role_state_bladestorm(PetView petSkin, bool logicTick, float deltaSeconds) {
            int animationFrame = petSkin.animFrame;
            SheepSkill skill = SheepSkill.getById(petSkin.readySkillId);
            SheepSkillSubBladestorm config = SheepSkillSubBladestorm.getById(skill.id);

            if (logicTick) {
                FindTarResult result = UtilFind.findTar(petSkin, config.findR);
                PetView target = result.atkTar ?? result.moveTar ?? result.moveBoss;
                if (target != null) Util.dirTar(petSkin, target);
                petSkin.logicMove(
                    petSkin.posX + petSkin.dirX * config.speed * deltaSeconds * 3f,
                    petSkin.posY + petSkin.dirY * config.speed * deltaSeconds * 3f
                );
            }

            foreach (int attackFrame in config.atkFrames ?? Array.Empty<int>()) {
                if (animationFrame != attackFrame) continue;
                UtilAck.ackMe(petSkin, config.spiltRadiusBet, config.atkBet, config.atkFindR);
                break;
            }

            if (animationFrame >= config.endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
        }

        public void update_role_state_palm(PetView petSkin) {
            int animationFrame = petSkin.animFrame;
            SheepSkill skill = SheepSkill.getById(petSkin.readySkillId);
            SheepSkillSubPalm config = SheepSkillSubPalm.getById(skill.id);

            foreach (int healFrame in config.healFrames ?? Array.Empty<int>()) {
                if (animationFrame != healFrame) continue;
                int heal = Mathf.FloorToInt((petSkin.conf.hp - petSkin.curHp) * (config.healHealthPercent / 100f));
                UtilAck.hurtByRole(petSkin, petSkin, -heal);
                break;
            }

            foreach (int attackFrame in config.atkFrames ?? Array.Empty<int>()) {
                if (animationFrame != attackFrame) continue;
                UtilAck.ackMe(petSkin, config.spiltRadiusBet, config.atkBet, config.atkFindR);
                break;
            }

            int[] hitBackFrames = config.hitBackFrames ?? Array.Empty<int>();
            int[] hitBackDistances = config.hitBackDistances ?? Array.Empty<int>();
            for (int index = 0; index < hitBackFrames.Length; index++) {
                if (animationFrame != hitBackFrames[index]) continue;
                float distance = index < hitBackDistances.Length ? hitBackDistances[index] : 0f;
                UtilAck.hitBackMe(petSkin, config.spiltRadiusBet, config.atkFindR, distance);
                break;
            }

            if (animationFrame >= config.endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
        }

        public void update_role_state_callbullets(PetView petSkin) {
            int animationFrame = petSkin.animFrame;
            SheepSkill skill = SheepSkill.getById(petSkin.readySkillId);
            SheepSkillSubCallBullets config = SheepSkillSubCallBullets.getById(skill.id);
            int callCount = 0;

            if (config.frameStep != 0) {
                if (animationFrame % config.frameStep == 0) callCount = config.frameCnt;
            }
            else {
                int[] callFrames = config.callFrames ?? Array.Empty<int>();
                int[] callCounts = config.callCnts ?? Array.Empty<int>();
                for (int index = 0; index < callFrames.Length; index++) {
                    if (animationFrame != callFrames[index]) continue;
                    callCount = index < callCounts.Length ? callCounts[index] : 0;
                    break;
                }
            }

            for (int shotIndex = 0; shotIndex < callCount; shotIndex++) {
                if (config.type == 1) {
                    float startX = petSkin.posX + config.startOffsetPos[0];
                    float startY = petSkin.posY + config.startOffsetPos[1];
                    float startZ = config.startOffsetPos[2];
                    float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                    float endX = petSkin.posX + petSkin.dirX * config.len + config.endRadius * Mathf.Cos(angle);
                    float endY = petSkin.posY + petSkin.dirY * config.len + config.endRadius * Mathf.Sin(angle);
                    bullte_creates.Add(new BullteCreate {
                        view_pet = petSkin,
                        bulletId = config.bullet,
                        info = new Info {
                            startX = startX,
                            startY = startY,
                            startZ = startZ,
                            endX = endX,
                            endY = endY,
                            endZ = 0f,
                            hasStart = true,
                            hasEnd = true
                        }
                    });
                }
                else if (config.type == 2) {
                    float startZ = config.startOffsetPos[2];
                    float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                    float x = petSkin.posX + petSkin.dirX * config.len + config.endRadius * Mathf.Cos(angle);
                    float y = petSkin.posY + petSkin.dirY * config.len + config.endRadius * Mathf.Sin(angle);
                    bullte_creates.Add(new BullteCreate {
                        view_pet = petSkin,
                        bulletId = config.bullet,
                        info = new Info {
                            startX = x,
                            startY = y,
                            startZ = startZ,
                            endX = x,
                            endY = y,
                            endZ = 0f,
                            dirX = 0f,
                            dirY = 0f,
                            dirZ = -1f,
                            hasStart = true,
                            hasEnd = true
                        }
                    });
                }
                else if (config.type == 3) {
                    bullte_creates.Add(new BullteCreate {
                        view_pet = petSkin,
                        bulletId = config.bullet,
                        info = new Info {
                            dirX = 0f,
                            dirY = 0f,
                            dirZ = -1f,
                            angle = callCount > 0 ? 360f / callCount * shotIndex : 0f
                        }
                    });
                }
                else if (config.type == 4) {
                    bullte_creates.Add(new BullteCreate {
                        view_pet = petSkin,
                        bulletId = config.bullet,
                        info = new Info { dirX = 1f, dirY = 0f, dirZ = 0f }
                    });
                }
                else {
                    bullte_creates.Add(new BullteCreate {
                        view_pet = petSkin,
                        bulletId = config.bullet
                    });
                }
            }

            if (animationFrame >= config.endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
        }

        public void update_role_state_buff(PetView petSkin) {
            SheepSkill skill = SheepSkill.getById(petSkin.readySkillId);
            SheepSkillSubBuff config = SheepSkillSubBuff.getById(skill.id);
            if (petSkin.animFrame >= config.endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
        }

        public void update_role_state_rigidity(PetView petSkin) {
            SheepSkill skill = SheepSkill.getById(petSkin.readySkillId);
            SheepSkillSubRigidity config = SheepSkillSubRigidity.getById(skill.id);
            if (petSkin.animFrame >= config.endFrame) {
                petSkin.state = SheepRoleState.SpinAtk;
                petSkin.animType = SheepRoleAnimType.Attack;
                petSkin.readySkillId = config.endSkill;
            }
        }

        public void update_role_state_spinatk(PetView petSkin, bool logicTick, float deltaSeconds) {
            (int xn, int yn) block = Util.getXnYn(petSkin.posX, petSkin.posY);
            int animationFrame = petSkin.animFrame;
            SheepSkill skill = SheepSkill.getById(petSkin.readySkillId);
            SheepSkillSubSpinAtk config = SheepSkillSubSpinAtk.getById(skill.id);

            if (animationFrame == 1) {
                PetView target = UtilFind.findSortAck1(petSkin, petSkin.conf.findR);
                if (target != null) Util.dirTar(petSkin, target);
            }

            if (logicTick) {
                bool noShieldTarget = true;
                UtilFind.forNearBlocksByAckView(
                    petSkin,
                    block.xn,
                    block.yn,
                    petSkin.conf.findR,
                    target => {
                        if (target.isDie || target.camp == petSkin.camp || target.roleId == 0) return false;
                        bool canAttack = Util.isCanAckByRole(petSkin, target);
                        if (noShieldTarget &&
                            target.conf.roleType == SheepRoleType.dun_bing &&
                            canAttack) {
                            noShieldTarget = false;
                        }

                        if (!canAttack) return false;
                        UtilAck.ackTar(petSkin, target);
                        return false;
                    }
                );
                if (noShieldTarget) Util.moveTar(petSkin, null, deltaSeconds, true);
            }

            if (animationFrame >= config.endFrame) {
                petSkin.state = (SheepRoleState)config.endState;
                petSkin.animType = SheepRoleAnimType.Boom;
                petSkin.readySkillId = config.endSkill;
            }
        }

        public void update_role_state(PetView petSkin, bool logicTick, float deltaSeconds = 0.033f) {
            Util.subAtkCd(petSkin, deltaSeconds);

            switch (petSkin.state) {
                case SheepRoleState.Start:
                    if (logicTick) update_role_state_start(petSkin, true, deltaSeconds);
                    break;
                case SheepRoleState.In:
                    update_role_state_in(petSkin);
                    break;
                case SheepRoleState.Spurt:
                    if (logicTick) update_role_state_spurt(petSkin, true, deltaSeconds);
                    break;
                case SheepRoleState.Charge:
                    if (logicTick) update_role_state_charge(petSkin, true, deltaSeconds);
                    break;
                case SheepRoleState.ChargePlus:
                    if (logicTick) update_role_state_charge_plus(petSkin, true, deltaSeconds);
                    break;
                case SheepRoleState.SpinSpurt:
                    if (logicTick) update_role_state_spinspurt(petSkin, true, deltaSeconds);
                    break;
                case SheepRoleState.Move:
                    if (logicTick) update_role_state_move(petSkin, true, deltaSeconds);
                    break;
                case SheepRoleState.Attack:
                    update_role_state_attack(petSkin, logicTick, deltaSeconds);
                    break;
                case SheepRoleState.Killer:
                    update_role_state_killer(petSkin);
                    break;
                case SheepRoleState.Boom:
                    update_role_state_boom(petSkin);
                    break;
                case SheepRoleState.Invincible:
                    update_role_state_invincible(petSkin);
                    break;
                case SheepRoleState.Bladestorm:
                    update_role_state_bladestorm(petSkin, logicTick, deltaSeconds);
                    break;
                case SheepRoleState.Palm:
                    update_role_state_palm(petSkin);
                    break;
                case SheepRoleState.CallBullets:
                    update_role_state_callbullets(petSkin);
                    break;
                case SheepRoleState.Buff:
                    update_role_state_buff(petSkin);
                    break;
                case SheepRoleState.Rigidity:
                    update_role_state_rigidity(petSkin);
                    break;
                case SheepRoleState.SpinAtk:
                    update_role_state_spinatk(petSkin, logicTick, deltaSeconds);
                    break;
            }

            if (Mathf.Abs(petSkin.impulseX) > Mathf.Epsilon ||
                Mathf.Abs(petSkin.impulseY) > Mathf.Epsilon) {
                if (!petSkin.isDie && petSkin.curHp > 0f) {
                    petSkin.logicMove(
                        petSkin.animX + petSkin.impulseX,
                        petSkin.posY + petSkin.impulseY
                    );
                }

                petSkin.impulseX = 0f;
                petSkin.impulseY = 0f;
            }
        }

        public void update_role_state_start(PetView petSkin, bool logicTick, float deltaSeconds) {
            if (state == SheepRoomState.Start) {
                if (!logicTick) return;

                float distance = Util.dis(
                    petSkin.posX,
                    petSkin.posY,
                    petSkin.tarPosX,
                    petSkin.tarPosY
                );
                float speed = 3f * petSkin.conf.runSpeed;
                if (distance > speed * deltaSeconds) {
                    float[] direction = Util.dirTarByPos(petSkin, petSkin.tarPosX, petSkin.tarPosY);
                    petSkin.logicMove(
                        petSkin.posX + direction[0] * speed * deltaSeconds,
                        petSkin.posY + direction[1] * speed * deltaSeconds
                    );
                }
                else {
                    petSkin.logicMove(petSkin.tarPosX, petSkin.tarPosY);
                }

                return;
            }

            if (petSkin.conf.skillSpurt != 0) {
                SheepSkill skill = SheepSkill.getById(petSkin.conf.skillSpurt);
                if (skill.skillType == SheepSkillType.Charge) {
                    petSkin.state = SheepRoleState.Charge;
                    petSkin.subState = SheepRoleSubState.Spurt;
                    petSkin.animType = SheepRoleAnimType.Spurt;
                }
                else if (skill.skillType == SheepSkillType.SpinSpurt) {
                    petSkin.state = SheepRoleState.SpinSpurt;
                    petSkin.animType = SheepRoleAnimType.Attack;
                }
                else {
                    petSkin.state = SheepRoleState.Spurt;
                    petSkin.subState = SheepRoleSubState.Spurt;
                    petSkin.animType = petSkin.conf.isSpurtAnim
                        ? SheepRoleAnimType.Spurt
                        : SheepRoleAnimType.Idle;
                }
            }
            else {
                petSkin.state = SheepRoleState.Spurt;
                petSkin.subState = SheepRoleSubState.Spurt;
                petSkin.animType = petSkin.conf.isSpurtAnim
                    ? SheepRoleAnimType.Spurt
                    : SheepRoleAnimType.Idle;
            }
        }

        public void update_role_state_charge(PetView petSkin, bool logicTick, float deltaSeconds) {
            (int xn, int yn) block = Util.getXnYn(petSkin.posX, petSkin.posY);
            bool crossedEnd = petSkin.camp == SheepCamp.Red
                ? petSkin.posX > petSkin.conf.runEndX
                : petSkin.posX < -petSkin.conf.runEndX;

            if (crossedEnd) {
                bool hasBlockingEnemy = false;
                UtilFind.findNearBlocksByAckView(petSkin, block.xn, block.yn, 5, target => {
                    if (!target.isDie && target.camp != petSkin.camp && target.roleId != 0) {
                        hasBlockingEnemy = true;
                        return true;
                    }

                    return false;
                });

                if (hasBlockingEnemy) {
                    petSkin.state = SheepRoleState.Boom;
                    petSkin.subState = SheepRoleSubState.Boom;
                    SheepSkillSubCharge charge = SheepSkillSubCharge.getById(petSkin.conf.skillSpurt);
                    SheepSkillSubBoom boom = SheepSkillSubBoom.getById(charge.endSkill);
                    petSkin.animType = boom.isAnim != 0 ? SheepRoleAnimType.Boom : SheepRoleAnimType.Idle;
                    petSkin.readySkillId = boom.id;
                }
                else {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                }

                return;
            }

            bool hitHeavyTarget = false;
            UtilFind.findNearBlocksByAckView(petSkin, block.xn, block.yn, 5, target => {
                if (target.isDie || target.camp == petSkin.camp || target.roleId == 0 ||
                    !Util.isCanAckByRole(petSkin, target)) {
                    return false;
                }

                if (target.conf.roleType == SheepRoleType.xiao_bing) {
                    UtilAck.ackTar(petSkin, target);
                }
                else {
                    hitHeavyTarget = true;
                }

                return false;
            });

            if (hitHeavyTarget) {
                petSkin.state = SheepRoleState.Boom;
                petSkin.subState = SheepRoleSubState.Boom;
                SheepSkillSubCharge charge = SheepSkillSubCharge.getById(petSkin.conf.skillSpurt);
                SheepSkillSubBoom boom = SheepSkillSubBoom.getById(charge.endSkill);
                petSkin.animType = boom.isAnim != 0 ? SheepRoleAnimType.Boom : SheepRoleAnimType.Idle;
                petSkin.readySkillId = boom.id;
                return;
            }

            PetView archerTarget = null;
            UtilFind.findNearBlocksByAckView(petSkin, block.xn, block.yn, petSkin.conf.findR, target => {
                if (target.isDie || target.camp == petSkin.camp || target.roleId == 0) return false;
                if (target.conf.roleType != SheepRoleType.gong_jian_shou) return false;
                if (!Util.isCanAckByRole(petSkin, target)) return false;
                archerTarget = target;
                return true;
            });
            Util.moveTar(petSkin, archerTarget, deltaSeconds, logicTick);
        }

        public void update_role_state_charge_plus(PetView petSkin, bool logicTick, float deltaSeconds) {
            (int xn, int yn) block = Util.getXnYn(petSkin.posX, petSkin.posY);
            bool crossedEnd = petSkin.camp == SheepCamp.Red
                ? petSkin.posX > petSkin.conf.runEndX
                : petSkin.posX < -petSkin.conf.runEndX;

            if (crossedEnd) {
                petSkin.state = SheepRoleState.Boom;
                petSkin.subState = SheepRoleSubState.Boom;
                SheepSkillSubChargePlus charge = SheepSkillSubChargePlus.getById(petSkin.conf.skillSpurt);
                SheepSkillSubBoom boom = SheepSkillSubBoom.getById(charge.endSkill);
                petSkin.animType = SheepRoleAnimType.Boom;
                petSkin.readySkillId = boom.id;
                return;
            }

            UtilFind.findNearBlocksByAckView(petSkin, block.xn, block.yn, 5, target => {
                if (target.isDie || target.camp == petSkin.camp || target.roleId == 0 ||
                    !Util.isCanAckByRole(petSkin, target)) {
                    return false;
                }

                if (target.curHp < SheepConfig.beheadLine) {
                    target.isDie = true;
                    target.state = SheepRoleState.Dead;
                }
                else {
                    SheepRoleTypeInfo config = petSkin.conf;
                    UtilAck.ackMe(
                        petSkin,
                        config.collideR,
                        0f,
                        config.findR,
                        config.hitBackDistance
                    );
                }

                return false;
            });
            Util.moveTar(petSkin, null, deltaSeconds, logicTick);
        }

        public void update_role_state_spinspurt(PetView petSkin, bool logicTick, float deltaSeconds) {
            (int xn, int yn) block = Util.getXnYn(petSkin.posX, petSkin.posY);
            bool crossedEnd = petSkin.camp == SheepCamp.Red
                ? petSkin.posX > petSkin.conf.runEndX
                : petSkin.posX < -petSkin.conf.runEndX;

            if (crossedEnd) {
                petSkin.state = SheepRoleState.Boom;
                petSkin.subState = SheepRoleSubState.Boom;
                SheepSkillSubSpinSpurt spinSpurt = SheepSkillSubSpinSpurt.getById(petSkin.conf.skillSpurt);
                SheepSkillSubBoom boom = SheepSkillSubBoom.getById(spinSpurt.endSkill);
                petSkin.animType = boom.isAnim != 0 ? SheepRoleAnimType.Boom : SheepRoleAnimType.Idle;
                petSkin.readySkillId = boom.id;
                return;
            }

            Util.moveTar(petSkin, null, deltaSeconds, logicTick);
            UtilFind.forNearBlocksByAckView(
                petSkin,
                block.xn,
                block.yn,
                petSkin.conf.findR,
                target => {
                    if (target.isDie || target.camp == petSkin.camp || target.roleId == 0 ||
                        !Util.isCanAckByRole(petSkin, target)) {
                        return false;
                    }

                    UtilAck.ackTar(petSkin, target);
                    return false;
                }
            );
        }

        public void update_role_state_spurt(PetView petSkin, bool logicTick, float deltaSeconds) {
            if (petSkin.conf.skillSpurt != 0) {
                SheepSkill skill = SheepSkill.getById(petSkin.conf.skillSpurt);
                FindTarResult result;

                if (skill.skillType == SheepSkillType.Boom) {
                    SheepSkillSubBoom boom = SheepSkillSubBoom.getById(skill.id);
                    result = UtilFind.findTar(petSkin);
                    if (result.atkTar != null || result.moveBoss != null) {
                        petSkin.state = SheepRoleState.Boom;
                        petSkin.subState = SheepRoleSubState.Boom;
                        petSkin.animType = boom.isAnim != 0 ? SheepRoleAnimType.Boom : SheepRoleAnimType.Idle;
                        petSkin.readySkillId = boom.id;
                        return;
                    }

                    Util.moveTar(petSkin, null, deltaSeconds, logicTick);
                    return;
                }

                if (skill.skillType == SheepSkillType.Killer) {
                    SheepSkillSubKiller killer = SheepSkillSubKiller.getById(skill.id);
                    result = UtilFind.findTar(petSkin);
                    if (result.atkTar != null) {
                        petSkin.state = SheepRoleState.Killer;
                        petSkin.subState = SheepRoleSubState.KillerStart;
                        petSkin.animType = SheepRoleAnimType.Killer;
                        petSkin.readySkillId = killer.id;
                        return;
                    }

                    if (result.moveBoss != null) {
                        petSkin.state = SheepRoleState.Move;
                        petSkin.subState = SheepRoleSubState.MoveBoss;
                        petSkin.animType = SheepRoleAnimType.Idle;
                        Util.moveTar(petSkin, result.moveBoss, deltaSeconds, logicTick);
                        return;
                    }

                    Util.moveTar(petSkin, null, deltaSeconds, logicTick);
                    return;
                }

                if (skill.skillType == SheepSkillType.Bullet) {
                    SheepSkillSubBullet bullet = SheepSkillSubBullet.getById(skill.id);
                    result = UtilFind.findTar(petSkin);
                    if (result.atkTar != null || result.moveTar != null || result.moveBoss != null) {
                        bullte_creates.Add(new BullteCreate {
                            view_pet = petSkin,
                            bulletId = bullet.bullet
                        });
                    }

                    if (result.atkTar != null) {
                        petSkin.state = SheepRoleState.Attack;
                        petSkin.subState = SheepRoleSubState.AttackAwait;
                        return;
                    }

                    if (result.moveTar != null) {
                        petSkin.state = SheepRoleState.Move;
                        petSkin.subState = SheepRoleSubState.MoveTar;
                        Util.moveTar(petSkin, result.moveTar, deltaSeconds, logicTick);
                        return;
                    }

                    if (result.moveBoss != null) {
                        petSkin.state = SheepRoleState.Move;
                        petSkin.subState = SheepRoleSubState.MoveBoss;
                        petSkin.animType = SheepRoleAnimType.Idle;
                        Util.moveTar(petSkin, result.moveBoss, deltaSeconds, logicTick);
                        return;
                    }

                    Util.moveTar(petSkin, null, deltaSeconds, logicTick);
                    return;
                }

                if (skill.skillType == SheepSkillType.CallBullets) {
                    SheepSkillSubCallBullets callBullets = SheepSkillSubCallBullets.getById(skill.id);
                    result = UtilFind.findTar(petSkin);
                    if (result.atkTar != null || result.moveBoss != null) {
                        petSkin.state = SheepRoleState.CallBullets;
                        petSkin.subState = SheepRoleSubState.CallBullets;
                        petSkin.animType = callBullets.isAnim != 0
                            ? SheepRoleAnimType.CallBullets
                            : SheepRoleAnimType.Idle;
                        petSkin.readySkillId = callBullets.id;
                        return;
                    }

                    Util.moveTar(petSkin, null, deltaSeconds, logicTick);
                    return;
                }
            }

            FindTarResult normalResult = UtilFind.findTar(petSkin);
            if (normalResult.atkTar != null) {
                petSkin.state = SheepRoleState.Attack;
                petSkin.subState = SheepRoleSubState.AttackAwait;
                return;
            }

            if (normalResult.moveTar != null) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveTar;
                Util.moveTar(petSkin, normalResult.moveTar, deltaSeconds, logicTick);
                return;
            }

            if (normalResult.moveBoss != null) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                Util.moveTar(petSkin, normalResult.moveBoss, deltaSeconds, logicTick);
                return;
            }

            Util.moveTar(petSkin, null, deltaSeconds, logicTick);
        }

        public void update_role_anim(PetView pet) {
            pet.animFrame++;
        }

        public void produce_pets(int typeId, int count, SheepCamp camp) {
            if (count <= 0) return;
            Dictionary<int, SheepCallInfo> callInfos = camp == SheepCamp.Red ? redCallInfos : blueCallInfos;

            if (!callInfos.TryGetValue(typeId, out SheepCallInfo callInfo)) {
                callInfo = new SheepCallInfo {
                    camp = camp,
                    type = typeId,
                    count = 0,
                    frame = 0,
                    count_line = 0,
                    items = Array.Empty<int>(),
                    pets = new List<SheepCallInfoPet>()
                };
                callInfos.Add(typeId, callInfo);
            }

            callInfo.count += count;
            callInfo.pets.Add(new SheepCallInfoPet {
                camp = camp,
                count = count
            });
        }

        public void consume(SheepCtl sheepCtl, float deltaMs) {
            autoTime += deltaMs / 1000f;
            if (isAutoCall && autoTime > SheepConfig.systemAutomaticTroopsIntervalTime) {
                autoTime = 0f;
                if (pets[0].Count + pets[1].Count < SheepConfig.systemLongerAutomaticallyDispatch) {
                    for (int campIndex = 0; campIndex < 2; campIndex++) {
                        if (pets[campIndex].Count < SheepConfig.systemAutomaticallyMaxTroops) {
                            produce_pets(
                                SheepConfig.WarmUpID,
                                SheepConfig.systemAutomaticallyTroopsOneNumber,
                                (SheepCamp)campIndex
                            );
                        }
                    }
                }
            }

            ConsumeCallMap(redCallInfos, SheepCamp.Red, sheepCtl);
            ConsumeCallMap(blueCallInfos, SheepCamp.Blue, sheepCtl);
        }

        private void ConsumeCallMap(
            Dictionary<int, SheepCallInfo> callInfos,
            SheepCamp camp,
            SheepCtl sheepCtl
        ) {
            List<int> removeKeys = new List<int>();
            List<KeyValuePair<int, SheepCallInfo>> snapshot =
                new List<KeyValuePair<int, SheepCallInfo>>(callInfos);

            foreach (KeyValuePair<int, SheepCallInfo> pair in snapshot) {
                int roleId = pair.Key;
                SheepCallInfo callInfo = pair.Value;
                if (callInfo == null || callInfo.count <= 0) {
                    removeKeys.Add(roleId);
                    continue;
                }

                SheepRoleTypeInfo roleConfig = SheepRoleTypeInfo.getById(roleId);
                SheepRoleFormation formation = SheepRoleFormation.getById(roleConfig.formationId);
                int roleTypeIndex = RoleTypeIndex(roleConfig.roleType);
                EnsurePerfCapacity(roleTypeIndex);
                int existingCount = camp == SheepCamp.Red
                    ? perfStat.redNums[roleTypeIndex]
                    : perfStat.blueNums[roleTypeIndex];

                if (roleConfig.roleType == SheepRoleType.xiao_bing && existingCount > 14500) continue;
                if (roleConfig.roleType == SheepRoleType.ci_ke && existingCount > 9500) continue;

                callInfo.frame++;
                if (callInfo.frame <= formation.frameItemX) continue;
                callInfo.frame = 0;

                if (formation.formationType == SheepRoleFormationType.RectangleTidy) {
                    ConsumeRectangleTidy(callInfo, roleId, camp, formation, sheepCtl);
                }
                else if (formation.formationType == SheepRoleFormationType.AngleTidy) {
                    ConsumeAngleTidy(callInfo, roleId, camp, formation, sheepCtl);
                }
                else {
                    // Random 编队的位置由 createPetView 计算。每个生成帧按 frameMaxCount 释放。
                    int count = Math.Max(1, formation.frameMaxCount);
                    for (int index = 0; index < count && callInfo.pets.Count > 0; index++) {
                        SheepCallInfoPet source = callInfo.pets[0];
                        source.count--;
                        callInfo.count--;
                        createPetView(
                            sheepCtl,
                            source.player ?? source.camp,
                            roleId,
                            Math.Max(1, callInfo.count + 1),
                            1,
                            true,
                            null,
                            source.booms != null && source.booms.Count > 0 && source.booms.Pop()
                        );
                        if (source.count <= 0) callInfo.pets.RemoveAt(0);
                    }
                }

                if (callInfo.count <= 0 || callInfo.pets.Count == 0) {
                    removeKeys.Add(roleId);
                }
            }

            foreach (int key in removeKeys) callInfos.Remove(key);
        }

        private void ConsumeRectangleTidy(
            SheepCallInfo callInfo,
            int roleId,
            SheepCamp camp,
            SheepRoleFormation formation,
            SheepCtl sheepCtl
        ) {
            int itemNumY = Math.Max(1, formation.itemNumY);
            float itemY = formation.itemY;
            int gapEvery = Math.Max(1, formation.itemYGapNum);
            float gap = formation.itemYGap;
            float startX = formation.startX + sheepMode.startAddX;
            float x = camp == SheepCamp.Red ? -startX : startX;
            int lineIndex = 0;

            while (callInfo.pets.Count > 0 && lineIndex < itemNumY) {
                int halfIndex = Mathf.FloorToInt(lineIndex / 2f);
                float y;
                if (itemNumY % 2 == 0) {
                    y = lineIndex % 2 == 0
                        ? itemY * halfIndex + itemY / 2f + Mathf.FloorToInt(halfIndex / (float)gapEvery + 1f) * gap
                        : -itemY * halfIndex - itemY / 2f - Mathf.FloorToInt(halfIndex / (float)gapEvery + 1f) * gap;
                }
                else {
                    y = lineIndex % 2 == 0
                        ? itemY * halfIndex + Mathf.FloorToInt(halfIndex / (float)gapEvery) * gap
                        : -itemY * (halfIndex + 1) - Mathf.FloorToInt(halfIndex / (float)gapEvery) * gap;
                }

                SheepCallInfoPet source = callInfo.pets[0];
                source.count--;
                callInfo.count--;
                lineIndex++;

                bool isBoom = source.booms != null && source.booms.Count > 0 && source.booms.Pop();
                createPetView(
                    sheepCtl,
                    source.player ?? source.camp,
                    roleId,
                    itemNumY,
                    1,
                    true,
                    new Vector3(x, y, 0f),
                    isBoom
                );

                if (source.count <= 0) callInfo.pets.RemoveAt(0);
            }

            callInfo.count_line++;
            if (callInfo.count_line >= Math.Max(1, formation.itemNumX)) {
                callInfo.frame -= formation.itemYGapFrame;
                callInfo.count_line = 0;
            }
        }

        private void ConsumeAngleTidy(
            SheepCallInfo callInfo,
            int roleId,
            SheepCamp camp,
            SheepRoleFormation formation,
            SheepCtl sheepCtl
        ) {
            int angleSpan = 2 * formation.maxAngle - formation.minAngle;
            int groupCount = Math.Max(1, Mathf.FloorToInt(angleSpan / (float)Math.Max(1, formation.startStepAngle)));
            int maxPerFrame = groupCount;
            int spawned = 0;

            while (callInfo.pets.Count > 0 && spawned < maxPerFrame) {
                SheepCallInfoPet source = callInfo.pets[0];
                int sourceCount = source.count;

                for (int index = 0; index < sourceCount && spawned < maxPerFrame; index++) {
                    source.count--;
                    callInfo.count--;
                    spawned++;

                    int ring = Mathf.FloorToInt(spawned / (float)groupCount);
                    float radius = formation.startR + sheepMode.startAddR + formation.startStepR * ring;
                    float angleDegrees = (spawned % 2 == 0 ? 1f : -1f) *
                                         (Mathf.FloorToInt((spawned % groupCount) / 2f) * formation.startStepAngle +
                                          formation.minAngle);
                    float angleRadians = angleDegrees * Mathf.Deg2Rad;
                    float offsetX = Mathf.Cos(angleRadians) * radius;
                    float offsetY = Mathf.Sin(angleRadians) * radius;
                    float bossX = sheepMode.loongX;
                    Vector3 position = camp == SheepCamp.Red
                        ? new Vector3(bossX - offsetX, offsetY, 0f)
                        : new Vector3(offsetX - bossX, offsetY, 0f);

                    bool isBoom = source.booms != null && source.booms.Count > 0 && source.booms.Pop();
                    createPetView(
                        sheepCtl,
                        source.player ?? source.camp,
                        roleId,
                        groupCount,
                        1,
                        true,
                        position,
                        isBoom
                    );
                }

                if (source.count <= 0) callInfo.pets.RemoveAt(0);
            }
        }

        public Vector3 getPetStartEndPos(int roleId, SheepCamp camp) {
            Dictionary<int, int> counts = petStartCounts[CampIndex(camp)];
            counts.TryGetValue(roleId, out int count);
            counts[roleId] = count + 1;

            SheepRoleTypeInfo roleConfig = SheepRoleTypeInfo.getById(roleId);
            SheepRoleFormation formation = SheepRoleFormation.getById(roleConfig.formationId);
            int itemNumY = Math.Max(1, formation.preItemNumY);
            float x = formation.preStartX + Mathf.FloorToInt(count / (float)itemNumY) * formation.preItemX;
            int itemIndex = count % itemNumY;
            float y;

            if (itemNumY % 2 == 0) {
                y = itemIndex % 2 == 0
                    ? formation.preItemY * Mathf.FloorToInt(itemIndex / 2f) + formation.preItemY / 2f
                    : -formation.preItemY * Mathf.FloorToInt(itemIndex / 2f) - formation.preItemY / 2f;
            }
            else {
                y = itemIndex % 2 == 0
                    ? formation.preItemY * Mathf.FloorToInt(itemIndex / 2f)
                    : -formation.preItemY * Mathf.FloorToInt(itemIndex / 2f + 1f);
            }

            if (camp == SheepCamp.Red) x *= -1f;
            return new Vector3(x, y + UnityEngine.Random.Range(-1f, 1f), 0f);
        }

        public void clearCallPets() {
            redCallInfos.Clear();
            blueCallInfos.Clear();
        }

        public void clearBlocks() {
            Array.Clear(attackViews[0], 0, attackViews[0].Length);
            Array.Clear(attackViews[1], 0, attackViews[1].Length);
            Array.Clear(attackView1s[0], 0, attackView1s[0].Length);
            Array.Clear(attackView1s[1], 0, attackView1s[1].Length);

            for (int group = 0; group < SheepConfig.MaxGroupCount; group++) {
                Array.Clear(collisionViews[0][group], 0, collisionViews[0][group].Length);
                Array.Clear(collisionViews[1][group], 0, collisionViews[1][group].Length);
                Array.Clear(collisionView1s[0][group], 0, collisionView1s[0][group].Length);
                Array.Clear(collisionView1s[1][group], 0, collisionView1s[1][group].Length);
            }

            pre_blocks.Clear();
        }

        public IndexLen getBlockByIndex(IndexLen[] blocks, int blockIndex) {
            if (blocks == null) throw new ArgumentNullException(nameof(blocks));
            if (blockIndex < 0 || blockIndex >= blocks.Length) {
                throw new ArgumentOutOfRangeException(nameof(blockIndex), blockIndex, "blockIndex 越界");
            }

            if (blocks[blockIndex] == null) blocks[blockIndex] = new IndexLen();
            return blocks[blockIndex];
        }

        public void mainClearBlocks() {
            if (isChangeAckFlags == null) isChangeAckFlags = new[] { true, true };
            if (isChangeCollsionFlags == null) {
                isChangeCollsionFlags = new bool[2][];
                isChangeCollsionFlags[0] = new bool[SheepConfig.MaxGroupCount];
                isChangeCollsionFlags[1] = new bool[SheepConfig.MaxGroupCount];
                for (int group = 0; group < SheepConfig.MaxGroupCount; group++) {
                    isChangeCollsionFlags[0][group] = true;
                    isChangeCollsionFlags[1][group] = true;
                }
            }

            for (int camp = 0; camp < 2; camp++) {
                if (isChangeAckFlags[camp]) Array.Clear(attackViews[camp], 0, attackViews[camp].Length);
                isChangeAckFlags[camp] = false;
                for (int group = 0; group < SheepConfig.MaxGroupCount; group++) {
                    if (isChangeCollsionFlags[camp][group]) {
                        Array.Clear(collisionViews[camp][group], 0, collisionViews[camp][group].Length);
                    }

                    isChangeCollsionFlags[camp][group] = false;
                }
            }

            pre_blocks.Clear();
        }

        public void mainPreAddBlock(int blockIndex, int buffIndex, SheepCamp camp, int collideId) {
            if (blockIndex < 0 || blockIndex >= MaxCount) return;
            int campIndex = CampIndex(camp);
            int safeCollideId = Mathf.Clamp(collideId, 0, SheepConfig.MaxGroupCount - 1);

            if (!pre_blocks.TryGetValue(blockIndex, out Dictionary<int, Dictionary<int, List<int>>> camps)) {
                camps = new Dictionary<int, Dictionary<int, List<int>>>();
                pre_blocks.Add(blockIndex, camps);
            }

            if (!camps.TryGetValue(campIndex, out Dictionary<int, List<int>> groups)) {
                groups = new Dictionary<int, List<int>>();
                camps.Add(campIndex, groups);
            }

            if (!groups.TryGetValue(safeCollideId, out List<int> petIndices)) {
                petIndices = new List<int>();
                groups.Add(safeCollideId, petIndices);
            }

            petIndices.Add(buffIndex);

            if (isChangeAckFlags != null && !isChangeAckFlags[campIndex]) {
                isChangeAckFlags[campIndex] = true;
            }

            if (isChangeCollsionFlags != null && !isChangeCollsionFlags[campIndex][safeCollideId]) {
                isChangeCollsionFlags[campIndex][safeCollideId] = true;
            }
        }

        public void mainSyncBlocksToWokers() {
            int[] attackOffsets = new int[2];
            int[][] collisionOffsets = {
                new int[SheepConfig.MaxGroupCount],
                new int[SheepConfig.MaxGroupCount]
            };

            foreach (KeyValuePair<int, Dictionary<int, Dictionary<int, List<int>>>> blockPair in pre_blocks) {
                int blockIndex = blockPair.Key;
                foreach (KeyValuePair<int, Dictionary<int, List<int>>> campPair in blockPair.Value) {
                    int camp = campPair.Key;
                    int totalInBlock = 0;

                    foreach (KeyValuePair<int, List<int>> groupPair in campPair.Value) {
                        int group = groupPair.Key;
                        List<int> indices = groupPair.Value;
                        if (indices == null || indices.Count == 0) continue;

                        int offset = collisionOffsets[camp][group];
                        int writable = Math.Min(indices.Count, collisionView1s[camp][group].Length - offset);
                        if (writable <= 0) continue;

                        IndexLen collisionBlock = getBlockByIndex(collisionViews[camp][group], blockIndex);
                        collisionBlock.Index = offset;
                        collisionBlock.Len = writable;
                        for (int index = 0; index < writable; index++) {
                            collisionView1s[camp][group][offset + index] = indices[index];
                        }

                        collisionOffsets[camp][group] += writable;
                        totalInBlock += writable;
                    }

                    if (totalInBlock == 0) continue;
                    int attackOffset = attackOffsets[camp];
                    int maxWritable = attackView1s[camp].Length - attackOffset;
                    int written = 0;
                    foreach (KeyValuePair<int, List<int>> groupPair in campPair.Value) {
                        List<int> indices = groupPair.Value;
                        if (indices == null) continue;
                        foreach (int petIndex in indices) {
                            if (written >= maxWritable) break;
                            attackView1s[camp][attackOffset + written] = petIndex;
                            written++;
                        }

                        if (written >= maxWritable) break;
                    }

                    IndexLen attackBlock = getBlockByIndex(attackViews[camp], blockIndex);
                    attackBlock.Index = attackOffset;
                    attackBlock.Len = written;
                    attackOffsets[camp] += written;
                }
            }
        }

        public void forEachBlock(IndexLen[] blocks, int[] indices, int blockIndex, Action<int> callback) {
            IndexLen block = getBlockByIndex(blocks, blockIndex);
            for (int index = 0; index < block.Len; index++) {
                callback(indices[block.Index + index]);
            }
        }

        public bool findBlock(IndexLen[] blocks, int[] indices, int blockIndex, Func<int, bool> callback) {
            IndexLen block = getBlockByIndex(blocks, blockIndex);
            if (block.Len == 0) return false;

            for (int index = 0; index < block.Len; index++) {
                int secondLevelIndex = block.Index + index;
                if (secondLevelIndex < 0 || secondLevelIndex >= indices.Length) {
                    throw new IndexOutOfRangeException("二级空间索引越界");
                }

                if (callback(indices[secondLevelIndex])) return true;
            }

            return false;
        }

        public static PetView createPetView(
            SheepCtl sheepCtl,
            SheepCamp camp,
            int roleType,
            int formationCount = 1,
            int unused = 0,
            bool addToWorld = true,
            Vector3? position = null,
            bool isBoom = false
        ) {
            SheepMgr manager = sheepMgr;
            if (manager == null ||
                (manager.state != SheepRoomState.Run && manager.state != SheepRoomState.Start)) {
                return null;
            }

            SheepRoleTypeInfo roleConfig = SheepRoleTypeInfo.getById(roleType);
            PetView petSource = new PetView(-1) {
                uids = new List<int>(),
                conf = roleConfig,
                camp = camp,
                petId = roleType,
                isDie = false,
                scale = roleConfig.scale,
                isBoom = isBoom,
                buff_index = -1,
                view_pet = null,
                attacher = new BuffTimeAttacher(),
                skinId = roleConfig.animId
            };

            SheepRoleFormation formation = SheepRoleFormation.getById(roleConfig.formationId);
            Vector3 spawnPosition;
            if (formation.formationType == SheepRoleFormationType.AngleRandom) {
                float density = Math.Max(1, formation.angleDensity);
                float maxAngle = Mathf.Min(
                    (formationCount / density + formation.baseTimes) * formation.startAngle,
                    formation.maxAngle
                );
                float angle = UnityEngine.Random.Range(-maxAngle, maxAngle);
                angle += angle > 0f ? formation.minAngle : -formation.minAngle;
                float radius = formation.startR + sheepMode.startAddR;
                float offsetX = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
                float offsetY = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
                spawnPosition = camp == SheepCamp.Red
                    ? new Vector3(sheepMode.loongX - offsetX, offsetY, 0f)
                    : new Vector3(offsetX - sheepMode.loongX, offsetY, 0f);
            }
            else if (formation.formationType == SheepRoleFormationType.RectangleRandom) {
                float density = Math.Max(1, formation.density);
                float maxScope = Mathf.Min(
                    (formationCount / density + formation.baseTimes) * formation.startScope,
                    formation.maxScope
                );
                float y = UnityEngine.Random.Range(-maxScope, maxScope);
                if (formation.minScope != 0) y += y > 0f ? formation.minScope : -formation.minScope;
                float x = formation.startX + sheepMode.startAddX;
                x = camp == SheepCamp.Red ? -Mathf.Abs(x) : Mathf.Abs(x);
                spawnPosition = new Vector3(x, y, 0f);
            }
            else {
                spawnPosition = position ?? new Vector3(
                    camp == SheepCamp.Red
                        ? -(formation.startX + sheepMode.startAddX)
                        : formation.startX + sheepMode.startAddX,
                    0f,
                    0f
                );
            }

            petSource.position = spawnPosition;
            petSource.pos = spawnPosition;
            if (addToWorld) manager.addPet(petSource, camp);
            manager.pre_add_pet(petSource);
            return petSource;
        }
    }
}