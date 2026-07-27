using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using scx.GridMap;
using UnityEngine;
using static rvb.scripts.BullteCreate;
using static rvb.scripts.SheepModes;
using static rvb.scripts.EventBus;

namespace rvb.scripts {
    public class SheepMgr {
        public static SheepMgr inc;


        // 自动出兵计时器
        public float autoTime = 0f;

        // 游戏模式 (外部会设置)
        public int gameMode = 0;

        // 时间模式 (外部会设置)
        public int timeMode = 2;

        // boss 血量 (外部会设置)
        public int loongHp = 10000;


        // 地块比例
        public float plotRatio = 0.5f;

        // 核心状态机
        public SheepRoomState state = SheepRoomState.Ready;

        public int gameIndex = 0;
        public float gameStartTimerForBuff = 0;

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


        public long updateTime = 0;

        public int redBuffCount;
        public int blueBuffCount;


        public System.Random logicRandom = new System.Random(123);

        //********************************** 以下字段 待处理 **********************************************

        // -------------------- 可选桥接回调 --------------------
        public Action<PetView> OnRoleRender;
        public Action<BulletView> OnBulletRender;
        public Func<PetView, int> AnimationFrameCountResolver;


        public int plotRatioIndex;


        // ********************** 确定字段 *******************************

        // 红蓝 boss
        public Boss[] boss = { null, null };

        // 当前在场上的角色
        public List<PetView> pets = new();

        // 准备添加到下一帧的 角色
        public List<PetView> pre_pets = new();

        // 准备删除的 角色, 这里用 Set 保证查询速度
        public HashSet<PetView> del_pets = new();

        // 当前在场上的子弹
        public List<BulletView> bullets = new();

        // 准备添加到下一帧的 子弹
        public List<BulletView> pre_bullets = new();

        // 准备删除的 子弹, 这里用 Set 保证查询速度
        public HashSet<BulletView> del_bullets = new();

        // 角色自增 id 
        public int petId = 0;

        // 子弹自增 id
        public int bulletId = 0;

        // 场上角色数量, 按照 [阵营][角色类型] 存储
        public int[][] petCounts = new[] {
            new int[(int)SheepRoleType.Count],
            new int[(int)SheepRoleType.Count]
        };

        // 格子空间, 用于加速索敌碰撞
        public GridMap<SheepCell> gridMap;

        // 红方召唤池
        public Dictionary<int, SheepCallInfo> redCallInfos = new Dictionary<int, SheepCallInfo>();

        // 蓝方召唤池
        public Dictionary<int, SheepCallInfo> blueCallInfos = new Dictionary<int, SheepCallInfo>();

        // 红蓝双方 每次执行多少逻辑帧 
        public int[] logic_counts = { 1, 1 };

        // 是否自动出兵
        public bool isAutoCall = true;

        public SheepMgr(SheepCtl sheepCtl) {
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

            this.gameIndex = 0;
            this.gameStartTimerForBuff = 0;

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


            this.updateTime = 0;

            this.redCallInfos = new Dictionary<int, SheepCallInfo>();

            this.blueCallInfos = new Dictionary<int, SheepCallInfo>();


            // ************************ 以下待整理 **************************

            this.redBuffCount = 0;
            this.blueBuffCount = 0;


            // 绑定 system
            Util.system = this;
            UtilFind.system = this;
            UtilAck.system = this;
            inc = this;


            gridMap = new GridMap<SheepCell>(
                -SheepConfig.w / 2f, -SheepConfig.h / 2f,
                SheepConfig.w, SheepConfig.h,
                SheepConfig.d,
                (gridX, gridY, worldStartX, worldStartY, worldEndX, worldEndY) =>
                    new SheepCell(gridX, gridY, worldStartX, worldStartY, worldEndX, worldEndY)
            );
        }

        // ************************* 生成相关 **************************

        // 获取 petId
        public int getNextPetId() {
            return ++petId;
        }

        // 获取 bulletId
        public int getNextBulletId() {
            return ++bulletId;
        }

        // 添加单位 下一帧才会使用
        public void addPrePet(PetView pet) {
            pre_pets.Add(pet);
        }

        // 添加子弹 下一帧才会使用
        public void addPreBullet(BulletView bullet) {
            pre_bullets.Add(bullet);
        }

        // 将 pre_pets 应用到 pets 中
        public void applyPrePets() {
            foreach (var pet in pre_pets) {
                addPet(pet);
            }

            pre_pets.Clear();
        }

        // 将 pre_bullets 应用到 bullets 中
        public void applyPreBullets() {
            foreach (var bullet in pre_bullets) {
                addBullet(bullet);
            }

            pre_bullets.Clear();
        }

        // ********************* 删除相关 **************************

        // 添加 删除单位.
        public void addDelPet(PetView pet) {
            del_pets.Add(pet);
        }

        // 添加 删除子弹.
        public void addDelBullet(BulletView bullet) {
            del_bullets.Add(bullet);
        }

        // 从 pets 中 删除 del_pets 中的 单位
        public HashSet<PetView> applyDelPets() {
            // 复制一份方便 渲染层处理
            var copy = new HashSet<PetView>(del_pets);

            // 应用移除

            foreach (var pet in del_pets) {
                delPet(pet);
            }

            // 清空
            del_pets.Clear();


            return copy;
        }

        // 从 bullets 中 删除 del_bullets 中的 对象
        public HashSet<BulletView> applyDelBullets() {
            // 复制一份方便 渲染层处理
            var copy = new HashSet<BulletView>(del_bullets);
            // 应用移除
            foreach (var bullet in del_bullets) {
                delBullet(bullet);
            }

            // 清空
            del_bullets.Clear();
            return copy;
        }


        // ****************** 场上单位相关 *************************

        // 添加单位, 不要在逻辑帧循环中调用
        public void addPet(PetView pet) {
            pets.Add(pet);
            petCounts[(int)pet.camp][(int)pet.conf.roleType] += 1;
        }

        // 添加子弹, 不要在逻辑帧循环中调用
        public void addBullet(BulletView bullet) {
            bullets.Add(bullet);
        }

        // 删除单位, 不要在逻辑帧循环中调用
        public void delPet(PetView pet) {
            pets.Remove(pet);
            petCounts[(int)pet.camp][(int)pet.conf.roleType] -= 1;
        }

        // 添加子弹, 不要在逻辑帧循环中调用
        public void delBullet(BulletView bullet) {
            bullets.Remove(bullet);
        }


        // ******************** 格子相关 **************************

        // 重建格子
        public void rebuildGridMap() {
            // 清空格子
            gridMap.forEachCell(cell => {
                cell.clearPets();
                return false;
            });

            // 重建格子
            foreach (var pet in pets) {
                var cell = gridMap.getCellByWorldPositionSafe(
                    pet.posX,
                    pet.posY
                );
                cell.addPet(pet);
            }
        }

        // 根据阵营获取 pet 总数
        public long getPetCount(SheepCamp camp) {
            var count = 0;
            var p1 = petCounts[(int)camp];
            foreach (var i in p1) {
                count += i;
            }

            return count;
        }


        // ***************************** 旧方法 ************************************

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

        public void clearPets() {
            this.pets.Clear();
        }

