using System;
using System.Collections.Generic;
using System.Linq;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static rvb.scripts.BullteCreate;
using static rvb.scripts.SheepModes;
using static rvb.scripts.EventBus;
using Random = UnityEngine.Random;

namespace rvb.scripts {
    /// <summary>
    /// 羊了个羊战斗逻辑管理器。
    ///
    /// 说明：原 Cocos 版本把逻辑、渲染和 SheepCtl 组件混在一起。
    /// 本版本保留战斗逻辑，渲染相关调用均为可选 dynamic 桥接或回调，
    /// 因而可以接入你自己的 SheepCtl/渲染实现。
    /// </summary>
    public class SheepMgr {
        
        public static SheepMgr inc;

        // 是否自动出兵
        public bool isAutoCall = true;

        // 自动出兵计时器
        public float autoTime = 0f;

        // 游戏模式 (外部会设置)
        public int gameMode = 0;

        // 时间模式 (外部会设置)
        public int timeMode = 2;

        // boss 血量 (外部会设置)
        public int loongHp = 10000;

        // 红蓝 boss
        public Boss[] boss = { null, null };

        // 地块比例
        public float plotRatio = 0.5f;

        // 核心状态机
        public SheepRoomState state = SheepRoomState.Ready;

        // 尝试角色 (todo 但是是哪一种? 当前在场上的? )
        public HashSet<PetView>[] pets = new[] { new HashSet<PetView>(), new HashSet<PetView>() };

        public int gameIndex = 0;
        public float gameStartTimerForBuff = 0;
        public Vector3 cameraEulerAngles = Vector3.zero;
        public long endTime = 0;
        public List<int>[] preBuffs = new[] { new List<int>(), new List<int>() };
        public List<Buff>[] buffs = new[] { new List<Buff>(), new List<Buff>() };
        public int[] countNewBuffs = new[] { 0, 0 };
        public int[] countBuffs = new[] { 0, 0 };
        public int[] countShowBuffs = new[] { 0, 0 };

        // 反击时刻标识符 (防止多次触发反击时刻)
        public bool[] flagLongBuffs = new[] { false, false };

        public Dictionary<int, int>[] petStartCounts = { new Dictionary<int, int>(), new Dictionary<int, int>() };
        public List<PetView> god_view_pets = new List<PetView>();