        public int getBlockIndex(Vector3 e) {
            var t = Math.Floor(e.x / SheepConfig.d + SheepConfig.w / SheepConfig.d / 2);
            var o = Math.Floor(e.y / SheepConfig.d + SheepConfig.h / SheepConfig.d / 2);
            return (int)(t * SheepConfig.line_w + o);
        }


        public void clearPetViews() {
            pets.Clear();
        }


        public void clearViewBullets() {
            foreach (var viewElement in this.bullets) {
                if (viewElement != null) {
                    viewElement.clear();
                }
            }

            foreach (var viewElement in this.pre_bullets) {
                if (viewElement != null) {
                    viewElement.clear();
                }
            }
        }


        public void game_run(SheepCtl sheepCtl) {
            // 清理游戏数据
            this.game_clear();

            var i = this.gameIndex;

            this.updateTime = NowMs();

            //只有 游戏处于运行中 或者 局数未改变
            while (i == this.gameIndex && (this.state == SheepRoomState.Run || this.state == SheepRoomState.Start)) {
                try {
                    var lastUpdateTime = this.updateTime;
                    this.updateTime = NowMs();

                    var diff = this.updateTime - lastUpdateTime;

                    if (diff >= 100) {
                        Debug.LogWarning("主线程更新逻辑耗时过长: " + diff + "ms");
                    }

                    if (diff < 33) {
                        Thread.Sleep((int)(33 - diff));
                    }


                    var o = NowMs() - lastUpdateTime;
                    this.game_update(sheepCtl, o);
                }
                catch (Exception e) {
                    Debug.LogWarning("主线程更新逻辑错误:" + e);
                    return;
                }
            }
        }

        public (HashSet<PetView> del_pets, HashSet<BulletView> del_bullets) game_update(SheepCtl sheepCtl, float i) {
            // 处理召唤兵
            this.consume(i);

            // 执行主逻辑
            return this.role_logic(sheepCtl, i);
        }

        public bool updateBoss(SheepCtl sheepCtl, float dt, long c) {
            var isEnd = false;
            for (var i = 0; i < this.boss.Length; i++) {
                var t = this.boss[i];
                var index = i;


                var viewPet = this.boss[index];
                var camp = viewPet.camp;
                var state = viewPet.state;

                if ((int)state == (int)SheepBossState.Ready) {
                    viewPet.curHp = t.curHp;
                    t.comProgress.setVue(t.curHp);
                    viewPet.state = (SheepRoleState)(int)SheepBossState.NomalRun;
                }
                else if ((int)state == (int)SheepBossState.AwakeAnim || (int)state == (int)SheepBossState.UnAwakeAnim) {
                    t.comProgress.setVue(t.comProgress._vue);
                }
                else if ((int)state == (int)SheepBossState.Dead) {
                }
                else {
                    var curHp = viewPet.curHp;
                    if (curHp <= 0) {
                        curHp = 0;
                    }

                    var d = t.comProgress._vue;
                    var _ = d - curHp;

                    if (_ != 0 && curHp != 0) {
                        var S = this.countBuffs[1 - (int)camp];
                        if (S > 0) {
                            var b = 1 + SheepConfig.buffDragonDamageIncreseRate * S;
                            b += 0;
                            _ = (float)Math.Floor(_ * b);
                            curHp = d - _;
                            viewPet.curHp = curHp;
                        }

                        var I = this.countBuffs[(int)camp];
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

                    var R = this.countShowBuffs[(int)camp];
                    var M = this.countBuffs[(int)camp];

                    if (!this.flagLongBuffs[(int)camp] && curHp < this.loongHp * SheepConfig.counterHpRatio) {
                        this.flagLongBuffs[(int)camp] = true;
                        t.backStateTime = c;
                        this.preBuffs[(int)camp].Add(0);
                        sheepCtl.comMatch.showDoubleAnim(camp);
                        sheepCtl.comUIAnim.backAnim(camp);
                        sheepCtl.cameraCtl.onShake(SheepConfig.shockBeginNumber);
                    }
                    else if (t.backStateTime != 0 && c - t.backStateTime > 12e4 && M - R == 0) {
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

                    var unuse = viewPet.curAckFrame;

                    var T = 0;
                    var D = this.plotRatio;

                    for (var A = 0; A < SheepConfig.loongStateSwitching.Length; A++) {
                        if (D <= SheepConfig.loongStateSwitching[A]) {
                            T = A;
                            break;
                        }
                    }

                    this.plotRatioIndex = T;
                    t.updateState(sheepCtl, this, T + 1);
                    t.updateStateJJL(sheepCtl, this, T + 1);
                }
            }


            return isEnd;
        }

        public void clear_pets() {
            foreach (var petView in del_pets) {
            }
        }

        public void del_bullet(BulletView e) {
            var bullet = e;
            bullet.id = 0;
            addDelBullet(bullet);
        }

        public void del_pet(PetView e) {
            e.isActive = false;
            e.isDie = true;
            e.id = 0;
            addDelPet(e);
        }

        public void clear_bullets() {
            foreach (var bulletView in del_bullets) {
            }
        }

// todo
        public void game_clear() {
            this.clearPetViews();

            this.clearViewBullets();
            this.clear_pets();
            this.clear_bullets();
            InitializeBossView(SheepCamp.Red);
            InitializeBossView(SheepCamp.Blue);
        }

// todo        
        private void InitializeBossView(SheepCamp camp) {
            int index = (int)camp;
            Boss view = new Boss(index);
            // view.clear();
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

            view.curHp = 99999;
            boss[(int)camp] = view;
        }

        public (HashSet<PetView> del_pets, HashSet<BulletView> del_bullets) role_logic(SheepCtl sheepCtl, float dt) {
            this.logic_counts[(int)SheepCamp.Red] = this.redBuffCount > 0 ? 2 : 1;
            this.logic_counts[(int)SheepCamp.Blue] = this.blueBuffCount > 0 ? 2 : 1;

            applyPrePets();

            applyPreBullets();


            // 更新 pet
            this.update_role();

            // 更新 bullet
            this.update_bullet();


            var isEnd = false;

            var now = NowMs();


            if (this.endTime != 0 && this.endTime < NowMs()) {
                eventBus.emit(EventType.RoomStateEnd);
                isEnd = true;
                this.endTime = 0;
                return (new HashSet<PetView>(), new HashSet<BulletView>());
            }


            this.countNewBuffs = new[] { 0, 0 };
            this.countBuffs = new[] { 0, 0 };
            this.countShowBuffs = new[] { 0, 0 };

            // console.log(sheepMgr.buffs)
            for (var i = 0; i < this.buffs.Length; i++) {
                var r = this.buffs[i];
                var s = i;
                if (r.Count != 0 && r[0].time < this.gameStartTimerForBuff) {
                    r.RemoveAt(0);
                    this.buffs[s] = r;
                }

                foreach (var o in r) {
                    this.countBuffs[s] += o.count != 0 ? o.count : SheepConfig.counterBuffNumber;
                    this.countShowBuffs[s] += o.count;
                }
            }

            for (var i = 0; i < this.preBuffs.Length; i++) {
                var r = this.preBuffs[i];
                var s = i;

                if (r.Count == 0) {
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
                    this.buffs[s].Add(new Buff() {
                        time = (int)(this.gameStartTimerForBuff + 1000 * SheepConfig.counterTime),
                        count = 0
                    });

                    if (r.Count > 1) {
                        this.buffs[s].Add(new Buff() {
                            time = (int)(this.gameStartTimerForBuff + 1000 * SheepConfig.buffLastTime),
                            count = sum
                        });
                    }
                }
                else {
                    this.buffs[s].Add(new Buff() {
                        time = (int)(this.gameStartTimerForBuff + 1000 * SheepConfig.buffLastTime),
                        count = sum
                    });
                }

                this.preBuffs[s] = new List<int>();
                this.countNewBuffs[s] += sum;
            }

            isEnd = this.updateBoss(sheepCtl, dt, now);

            if (isEnd) {
                return (new HashSet<PetView>(), new HashSet<BulletView>());
            }

            var _redBuffCount = 0;
            var _blueBuffCount = 0;


            foreach (var y in this.pets) {
                updateSkinPet(y, sheepCtl, this, this, dt);

                int[] M;
                var D = y;
                var A = D.state;
                var P = D.animType;
                var W = D.animFrame;

                var fgs = sheepCtl.comImages.roles_framess[(int)y.camp];

                var ghg = fgs[(int)y.skinId];

                M = ghg[(int)P];

                if (null == M) {
                    Debug.LogError("找不到动画 " + y.camp + " " + y.skinId + " " + P);
                }

                if (A == SheepRoleState.In && W >= M.Length - 1) {
                    var E = SheepSkill.getById(D.readySkillId);
                    if (E != null) {
                        if (E.skillType == SheepSkillType.Boom) {
                            var F = SheepSkillSubBoom.getById(E.id);
                            D.state = SheepRoleState.Boom;
                            if (F.isAnim != 0) {
                                D.animType = SheepRoleAnimType.Boom;
                            }
                            else {
                                D.animType = SheepRoleAnimType.Idle;
                            }
                        }
                    }
                    else {
                        D.state = SheepRoleState.Move;
                        D.animType = SheepRoleAnimType.Idle;
                    }
                }
                else if (A == SheepRoleState.Dead && W >= M.Length - 1) {
                    D.state = SheepRoleState.Res;
                    D.animType = SheepRoleAnimType.None;
                    del_pet(D);
                }
                else if (A == SheepRoleState.Up && W >= M.Length - 1) {
                    D.state = SheepRoleState.In;
                    D.animType = SheepRoleAnimType.In;
                }
                else if (A == SheepRoleState.Buff) {
                    var V = SheepSkillSubBuff.getById(D.readySkillId);
                    var U = D.animFrame;
                    if (U > V.buffStratFrame && U < V.buffEndFrame) {
                        if (y.camp == SheepCamp.Blue) {
                            _blueBuffCount += 1;
                        }
                        else {
                            _redBuffCount += 1;
                        }
                    }
                }
            }


            foreach (var X in bullets) {
                if (X.isDie) {
                    continue;
                }

                if (X.frame >= X.conf.endFrame) {
                    X.isDie = true;
                    this.del_bullet(X);
                }
                else {
                    var z = X.roleIndex.conf.splitN;

                    for (var O = -z; O <= z; ++O) {
                        for (var Q = -z; Q <= z; ++Q) {
                            var Z = Util.getIndexByXY(X.x + O, X.y + Q);
                            sheepCtl.comImages.mesh_block.addFrameBlockCamp(Z, X.camp);
                        }
                    }
                }
            }


            rebuildGridMap();

            this.redBuffCount = _redBuffCount;
            this.blueBuffCount = _blueBuffCount;


            var del_pets1 = applyDelPets();

            var del_bullets1 = applyDelBullets();

            return (del_pets1, del_bullets1);
        }


        public void update_role() {
            foreach (var dddd in boss) {
                var viewPet = dddd;
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
                }

                viewPet = null;
            }


            foreach (var dddd in pets) {
                var viewPet = dddd;
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
                }

                viewPet = null;
            }
        }

        public void update_bullet() {
            foreach (var t in bullets) {
                if (t.isDie) {
                    continue;
                }

                if (t.id != 0 && t.conf.animId != 0) {
                    var e = t;
                    OnBulletRender?.Invoke(e);
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
                                if (boss[0] == t.tarRoleIndex || boss[1] == t.tarRoleIndex) {
                                    var o1 = t.tarRoleIndex;
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
                            var _ = t.roleIndex;
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
                    var e = t.roleIndex;
                    var i1 = t.tarRoleIndex;
                    createBullet(new BullteCreate() {
                        view_pet = e,
                        bulletId = r,
                        view_tar_pet = i1,
                        info = new Info() { startX = t.x, startY = t.y, startZ = 100 }
                    });
                }
            }
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

        public void update_boss_state(PetView e) {
            switch ((SheepBossState)(int)e.state) {
                case SheepBossState.NomalRun:
                case SheepBossState.AwakeRun:
                case SheepBossState.BackRun:
                    var t = e.conf;
                    var i = e.curAckFrame;
                    if (0 == i) {
                        var (i9, o) = Util.getXnYn(e.posX, e.posY);
                        var l = false;
                        UtilFind.findNearBlocksByAckView(e, i9, o,
                            (int)Math.Floor((double)(t.findR * SheepConfig.loongExaminationRangeBet)), (t8 => {
                                if (!!l) {
                                    return true;
                                }
                                else {
                                    if (!!Util.isCanAckByRole(e, t8)) {
                                        l = true;
                                        return true;
                                    }
                                    else {
                                        return false;
                                    }
                                }
                            }));
                        if (!l) {
                            break;
                        }
                    }

                    i += 1;
                    e.curAckFrame = i;
                    if (i == (int)Math.Floor(t.readyAtks[0] / 3f)) {
                        var (i3, s) = Util.getXnYn(e.posX, e.posY);
                        UtilFind.forfeachBlocksByAckView(e.camp, i3, s, t.findR, t5 => {
                            if (Util.isCanAckByRole(e, t5)) {
                                UtilAck.hurtByRole(e, t5, e.conf.atk);
                            }
                        });
                    }

                    if (i >= Math.Floor(1e3 * t.atkCd / 100)) {
                        e.curAckFrame = 0;
                    }

                    break;
            }
        }

        public void update_role_state_in(PetView petSkin) {
            if (petSkin.conf.skillIn != 0) {
                var t = SheepSkill.getById(petSkin.conf.skillIn);
                if (t.skillType == SheepSkillType.Boom) {
                    var i = SheepSkillSubBoom.getById(t.id);
                    if (1 == petSkin.animFrame) {
                        var t1 = petSkin.camp == SheepCamp.Red ? -1200 : 1200;
                        var xnyn = Util.getXnYn(t1, 0);
                        var o = xnyn.xn;
                        var l = xnyn.yn;
                        PetView n = null;
                        UtilFind.findNearBlocksByAckView(petSkin, o, l, 100, e => {
                            n = e;
                            return true;
                        });
                        if (n != null) {
                            petSkin.posBefX = petSkin.posX;
                            petSkin.posBefY = petSkin.posY;
                            petSkin.posX = n.posX;
                            petSkin.posY = n.posY;
                            petSkin.animX = petSkin.posX;
                            petSkin.animY = petSkin.posY;
                        }
                        else {
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

            if (s != null) {
                petSkin.state = SheepRoleState.Attack;
                petSkin.subState = SheepRoleSubState.AttackAwait;
                return;
            }

            if (o != null) {
                petSkin.subState = SheepRoleSubState.MoveTar;
                Util.moveTar(petSkin, o, i, t);
                return;
            }

            if (l != null) {
                petSkin.subState = SheepRoleSubState.MoveBoss;
                Util.moveTar(petSkin, l, i, t);
                return;
            }

            Debug.LogError("移动状态没有目标??");
        }

        public void update_role_state_attack(PetView petSkin, bool t, float i) {
            var o = petSkin.conf.atkMoveType;
            if (petSkin.conf.isLoongStopDistance != 0) {
                var t3 = sheepMode;
                var i1 = petSkin.conf.loongStopDistanceR;
                if (Util.dis(petSkin.posX, petSkin.posY, petSkin.camp == SheepCamp.Red ? t3.loongX : -t3.loongX, 0) <=
                    i1) {
                    o = (int)SheepRoleAtkMoveType.None;
                }
            }

            if (petSkin.subState == SheepRoleSubState.AttackAwait) {
                if (!Util.isAtkCd(petSkin)) {
                    petSkin.subState = SheepRoleSubState.AttackAnim;
                    petSkin.animType = SheepRoleAnimType.Attack;
                }
            }
            else if (petSkin.subState == SheepRoleSubState.AttackAnim) {
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
                        }
                        else if (petSkin.conf.atkType == SheepRoleAtkType.Throw) {
                            i5 = UtilFind.findSortAck(petSkin, petSkin.conf.findR);
                            if (petSkin.conf.roleType == SheepRoleType.pao_che) {
                                var t6 = Util.getBackBoss(petSkin.camp);
                                if (Util.isCanAckByRole(petSkin, t6)) {
                                    i5 = t6;
                                }
                            }
                        }
                        else {
                            i5 = UtilFind.findNearAck(petSkin);
                        }

                        if (t3.bullet != null && 0 != t3.bullet.Length) {
                            if (i5 != null) {
                                createBullet(new BullteCreate() {
                                    view_pet = petSkin,
                                    bulletId = t3.bullet[petSkin.camp == SheepCamp.Red ? 0 : 1],
                                    view_tar_pet = i5
                                });
                            }
                            else {
                                createBullet(new BullteCreate() {
                                    view_pet = petSkin,
                                    bulletId = t3.bullet[petSkin.camp == SheepCamp.Red ? 0 : 1]
                                });
                            }
                        }
                        else {
                            if (i5 != null) {
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
                    if (t5 != null) {
                        petSkin.subState = SheepRoleSubState.AttackAwait;
                        petSkin.animType = SheepRoleAnimType.Idle;
                        return;
                    }

                    if (i5 != null) {
                        petSkin.state = SheepRoleState.Move;
                        petSkin.subState = SheepRoleSubState.MoveTar;
                        petSkin.animType = SheepRoleAnimType.Idle;
                        return;
                    }

                    if (s != null) {
                        petSkin.state = SheepRoleState.Move;
                        petSkin.subState = SheepRoleSubState.MoveBoss;
                        petSkin.animType = SheepRoleAnimType.Idle;
                        return;
                    }
                }
            }

            if (t && (o == (int)SheepRoleAtkMoveType.Move || o == (int)SheepRoleAtkMoveType.CdMove &&
                    petSkin.subState == SheepRoleSubState.AttackAwait)) {
                var s = UtilFind.findNearAck(petSkin);
                if (s != null && Util.disByRole(petSkin, s) > petSkin.conf.atkMinMoveR + s.conf.collideR) {
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

                if (i3) {
                    Debug.LogWarning("刺客被中断，直接回到移动状态");
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return;
                }

                var o = UtilFind.findFarAck(petSkin, t.findR);
                if (o != null) {
                    petSkin.logicMove(o.posX, o.posY);
                }
                else {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                }
            }

            if (i == t.atkFrame) {
                UtilAck.ackMe(petSkin, t.spiltRadiusBet, t.atkBet, t.atkFindR);
            }

            if (i >= t.endFrame) {
                var i1 = (int)petSkin.subState;
                if (i1 == (int)SheepRoleSubState.KillerEnd || i1 - (int)SheepRoleSubState.KillerStart >= t.cnt) {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return;
                }

                petSkin.subState = (SheepRoleSubState)((int)i1 + 1);
                petSkin.animType = SheepRoleAnimType.Killer;
            }
        }

        public void update_role_state_boom(PetView petSkin) {
            var t = SheepSkill.getById(petSkin.readySkillId);
            var i = SheepSkillSubBoom.getById(t.id);
            var s = petSkin.animFrame;
            if (s == i.atkFrame) {
                var t1 = new List<SheepRoleType>();
                if (petSkin.conf.roleType != SheepRoleType.chong_feng_bing &&
                    petSkin.conf.roleType != SheepRoleType.qi_lin) {
                }
                else {
                    t1.Add(SheepRoleType.qi_lin);
                }

                UtilAck.ackMe(petSkin, i.spiltRadiusBet, i.atkBet, i.atkFindR, i.hitBackDistance, t1);
            }

            if (s >= i.endFrame) {
                petSkin.isLock = false;
                if (i.endState == (int)SheepRoleState.Move) {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                }
                else if (i.endState == (int)SheepRoleState.Rigidity) {
                    petSkin.state = SheepRoleState.Rigidity;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    petSkin.readySkillId = i.endSkill;
                }
                else {
                    if (i.endState == (int)SheepRoleState.Dead) {
                        petSkin.isDie = true;
                        petSkin.state = SheepRoleState.Dead;
                    }
                    else if (i.endState == (int)SheepRoleState.Palm) {
                        petSkin.state = SheepRoleState.Palm;
                        petSkin.subState = SheepRoleSubState.Palm;
                        petSkin.animType = SheepRoleAnimType.Palm;
                        petSkin.readySkillId = i.endSkill;
                    }
                    else {
                        Debug.LogError("endState错误");
                    }
                }
            }
        }

        public void update_role_state_invincible(PetView petSkin) {
            var t = petSkin.animFrame;
            var i = SheepSkill.getById(petSkin.readySkillId);
            var s = SheepSkillSubInvincible.getById(i.id);
            var o = s.healFrames;
            foreach (var i1 in o) {
                if (t == i1) {
                    var t3 = (float)Math.Floor((petSkin.conf.hp - petSkin.curHp) * (s.healHealthPercent / 100f));
                    UtilAck.hurtByRole(petSkin, petSkin, -t3);
                    break;
                }
            }

            var l = s.atkFrames;
            foreach (var i2 in l) {
                if (t == i2) {
                    UtilAck.ackMe(petSkin, s.spiltRadiusBet, s.atkBet, s.atkFindR);
                    break;
                }
            }

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
                var s1 = fff.moveTar;
                var o1 = fff.moveBoss;
                PetView n = null;
                if (t1 != null) {
                    n = t1;
                }
                else if (s1 != null) {
                    n = s1;
                }
                else if (o1 != null) {
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

            if (s >= l.endFrame) {
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
            foreach (var i in o1) {
                if (t1 == i) {
                    var t = (float)Math.Floor((petSkin.conf.hp - petSkin.curHp) * (s.healHealthPercent / 100f));
                    UtilAck.hurtByRole(petSkin, petSkin, -t);
                    break;
                }
            }

            var l1 = s.atkFrames;
            foreach (var i in l1) {
                if (t1 == i) {
                    UtilAck.ackMe(petSkin, s.spiltRadiusBet, s.atkBet, s.atkFindR);
                    break;
                }
            }

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
            if (s.frameStep != 0) {
                if (t % s.frameStep == 0) {
                    o = s.frameCnt;
                }
            }
            else {
                var e = s.callFrames;
                for (var i1 = 0; i1 < e.Length; i1++) {
                    if (t == e[i1]) {
                        o = s.callCnts[i1];
                        break;
                    }
                }
            }

            if (o != 0) {
                for (var t1 = 0; t1 < o; t1++) {
                    if (1 == s.type) {
                        var t3 = petSkin.posX + s.startOffsetPos[0];
                        var i3 = petSkin.posY + s.startOffsetPos[1];
                        var o3 = s.startOffsetPos[2];
                        var l = 360 * RandomFloat(0f, 1f);
                        var n = petSkin.posX + petSkin.dirX * s.len + s.endRadius * Math.Cos(l);
                        var r = petSkin.posY + petSkin.dirY * s.len + s.endRadius * Math.Sin(l);
                        var a = 0;
                        createBullet(new BullteCreate() {
                            view_pet = petSkin,
                            bulletId = s.bullet,
                            info = new Info()
                                { startX = t3, startY = i3, startZ = o3, endX = (float)n, endY = (float)r, endZ = a }
                        });
                    }
                    else if (2 == s.type) {
                        var t4 = s.startOffsetPos[2];
                        var i5 = 360 * RandomFloat(0f, 1f);
                        var o5 = petSkin.posX + petSkin.dirX * s.len + s.endRadius * Math.Cos(i5);
                        var l = petSkin.posY + petSkin.dirY * s.len + s.endRadius * Math.Sin(i5);
                        var n = 0;
                        createBullet(new BullteCreate() {
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
                    }
                    else if (3 == s.type) {
                        createBullet(new BullteCreate() {
                            view_pet = petSkin,
                            bulletId = s.bullet,
                            info = new Info() { dirX = 0, dirY = 0, dirZ = -1, angle = 360f / o * t1 }
                        });
                    }
                    else if (4 == s.type) {
                        createBullet(new BullteCreate() {
                            view_pet = petSkin,
                            bulletId = s.bullet,
                            info = new Info() { dirX = 1, dirY = 0, dirZ = 0 }
                        });
                    }
                    else {
                        createBullet(new BullteCreate() { view_pet = petSkin, bulletId = s.bullet });
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
            if (t >= SheepSkillSubBuff.getById(i.id).endFrame) {
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

                if (t1 != null) {
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
                    this.update_role_state_attack(petSkin, t, i);
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
                    this.update_role_state_bladestorm(petSkin, t, i);
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
                    this.update_role_state_spinatk(petSkin, t, i);
                    break;
            }

            if (petSkin.impulseX != 0 || petSkin.impulseY != 0) {
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
                        var o3 = new Vector3() { x = petSkin.posX, y = petSkin.posY };
                        var l3 = new Vector3() { x = t3 * r * s, y = i3 * r * s };
                        var n3 = new Vector3() { x = o3.x + l3.x, y = o3.y + l3.y };
                        petSkin.logicMove(n3.x, n3.y);
                    }
                    else {
                        petSkin.logicMove(o, l);
                    }
                }
            }
            else if (petSkin.conf.skillSpurt != 0) {
                var t1 = SheepSkill.getById(petSkin.conf.skillSpurt);
                if (t1.skillType == SheepSkillType.Charge) {
                    petSkin.state = SheepRoleState.Charge;
                    petSkin.subState = SheepRoleSubState.Spurt;
                    petSkin.animType = SheepRoleAnimType.Spurt;
                }
                else if (t1.skillType == SheepSkillType.SpinSpurt) {
                    petSkin.state = SheepRoleState.SpinSpurt;
                    petSkin.animType = SheepRoleAnimType.Attack;
                }
                else {
                    petSkin.state = SheepRoleState.Spurt;
                    petSkin.subState = SheepRoleSubState.Spurt;
                    if (petSkin.conf.isSpurtAnim) {
                        petSkin.animType = SheepRoleAnimType.Spurt;
                    }
                    else {
                        petSkin.animType = SheepRoleAnimType.Idle;
                    }
                }
            }
            else {
                petSkin.state = SheepRoleState.Spurt;
                petSkin.subState = SheepRoleSubState.Spurt;
                if (petSkin.conf.isSpurtAnim) {
                    petSkin.animType = SheepRoleAnimType.Spurt;
                }
                else {
                    petSkin.animType = SheepRoleAnimType.Idle;
                }
            }
        }

        public void update_role_state_charge(PetView e, bool t, float i) {
            var o = e.posX;
            var l = e.posY;
            var (n, r) = Util.getXnYn(o, l);
            if (e.camp == SheepCamp.Red && e.posX > e.conf.runEndX ||
                e.camp == SheepCamp.Blue && e.posX < -e.conf.runEndX) {
                var t6 = false;
                UtilFind.findNearBlocksByAckView(e, n, r, 5, i8 => {
                    if (i8.isDie || i8.camp == e.camp || 0 == i8.roleId) {
                    }
                    else {
                        t6 = true;
                    }

                    return t6;
                });
                if (t6) {
                    e.state = SheepRoleState.Boom;
                    e.subState = SheepRoleSubState.Boom;
                    var t3 = SheepSkillSubCharge.getById(e.conf.skillSpurt);
                    var i3 = SheepSkillSubBoom.getById(t3.endSkill);
                    if (i3.isAnim != 0) {
                        e.animType = SheepRoleAnimType.Boom;
                    }
                    else {
                        e.animType = SheepRoleAnimType.Idle;
                    }

                    e.readySkillId = i3.id;
                }
                else {
                    e.state = SheepRoleState.Move;
                    e.subState = SheepRoleSubState.MoveBoss;
                    e.animType = SheepRoleAnimType.Idle;
                }
            }
            else {
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
                    if (i8.isAnim != 0) {
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
            var (n, r) = Util.getXnYn(o, l);
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
            if (e.camp == SheepCamp.Red && e.posX > e.conf.runEndX ||
                e.camp == SheepCamp.Blue && e.posX < -e.conf.runEndX) {
                e.state = SheepRoleState.Boom;
                e.subState = SheepRoleSubState.Boom;
                var t1 = SheepSkillSubSpinSpurt.getById(e.conf.skillSpurt);
                var i1 = SheepSkillSubBoom.getById(t1.endSkill);
                if (i1.isAnim != 0) {
                    e.animType = SheepRoleAnimType.Boom;
                }
                else {
                    e.animType = SheepRoleAnimType.Idle;
                }

                e.readySkillId = i1.id;
            }
            else {
                Util.moveTar(e, null, i, t);
                UtilFind.forNearBlocksByAckView(e, n, r, e.conf.findR,
                    t2 => {
                        if (t2.isDie || t2.camp == e.camp || 0 == t2.roleId || !Util.isCanAckByRole(e, t2)) {
                            return false;
                        }

                        UtilAck.ackTar(e, t2);
                        return false;
                    });
            }
        }

        public void update_role_state_spurt(PetView e, bool t, float i) {
            if (e.conf.skillSpurt != 0) {
                var s = SheepSkill.getById(e.conf.skillSpurt);
                if (s.skillType == SheepSkillType.Boom) {
                    var o = SheepSkillSubBoom.getById(s.id);
                    var fff = UtilFind.findTar(e);
                    var l = fff.atkTar;
                    var n = fff.moveTar;
                    var r = fff.moveBoss;

                    if (l != null || r != null) {
                        e.state = SheepRoleState.Boom;
                        e.subState = SheepRoleSubState.Boom;
                        if (o.isAnim != 0) {
                            e.animType = SheepRoleAnimType.Boom;
                        }
                        else {
                            e.animType = SheepRoleAnimType.Idle;
                        }

                        e.readySkillId = o.id;
                        return;
                    }

                    Util.moveTar(e, null, i, t);
                }
                else if (s.skillType == SheepSkillType.Killer) {
                    var o = SheepSkillSubKiller.getById(s.id);
                    var fff = UtilFind.findTar(e);

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
                }
                else if (s.skillType == SheepSkillType.Bullet) {
                    var o = SheepSkillSubBullet.getById(s.id);
                    var fff = UtilFind.findTar(e);
                    var l = fff.atkTar;
                    var n = fff.moveTar;
                    var r = fff.moveBoss;

                    if (l != null || n != null || r != null) {
                        createBullet(new BullteCreate() {
                            view_pet = e,
                            bulletId = o.bullet
                        });
                    }

                    if (l != null) {
                        e.state = SheepRoleState.Attack;
                        e.subState = SheepRoleSubState.AttackAwait;
                        return;
                    }

                    if (n != null) {
                        e.state = SheepRoleState.Move;
                        e.subState = SheepRoleSubState.MoveTar;
                        Util.moveTar(e, n, i, t);
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
                }
                else if (s.skillType == SheepSkillType.CallBullets) {
                    var o = SheepSkillSubCallBullets.getById(s.id);
                    var fff = UtilFind.findTar(e);
                    var l = fff.atkTar;
                    var n = fff.moveTar;
                    var r = fff.moveBoss;

                    if (l != null || r != null) {
                        e.state = SheepRoleState.CallBullets;
                        e.subState = SheepRoleSubState.CallBullets;
                        if (o.isAnim != 0) {
                            e.animType = SheepRoleAnimType.CallBullets;
                        }
                        else {
                            e.animType = SheepRoleAnimType.Idle;
                        }

                        e.readySkillId = o.id;
                        return;
                    }

                    Util.moveTar(e, null, i, t);
                }
            }
            else {
                var fff = UtilFind.findTar(e);
                var s = fff.atkTar;
                var o = fff.moveTar;
                var l = fff.moveBoss;

                if (s != null) {
                    e.state = SheepRoleState.Attack;
                    e.subState = SheepRoleSubState.AttackAwait;
                    return;
                }

                if (o != null) {
                    e.state = SheepRoleState.Move;
                    e.subState = SheepRoleSubState.MoveTar;
                    Util.moveTar(e, o, i, t);
                    return;
                }

                if (l != null) {
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
            OnRoleRender?.Invoke(e);
        }

        public void produce_pets(int typeID, int count, SheepCamp camp) {
            // 根据阵营 获取 map
            var callInfos = camp == SheepCamp.Red ? this.redCallInfos : this.blueCallInfos;

            // 没有就创建一个 然后加进去
            if (!callInfos.TryGetValue(typeID, out SheepCallInfo sheepCallInfo)) {
                sheepCallInfo = new SheepCallInfo();
                sheepCallInfo.camp = camp;
                sheepCallInfo.type = typeID;
                sheepCallInfo.count = 0;
                sheepCallInfo.frame = 0;
                sheepCallInfo.count_line = 0;

                sheepCallInfo.items = new int[] { };
                sheepCallInfo.pets = new List<SheepCallInfoPet>();
                callInfos[typeID] = sheepCallInfo;
            }

            //  爆炸是什么意思? 用什么用?
            sheepCallInfo.count += count;
            sheepCallInfo.pets.Add(new SheepCallInfoPet() { camp = camp, count = count });
        }

        public void consume(float t) {
            var o = this;

            this.autoTime += t;

            if (this.isAutoCall && this.autoTime > SheepConfig.systemAutomaticTroopsIntervalTime) {
                this.autoTime = 0;
                if (this.pets.Count < SheepConfig.systemLongerAutomaticallyDispatch) {
                    foreach (var e in new SheepCamp[] { SheepCamp.Red, SheepCamp.Blue }) {
                        if (getPetCount(e) < SheepConfig.systemAutomaticallyMaxTroops) {
                            o.produce_pets(SheepConfig.WarmUpID, SheepConfig.systemAutomaticallyTroopsOneNumber, e);
                        }
                    }
                }
            }

            foreach (var t1 in new[] { this.redCallInfos, this.blueCallInfos }) {
                var n = t1 == o.redCallInfos ? SheepCamp.Red : SheepCamp.Blue;

                foreach (var ee in t1.ToArray()) {
                    var o1 = ee.Value;
                    var a = ee.Key;

                    if (o1.count <= 0) {
                        continue;
                    }

                    var c = SheepRoleTypeInfo.getById(a);
                    var formation = SheepRoleFormation.getById(c.formationId);

                    var u = petCounts[(int)n][(int)c.roleType];

                    if (c.roleType == SheepRoleType.xiao_bing) {
                        if (u > 14500) {
                            continue;
                        }
                    }
                    else if (c.roleType == SheepRoleType.ci_ke && u > 9500) {
                        continue;
                    }

                    o1.frame += 1;

                    // 限制每帧生成的单位
                    if (o1.frame <= formation.frameItemX && false) {
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
                                }
                                else {
                                    M = (float)(-m * T - m / 2.0 - Math.Floor(T / d + 1) * g);
                                }
                            }
                            else {
                                if (I % 2 == 0) {
                                    M = (float)(m * Math.Floor(T) + Math.Floor(T / d) * g);
                                }
                                else {
                                    M = (float)(-m * Math.Floor(T + 1) - Math.Floor(T / d) * g);
                                }
                            }

                            var C = pets[0];
                            C.count -= 1;
                            o1.count -= 1;
                            I += 1;

                            var R = new Vector3(S, M, 0f);
                            if (C.booms != null && C.booms.Count > 0) {
                                createPetView(C.camp, a, h, R, C.booms.Pop());
                            }
                            else {
                                createPetView(C.camp, a, h, R, false);
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

                        if (o1.count <= 0) {
                            t1.Remove(a);
                        }
                    }
                    else if (formation.formationType == SheepRoleFormationType.AngleTidy) {
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

                                var E = (b % 2 == 0 ? 1 : -1) *
                                        (Math.Floor(b % G / 2) * formation.startStepAngle + formation.minAngle);

                                var x = Math.Cos(E * Math.PI / 180) * N;
                                float F = (float)(Math.Sin(E * Math.PI / 180) * N);
                                var X = new Vector3();
                                var L = sheepMode.loongX;
                                X = n == SheepCamp.Red
                                    ? new Vector3((float)(L - x), F, 0)
                                    : new Vector3((float)(x - L), F, 0);
                                if (P.booms != null && P.booms.Count > 0) {
                                    createPetView(P.player, a, (int)G, X, P.booms.Pop());
                                }
                                else {
                                    createPetView(P.camp, a, (int)G, X, false);
                                }
                            }

                            if (P.count <= 0) {
                                pets.RemoveAt(0);
                            }
                        }
                    }
                }
            }
        }

        public Vector3 getPetStartEndPos(int petId, SheepCamp camp) {
            var petStartCount = this.petStartCounts[(int)camp];
            var a = petStartCount.GetValueOrDefault(petId, 0);
            petStartCount[petId] = a + 1;
            var sheepRoleTypeInfo = SheepRoleTypeInfo.getById(petId);
            if (sheepRoleTypeInfo == null) {
                Debug.LogError("SheepMgr.getPetStartEndPos roleId=" + petId + " not found");
            }

            var sheepRoleFormation = SheepRoleFormation.getById(sheepRoleTypeInfo.formationId);
            if (sheepRoleFormation == null) {
                Debug.LogError("SheepMgr.getPetStartEndPos formationId=" + sheepRoleTypeInfo.formationId +
                               " not found");
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
                }
                else {
                    d = (int)(-u * Math.Floor(m / 2) - u / 2);
                }
            }
            else {
                if (m % 2 == 0) {
                    d = (int)(u * Math.Floor(m / 2));
                }
                else {
                    d = (int)(-u * Math.Floor(m / 2 + 1));
                }
            }

            if (camp == SheepCamp.Red) {
                h *= -1;
            }

            return new Vector3((float)h, d + RandomFloat(-1, 1), 0);
        }

        public void clearCallPets() {
            redCallInfos.Clear();
            blueCallInfos.Clear();
        }


        // 创建 单位, 下一帧才会生效
        public void createPetView(SheepCamp camp, int roleType, int a, Vector3 f, bool s) {
            if (this.state != SheepRoomState.Run && this.state != SheepRoomState.Start) {
                return;
            }

            var sheepRoleTypeInfo = SheepRoleTypeInfo.getById(roleType);

            var petSkin = new PetView();

            petSkin.conf = sheepRoleTypeInfo;
            petSkin.camp = camp;
            petSkin.petId = roleType;
            petSkin.isDie = false;
            petSkin.scale = petSkin.conf.scale;
            petSkin.isBoom = s; //  这里不能写死

            petSkin.attacher = new BuffTimeAttacher();

            petSkin.skinId = petSkin.conf.animId;


            var formation = SheepRoleFormation.getById(sheepRoleTypeInfo.formationId);

            if (formation.formationType == SheepRoleFormationType.AngleRandom) {
                var T = Math.Min((a / (float)formation.angleDensity + formation.baseTimes) * formation.startAngle,
                    formation.maxAngle);
                T = RandomFloat(-T, T);
                T += T > 0 ? formation.minAngle : -formation.minAngle;

                var A = formation.startR + sheepMode.startAddR;
                var H = Math.Cos(T * Math.PI / 180) * A;
                var P = Math.Sin(T * Math.PI / 180) * A;
                var M = sheepMode.loongX;

                if (petSkin.camp == SheepCamp.Red) {
                    var x = new Vector3((float)(M - H), (float)P, 0);
                    petSkin.position = x;
                }
                else {
                    var D = new Vector3((float)(H - M), (float)P, 0);
                    petSkin.position = D;
                }
            }
            else if (formation.formationType == SheepRoleFormationType.RectangleRandom) {
                var F = Math.Min((a / (float)formation.density + formation.baseTimes) * formation.startScope,
                    formation.maxScope);
                var N = RandomFloat(-F, F);
                if (formation.minScope != 0) {
                    N += N > 0 ? formation.minScope : -formation.minScope;
                }

                var W = 0f;
                var E = formation.startX + sheepMode.startAddX;
                W = camp == SheepCamp.Red ? -Math.Abs(E) : Math.Abs(E);
                var O = new Vector3(W, N, 0f);
                petSkin.position = O;
            }
            else {
                petSkin.position = f;
            }


            Vector3 p1 = petSkin.position.Value;
            int x7 = Mathf.FloorToInt(p1.x);
            int y7 = Mathf.FloorToInt(p1.y);

            int blockIndex = this.getBlockIndex(new Vector3(x7, y7, 0));

            petSkin.id = this.getNextPetId();

            petSkin.isActive = true;
            petSkin.isDie = false;
            petSkin.roleId = petSkin.petId;

            if (petSkin.petId != 0) {
                if (this.state == SheepRoomState.Start) {
                    petSkin.state = SheepRoleState.Start;
                    petSkin.subState = SheepRoleSubState.Start;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    petSkin.animFrame = this.RandomInt(0, 10);
                }
                else if (petSkin.conf.skillIn != 0) {
                    petSkin.state = SheepRoleState.In;
                    petSkin.subState = SheepRoleSubState.In;
                    petSkin.animType = SheepRoleAnimType.In;
                    petSkin.animFrame = 0;
                }
                else if (petSkin.conf.startState == SheepRoleState.In) {
                    petSkin.state = petSkin.conf.startState;
                    petSkin.subState = SheepRoleSubState.In;
                    petSkin.animType = SheepRoleAnimType.In;
                    petSkin.animFrame = 0;
                }
                else if (petSkin.conf.startState == SheepRoleState.SpinSpurt) {
                    petSkin.state = petSkin.conf.startState;
                    petSkin.animType = SheepRoleAnimType.Attack;
                    petSkin.animFrame = 0;
                }
                else {
                    petSkin.state = petSkin.conf.startState;
                    petSkin.subState = SheepRoleSubState.Spurt;

                    if (petSkin.conf.isSpurtAnim) {
                        petSkin.animType = SheepRoleAnimType.Spurt;
                        petSkin.animFrame = this.RandomInt(0, 10);
                    }
                    else {
                        petSkin.animType = SheepRoleAnimType.Idle;
                        petSkin.animFrame = this.RandomInt(0, 10);
                    }
                }
            }

            petSkin.frame = 0;
            petSkin.posBefX = x7;
            petSkin.posBefY = y7;
            petSkin.animX = x7;
            petSkin.animY = y7;
            petSkin.posX = x7;
            petSkin.posY = y7;
            petSkin.befBlockIndex = blockIndex;
            petSkin.blockIndex = blockIndex;

            if (petSkin.petId != 0 && this.state == SheepRoomState.Start) {
                var m = this.getPetStartEndPos(petSkin.petId, petSkin.camp);

                petSkin.tarPosX = m.x;
                petSkin.tarPosY = m.y;
                petSkin.animY = m.y;
                petSkin.posBefY = m.y;
                petSkin.posY = m.y;
            }

            var roleFormation = SheepRoleFormation.getById(petSkin.conf.formationId);
            float d7 = petSkin.camp == SheepCamp.Red ? 1 : -1;

            if (roleFormation.formationType == SheepRoleFormationType.RectangleTidy ||
                roleFormation.formationType == SheepRoleFormationType.RectangleRandom) {
                petSkin.dirX = d7;
                petSkin.dirY = 0;
            }
            else if (roleFormation.formationType == SheepRoleFormationType.AngleTidy ||
                     roleFormation.formationType == SheepRoleFormationType.AngleRandom) {
                Vector3 g = new Vector3(
                    d7 * sheepMode.loongX - x7,
                    0 - y7,
                    0
                ).normalized;

                petSkin.dirX = g.x;
                petSkin.dirY = g.y;
            }

            if (petSkin.petId != 0 && this.state == SheepRoomState.Start) {
                petSkin.isConnNot = true;
            }
            else {
                petSkin.isConnNot = false;
            }

            petSkin.tarIndex = -1;
            petSkin.tarId = -1;
            petSkin.curHp = petSkin.conf.hp;
            petSkin.curAtkBuff = 0;

            if (petSkin.isBoom) {
                petSkin.isConnNot = true;
                petSkin.isBoom = true;
            }
            else {
                petSkin.isBoom = false;
            }

            foreach (var b1 in this.buffs) {
                foreach (var b2 in b1) {
                    double time = (b2.time - this.gameStartTimerForBuff) / 1e3;

                    int r = b2.count;

                    addGeneralOrderBuff(petSkin, petSkin, time, r);
                }
            }

            if (this.state == SheepRoomState.Start && petSkin.conf.roleType == SheepRoleType.yang_shen) {
                this.god_view_pets.Add(petSkin);
            }

            this.addPrePet(petSkin);

            petSkin.pos = petSkin.position;
        }


        // 生成 子弹 下一帧才会生效
        public void createBullet(BullteCreate tttt) {
            int bulletId = tttt.bulletId;
            PetView view_pet = tttt.view_pet;
            PetView view_tar_pet = tttt.view_tar_pet;
            Info l = tttt.info;

            var n = SheepBullet.getById(bulletId);
            var r = view_pet != null ? view_pet.camp == SheepCamp.Red ? n.startOffsetX : -n.startOffsetX : 0;
            var preBullet = new BulletView();
            preBullet.id = getNextBulletId();
            preBullet.bulletId = bulletId;
            preBullet.roleUid = view_pet != null ? view_pet.id : 0;
            preBullet.roleIndex = view_pet;


            preBullet.camp = view_pet != null ? view_pet.camp : l.camp;
            if (view_tar_pet != null && 0 == view_tar_pet.roleId) {
                preBullet.tarRoleIndex = view_tar_pet;
            }
            else {
                preBullet.tarRoleIndex = null;
            }

            if (n.moveType == (int)SheepBulletMoveType.Fixed) {
                var t = l != null && l.startX != 0 ? l.startX :
                    view_pet != null && view_pet.posX != 0 ? view_pet.posX : 0;
                preBullet.x = t;
                var s = l != null && l.startY != 0 ? l.startY :
                    view_pet != null && view_pet.posY != 0 ? view_pet.posY : 0;
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
                var t = view_pet != null ? view_pet.posX : l.startX;
                var s = view_pet != null ? view_pet.posY : l.startY;
                var c = view_tar_pet != null ? view_tar_pet.posX : view_pet.tarPosX;
                var f = view_tar_pet != null ? view_tar_pet.posY : view_pet.tarPosY;
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
                    var t = l.endX - l.startX;
                    var i = l.endY - l.startY;
                    var s = l.endZ - l.startZ;
                    var o = Math.Sqrt(t * t + i * i);
                    preBullet.dirX = (float)(t / o);
                    preBullet.dirY = (float)(i / o);
                    preBullet.dirZ = (float)(s / o);
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
                var t = l.endX - l.startX;
                var i = l.endY - l.startY;
                var s = l.endZ - l.startZ;
                var o = Math.Sqrt(t * t + i * i);
                preBullet.dirX = (float)(t / o);
                preBullet.dirY = (float)(i / o);
                preBullet.dirZ = (float)(s / o);
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
            addPreBullet(preBullet);
        }


        public void updateSkinPet(PetView ppp, SheepCtl e, SheepMgr t, SheepMgr n, double o) {
            PetView a = ppp;
            PetView i = a;

            if (i.state == SheepRoleState.Merge) {
                return;
            }

            bool isDie = i.isDie;
            int blockIndex = i.blockIndex;
            float curHp = i.curHp;

            if (isDie) {
                return;
            }

            if (curHp <= 0) {
                isDie = true;
                i.isDie = isDie;
                i.state = SheepRoleState.Dead;
            }

            if (isDie) {
                i.state = SheepRoleState.Dead;
                i.subState = SheepRoleSubState.Dead;

                if (i.conf.roleType != SheepRoleType.qi_lin) {
                    i.animType = SheepRoleAnimType.Dead;
                }

                if (i.conf.deadAnimType != null && i.conf.deadAnimType.Length != 0) {
                    i.animType = (SheepRoleAnimType)arrOn(i.conf.deadAnimType);
                }

                if (i.conf.roleType == SheepRoleType.xiao_bing) {
                    i.animFrame = RandomInt(0, 10);
                }

                ppp.isDie = true;
                // this.id = 0;
                ppp.attacher.clear();
            }

            if (!isDie) {
                int S = i.conf.detectCollideR;

                for (int y = -S; y <= S; ++y) {
                    for (int v = -S; v <= S; ++v) {
                        e.comImages.mesh_block.addFrameBlockCamp(blockIndex, ppp.camp);
                    }
                }

                Vector3 B = new Vector3(i.animX, i.animY, 0);

                a.position = B;
            }

            if (!isDie) {
                int countNewBuff = n.countNewBuffs[(int)ppp.camp];

                if (countNewBuff != 0) {
                    addGeneralOrderBuff(ppp, i, SheepConfig.buffLastTime, countNewBuff);
                }
            }

            ppp.attacher.updateTimer(o / 1e3);
        }

        public void addGeneralOrderBuff(PetView ppp, PetView e, double t, int n) {
            PetView o = ppp;

            ppp.attacher.addIndependBuff(
                BuffID.GeneralOrder,
                t,
                buff => {
                    int addHp = (int)Math.Floor(
                        o.conf.hp *
                        SheepConfig.buffHpIncreaseRate *
                        n
                    );

                    float addAtk =
                        n *
                        SheepConfig.buffAtkIncreaseRate *
                        100;

                    buff.arg = (
                        addHp: addHp,
                        addAtk: addAtk
                    );

                    var arg = ((int addHp, float addAtk))buff.arg;

                    e.curHp += arg.addHp;
                    e.curAtkBuff += arg.addAtk;
                },
                buff => {
                    var arg = ((int addHp, float addAtk))buff.arg;

                    e.curHp -= arg.addHp;
                    e.curAtkBuff -= arg.addAtk;
                }
            );
        }

        public static long NowMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public int RandomInt(
            int minInclusive,
            int maxExclusive) {
            return logicRandom.Next(
                minInclusive,
                maxExclusive
            );
        }

        public float Random01() {
            return (float)logicRandom.NextDouble();
        }

        public float RandomFloat(
            float minInclusive,
            float maxExclusive) {
            return minInclusive +
                   (maxExclusive - minInclusive) *
                   (float)logicRandom.NextDouble();
        }

        public T arrOn<T>(T[] r) {
            return r[RandomInt(0, r.Length)];
        }
    }
}