        public PerfStat perfStat = new PerfStat() {
            redNums = new[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            blueNums = new[] { 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public PetView[] view_pets = new PetView[] { };
        public BulletView[] view_bullets = new BulletView[] { };
        public BulletView[] pre_view_bullets = new BulletView[] { };
        public long updateTime = 0;
        public List<PetView> petsAdd = new List<PetView>();
        public Stack<int> petsDel = new Stack<int>();
        public int petCount = 0;
        public Stack<int> bulletsDel = new Stack<int>();
        public int bulletCount = 0;
        public int bulletId = 0;

        public int[] logic_counts = { 1, 1 };

        public List<BullteCreate> bullte_creates = new List<BullteCreate>();

        public Dictionary<int, Dictionary<int, Dictionary<int, List<int>>>> pre_blocks =
            new Dictionary<int, Dictionary<int, Dictionary<int, List<int>>>>();

        public bool[][] isChangeCollsionFlags = null;
        public bool[] isChangeAckFlags = null;

        public int MaxCount = SheepConfig.line_w * SheepConfig.line_w;

        public IndexLen[][] attackViews;

        public int[][] attackView1s = new int[][] {
            new int[SheepConfig.MaxPetCount],
            new int[SheepConfig.MaxPetCount]
        };

        public IndexLen[][][] collisionViews = new IndexLen[][][] { };

        public int[][][] collisionView1s = new int[][][] { };

        public Dictionary<int, SheepCallInfo> redCallInfos = new Dictionary<int, SheepCallInfo>();

        public Dictionary<int, SheepCallInfo> blueCallInfos = new Dictionary<int, SheepCallInfo>();

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

        public int petId = 0;

        //********************************** 以下字段 待处理 **********************************************

        /// <summary>逻辑侧 Boss 当前已结算生命。</summary>
        public float[] bossHp={0,0};

        public long[] bossBackStateTime={0,0};

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


        public int plotRatioIndex;

        public SheepMgr(SheepCtl sheepCtl) {
            // 是否自动出兵
            this.isAutoCall = true;

            // 自动出兵计时器
            this.autoTime = 0;

            // 游戏模式 (外部会设置)
            this.gameMode = 0;

            // 时间模式 (外部会设置)
            this.timeMode = 2;

            // boss 血量 (外部会设置)
            this.loongHp = 10000;

            // 红蓝 boss
            this.boss = sheepCtl.boss;

            // 地块比例
            this.plotRatio = 0.5f;

            // 核心状态机
            this.state = SheepRoomState.Ready;

            // 尝试角色 (todo 但是是哪一种? 当前在场上的? )
            this.pets = new[] { new HashSet<PetView>(), new HashSet<PetView>() };

            this.gameIndex = 0;
            this.gameStartTimerForBuff = 0;
            this.cameraEulerAngles = new Vector3();
            this.endTime = 0;
            this.preBuffs = new[] { new List<int>(), new List<int>() };
            this.buffs = new[] { new List<Buff>(), new List<Buff>() };
            this.countNewBuffs = new[] { 0, 0 };
            this.countBuffs = new[] { 0, 0 };
            this.countShowBuffs = new[] { 0, 0 };

            // 反击时刻标识符 (防止多次触发反击时刻)
            this.flagLongBuffs = new[] { false, false };

            this.petStartCounts = new[] { new Dictionary<int, int>(), new Dictionary<int, int>() };
            this.god_view_pets = new List<PetView>();
            this.perfStat = new PerfStat() {
                redNums = new[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                blueNums = new[] { 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            this.view_pets = new PetView[SheepConfig.MaxPetCount];
            this.view_bullets = new BulletView[SheepConfig.MaxBulletCount];
            this.pre_view_bullets = new BulletView[SheepConfig.MaxBulletCount];
            this.updateTime = 0;
            this.petsAdd = new List<PetView>();
            this.petsDel = new Stack<int>();
            this.petCount = 0;
            this.bulletsDel = new Stack<int>();
            this.bulletCount = 0;
            this.bulletId = 0;

            this.logic_counts = new[] { 1, 1 };

            this.bullte_creates = new List<BullteCreate>();

            this.pre_blocks = new Dictionary<int, Dictionary<int, Dictionary<int, List<int>>>>();
            this.isChangeCollsionFlags = null;
            this.isChangeAckFlags = null;

            this.MaxCount = SheepConfig.line_w * SheepConfig.line_w;

            this.attackViews = new IndexLen[][] {
                new IndexLen[MaxCount],
                new IndexLen[MaxCount],
            };

            this.attackView1s = new int[][] {
                new int[SheepConfig.MaxPetCount],
                new int[SheepConfig.MaxPetCount]
            };

            this.collisionViews = new IndexLen[][][] {
                new IndexLen[SheepConfig.MaxGroupCount][],
                new IndexLen[SheepConfig.MaxGroupCount][]
            };

            for (var e = 0; e < SheepConfig.MaxGroupCount; e++) {
                this.collisionViews[(int)SheepCamp.Red][e] = new IndexLen[this.MaxCount];
                this.collisionViews[(int)SheepCamp.Blue][e] = new IndexLen[this.MaxCount];
            }

            this.collisionView1s = new int[][][] {
                new int[SheepConfig.MaxGroupCount][],
                new int[SheepConfig.MaxGroupCount][]
            };

            for (var e = 0; e < SheepConfig.MaxGroupCount; e++) {
                this.collisionView1s[(int)SheepCamp.Red][e] = new int[SheepConfig.MaxPetCount];
                this.collisionView1s[(int)SheepCamp.Blue][e] = new int[SheepConfig.MaxPetCount];
            }

            /**
             * 红方召唤池
             * key 是 类型 id
             * @type {Map<Number,SheepCallInfo>}
             */
            this.redCallInfos = new Dictionary<int, SheepCallInfo>();

            /**
             * 蓝方召唤池
             * key 是 类型 id
             * @type {Map<Number,SheepCallInfo>}
             */
            this.blueCallInfos = new Dictionary<int, SheepCallInfo>();

            /**
             * 是否自动出兵
             * @type {boolean}
             */


            // ************************ 以下待整理 **************************

            /**
             * @type ComSheepImages
             */
            this.comImages = sheepCtl.comImages;

            this.cur_rob_role_index = 0;
            this.cur_rob_bullet_index = 0;
            this.cur_rob_role_mesh_index = 0;
            this.cur_rob_bullet_mesh_index = 0;
            this.cur_rob_star_mesh_index = 0;
            this.roleMaxIndex = 0;
            this.bulletMaxIndex = 0;
            this.preBulletIndex = 0;
            this.curIndexImages = null;
            this.redBuffCount = 0;
            this.blueBuffCount = 0;

            // 角色 id 分配器
            this.petId = 0;


            // 绑定 system
            Util.system = this;
            UtilFind.system = this;
            UtilAck.system = this;
        }

        public void onGameStart() {
            this.gameStartTimerForBuff = 0;
            this.clearPets();
            this.clearCallPets();

            foreach (var e1 in this.petStartCounts) {
                e1.Clear();
            }

            this.preBuffs = new[] { new List<int>(), new List<int>() };
            this.buffs = new[] { new List<Buff>(), new List<Buff>() };
            this.countNewBuffs = new[] { 0, 0 };
            this.countBuffs = new[] { 0, 0 };
            this.countShowBuffs = new[] { 0, 0 };
            this.flagLongBuffs = new[] { false, false };
            var e2 = SheepCtl.instance;
            e2.comMatch.updateWinloops();
        }

        public void onGameRun() {
            foreach (var e in this.god_view_pets) {
                e.state = SheepRoleState.Palm;
                e.subState = SheepRoleSubState.Palm;
                e.animType = SheepRoleAnimType.Palm;
                e.readySkillId = 70002;
            }
        }

        public void onGameEnd() {
            this.god_view_pets = new List<PetView>();
        }

        public void setState(SheepRoomState e) {
            this.state = e;
            Debug.Log("房间状态改变: " + e);
            eventBus.emit(EventType.RoomState, (state = e));
        }

        public void addPet(PetView e, SheepCamp camp) {
            this.pets[(int)camp].Add(e);
            if (camp == SheepCamp.Red) {
                this.perfStat.redNums[(int)e.conf.roleType]++;
            } else {
                this.perfStat.blueNums[(int)e.conf.roleType]++;
            }
        }

        public void delPet(PetView e) {
            this.pets[(int)SheepCamp.Red].Remove(e);
            this.pets[(int)SheepCamp.Blue].Remove(e);
            if (e.camp == SheepCamp.Red) {
                this.perfStat.redNums[(int)e.conf.roleType]--;
            } else {
                this.perfStat.blueNums[(int)e.conf.roleType]--;
            }
        }

        public void clearPets() {
            this.pets[(int)SheepCamp.Red].Clear();
            this.pets[(int)SheepCamp.Blue].Clear();
            Array.Fill(this.perfStat.redNums,0);
            Array.Fill(this.perfStat.blueNums,0);
        }

        public int getBlockIndex(Vector3 e) {
            var t = Math.Floor(e.x / SheepConfig.d + SheepConfig.w / SheepConfig.d / 2);
            var o = Math.Floor(e.y / SheepConfig.d + SheepConfig.h / SheepConfig.d / 2);
            return (int)(t * SheepConfig.line_w + o);
        }

        public int getNextPetId() {
            return ++petId;
        }

        public int rob_role(int t) {
            var old = this.cur_rob_role_index;
            this.cur_rob_role_index += t;
            return old;
        }

        public int rob_bullet(int t) {
            var old = this.cur_rob_bullet_index;
            this.cur_rob_bullet_index += t;
            return old;
        }

        public int rob_pre_bullet(int t) {
            var old = this.preBulletIndex;
            this.preBulletIndex += t;
            return old;
        }

        public void clearPetViews() {
            foreach (var element in this.view_pets) {
                if (element!=null) {
                    element.clear();    
                }
            }
        }

        public PetView getPetView(int petIndex) {
            if (petIndex < 0 || petIndex >= SheepConfig.MaxPetCount) {
                return null;
            }
            var pet = this.view_pets[petIndex];
            if (pet==null) {
                pet = new PetView(petIndex);
                pet.sheepMgr = this;
                this.view_pets[petIndex] = pet;
            }

            return pet;
        }

        public void clearViewBullets() {
            foreach (var viewElement in this.view_bullets) {
                if (viewElement!=null) {
                    viewElement.clear();    
                }
                
            }
            foreach (var viewElement in this.pre_view_bullets) {
                if (viewElement != null) {
                    viewElement.clear();    
                }
                
            }
        }

        public BulletView getBulletView(int e) {
            if (e < 0 || e >= SheepConfig.MaxBulletCount) {
                return null;
            }
            var bullet = this.view_bullets[e];
            if (bullet==null) {
                bullet = new BulletView();
                this.view_bullets[e] = bullet;
            }

            return bullet;
        }

        public BulletView getBulletPreView(int e) {
            if (e < 0 || e >= SheepConfig.MaxBulletCount) {
                return null;
            }
            var bullet = this.pre_view_bullets[e];
            if (bullet==null) {
                bullet = new BulletView();
                this.pre_view_bullets[e] = bullet;
            }

            return bullet;
        }
// todo
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
// todo
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

        public void game_update(SheepMgr sheepMgr, SheepCtl sheepCtl, float i) {
            try {
                // 处理召唤兵
                this.consume(sheepCtl, i);

                this.buff_add_pets();

                this.buff_add_bullets();

                // 要处理的总数量
                var n = sheepMgr.pets[(int)SheepCamp.Red].Count + sheepMgr.pets[(int)SheepCamp.Blue].Count;
                if (n <= 0) {
                    return;
                }

                this.cur_rob_role_index = 0;
                this.cur_rob_bullet_index = 0;

                this.curIndexImages = this.comImages.startAdd();

                // 执行主逻辑
                this.role_logic();

                this.comImages.endAdd();

                this.update_merge_workers(sheepMgr, sheepCtl, i);

            } catch (Exception err) {
                Debug.LogError("update逻辑错误 "+ err);
                throw;
            }
        }

        public void update_merge_workers(SheepMgr sheepMgr, SheepCtl sheepCtl, float dt) {

        var isEnd = false;

        var now = NowMs();

        this.mainClearBlocks();
        sheepCtl.comImages.mesh_block.onFrameUpdateStart();


        if (sheepMgr.endTime!=0 && sheepMgr.endTime < NowMs()) {
            eventBus.emit(EventType.RoomStateEnd);
            isEnd = true;
            sheepMgr.endTime = 0;
            return;
        }


        sheepMgr.countNewBuffs = new[] { 0, 0 };
        sheepMgr.countBuffs = new[] { 0, 0 };
        sheepMgr.countShowBuffs = new[] { 0, 0 };

        // console.log(sheepMgr.buffs)
        for (var i = 0; i < sheepMgr.buffs.Length; i++) {
            var r=sheepMgr.buffs[i];
            var s=i;
            if (r.Count!=0 && r[0].time < sheepMgr.gameStartTimerForBuff) {
                r.RemoveAt(0);
                sheepMgr.buffs[s] = r;
            }

            foreach (var o in r) {
                sheepMgr.countBuffs[s] += o.count!=0 ?o.count : SheepConfig.counterBuffNumber;
                sheepMgr.countShowBuffs[s] += o.count;
            }
        }
        
        for (var i = 0; i < sheepMgr.preBuffs.Length; i++) {
            var r=sheepMgr.preBuffs[i];
            var s=i;
            
            if (r.Count==0) {
                continue;
            }

            var sum = 0;
            var hasZero = false;

            foreach (var f in r) {
                if (0 == f) {
                    hasZero = true;
                }

                sum += f;
            }

            if (hasZero) {

                sheepMgr.buffs[s].Add(new Buff(){
                    time= (int)(sheepMgr.gameStartTimerForBuff + 1000 * SheepConfig.counterTime),
                    count= 0
                });

                if (r.Count > 1) {
                    sheepMgr.buffs[s].Add(new Buff(){
                        time= (int)(sheepMgr.gameStartTimerForBuff + 1000 * SheepConfig.buffLastTime),
                        count= sum
                    });
                }

            } else {
                sheepMgr.buffs[s].Add(new Buff(){
                    time= (int)(sheepMgr.gameStartTimerForBuff + 1000 * SheepConfig.buffLastTime),
                    count= sum
                });
            }

            sheepMgr.preBuffs[s] = new List<int>();
            sheepMgr.countNewBuffs[s] += sum;


        }

        isEnd = this.updateBoss(sheepMgr, sheepCtl, dt, now);

        if (isEnd) {
            return;
        }

        var h = new List<PetView>();
        foreach (var e in sheepMgr.pets) {
            foreach (var e1 in e) {
                h.Add(e1);
            }
        }

        var _ = false;
        if (!sheepMgr.cameraEulerAngles.Equals(sheepCtl.cameraCtl.camera.node.eulerAngles)) {
            _ = true;
            sheepMgr.cameraEulerAngles = sheepCtl.cameraCtl.camera.node.eulerAngles;
        }

        var b = 0;
        var I = 0;

        var x = h;

        for (var B = 0; B < x.Count; B++) {
            var y = x[B];
            if (y.buff_index == -1) {
                continue;
            }

            y.updateSkin(sheepCtl, this, sheepMgr, dt);

            int[] M;
            var D = y.view_pet;
            var A = D.state;
            var P = D.animType;
            var W = D.animFrame;

            var fgs = sheepCtl.comImages.roles_framess[(int)y.camp];

            var ghg = fgs[(int)y.skinId];

            M = ghg[(int)P];

            if (null == M) {
                Debug.LogError("找不到动画 "+ y.camp + " "+ y.skinId+" "+ P);
            }

            if (A == SheepRoleState.In && W >= M.Length - 1) {
                var E = SheepSkill.getById(D.readySkillId);
                if (E!=null) {
                    if (E.skillType == SheepSkillType.Boom) {
                        var F = SheepSkillSubBoom.getById(E.id);
                        D.state = SheepRoleState.Boom;
                        if (F.isAnim!=0) {
                            D.animType = SheepRoleAnimType.Boom;
                        } else {
                            D.animType = SheepRoleAnimType.Idle;
                        }
                    }
                } else {
                    D.state = SheepRoleState.Move;
                    D.animType = SheepRoleAnimType.Idle;
                }
            } else if (A == SheepRoleState.Dead && W >= M.Length - 1) {
                D.state = SheepRoleState.Res;
                D.animType = SheepRoleAnimType.None;
                y.onRes(sheepCtl, sheepMgr);
            } else if (A == SheepRoleState.Up && W >= M.Length - 1) {
                D.state = SheepRoleState.In;
                D.animType = SheepRoleAnimType.In;
            } else if (A == SheepRoleState.Buff) {
                var V = SheepSkillSubBuff.getById(D.readySkillId);
                var U = D.animFrame;
                if (U > V.buffStratFrame && U < V.buffEndFrame) {
                    if (y.camp == SheepCamp.Blue) {
                        I += 1;
                    } else {
                        b += 1;
                    }
                }
            }

        }

        var j = 0;
        for (var G = 0; G < this.bulletCount; ++G) {
            var X = this.getBulletView(G);
            if (X.isDie) {
                continue;
            }

            if (X.frame >= X.conf.endFrame) {
                X.isDie = true;
                this.buff_del_bullet(G);
            } else {

                var z = this.getPetView(X.roleIndex).conf.splitN;

                for (var O = -z; O <= z; ++O) {
                    for (var Q = -z; Q <= z; ++Q) {
                        var Z = Util.getIndexByXY(X.x + O, X.y + Q);
                        sheepCtl.comImages.mesh_block.addFrameBlockCamp(Z, X.camp);
                    }
                }

                ++j;
            }
        }

        this.mainSyncBlocksToWokers();
        sheepCtl.comImages.mesh_block.onFrameUpdateEnd(sheepMgr);
        this.redBuffCount = b;
        this.blueBuffCount = I;

        if (isEnd) {
            return;
        }

        this.roleMaxIndex = this.petCount;
        this.bulletMaxIndex = this.bulletCount;

        }
// todo
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

        public bool updateBoss(SheepMgr sheepMgr, SheepCtl sheepCtl, float dt, long c) {

            var isEnd = false;
            for (var i = 0; i < sheepMgr.boss.Length; i++) {
                var t= sheepMgr.boss[i];
                var index = i;
                
                
            var viewPet = this.getPetView(index);
            var camp = viewPet.camp;
            var state = viewPet.state;

            if ((int)state == (int)SheepBossState.Ready) {

                viewPet.curHp = t.curHp;
                t.comProgress.setVue(t.curHp);
                viewPet.state = (SheepRoleState)(int)SheepBossState.NomalRun;

            } else if ((int)state == (int)SheepBossState.AwakeAnim || (int)state == (int)SheepBossState.UnAwakeAnim) {

                t.comProgress.setVue(t.comProgress._vue);

            } else if ((int)state == (int)SheepBossState.Dead) {

            } else {
                var curHp = viewPet.curHp;
                if (curHp <= 0) {
                    curHp = 0;
                }

                var d = t.comProgress._vue;
                var _ = d - curHp;

                if (_!=0 && curHp!=0) {

                    var S = sheepMgr.countBuffs[1 - (int)camp];
                    if (S > 0) {
                        var b = 1 + SheepConfig.buffDragonDamageIncreseRate * S;
                        b += 0;
                        _ = (float)Math.Floor(_ * b);
                        curHp = d - _;
                        viewPet.curHp = curHp;
                    }

                    var I = sheepMgr.countBuffs[(int)camp];
                    if (I > 0) {
                        var B = Math.Pow(1 - SheepConfig.buffDragonReduceRate, I);
                        B -= 0;
                        if (B < 1 - SheepConfig.buffDragonMaxReduceRate) {
                            B = 1 - SheepConfig.buffDragonMaxReduceRate;
                        }

                        _ = (float)Math.Floor(_ * B);
                        curHp = d - _;
                        viewPet.curHp = curHp;
                    }

                }

                if (t.subShield() && _ > 1) {
                    curHp = d - 1;
                    if (curHp < 0) {
                        curHp = 0;
                    }
                    _ = 1;
                    viewPet.curHp = curHp;
                }

                if (curHp != d) {
                    t.comProgress.setVue(curHp);
                    t.curHp = curHp;
                    SheepAnims.showBossBlood(sheepCtl, t, _);
                    t.hitAnim();
                }

                var R = sheepMgr.countShowBuffs[(int)camp];
                var M = sheepMgr.countBuffs[(int)camp];

                if (!sheepMgr.flagLongBuffs[(int)camp] && curHp < sheepMgr.loongHp * SheepConfig.counterHpRatio) {
                    sheepMgr.flagLongBuffs[(int)camp] = true;
                    t.backStateTime = c;
                    sheepMgr.preBuffs[(int)camp].Add(0);
                    sheepCtl.comMatch.showDoubleAnim(camp);
                    sheepCtl.comUIAnim.backAnim(camp);
                    sheepCtl.cameraCtl.onShake(SheepConfig.shockBeginNumber);
                } else if (t.backStateTime!=0 && c - t.backStateTime > 12e4 && M - R == 0) {
                    t.backStateTime = 0;
                    sheepCtl.comMatch.hideDoubleAnim(camp);
                    sheepCtl.comUIAnim.backSuccessAnim(camp);
                    sheepCtl.cameraCtl.onShake(SheepConfig.shockEndNumber);
                }

                if (curHp <= 0) {
                    viewPet.state = (SheepRoleState)(int)SheepBossState.Dead;
                    viewPet.isDie = true;
                    t.curHp = 0;
                    eventBus.emit(EventType.RoomStateEnd);
                    isEnd = true;
                    continue;
                }

                var unuse=viewPet.curAckFrame;

                var T = 0;
                var D = sheepMgr.plotRatio;

                for (var A = 0; A < SheepConfig.loongStateSwitching.Length; A++) {
                    if (D <= SheepConfig.loongStateSwitching[A]) {
                        T = A;
                        break;
                    }
                }

                sheepMgr.plotRatioIndex = T;
                t.updateState(sheepCtl, sheepMgr, T + 1);
                t.updateStateJJL(sheepCtl, sheepMgr, T + 1);

            }
            }
        
        
        return isEnd;
        }

        public void pre_add_pet(PetView e) {
            this.petsAdd.Add(e);
        }

        public void buff_add_pets() {
            if (this.petsAdd.Count <= 0) {
                return;
            }

            for (; this.petsAdd.Count!=0;) {
                if (!this.petsDel.TryPop(out var e)) {
                    if (this.petCount >= SheepConfig.MaxPetCount - 1) {
                        Debug.LogWarning("预加入怪物加入buff超过最大数量"+ this.petCount+" "+SheepConfig.MaxPetCount);
                        break;
                    }

                    e = this.petCount++;
                }
                var t = this.petsAdd[0];
                 this.petsAdd.RemoveAt(0);
                var r = this.getPetView(e);
                t.init(e, r);
            }
        }

        public void buff_del_pet(int e) {
            var pet = this.getPetView(e);
            pet.isDie = true;
            pet.id = 0;
            this.petsDel.Push(e);
        }

        public void clear_pets() {
            this.cur_rob_role_index = 0;
            this.roleMaxIndex = 0;
            this.petCount = 0;
            this.petsAdd.Clear();
            this.petsDel.Clear();
        }

        public void buff_add_bullets() {
            var e = this.preBulletIndex;
            if (e!=0) {
                for (; e!=0;) {
                    if (!this.bulletsDel.TryPop(out var t)) {
                        if (this.bulletCount >= SheepConfig.MaxBulletCount - 1) {
                            Debug.LogWarning("预加入子弹加入buff超过最大数量"+ this.bulletCount+ SheepConfig.MaxBulletCount);
                            break;
                        }

                        t = this.bulletCount++;
                    }
                    --e;

                    this.getBulletView(t).init(++this.bulletId, this.pre_view_bullets[e]);
                }

                this.preBulletIndex = 0;
            }
        }

        public void buff_del_bullet(int e) {
            var bullet = this.getBulletView(e);
            bullet.id = 0;
            this.bulletsDel.Push(e);
        }

        public void clear_bullets() {
            this.cur_rob_bullet_index = 0;
            this.bulletMaxIndex = 0;
            this.bulletCount = 0;
            this.bulletsDel.Clear();
        }
// todo
        public void game_clear() {
            this.clearBlocks();
            this.clearPetViews();
            this.preBulletIndex = 0;
            this.clearViewBullets();
            this.clear_pets();
            this.clear_bullets();
            InitializeBossView(SheepCamp.Red);
            InitializeBossView(SheepCamp.Blue);
            petCount = 2;
            roleMaxIndex = 2;
        }
// todo
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

        public int rob_role_task(int count, CurIndexImages curIndexImages) {
            var start = this.rob_role(count);
            var end = start + count;
            var i = this.update_role(start, end);
            this.comImages.update_role(curIndexImages);
            return i;
        }

        public int rob_bullet_task(int count, CurIndexImages curIndexImages) {
            var start = this.rob_bullet(count);
            var end = start + count;
            var i = this.update_bullet(start, end);
            this.comImages.update_bullet(curIndexImages);
            return i;
        }

        public int update_role(int start, int end) {
            for (var i = start; i < end; i++) {
                var viewPet = this.getPetView(i);
                if (!viewPet.isActive) {
                    viewPet = null;
                    continue;
                }

                var t = viewPet.isDie;
                if (0 == viewPet.roleId) {
                    var i1 = this.update_frame(viewPet);
                    if (!t && i1) {
                        this.update_boss_state(viewPet);
                    }

                    this.update_role_anim(viewPet);
                }
                else {
                    var i1 = (int)viewPet.camp;
                    var s = this.logic_counts[i1];
                    for (var i2 = 0; i2 < s; i2++) {
                        var i3 = this.update_frame(viewPet);
                        if (!t) {
                            this.update_role_state(viewPet, i3);
                        }

                        this.update_role_anim(viewPet);
                    }

                    var o = viewPet;
                    this.comImages.addRole(o);
                }

                viewPet = null;
            }

            return end - start;
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
                        for (var i1 = 0; i1 < e.Length; i1++) {
                            var n1 = e[i1];
                            if (-1 == n1 || n1 == l) {
                                if (0 == t.tarRoleIndex || 1 == t.tarRoleIndex) {
                                    var o1 = this.getPetView(t.tarRoleIndex);
                                    if (UtilAck.isCanAckByBullet(t, o1, i1)) {
                                        UtilAck.hurtByBullet(t, o1, t.atkVue);
                                    }
                                }
                                else
                                    UtilFind.forfeachBlocksByAckView(t.camp, s, o, t.conf.findR,
                                        (e => {
                                            if (UtilAck.isCanAckByBullet(t, e, i1)) {
                                                UtilAck.hurtByBullet(t, e, t.atkVue);
                                            }
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
                            break;
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
// todo
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
            if (petSkin.conf.skillIn!=0) {
                var t = SheepSkill.getById(petSkin.conf.skillIn);
                if (t.skillType == SheepSkillType.Boom) {
                    var i = SheepSkillSubBoom.getById(t.id);
                    if (1 == petSkin.animFrame) {
                        var t1 = petSkin.camp == SheepCamp.Red ? -1200 : 1200;
                        var xnyn = Util.getXnYn(t1, 0);
                        var o= xnyn.xn;
                        var l = xnyn.yn;
                        PetView n = null;
                        UtilFind.findNearBlocksByAckView(petSkin, o, l, 100, e => {
                            n = e;
                            return true;
                        });
                        if (n!=null) {
                            petSkin.posBefX = petSkin.posX;
                            petSkin.posBefY = petSkin.posY;
                            petSkin.posX = n.posX;
                            petSkin.posY = n.posY;
                            petSkin.animX = petSkin.posX;
                            petSkin.animY = petSkin.posY;
                        } else {
                            petSkin.posBefX = t1;
                            petSkin.posBefY = 0;
                            petSkin.posX = t1;
                            petSkin.posY = 0;
                            petSkin.animX = t1;
                            petSkin.animY = 0;
                        }
                        petSkin.readySkillId = i.id;
                        petSkin.isLock = true;
                    }
                }
            }
        }

        public void update_role_state_move(PetView petSkin, bool t, float i) {
            if (petSkin.isLock) {
                return;
            }
            var fff = UtilFind.findTar(petSkin);
            var s = fff.atkTar;
            var o = fff.moveTar;
            var l = fff.moveBoss;
            
            if (s!=null) {
                petSkin.state = SheepRoleState.Attack;
                petSkin.subState = SheepRoleSubState.AttackAwait;
                return;
            }
            if (o!=null) {
                petSkin.subState = SheepRoleSubState.MoveTar;
                Util.moveTar(petSkin, o, i, t);
                return;
            }
            if (l!=null) {
                petSkin.subState = SheepRoleSubState.MoveBoss;
                Util.moveTar(petSkin, l, i, t);
                return;
            }

            Debug.LogError("移动状态没有目标??");
        }

        public void update_role_state_attack(PetView petSkin, bool t, float i) {
            var o = petSkin.conf.atkMoveType;
        if (petSkin.conf.isLoongStopDistance!=0) {
            var t3 = sheepMode;
            var i1 = petSkin.conf.loongStopDistanceR;
            if (Util.dis(petSkin.posX, petSkin.posY, petSkin.camp == SheepCamp.Red ? t3.loongX : -t3.loongX, 0) <= i1) {
                o = (int)SheepRoleAtkMoveType.None;
            }
        }
        if (petSkin.subState == SheepRoleSubState.AttackAwait) {
            if (!Util.isAtkCd(petSkin)) {
                petSkin.subState = SheepRoleSubState.AttackAnim;
                petSkin.animType = SheepRoleAnimType.Attack;
            }
        } else if (petSkin.subState == SheepRoleSubState.AttackAnim) {
            var t3 = petSkin.conf;
            var i7 = t3.finishAtk;
            var atkCd = t3.atkCd;
            var l = petSkin.animFrame;
            var n = t3.readyAtks;
            foreach (var i9 in n) {
                if (l == i9) {
                    PetView i5 = null;
                    if (petSkin.conf.atkType == SheepRoleAtkType.Nearest) {
                        i5 = UtilFind.findNearAck(petSkin);
                    } else if (petSkin.conf.atkType == SheepRoleAtkType.Throw) {
                        i5 = UtilFind.findSortAck(petSkin, petSkin.conf.findR);
                        if (petSkin.conf.roleType == SheepRoleType.pao_che) {
                            var t6 = Util.getBackBoss(petSkin.camp);
                            if (Util.isCanAckByRole(petSkin, t6)) {
                                i5 = t6;
                            }
                        }
                    } else {
                        i5 = UtilFind.findNearAck(petSkin);
                    }
                    if (t3.bullet!=null && 0 != t3.bullet.Length) {
                        if (i5!=null) {
                            this.bullte_creates.Add(new BullteCreate(){
                                view_pet= petSkin,
                                bulletId= t3.bullet[petSkin.camp == SheepCamp.Red ? 0 : 1],
                                view_tar_pet= i5
                            });
                        } else {
                            this.bullte_creates.Add(new BullteCreate(){
                                view_pet= petSkin,
                                bulletId= t3.bullet[petSkin.camp == SheepCamp.Red ? 0 : 1]
                            });
                        }
                    } else {
                        if (i5!=null) {
                            UtilAck.ackTar(petSkin, i5);    
                        }
                    }

                    break;
                }
            }
            if (l >= i7) {
                Util.resetAtkCd(petSkin, atkCd);
                var fff = UtilFind.findTar(petSkin);
                var t5 = fff.atkTar;
                var i5 = fff.moveTar;
                var s = fff.moveBoss;
                if (t5!=null) {
                    petSkin.subState = SheepRoleSubState.AttackAwait;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return;
                }
                if (i5!=null) {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveTar;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return;
                }
                if (s!=null) {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return;
                }
            }
        }
        if (t && (o == (int)SheepRoleAtkMoveType.Move || o == (int)SheepRoleAtkMoveType.CdMove && petSkin.subState == SheepRoleSubState.AttackAwait)) {
            var s = UtilFind.findNearAck(petSkin);
            if (s!=null&&Util.disByRole(petSkin, s) > petSkin.conf.atkMinMoveR + s.conf.collideR) {
                Util.moveTar(petSkin, s, i, t);
            }
        }
        }

        public void update_role_state_killer(PetView petSkin) {
            var t = SheepSkillSubKiller.getById(petSkin.readySkillId);
            var i = petSkin.animFrame;
            if (i == t.findMoveFrame) {
                var i3 = false;
                var s = petSkin.conf;
                if (petSkin.conf.roleType == SheepRoleType.ci_ke) {
                    UtilFind.foreachFront(petSkin, (e => {
                        if (e.conf.roleType != SheepRoleType.dun_bing) {
                            
                        }
                        else {
                            i3 = true;
                        }
                    }), s.findR, 60);

                }
                if ( i3) {
                    Debug.LogWarning("刺客被中断，直接回到移动状态");
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return;
                }
                var o = UtilFind.findFarAck(petSkin, t.findR);
                if (o!=null) {
                    petSkin.logicMove(o.posX, o.posY);
                }
                else {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin .animType = SheepRoleAnimType.Idle;
                }
                
            }

            if (i == t.atkFrame) {
                UtilAck.ackMe(petSkin, t.spiltRadiusBet, t.atkBet, t.atkFindR);
            }
            
            if (i >= t.endFrame) {
                var i1 = petSkin.subState;
                if (i1 == SheepRoleSubState.KillerEnd || i1 - SheepRoleSubState.KillerStart >= t.cnt) {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return;
                }
                petSkin.subState = (SheepRoleSubState)((int) i1 + 1);
                petSkin.animType = SheepRoleAnimType.Killer;
            }
        }
// todo
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
            var t = petSkin.animFrame;
            var i = SheepSkill.getById(petSkin.readySkillId);
            var s = SheepSkillSubInvincible.getById(i.id);
            var o = s.healFrames;
            foreach (var i1 in o) {if (t == i1) {
                var t3 =(float) Math.Floor((petSkin.conf.hp - petSkin.curHp) * (s.healHealthPercent / 100f));
                UtilAck.hurtByRole(petSkin, petSkin, -t3);
                break;
            }}
            var l = s.atkFrames;
            foreach (var i2 in l) {if (t == i2) {
                UtilAck.ackMe(petSkin, s.spiltRadiusBet, s.atkBet, s.atkFindR);
                break;
            }}

            if (t >= s.endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
            
        }

        public void update_role_state_bladestorm(PetView petSkin, bool t, float i) {
            var s = petSkin.animFrame;
            var o = SheepSkill.getById(petSkin.readySkillId);
            var l = SheepSkillSubBladestorm.getById(o.id);
            if (t) {
                var fff = UtilFind.findTar(petSkin, l.findR);
                var t1 = fff.atkTar;
                var s1=fff.moveTar;
                var o1 = fff.moveBoss;
                PetView n=null;
                if (t1!=null) {
                    n = t1;
                }else if (s1!=null) {
                    n = s1;
                }else if (o1 != null) {
                    n = o1;
                }
                Util.dirTar(petSkin, n);
                var r = l.speed;
                var x = petSkin.posX + petSkin.dirX * r * i * 3f;
                var y = petSkin.posY + petSkin.dirY * r * i * 3f;
                petSkin.logicMove(x, y);
            }
            var n1 = l.atkFrames;
            foreach (var t3 in n1) {
                if (s == t3) {
                    UtilAck.ackMe(petSkin, l.spiltRadiusBet, l.atkBet, l.atkFindR);
                    break;
                }
            }
            if(s >= l.endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
        }

        public void update_role_state_palm(PetView petSkin) {
            var t1 = petSkin.animFrame;
            var i1 = SheepSkill.getById(petSkin.readySkillId);
            var s = SheepSkillSubPalm.getById(i1.id);
            var o1 = s.healFrames;
            foreach (var i in o1){ if (t1 == i) {
                var t =(float) Math.Floor((petSkin.conf.hp - petSkin.curHp) * (s.healHealthPercent / 100f));
                UtilAck.hurtByRole(petSkin, petSkin, -t);
                break;
            }}
            var l1 = s.atkFrames;
            foreach (var i in l1) {if (t1 == i) {
                UtilAck.ackMe(petSkin, s.spiltRadiusBet, s.atkBet, s.atkFindR);
                break;
            }}
            var n = s.hitBackFrames;
            for (var i = 0; i < n.Length; i++) {
                var o = n[i];
                var l = s.hitBackDistances[i];
                if (t1 == o) {
                    UtilAck.hitBackMe(petSkin, s.spiltRadiusBet, s.atkFindR, l);
                    break;
                }
            }

            if (t1 >= s.endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
             
        }

        public void update_role_state_callbullets(PetView petSkin) {
            var t = petSkin.animFrame;
        var i = SheepSkill.getById(petSkin.readySkillId);
        var s = SheepSkillSubCallBullets.getById(i.id);
        var o = 0;
        if (s.frameStep!=0) {
            if (t % s.frameStep == 0) {
                o = s.frameCnt;
            }
        } else {
            var e = s.callFrames;
            for (var i1 = 0; i1 < e.Length; i1++) {
                if (t == e[i1]) {
                    o = s.callCnts[i1];
                    break;
                }
            }
        }
        if (o!=0) {
            for (var t1 = 0; t1 < o; t1++) {
                if (1 == s.type) {
                    var t3 = petSkin.posX + s.startOffsetPos[0];
                    var i3 = petSkin.posY + s.startOffsetPos[1];
                    var o3 = s.startOffsetPos[2];
                    var l = 360 * Random.Range(0f,1f);
                    var n = petSkin.posX + petSkin.dirX * s.len + s.endRadius * Math.Cos(l);
                    var r = petSkin.posY + petSkin.dirY * s.len + s.endRadius * Math.Sin(l);
                    var a = 0;
                    this.bullte_creates.Add(new BullteCreate() {
                        view_pet = petSkin,
                        bulletId = s.bullet,
                        info = new Info() { startX = t3, startY = i3, startZ = o3, endX = (float)n, endY = (float)r, endZ = a }
                    });
                } else if (2 == s.type) {
                    var t4 = s.startOffsetPos[2];
                    var i5 = 360 * Random.Range(0f,1f);
                    var o5 = petSkin.posX + petSkin.dirX * s.len + s.endRadius * Math.Cos(i5);
                    var l = petSkin.posY + petSkin.dirY * s.len + s.endRadius * Math.Sin(i5);
                    var n = 0;
                    this.bullte_creates.Add(new BullteCreate() {
                        view_pet = petSkin,
                        bulletId = s.bullet,
                        info = new Info() {
                            startX = (float)o5,
                            startY = (float)l,
                            startZ = t4,
                            endX = (float)o5,
                            endY = (float)l,
                            endZ = n,
                            dirX = 0,
                            dirY = 0,
                            dirZ = -1
                        }
                    });
                } else if (3 == s.type) {
                    this.bullte_creates.Add(new BullteCreate(){
                        view_pet= petSkin,
                        bulletId= s.bullet,
                        info= new Info(){dirX= 0, dirY= 0, dirZ= -1, angle= 360f / o * t1}
                    });
                } else if (4 == s.type) {
                    this.bullte_creates.Add(new BullteCreate(){
                        view_pet= petSkin,
                        bulletId= s.bullet,
                        info=new Info() {dirX= 1, dirY= 0, dirZ= 0}
                    });
                } else {
                    this.bullte_creates.Add(new BullteCreate(){view_pet= petSkin, bulletId= s.bullet});
                }
            }
        }

        if (t >= s.endFrame) {
            petSkin.state = SheepRoleState.Move;
            petSkin.subState = SheepRoleSubState.MoveBoss;
            petSkin.animType = SheepRoleAnimType.Idle;
        }
         
        }

        public void update_role_state_buff(PetView petSkin) {
            var t = petSkin.animFrame;
            var i = SheepSkill.getById(petSkin.readySkillId);
            if(t >= SheepSkillSubBuff.getById(i.id).endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
        }

        public void update_role_state_rigidity(PetView petSkin) {
            var t = SheepSkill.getById(petSkin.readySkillId);
            var i = SheepSkillSubRigidity.getById(t.id);
            if (petSkin.animFrame >= i.endFrame) {
                petSkin.state = SheepRoleState.SpinAtk;
                petSkin.animType = SheepRoleAnimType.Attack;
                petSkin.readySkillId = i.endSkill;
            }
        }

        public void update_role_state_spinatk(PetView petSkin, bool t, float i) {
            var s = petSkin.posX;
            var o = petSkin.posY;
            var xnyn = Util.getXnYn(s, o);
            var l = xnyn.xn;
            var n = xnyn.yn;
            var r = petSkin.animFrame;
            var a = SheepSkill.getById(petSkin.readySkillId);
            var c = SheepSkillSubSpinAtk.getById(a.id);
            if (1 == r) {
                var t1 = UtilFind.findSortAck1(petSkin, petSkin.conf.findR);

                if (t1!=null) {
                    Util.dirTar(petSkin, t1);
                }
            }
            if (t) {
                var s1 = true;
                UtilFind.forNearBlocksByAckView(petSkin, l, n, petSkin.conf.findR,
                    t1 => {
                        if (t1.isDie || t1.camp == petSkin.camp || 0 == t1.roleId) {
                            return false;
                        }

                        if (s1 && t1.conf.roleType == SheepRoleType.dun_bing && Util.isCanAckByRole(petSkin, t1)) {
                            s1 = false;
                        }
                        
                        if (!Util.isCanAckByRole(petSkin, t1)) {
                            return false;
                        }

                        UtilAck.ackTar(petSkin, t1);
                        return false;
                    });
                if (s1) {
                    Util.moveTar(petSkin, null, i, t);
                }
            }

            if (r >= c.endFrame) {
                petSkin.state = (SheepRoleState)c.endState;
                petSkin.animType = SheepRoleAnimType.Boom;
                petSkin.readySkillId = c.endSkill;
            }
        }

        public void update_role_state(PetView petSkin, bool t, float i = 0.033f) {
            Util.subAtkCd(petSkin, i);
        switch (petSkin.state) {
            case SheepRoleState.Start:
                if (!t) {
                    break;
                }
                this.update_role_state_start(petSkin, t, i);
                break;
            case SheepRoleState.In:
                // 这一整段都是羊神专属进场逻辑 !!!
                this.update_role_state_in(petSkin);
                break;
            case SheepRoleState.Spurt:
                if (!t) {
                    break;
                }
                this.update_role_state_spurt(petSkin, t, i);
                break;
            case SheepRoleState.Charge:
                if (!t) {
                    break;
                }
                this.update_role_state_charge(petSkin, t, i);
                break;
            case SheepRoleState.ChargePlus:
                if (!t) {
                    break;
                }
                this.update_role_state_charge_plus(petSkin, t, i);
                break;
            case SheepRoleState.SpinSpurt:
                if (!t) {
                    break;
                }
                this.update_role_state_spinspurt(petSkin, t, i);
                break;
            case SheepRoleState.Move:
                if (!t) {
                    break;
                }

                this.update_role_state_move(petSkin, t, i);
                break;
            case SheepRoleState.Attack:
                this.update_role_state_attack(petSkin,t,i);
                break;
            case SheepRoleState.Killer:
                this.update_role_state_killer(petSkin);
                break;
            case SheepRoleState.Boom:
                this.update_role_state_boom(petSkin);
                break;
            case SheepRoleState.Invincible:
                this.update_role_state_invincible(petSkin);
                break;
            case SheepRoleState.Bladestorm:
                this.update_role_state_bladestorm(petSkin,t,i);
                break;
            case SheepRoleState.Palm:
                this.update_role_state_palm(petSkin);
                break;
            case SheepRoleState.CallBullets:
                this.update_role_state_callbullets(petSkin);
                break;
            case SheepRoleState.Buff:
                this.update_role_state_buff(petSkin);
                break;
            case SheepRoleState.Rigidity:
                this.update_role_state_rigidity(petSkin);
                break;
            case SheepRoleState.SpinAtk:
                this.update_role_state_spinatk(petSkin,t,i);
                break;
        }

        if (petSkin.impulseX!=0 || petSkin.impulseY!=0) {
            if (!petSkin.isDie && petSkin.curHp > 0) {
                var t1 = petSkin.impulseX;
                var i1 = petSkin.impulseY;
                petSkin.logicMove(petSkin.animX + t1, petSkin.posY + i1);
            }
            petSkin.impulseX = 0;
            petSkin.impulseY = 0;
        }
        }

        public void update_role_state_start(PetView petSkin, bool t, float s) {
            if (this.state == SheepRoomState.Start) {
                if (t) {
                    var t2 = petSkin.posX;
                    var i = petSkin.posY;
                    var o = petSkin.tarPosX;
                    var l = petSkin.tarPosY;
                    var n = Util.dis(t2, i, o, l);
                    var r = 3 * petSkin.conf.runSpeed;
                    if (n > r * s) {
                        var ddd = Util.dirTarByPos(petSkin, petSkin.tarPosX, petSkin.tarPosY);
                        var t3 = ddd[0];
                        var i3 = ddd[1];
                        var o3 = new Vector3(){x= petSkin.posX, y= petSkin.posY};
                        var l3 = new Vector3(){x= t3 * r * s, y= i3 * r * s};
                        var n3 = new Vector3(){x= o3.x + l3.x, y= o3.y + l3.y};
                        petSkin.logicMove(n3.x, n3.y);
                    } else {
                        petSkin.logicMove(o, l);
                    }
                }
            } else if (petSkin.conf.skillSpurt!=0) {
                var t1 = SheepSkill.getById(petSkin.conf.skillSpurt);
                if (t1.skillType == SheepSkillType.Charge) {
                    petSkin.state = SheepRoleState.Charge;
                    petSkin.subState = SheepRoleSubState.Spurt;
                    petSkin.animType = SheepRoleAnimType.Spurt;
                } else if (t1.skillType == SheepSkillType.SpinSpurt) {
                    petSkin.state = SheepRoleState.SpinSpurt;
                    petSkin.animType = SheepRoleAnimType.Attack;
                } else {
                    petSkin.state = SheepRoleState.Spurt;
                    petSkin.subState = SheepRoleSubState.Spurt;
                    if (petSkin.conf.isSpurtAnim) {
                        petSkin.animType = SheepRoleAnimType.Spurt;
                    } else {
                        petSkin.animType = SheepRoleAnimType.Idle;
                    }
                }
            } else {
                petSkin.state = SheepRoleState.Spurt;
                petSkin.subState = SheepRoleSubState.Spurt;
                if (petSkin.conf.isSpurtAnim) {
                    petSkin.animType = SheepRoleAnimType.Spurt;
                } else {
                    petSkin.animType = SheepRoleAnimType.Idle;
                }
            }
        }

        public void update_role_state_charge(PetView e, bool t, float i) {
            var o = e.posX;
            var l = e.posY;
           var (n, r) = Util.getXnYn(o, l);
        if (e.camp == SheepCamp.Red && e.posX > e.conf.runEndX || e.camp == SheepCamp.Blue && e.posX < -e.conf.runEndX) {
            var t6 = false;
            UtilFind.findNearBlocksByAckView(e, n, r, 5, i8 => {
                if (i8.isDie || i8.camp == e.camp || 0 == i8.roleId) {
                    
                }
                else {
                    t6 = true;
                }

                return t6;
            });
            if ( t6) {
                e.state = SheepRoleState.Boom;
                e.subState = SheepRoleSubState.Boom;
                var t3 = SheepSkillSubCharge.getById(e.conf.skillSpurt);
                var i3 = SheepSkillSubBoom.getById(t3.endSkill);
                if (i3.isAnim!=0) {
                    e.animType = SheepRoleAnimType.Boom;   
                }
                else {
                    e.animType = SheepRoleAnimType.Idle;
                }

                e.readySkillId = i3.id;
            } else {
                e.state = SheepRoleState.Move;
                e.subState = SheepRoleSubState.MoveBoss;
                e.animType = SheepRoleAnimType.Idle;
            }
        } else {
            var s = false;
            UtilFind.findNearBlocksByAckView(e, n, r, 5, t8 => {
                if (!t8.isDie && t8.camp != e.camp && 0 != t8.roleId && Util.isCanAckByRole(e, t8)) {
                    if (t8.conf.roleType == SheepRoleType.xiao_bing) {
                        var i = t8;
                        UtilAck.ackTar(e, i);
                    }
                    else {
                        s = true;
                    }
                }

                return false;
            });
            if (s) {
                e.state = SheepRoleState.Boom;
                e.subState = SheepRoleSubState.Boom;
                var t8 = SheepSkillSubCharge.getById(e.conf.skillSpurt);
                var i8 = SheepSkillSubBoom.getById(t8.endSkill);
                if (i8.isAnim!=0) {
                    e.animType = SheepRoleAnimType.Boom;
                }
                else {
                    e.animType = SheepRoleAnimType.Idle;
                }

                e.readySkillId = i8.id;
                 return;
            }
            PetView o3 = null;
            UtilFind.findNearBlocksByAckView(e, n, r, e.conf.findR, t4 => {
                // 跳过：死亡的、同阵营的、没有 roleId 的
                if (t4.isDie || t4.camp == e.camp || t4.roleId == 0) {
                    return false;
                }

                // 只允许 roleType = role3
                if (t4.conf.roleType != SheepRoleType.gong_jian_shou) {
                    return false;
                }

                // 必须可攻击
                if (!Util.isCanAckByRole(e, t4)) {
                    return false;
                }

                // 如果满足条件，克隆并返回 true
                o3 = t4;
                return true;
            });
            Util.moveTar(e, o3, i, t);
        }
        }

        public void update_role_state_charge_plus(PetView e, bool t, float i) {
            var o = e.posX;
            var l = e.posY;
            var ( n,  r) = Util.getXnYn(o, l);
            if (e.camp == SheepCamp.Red && e.posX > e.conf.runEndX ||
                e.camp == SheepCamp.Blue && e.posX < -e.conf.runEndX) {
                e.state = SheepRoleState.Boom;
                e.subState = SheepRoleSubState.Boom;
                var t3 = SheepSkillSubChargePlus.getById(e.conf.skillSpurt);
                var i3 = SheepSkillSubBoom.getById(t3.endSkill);
                e.animType = SheepRoleAnimType.Boom;
                e.readySkillId = i3.id;
            }
            else {
                UtilFind.findNearBlocksByAckView(e, n, r, 5, tt2 => {
                    if (!tt2.isDie && tt2.camp != e.camp && 0 != tt2.roleId && Util.isCanAckByRole(e, tt2)) {
                        var i7 = SheepConfig.beheadLine;
                        if (tt2.curHp < i7) {
                            tt2.isDie = true;
                            tt2.state = SheepRoleState.Dead;
                        }
                        else {
                            var t1 = e.conf;
                            UtilAck.ackMe(e, t1.collideR, 0, t1.findR, t1.hitBackDistance);
                        }
                    }

                    return false;
                });

                Util.moveTar(e, null, i, t);
            }
        }

        public void update_role_state_spinspurt(PetView e, bool t, float i) {
            var o = e.posX;
            var l = e.posY;
            (int n, int r) = Util.getXnYn(o, l);
            if (e.camp == SheepCamp.Red && e.posX > e.conf.runEndX || e.camp == SheepCamp.Blue && e.posX < -e.conf.runEndX) {
                e.state = SheepRoleState.Boom;
                e.subState = SheepRoleSubState.Boom;
                var t1 = SheepSkillSubSpinSpurt.getById(e.conf.skillSpurt);
                var i1 = SheepSkillSubBoom.getById(t1.endSkill);
                if (i1.isAnim!=0) {
                    e.animType = SheepRoleAnimType.Boom;
                }else{ e.animType = SheepRoleAnimType.Idle;}
                e.readySkillId = i1.id;
            } else {
                Util.moveTar(e, null, i, t);
                UtilFind.forNearBlocksByAckView(e, n, r, e.conf.findR,
                    t2 => {
                        if (t2.isDie || t2.camp == e.camp || 0 == t2.roleId||!Util.isCanAckByRole(e, t2)) {
                            return false; 
                        }  
                        UtilAck.ackTar(e, t2);
                         return false;
                    });
            }
        }

        public void update_role_state_spurt(PetView e, bool t, float i) {
            if (e.conf.skillSpurt!=0) {
            var s = SheepSkill.getById(e.conf.skillSpurt);
            if (s.skillType == SheepSkillType.Boom) {
                var o = SheepSkillSubBoom.getById(s.id);
                var fff =UtilFind.findTar(e);
                var l = fff.atkTar ;
                var n=fff.moveTar;
                var r = fff.moveBoss;

                if (l != null || r != null) {
                    e.state = SheepRoleState.Boom;
                    e.subState = SheepRoleSubState.Boom;
                    if (o.isAnim!=0) {
                        e.animType = SheepRoleAnimType.Boom;
                    }
                    else {
                        e.animType = SheepRoleAnimType.Idle;
                    }
                     e.readySkillId = o.id;    
                     return;
                }

                Util.moveTar(e, null, i, t);
            } else if (s.skillType == SheepSkillType.Killer) {
                var o = SheepSkillSubKiller.getById(s.id);
                var fff=UtilFind.findTar(e);

                var l = fff.atkTar;
                var n = fff.moveTar;
                var r = fff.moveBoss;

                if (l != null) {
                    e.state = SheepRoleState.Killer;
                    e.subState = SheepRoleSubState.KillerStart;
                    e.animType = SheepRoleAnimType.Killer;
                    e.readySkillId = o.id;
                    return;
                }


                if (r != null) {
                    e.state = SheepRoleState.Move;
                    e.subState = SheepRoleSubState.MoveBoss;
                    e.animType = SheepRoleAnimType.Idle; 
                    Util.moveTar(e, r, i, t);
                    return;
                }

                Util.moveTar(e, null, i, t);
            } else if (s.skillType == SheepSkillType.Bullet) {
                var o = SheepSkillSubBullet.getById(s.id);
                var fff = UtilFind.findTar(e);
                var l = fff.atkTar;
                var n = fff.moveTar;
                var r = fff.moveBoss;
           
                if (l!=null || n!=null || r!=null) {
                    this.bullte_creates.Add(new BullteCreate() {
                        view_pet = e,
                        bulletId = o.bullet
                    });
                }
                if (l!=null) {
                    e.state = SheepRoleState.Attack;
                    e.subState = SheepRoleSubState.AttackAwait;
                    return ;
                }
                if (n!=null) {
                    e.state = SheepRoleState.Move;
                    e.subState = SheepRoleSubState.MoveTar;
                    Util.moveTar(e, n, i, t);
                    return;
                }
                if (r!=null) {
                    e.state = SheepRoleState.Move;
                    e.subState = SheepRoleSubState.MoveBoss;
                    e.animType = SheepRoleAnimType.Idle;
                       Util.moveTar(e, r, i, t);
                       return;
                }

                Util.moveTar(e, null, i, t);
            } else if (s.skillType == SheepSkillType.CallBullets) {
                var o = SheepSkillSubCallBullets.getById(s.id);
                var fff = UtilFind.findTar(e);
                var l = fff.atkTar;
                var n = fff.moveTar;
                var r = fff.moveBoss;

                if (l != null || r != null) {
                    e.state = SheepRoleState.CallBullets;
                    e.subState = SheepRoleSubState.CallBullets;
                    if (o.isAnim != 0) {
                        e.animType = SheepRoleAnimType.CallBullets; }else{ e.animType = SheepRoleAnimType.Idle;}
                    e.readySkillId = o.id;
                    return;
                }

                Util.moveTar(e, null, i, t);
            }
        } else {
                var fff=UtilFind.findTar(e);
                var s = fff.atkTar;
                var o = fff.moveTar;
                var l = fff.moveBoss;
             
            if (s!=null) {
                e.state = SheepRoleState.Attack;
                e.subState = SheepRoleSubState.AttackAwait;
                return;
            }
            if (o!=null) {
                e.state = SheepRoleState.Move;
                e.subState = SheepRoleSubState.MoveTar;Util.moveTar(e, o, i, t);
                return;
            }
            if (l!=null) {
                e.state = SheepRoleState.Move;
                e.subState = SheepRoleSubState.MoveBoss;
                 Util.moveTar(e, l, i, t);
                return;
            }
            Util.moveTar(e, null, i, t);
            
            }
        }

        public void update_role_anim(PetView e) {
            e.animFrame = e.animFrame + 1;
            OnRoleRender(e);
        }

        public void produce_pets(int typeID, int count, SheepCamp camp) {
            // 根据阵营 获取 map
            var callInfos = camp == SheepCamp.Red ? this.redCallInfos : this.blueCallInfos;

            // 没有就创建一个 然后加进去
            if (!callInfos.TryGetValue(typeID,out  SheepCallInfo sheepCallInfo)) {
                sheepCallInfo = new SheepCallInfo();
                sheepCallInfo.camp = camp;
                sheepCallInfo.type = typeID;
                sheepCallInfo.count = 0;
                sheepCallInfo.frame = 0;
                sheepCallInfo.count_line = 0;

                sheepCallInfo.items = new int[]{};
                sheepCallInfo.pets = new List<SheepCallInfoPet>();
                callInfos[typeID] = sheepCallInfo;
            }

            // todo 爆炸是什么意思? 用什么用?
            sheepCallInfo.count += count;
            sheepCallInfo.pets.Add(new SheepCallInfoPet() { camp = camp, count = count });
        }

        public void consume(SheepCtl sheepCtl, float t) {
           var o = this;

        this.autoTime += t;

        if (this.isAutoCall && this.autoTime > SheepConfig.systemAutomaticTroopsIntervalTime) {
            this.autoTime = 0;
            if (this.pets[0].Count + this.pets[1].Count < SheepConfig.systemLongerAutomaticallyDispatch) {
                foreach (var e in new SheepCamp[]{SheepCamp.Red, SheepCamp.Blue}) {
                    if (this.pets[(int)e].Count < SheepConfig.systemAutomaticallyMaxTroops) {
                        o.produce_pets(SheepConfig.WarmUpID, SheepConfig.systemAutomaticallyTroopsOneNumber, e);
                    }
                }
            }
        }

        foreach (var t1 in new []{this.redCallInfos, this.blueCallInfos}) {
            var n = t1 == o.redCallInfos ? SheepCamp.Red : SheepCamp.Blue;
            
            foreach (var ee in t1.ToArray()) {
                var o1 = ee.Value;
                var a = ee.Key;
                
                if (o1.count <= 0) {
                    continue;
                }

                var c = SheepRoleTypeInfo.getById(a);
                var formation = SheepRoleFormation.getById(c.formationId);

                var u = n == SheepCamp.Red ? this.perfStat.redNums[(int)c.roleType] : this.perfStat.blueNums[(int)c.roleType];

                if (c.roleType == SheepRoleType.xiao_bing) {
                    if (u > 14500) {
                        continue;
                    }
                } else if (c.roleType == SheepRoleType.ci_ke && u > 9500) {
                    continue;
                }

                o1.frame += 1;

                // 限制每帧生成的单位
                if (o1.frame <= formation.frameItemX) {
                    continue;
                }

                o1.frame = 0;

                if (formation.formationType == SheepRoleFormationType.RectangleTidy) {
                    var h = formation.itemNumY;
                    var m = formation.itemY;
                    var d = formation.itemYGapNum;
                    var g = formation.itemYGap;

                    var y = formation.startX + sheepMode.startAddX;
                    var S = n == SheepCamp.Red ? -y : y;

                    var pets = o1.pets;
                    var I = 0;

                    for (; pets.Count > 0 && I < h;) {

                        var T = Math.Floor((double)(I / 2.0));
                        float M = 0;

                        if (h % 2 == 0) {
                            if (I % 2 == 0) {
                                M = (float)(m * T + m / 2.0 + Math.Floor(T / d + 1) * g);
                            } else {
                                M = (float)(-m * T - m / 2.0 - Math.Floor(T / d + 1) * g);
                            }
                        } else {
                            if (I % 2 == 0) {
                                M = (float)(m * Math.Floor(T) + Math.Floor(T / d) * g);
                            } else {
                                M = (float)(-m * Math.Floor(T + 1) - Math.Floor(T / d) * g);
                            }
                        }
                        var C = pets[0];
                        C.count -= 1;
                        o1.count -= 1;
                        I += 1;

                        var R = new Vector3(S, M);
                        if (C.booms!=null&&C.booms.Count>0) {
                            createPetView(sheepCtl, C.camp, a, h, 1, true, R, C.booms.Pop());
                        } else {
                            createPetView(sheepCtl, C.camp, a, h, 1, true, R);
                        }

                        if (C.count <= 0) {
                            pets.RemoveAt(0);
                        }
                    }
                    o1.count_line += 1;
                    if (o1.count_line >= formation.itemNumX) {
                        o1.frame -= formation.itemYGapFrame;
                        o1.count_line = 0;
                    }

                    if(o1.count <= 0) { t1.Remove(a);}
                } else if (formation.formationType == SheepRoleFormationType.AngleTidy) {

                    var k = 2 * formation.maxAngle - formation.minAngle;
                    double G = Math.Floor((double)(k / formation.startStepAngle));
                    var A = Math.Floor(1 * G);

                    var pets = o1.pets;
                    var b = 0;

                    for (; pets.Count > 0 && b < A;) {

                        var P = pets[0];
                        var w = P.count;

                        for (var D = 0; D < w && b < A; D++) {
                            P.count -= 1;
                            o1.count -= 1;
                            b += 1;

                            var _ = Math.Floor(b / G);
                            var N = formation.startR + sheepMode.startAddR + formation.startStepR * _;

                            var E = (b % 2 == 0 ? 1 : -1) * (Math.Floor(b % G / 2) * formation.startStepAngle + formation.minAngle);

                            var x = Math.Cos(E * Math.PI / 180) * N;
                            float F = (float)(Math.Sin(E * Math.PI / 180) * N);
                            var X = new Vector3();
                            var L = sheepMode.loongX;
                            X = n == SheepCamp.Red ? new Vector3((float)(L - x), F, 0) : new Vector3((float)(x - L), F, 0);
                            if (P.booms!=null&&P.booms.Count>0) {
                                createPetView(sheepCtl, P.player, a, (int)G, 1, true, X, P.booms.Pop());
                            } else {
                                createPetView(sheepCtl, P.camp, a, (int)G, 1, true, X);
                            }
                        }

                        if(P.count <= 0) { pets.RemoveAt(0);}
                    }
                }
            }
        
        }
      
        }

        public Vector3 getPetStartEndPos(int petId, SheepCamp camp) {
            var petStartCount = this.petStartCounts[(int)camp];
            var a = petStartCount.GetValueOrDefault(petId,0);
            petStartCount[petId]= a + 1;
            var sheepRoleTypeInfo = SheepRoleTypeInfo.getById(petId);
            if (sheepRoleTypeInfo==null) {
                Debug.LogError("SheepMgr.getPetStartEndPos roleId=" + petId + " not found");
            }
            var sheepRoleFormation = SheepRoleFormation.getById(sheepRoleTypeInfo.formationId);
            if (sheepRoleFormation==null) {
                Debug.LogError("SheepMgr.getPetStartEndPos formationId=" + sheepRoleTypeInfo.formationId + " not found");
            }
            var c = sheepRoleFormation.preItemNumY;
            var l = sheepRoleFormation.preItemX;
            var u = sheepRoleFormation.preItemY;
            var h = sheepRoleFormation.preStartX + Math.Floor((double)(a / c)) * l;
            var m = Math.Floor((double)(a % c));
            var d = 0;
            if (c % 2 == 0) {
                if (m % 2 == 0) {
                    d = (int)(u * Math.Floor(m / 2) + u / 2);
                } else {
                    d = (int)(-u * Math.Floor(m / 2) - u / 2);
                }
            } else {
                if (m % 2 == 0) {
                    d = (int)(u * Math.Floor(m / 2));
                } else {
                    d = (int)(-u * Math.Floor(m / 2 + 1));
                }
            }

            if (camp == SheepCamp.Red) {
                h *= -1;
            }

            return new Vector3((float)h, d + Random.Range(-1, 1), 0);
        }

        public void clearCallPets() {
            redCallInfos.Clear();
            blueCallInfos.Clear();
        }

        public void clearBlocks() {
            Array.Fill(this.attackViews[(int)SheepCamp.Red],null);
            Array.Fill(this.attackViews[(int)SheepCamp.Blue],null);
            Array.Fill(this.attackView1s[(int)SheepCamp.Red],0);
            Array.Fill(this.attackView1s[(int)SheepCamp.Blue],0);

            for (var e = 0; e < SheepConfig.MaxGroupCount; e++) {
                Array.Fill(this.collisionViews[(int)SheepCamp.Red][e],null);
                Array.Fill(this.collisionViews[(int)SheepCamp.Blue][e],null);
                Array.Fill(this.collisionView1s[(int)SheepCamp.Red][e],0);
                Array.Fill(this.collisionView1s[(int)SheepCamp.Blue][e],0);
            }

            this.pre_blocks.Clear();
        }

        public IndexLen getBlockByIndex(IndexLen[] e, int blockIndex) {
            if (e[blockIndex]==null){
                e[blockIndex]=new IndexLen(){
                    Len=0,
                    Index=0,
                };
            }
            return e[blockIndex];
        }

        public void mainClearBlocks() {
            if (null == this.isChangeAckFlags) {this.isChangeAckFlags =new[] {true, true};}
                
            if (null == this.isChangeCollsionFlags) {
                isChangeCollsionFlags = new bool[2][];
                isChangeCollsionFlags[0] = new bool[SheepConfig.MaxGroupCount];
                isChangeCollsionFlags[1] = new bool[SheepConfig.MaxGroupCount];
                for (var e = 0; e < SheepConfig.MaxGroupCount; e++) {
                    this.isChangeCollsionFlags[(int)SheepCamp.Red][e]=true;
                    this.isChangeCollsionFlags[(int)SheepCamp.Blue][e] = true;
                }
            }
            
            if(this.isChangeAckFlags[(int)SheepCamp.Red])  {Array.Fill(this.attackViews[(int)SheepCamp.Red],null);}
            this.isChangeAckFlags[(int)SheepCamp.Red] = false;
            if(this.isChangeAckFlags[(int)SheepCamp.Blue]) {Array.Fill(this.attackViews[(int)SheepCamp.Blue],null);}
            this.isChangeAckFlags[(int)SheepCamp.Blue] = false;
            for (var e = 0; e < SheepConfig.MaxGroupCount; e++) {
                if (this.isChangeCollsionFlags[(int)SheepCamp.Red][e]) {Array.Fill(this.collisionViews[(int)SheepCamp.Red][e],null);}
                this.isChangeCollsionFlags[(int)SheepCamp.Red][e] = false;
                if(this.isChangeCollsionFlags[(int)SheepCamp.Blue][e]){ Array.Fill(this.collisionViews[(int)SheepCamp.Blue][e],null);}
                this.isChangeCollsionFlags[(int)SheepCamp.Blue][e] = false;
            }
            
            this.pre_blocks.Clear();
            
        }

        public void mainPreAddBlock(int blockIndex, int buffIndex, SheepCamp camp, int collideId) {
            if (!this.pre_blocks.TryGetValue(blockIndex,out Dictionary<int, Dictionary<int, List<int>>> o)) {
                o = new Dictionary<int, Dictionary<int, List<int>>>();
                this.pre_blocks[blockIndex]= o;
            }
            
            if (!o.TryGetValue((int)camp,out Dictionary<int, List<int>> l)) {
                l = new Dictionary<int, List<int>>();
                o[(int)camp] = l;
            }
            
            if (!l.TryGetValue(collideId, out List<int> n)) {
                n = new List<int>();
                l[collideId] = n;
            }
            n.Add(buffIndex);
            if (this.isChangeAckFlags!=null && false == this.isChangeAckFlags[(int)camp]) {
                this.isChangeAckFlags[(int)camp] = true;
            }
            if (this.isChangeCollsionFlags!=null && false == this.isChangeCollsionFlags[(int)camp][collideId]) {
                this.isChangeCollsionFlags[(int)camp][collideId] = true;
            }
        }

        public void mainSyncBlocksToWokers() { 
        var e = new int[]{0, 0};
        var t = new List<int>[]{new List<int>(),new List<int>()};
        for (var e1 = 0; e1 < SheepConfig.MaxGroupCount; e1++) {
            t[(int)SheepCamp.Red].Add(0);
            t[(int)SheepCamp.Blue].Add(0);
        }
        foreach (var e2 in this.pre_blocks) {
            var s = e2.Key;
            var i1 = e2.Value;
            if (i1!=null && i1.Count!=0) {
                foreach (var e3 in i1) {
                    var i2 = e3.Value;
                    var o = e3.Key;
                    if (i2!=null && i2.Count!=0) {
                        var l = 0;
                        var n = t[o];
                        foreach (var e4 in i2) {
                            var e5 = e4.Value;
                            var t6 = e4.Key;
                            if (e5!=null && e5.Count!=0) {
                                var i = this.collisionViews[o][t6];
                                var a3 = this.collisionView1s[o][t6];
                                var c3 = n[t6];
                                var f3 = e5.Count;
                                l += f3;
                                var blockByIndex = this.getBlockByIndex(i,s);
                                blockByIndex.Index=  c3;
                                blockByIndex.Len=  f3;
                                
                                foreach (var e7 in e5) {
                                    a3[c3] = e7;
                                    c3++;
                                }
                                n[t6] = c3;
                            }
                        }
                        if (0 == l) {
                            return;
                        }
                        var a = this.attackViews[o];
                        var c = this.attackView1s[o];
                        var f = e[o];
                        var blockByIndex1 = this.getBlockByIndex(a,s);
                        blockByIndex1.Index=  f;
                        blockByIndex1.Len=  l;
                        foreach (var e9 in i2) {
                            var e10 = e9.Value;
                            var t8 = e9.Key;
                            if(e10!=null && e10.Count!=0) {
                                foreach (var e11 in e10) {
                                    c[f] = e11;
                                    f++;
                                }
                            }
                        }
                        e[o] = f;
                    }
                }
            }
        }
        }

        public void forEachBlock(IndexLen[] e, int[] t, int blockIndex, Action<int> callback) {
            var blockByIndex = this.getBlockByIndex(e, blockIndex);
            var o = blockByIndex.Index;
            var l = blockByIndex.Len;
            if (l!=0) {
                for (var e1 = 0; e1 < l; e1++) {
                    callback(t[o + e1]);
                }
            }
        }

        public bool findBlock(IndexLen[] e, int[] t, int blockIndex, Func<int, bool> callback) {
            var blockByIndex = this.getBlockByIndex(e, blockIndex);
            var Index = blockByIndex.Index;
            var Len = blockByIndex.Len;
            if (Len==0) {
                return false;
            }
            for (var j = 0; j < Len; j++) {
                var petIndex = t[Index + j];
                if (null == petIndex) {
                    throw new Exception("二级内存取出空???");
                }
                if (callback(petIndex)) {
                    return true;
                }
            }

            return false;
        }
// todo
        public PetView createPetView(
            SheepCtl sheepCtl,
            SheepCamp camp,
            int roleType,
            int formationCount = 1,
            int unused = 0,
            bool addToWorld = true,
            Vector3? position = null,
            bool isBoom = false
        ) {
            SheepMgr manager = this;
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
                skinId = roleConfig.animId,
                sheepMgr = this
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

// todo
        public static long NowMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

// todo
        public static T ReadExternal<T>(Func<T> getter, T fallback = default) {
            try {
                return getter != null ? getter() : fallback;
            }
            catch {
                return fallback;
            }
        }
// todo
        public static int CampIndex(SheepCamp camp) => (int)camp;

    }
}