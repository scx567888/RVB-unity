using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace rvb.scripts {
    public class SheepMgr {
        // 是否自动出兵
        public bool isAutoCall = true;

        // 自动出兵计时器
        public int autoTime = 0;

        // 游戏模式 (外部会设置)
        public int gameMode = 0;

        // 时间模式 (外部会设置)
        public int timeMode = 2;

        // boss 血量 (外部会设置)
        public int loongHp = 10000;

        // 红蓝 boss
        public PetView[] boss;

        // 地块比例
        public float plotRatio = 0.5f;

        // 核心状态机
        public SheepRoomState state = SheepRoomState.Ready;

        // 尝试角色 (todo 但是是哪一种? 当前在场上的? )
        public List<PetView>[] pets;

        public int gameIndex = 0;
        public int gameStartTimerForBuff = 0;
        public Vector3 cameraEulerAngles = new Vector3();
        public int endTime = 0;
        public int[][] preBuffs;
        public Buff[][] buffs;
        public int[] countNewBuffs;
        public int[] countBuffs;
        public int[] countShowBuffs;

        // 反击时刻标识符 (防止多次触发反击时刻)
        public bool[] flagLongBuffs;

        public object[] petStartCounts;
        public List<PetView> god_view_pets;
        public PerfStat perfStat;

        public PetView[] view_pets;
        public BulletView[] view_bullets;
        public BulletView[] pre_view_bullets;
        public object updateTime;
        public object petsAdd;
        public object petsDel;
        public object petCount;
        public object bulletsDel;
        public object bulletCount;
        public object bulletId;

        public int[] logic_counts;

        public BullteCreate[] bullte_creates;

        public object pre_blocks;
        public object isChangeCollsionFlags = null;
        public object isChangeAckFlags = null;

        public object MaxCount = SheepConfig.line_w * SheepConfig.line_w;

        public IndexLen[][] attackViews;

        public int[][] attackView1s;

        public IndexLen[][][] collisionViews;


        public int[][][] collisionView1s;


        /**
         * 红方召唤池
         * key 是 类型 id
         * @type {Map<Number,SheepCallInfo>}
         */
        public Dictionary<int, SheepCallInfo> redCallInfos;

        /**
         * 蓝方召唤池
         * key 是 类型 id
         * @type {Map<Number,SheepCallInfo>}
         */
        public Dictionary<int, SheepCallInfo> blueCallInfos;

        /**
         * 是否自动出兵
         * @type {boolean}
         */


        // ************************ 以下待整理 **************************

        /**
         * @type ComSheepImages
         */
        public object comImages;

        public int cur_rob_role_index;
        public int cur_rob_bullet_index;
        public int cur_rob_role_mesh_index;
        public int cur_rob_bullet_mesh_index;
        public int cur_rob_star_mesh_index;
        public int roleMaxIndex;
        public int bulletMaxIndex;
        public int preBulletIndex;
        public object curIndexImages;
        public int redBuffCount;
        public int blueBuffCount;

        // 角色 id 分配器
        public int petId;


        public SheepMgr() {
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
            this.boss = new PetView[] { null, null };

            // 地块比例
            this.plotRatio = 0.5f;

            // 核心状态机
            this.state = SheepRoomState.Ready;

            // 尝试角色 (todo 但是是哪一种? 当前在场上的? )
            this.pets = new object[] { null, null };

            this.gameIndex = 0;
            this.gameStartTimerForBuff = 0;
            this.cameraEulerAngles = new Vector3();
            this.endTime = 0;
            this.preBuffs = null;
            this.buffs = null;
            this.countNewBuffs = null;
            this.countBuffs = null;
            this.countShowBuffs = null;

            // 反击时刻标识符 (防止多次触发反击时刻)
            this.flagLongBuffs = null;

            this.petStartCounts = null;
            this.god_view_pets = null;
            this.perfStat = null;

            this.view_pets = null;
            this.view_bullets = null;
            this.pre_view_bullets = null;
            this.updateTime = null;
            this.petsAdd = null;
            this.petsDel = null;
            this.petCount = 0;
            this.bulletsDel = null;
            this.bulletCount = 0;
            this.bulletId = 0;

            this.logic_counts = null;

            this.bullte_creates = null;

            this.pre_blocks = null;
            this.isChangeCollsionFlags = null;
            this.isChangeAckFlags = null;

            this.MaxCount = SheepConfig.line_w * SheepConfig.line_w;

            this.attackViews = null;

            this.attackView1s = null;

            this.collisionViews = null;
            for (var e = 0; e < SheepConfig.MaxGroupCount; e++) {
            }

            this.collisionView1s = null;
            for (var e = 0; e < SheepConfig.MaxGroupCount; e++) {
            }

            /**
             * 红方召唤池
             * key 是 类型 id
             * @type {Map<Number,SheepCallInfo>}
             */
            this.redCallInfos = null;

            /**
             * 蓝方召唤池
             * key 是 类型 id
             * @type {Map<Number,SheepCallInfo>}
             */
            this.blueCallInfos = null;

            /**
             * 是否自动出兵
             * @type {boolean}
             */


            // ************************ 以下待整理 **************************

            /**
             * @type ComSheepImages
             */
            this.comImages = null;

            this.cur_rob_role_index = 0;
            this.cur_rob_bullet_index = 0;
            this.cur_rob_role_mesh_index = 0;
            this.cur_rob_bullet_mesh_index = 0;
            this.cur_rob_star_mesh_index = 0;
            this.roleMaxIndex = 0;
            this.bulletMaxIndex = 0;
            this.preBulletIndex = 0;
            this.curIndexImages = 0;
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
            this.petStartCounts.forEach((function(e, t) {
                e.clear();
            }));
            this.preBuffs = new int[][] {
                new int[] { },
                new int[] { },
            };
            this.buffs = new Buff[][] {
                new Buff[] { },
                new Buff[] { },
            };
            ;
            this.countNewBuffs = new int[] { 0, 0 };
            this.countBuffs = new int[] { 0, 0 };
            this.countShowBuffs = new int[] { 0, 0 };
            this.flagLongBuffs = new bool[] { false, false };
            // todo 待处理
            // var e = SheepCtl.instance;
            // e.comMatch.updateWinloops();
        }

        public void onGameRun() {
            this.god_view_pets.forEach((function(e) {
                e.state = SheepRoleState.Palm, e.subState = SheepRoleSubState.Palm, e.animType =
                    SheepRoleAnimType.Palm, e.readySkillId = 70002
            }));
        }

        public void onGameEnd() {
            this.god_view_pets = new PetView[] { };
        }

        public void setState(SheepRoomState e) {
            this.state = e;
            Debug.Log("房间状态改变" + e);
            // todo 待处理
            // eventBus.emit(EventType.RoomState, {state: e})
        }

        public void addPet(PetView e, SheepCamp camp) {
            this.pets[(int)camp].Add(e);
            if (camp == SheepCamp.Red) {
                this.perfStat.redNums[(int)e.conf.roleType]++;
            }
            else {
                this.perfStat.blueNums[(int)e.conf.roleType]++;
            }
        }

        public void delPet(PetView e) {
            this.pets[(int)SheepCamp.Red].Remove(e);
            this.pets[(int)SheepCamp.Blue].Remove(e);
            if (e.camp == SheepCamp.Red) {
                this.perfStat.redNums[(int)e.conf.roleType]--;
            }
            else {
                this.perfStat.blueNums[(int)e.conf.roleType]--;
            }
        }

        public void clearPets() {
            this.pets[(int)SheepCamp.Red].Clear();
            this.pets[(int)SheepCamp.Blue].Clear();
            this.perfStat.redNums = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            this.perfStat.blueNums = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        public int getBlockIndex(Vector3 e) {
            var t = Math.Floor(e.x / SheepConfig.d + SheepConfig.w / SheepConfig.d / 2);
            var o = Math.Floor(e.y / SheepConfig.d + SheepConfig.h / SheepConfig.d / 2);
            return (int)(t * SheepConfig.line_w + o);
        }

        public int getNextPetId() {
            return ++this.petId;
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
                element.clear();
            }
        }

        /**
    *
    * @param petIndex
    * @returns {PetView}
    */
        public PetView getPetView(int petIndex) {
            if (petIndex < 0 || petIndex >= SheepConfig.MaxPetCount) {
                return null;
            }

            var pet = this.view_pets[petIndex];
            if (pet == null) {
                pet = new PetView(petIndex);
                this.view_pets[petIndex] = pet;
            }

            return pet;
        }

        public void clearViewBullets() {
            foreach (var viewElement in this.view_bullets) {
                viewElement.clear();
            }

            foreach (var viewElement in this.pre_view_bullets) {
                viewElement.clear();
            }
        }

        /**
     *
     * @param e
     * @returns {BulletView}
     */
        public BulletView getBulletView(int e) {
            if (e < 0 || e >= SheepConfig.MaxBulletCount) {
                return null;
            }

            var bullet = this.view_bullets[e];
            if (bullet == null) {
                bullet = new BulletView();
                this.view_bullets[e] = bullet;
            }

            return bullet;
        }

        /**
     *
     * @param e
     * @returns {BulletView}
     */
        public BulletView getBulletPreView(int e) {
            if (e < 0 || e >= SheepConfig.MaxBulletCount) {
                return null;
            }

            var bullet = this.pre_view_bullets[e];
            if (bullet == null) {
                bullet = new BulletView();
                this.pre_view_bullets[e] = bullet;
            }

            return bullet;
        }

        /**
     *
     * @param e
     * @param bulletId
     * @param view_pet {PetView}
     * @param view_tar_pet {PetView}
     * @param l
     */
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

            if (n.moveType == SheepBulletMoveType.Fixed) {
                var t = l && l.startX || view_pet && view_pet.posX || 0;
                preBullet.x = t;
                var s = l && l.startY || view_pet && view_pet.posY || 0;
                preBullet.y = s;

                preBullet.startY = n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.dirX = 0;
                preBullet.dirY = 0;
                preBullet.dirZ = 1;
            }
            else if (n.moveType == SheepBulletMoveType.LineDir) {
                preBullet.x = view_pet.posX + r;
                preBullet.y = view_pet.posY + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.dirX = view_pet.dirX;
                preBullet.dirY = view_pet.dirY;
            }
            else if (n.moveType == SheepBulletMoveType.CurvePosFrame) {
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
            else if (n.moveType == SheepBulletMoveType.DirAngle) {
                preBullet.x = view_pet.posX + r;
                preBullet.y = view_pet.posY + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.dirX = l.dirX;
                preBullet.dirY = l.dirY;
                preBullet.dirZ = l.dirZ;
            }
            else if (n.moveType == SheepBulletMoveType.RadiusAngle) {
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
            else if (n.moveType == SheepBulletMoveType.LineDirEndPos) {
                preBullet.x = l.startX;
                preBullet.y = l.startY;
                preBullet.z = l.startZ;
                preBullet.startX = l.startX;
                preBullet.startY = l.startY;
                preBullet.startZ = l.startZ;
                preBullet.endX = l.endX;
                preBullet.endY = l.endY;
                preBullet.endZ = l.endZ;
                if (l.dirX || l.dirY || l.dirZ) {
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
            else if (n.moveType == SheepBulletMoveType.LinePosFrame) {
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
        }
        
        /**
  *
  * @param sheepCtl {SheepCtl}
  * @returns {Promise<void>}
  */
        public void game_run(object sheepCtl) {

            // 清理游戏数据
            this.game_clear();

            this.mainClearBlocks();

            let i = sheepMgr.gameIndex;

            this.updateTime = Date.now();

            //只有 游戏处于运行中 或者 局数未改变
            while (i === sheepMgr.gameIndex && (sheepMgr.state == SheepRoomState.Run || sheepMgr.state == SheepRoomState.Start)) {

                try {
                    let lastUpdateTime = this.updateTime
                    this.updateTime = Date.now();

                    let diff = this.updateTime - lastUpdateTime;

                    if (diff >= 100) {
                        console.warn("主线程更新逻辑耗时过长: " + diff + "ms");
                    }

                    if (diff < 33) {
                        await new Promise(e => setTimeout(e, 33 - diff));
                    }

                    if (this.comImages.isHasFreeImage()) {
                        let o = Date.now() - lastUpdateTime;
                        await this.game_update(sheepMgr, sheepCtl, o);
                    }

                } catch (e) {
                    console.error("主线程更新逻辑错误:", e);
                    return;
                }
            }
        }
        
        /**
     *
     * @param sheepMgr {SheepMgr}
     * @param sheepCtl {SheepCtl}
     * @param i 时间
     * @returns {Promise<void>}
     */
        public void game_update(sheepMgr, sheepCtl, i) {
            try {
                // 处理召唤兵
                this.consume(sheepCtl, i);

                this.buff_add_pets();

                this.buff_add_bullets();

                // 要处理的总数量
                let n = sheepMgr.pets[SheepCamp.Red].size + sheepMgr.pets[SheepCamp.Blue].size
                if (n <= 0) {
                    return
                }

                this.cur_rob_role_index = 0;
                this.cur_rob_bullet_index = 0;

                this.curIndexImages = this.comImages.startAdd();

                // 执行主逻辑
                this.role_logic();

                this.comImages.endAdd();

                this.update_merge_workers(sheepMgr, sheepCtl, i);

            } catch (err) {
                console.error("update逻辑错误", err);
                throw err;
            }
        }
        
        /**
     *
     * @param sheepMgr {SheepMgr}
     * @param sheepCtl {SheepCtl}
     * @param dt
     */
    public void update_merge_workers(SheepMgr sheepMgr,object sheepCtl,float dt) {

        let sheepConfig = SheepConfig;
        let isEnd = false;

        const now = Date.now();

        this.mainClearBlocks()
        sheepCtl.comImages.mesh_block.onFrameUpdateStart();


        if (sheepMgr.endTime && sheepMgr.endTime < Date.now()) {
            eventBus.emit(EventType.RoomStateEnd);
            isEnd = true
            sheepMgr.endTime = 0
            return;
        }


        sheepMgr.countNewBuffs = [0, 0];
        sheepMgr.countBuffs = [0, 0];
        sheepMgr.countShowBuffs = [0, 0];

        // console.log(sheepMgr.buffs)
        sheepMgr.buffs.forEach((r, s) => {

            if (r.length && r[0].time < sheepMgr.gameStartTimerForBuff) {
                r.shift();
                sheepMgr.buffs[s] = r;
            }

            for (const o of r) {
                sheepMgr.countBuffs[s] += o.count || SheepConfig.counterBuffNumber;
                sheepMgr.countShowBuffs[s] += o.count
            }

        });

        sheepMgr.preBuffs.forEach((r, s) => {
            if (!r.length) {
                return;
            }

            let sum = 0;
            let hasZero = false;

            for (const f of r) {
                if (0 == f) {
                    hasZero = true;
                }
                sum += f
            }

            if (hasZero) {

                sheepMgr.buffs[s].push({
                    time: sheepMgr.gameStartTimerForBuff + 1000 * sheepConfig.counterTime,
                    count: 0
                });

                if (r.length > 1) {
                    sheepMgr.buffs[s].push({
                        time: sheepMgr.gameStartTimerForBuff + 1000 * sheepConfig.buffLastTime,
                        count: sum
                    });
                }

            } else {
                sheepMgr.buffs[s].push({
                    time: sheepMgr.gameStartTimerForBuff + 1000 * sheepConfig.buffLastTime,
                    count: sum
                });
            }

            sheepMgr.preBuffs[s] = [];
            sheepMgr.countNewBuffs[s] += sum

        });

        isEnd = this.updateBoss(sheepMgr, sheepCtl, dt,  now)

        if (isEnd) {
            return;
        }

        let h = [];
        sheepMgr.pets.forEach(e => e.forEach(e => h.push(e)));
        let _ = false;
        if (!sheepMgr.cameraEulerAngles.equals(sheepCtl.cameraCtl.camera.node.eulerAngles)) {
            _ = true;
            sheepMgr.cameraEulerAngles = sheepCtl.cameraCtl.camera.node.eulerAngles.clone();
        }

        let b = 0;
        let I = 0;

        let x = h;

        for (let B = 0; B < x.length; B++) {
            let y = x[B];
            if (y.buff_index == -1) {
                continue;
            }

            y.updateSkin(sheepCtl, this, sheepMgr, dt);

            let M;
            let D = y.view_pet;
            let A = D.state;
            let P = D.animType;
            let W = D.animFrame;

            const fgs = sheepCtl.comImages.roles_framess[y.camp];

            const ghg = fgs[y.skinId];

            M = ghg[P];

            if (null == M) {
                console.warn("找不到动画", SheepCamp[y.camp], y.skinId, SheepRoleAnimType[P]);
            }

            if (A == SheepRoleState.In && W >= M.length - 1) {
                let E = SheepSkill.getById(D.readySkillId);
                if (E) {
                    if (E.skillType == SheepSkillType.Boom) {
                        let F = SheepSkillSubBoom.getById(E.id);
                        D.state = SheepRoleState.Boom;
                        if (F.isAnim) {
                            D.animType = SheepRoleAnimType.Boom
                        } else {
                            D.animType = SheepRoleAnimType.Idle
                        }
                    }
                } else {
                    D.state = SheepRoleState.Move;
                    D.animType = SheepRoleAnimType.Idle
                }
            } else if (A == SheepRoleState.Dead && W >= M.length - 1) {
                D.state = SheepRoleState.Res;
                D.animType = SheepRoleAnimType.None;
                y.onRes(sheepCtl, sheepMgr)
            } else if (A == SheepRoleState.Up && W >= M.length - 1) {
                D.state = SheepRoleState.In;
                D.animType = SheepRoleAnimType.In;
            } else if (A == SheepRoleState.Buff) {
                let V = SheepSkillSubBuff.getById(D.readySkillId);
                let U = D.animFrame;
                if (U > V.buffStratFrame && U < V.buffEndFrame) {
                    if (y.camp == SheepCamp.Blue) {
                        I += 1
                    } else {
                        b += 1
                    }
                }
            }

        }

        let j = 0;
        for (let G = 0; G < this.bulletCount; ++G) {
            let X = this.getBulletView(G);
            if (X.isDie) {
                continue;
            }

            if (X.frame >= X.conf.endFrame) {
                X.isDie = !0;
                this.buff_del_bullet(G);
            } else {

                let z = this.getPetView(X.roleIndex).conf.splitN;

                for (let O = -z; O <= z; ++O) {
                    for (let Q = -z; Q <= z; ++Q) {
                        let Z = Util.getIndexByXY(X.x + O, X.y + Q);
                        sheepCtl.comImages.mesh_block.addFrameBlockCamp(Z, X.camp)
                    }
                }
                ++j
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
        this.bulletMaxIndex = this.bulletCount

    }
        
        public void updateBoss(SheepMgr sheepMgr,object sheepCtl,float dt,  c) {
        var sheepConfig = SheepConfig;
        var isEnd = false;
        sheepMgr.boss.forEach((t, index, n) => {

            var viewPet = this.getPetView(index);
            var camp = viewPet.camp;
            var state = viewPet.state;

            if (state == SheepBossState.Ready) {

                viewPet.curHp = t.curHp;
                t.comProgress.setVue(t.curHp);
                viewPet.state = SheepBossState.NomalRun;

            }
            else if (state == SheepBossState.AwakeAnim || state == SheepBossState.UnAwakeAnim) {

                t.comProgress.setVue(t.comProgress._vue)

            }
            else if (state == SheepBossState.Dead) {

            }
            else {
                let curHp = viewPet.curHp;
                if (curHp <= 0) {
                    curHp = 0;
                }

                let d = t.comProgress._vue;
                let _ = d - curHp;

                if (_ && curHp) {

                    let S = sheepMgr.countBuffs[1 - camp];
                    if (S > 0) {
                        let b = 1 + sheepConfig.buffDragonDamageIncreseRate * S;
                        b += 0;
                        _ = Math.floor(_ * b)
                        curHp = d - _;
                        viewPet.curHp = curHp
                    }

                    let I = sheepMgr.countBuffs[camp];
                    if (I > 0) {
                        let B = Math.pow(1 - sheepConfig.buffDragonReduceRate, I);
                        B -= 0;
                        if (B < 1 - sheepConfig.buffDragonMaxReduceRate) {
                            B = 1 - sheepConfig.buffDragonMaxReduceRate;
                        }

                        _ = Math.floor(_ * B)
                        curHp = d - _;
                        viewPet.curHp = curHp
                    }

                }

                if (t.subShield() && _ > 1) {
                    curHp = d - 1
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

                let R = sheepMgr.countShowBuffs[camp];
                let M = sheepMgr.countBuffs[camp];

                if (!sheepMgr.flagLongBuffs[camp] && curHp < sheepMgr.loongHp * sheepConfig.counterHpRatio) {
                    sheepMgr.flagLongBuffs[camp] = true;
                    t.backStateTime = c;
                    sheepMgr.preBuffs[camp].push(0);
                    sheepCtl.comMatch.showDoubleAnim(camp);
                    sheepCtl.comUIAnim.backAnim(camp);
                    sheepCtl.cameraCtl.onShake(SheepConfig.shockBeginNumber)
                }
                else if (t.backStateTime && c - t.backStateTime > 12e4 && M - R == 0) {
                    t.backStateTime = 0;
                    sheepCtl.comMatch.hideDoubleAnim(camp);
                    sheepCtl.comUIAnim.backSuccessAnim(camp);
                    sheepCtl.cameraCtl.onShake(SheepConfig.shockEndNumber)
                }

                if (curHp <= 0) {
                    viewPet.state = SheepBossState.Dead;
                    viewPet.isDie = !0;
                    t.curHp = 0;
                    eventBus.emit(EventType.RoomStateEnd)
                    isEnd = true
                    return;
                }

                viewPet.curAckFrame;

                let T = 0;
                let D = sheepMgr.plotRatio;

                for (let A = 0; A < sheepConfig.loongStateSwitching.length; A++) {
                    if (D <= sheepConfig.loongStateSwitching[A]) {
                        T = A;
                        break
                    }
                }

                sheepMgr.plotRatioIndex = T;
                t.updateState(sheepCtl, sheepMgr, T + 1);
                t.updateStateJJL(sheepCtl, sheepMgr, T + 1)

            }
        });
        return isEnd;
    }
        
        
    /**
     *
     * @param e {PetView}
     */
    public void pre_add_pet(e) {
        this.petsAdd.push(e)
    }

    public void buff_add_pets() {
        if (this.petsAdd.length <= 0) {
            return;
        }

        for (; this.petsAdd.length;) {
            let e = this.petsDel.pop();
            if (null == e) {
                if (this.petCount >= SheepConfig.MaxPetCount - 1) {
                    console.warn("预加入怪物加入buff超过最大数量", this.petCount, SheepConfig.MaxPetCount);
                    break
                }
                e = this.petCount++
            }
            let t = this.petsAdd.shift();
            let r = this.getPetView(e);
            t.init(e, r)
        }
    }

    public void buff_del_pet(int e) {
        let pet = this.getPetView(e);
        pet.isDie = true;
        pet.id = 0;
        this.petsDel.push(e)
    }

    public void clear_pets() {
        this.cur_rob_role_index = 0;
        this.roleMaxIndex = 0;
        this.petCount = 0;
        this.petsAdd=[];
        this.petsDel.length = 0
    }

    public void buff_add_bullets() {
        let e = this.preBulletIndex;
        if (e) {
            for (; e;) {
                let t = this.bulletsDel.pop();
                if (null == t) {
                    if (this.bulletCount >= SheepConfig.MaxBulletCount - 1) {
                        warn("预加入子弹加入buff超过最大数量", this.bulletCount, SheepConfig.MaxBulletCount);
                        break
                    }
                    t = this.bulletCount++
                }
                --e;

                this.getBulletView(t).init(++this.bulletId,this.pre_view_bullets[ e] )
            }
            this.preBulletIndex = 0
        }
    }

    public void buff_del_bullet(e) {
        let bullet = this.getBulletView(e);
        bullet.id = 0;
        this.bulletsDel.push(e)
    }

    public void clear_bullets() {
        this.cur_rob_bullet_index = 0;
        this.bulletMaxIndex = 0;
        this.bulletCount = 0;
        this.bulletsDel.length = 0
    }

    public void game_clear() {
        this.clearBlocks();
        this.clearPetViews();
        this.preBulletIndex = 0;
        this.clearViewBullets();
        this.clear_pets();
        this.clear_bullets();
        this.pre_add_pet(sheepMgr.boss[SheepCamp.Red]);
        this.pre_add_pet(sheepMgr.boss[SheepCamp.Blue])
    }

    public void role_logic() {
        let t = Date.now();
        this.logic_counts[SheepCamp.Red] = this.redBuffCount > 0 ? 2 : 1;
        this.logic_counts[SheepCamp.Blue] = this.blueBuffCount > 0 ? 2 : 1;
        let curIndexImages = this.curIndexImages;
        let o = 0;
        if (this.roleMaxIndex) {
            o = this.rob_role_task(this.roleMaxIndex, curIndexImages);
        }
        let l = 0;
        if (this.bulletMaxIndex) {
            l = this.rob_bullet_task(this.bulletMaxIndex, curIndexImages);
        }
        if (this.bullte_creates.length) {
            let e = this.rob_pre_bullet(this.bullte_creates.length);
            for (const t of this.bullte_creates) {
                let i = e++;
                if (i > SheepConfig.MaxBulletCount) {
                    break;
                }
                this.copyBulletPreView(i, t.bulletId, t.view_pet, t.view_tar_pet, t.info)
            }
            this.bullte_creates = []
        }
        let n = true;

        n = true;

        let r = Date.now() - t;
        if (r > 33) {
            console.log(`${self.name} count_role:${o} count_bullet:${l}  耗时:${r}ms`)
        }
        return {
            time: r,
            isEndWorker: n
        }
    }

    public void rob_role_task(count, curIndexImages) {
        let start = this.rob_role(count);
        const end = start + count;
        let i = this.update_role(start, end);
        this.comImages.update_role(curIndexImages);
        return i;
    }

    public void rob_bullet_task(count, curIndexImages) {
        let start = this.rob_bullet(count);
        const end = start + count;
        const i = this.update_bullet(start, end);
        this.comImages.update_bullet(curIndexImages)
        return i;
    }

    public void update_role(start, end) {
        for (let i = start; i < end; i++) {
            let viewPet = this.getPetView(i);
            if (!viewPet.isActive) {
                viewPet = null;
                continue
            }
            let t = viewPet.isDie;
            if (0 == viewPet.roleId) {
                let i = this.update_frame(viewPet);
                if (!t && i) {
                    this.update_boss_state(viewPet);
                }
                this.update_role_anim(viewPet)
            } else {
                let i = viewPet.camp, s = this.logic_counts[i];
                for (let i = 0; i < s; i++) {
                    let i = this.update_frame(viewPet);
                    if (!t) {
                        this.update_role_state(viewPet, i);
                    }
                    this.update_role_anim(viewPet)
                }
                let o = viewPet;
                this.comImages.addRole(o);
            }
            viewPet = null
        }
        return end - start
    }

    public void update_bullet(start, end) {
        for (let i = start; i < end; i++) {
            if (i >= SheepConfig.MaxBulletCount) {
                return i - start;
            }
            let t = this.getBulletView(i);
            if (t.isDie) {
                continue;
            }
            if (t.id && t.conf.animId) {
                let e = t;
                this.comImages.addBullet(e)
            }
            let {xn: s, yn: o} = Util.getXnYn(t.x, t.y), l = t.frame, n = t.conf;
            {
                let e = t.conf.atkFrames;
                if (e) for (let i = 0; i < e.length; i++) {
                    const n = e[i];
                    if (-1 == n || n == l) {
                        if (0 == t.tarRoleIndex || 1 == t.tarRoleIndex) {
                            let o = this.getPetView(t.tarRoleIndex);
                            UtilAck.isCanAckByBullet(t, o, i) && UtilAck.hurtByBullet(t, o, t.atkVue )
                        } else UtilFind.forfeachBlocksByAckView(t.camp, s, o, t.conf.findR, (e => {
                            UtilAck.isCanAckByBullet(t, e, i) && UtilAck.hurtByBullet(t, e, t.atkVue)
                        }));
                        break
                    }
                }
                switch (t.conf.moveType) {
                    case SheepBulletMoveType.Fixed:
                        break;
                    case SheepBulletMoveType.LineDir:
                        t.x = t.x + t.dirX * n.speed * .033, t.y = t.y + t.dirY * n.speed * .033;
                        break;
                    case SheepBulletMoveType.LinePosFrame:
                        t.x = t.startX + (t.endX - t.startX) * l / n.moveTimeFrame, t.y = t.startY + (t.endY - t.startY) * l / n.moveTimeFrame, t.z = t.startZ + (t.endZ - t.startZ) * l / n.moveTimeFrame;
                        break;
                    case SheepBulletMoveType.LineTarFrame:
                        break;
                    case SheepBulletMoveType.CurvePosFrame:
                        let e = (t.startX + t.endX) / 2, i = (t.startY + t.endY) / 2, s = n.curveHigh,
                            o = t.startX + (e - t.startX) * l / n.moveTimeFrame,
                            r = e + (t.endX - e) * l / n.moveTimeFrame,
                            a = t.startY + (i - t.startY) * l / n.moveTimeFrame,
                            c = i + (t.endY - i) * l / n.moveTimeFrame,
                            f = t.startZ + (s - t.startZ) * l / n.moveTimeFrame,
                            h = s + (t.endZ - s) * l / n.moveTimeFrame;
                        t.x = o + (r - o) * l / n.moveTimeFrame, t.y = a + (c - a) * l / n.moveTimeFrame, t.z = f + (h - f) * l / n.moveTimeFrame;
                        let p = t.endX - t.startX > 0 ? 1 : -1, u = .2 * p, d = .8, g = p, S = 0, m = -.2 * p,
                            y = -.8, k = u + (g - u) * l / n.endFrame, B = d + (S - d) * l / n.endFrame,
                            w = g + (m - g) * l / n.endFrame, R = S + (y - S) * l / n.endFrame;
                        t.dirX = k + (w - k) * l / n.endFrame, t.dirY = 0, t.dirZ = B + (R - B) * l / n.endFrame;
                        break;
                    case SheepBulletMoveType.CurveTarFrame:
                        break;
                    case SheepBulletMoveType.LineDirEndPos:
                        t.x = t.x + t.dirX * n.speed * .033, t.y = t.y + t.dirY * n.speed * .033, t.z = t.z + t.dirZ * n.speed * .033;
                        break;
                    case SheepBulletMoveType.RadiusAngle:
                        t.angle += n.speed;
                        let x = t.roleUid, _ = this.getPetView(t.roleIndex);
                        x == _.id ? (t.x = _.animX + n.radius * Math.cos(t.angle), t.y = _.animY + n.radius * Math.sin(t.angle)) : t.isDie = !0;
                        break;
                    case SheepBulletMoveType.DirAngle:
                        t.x = t.x + t.dirX * n.speed * .033, t.y = t.y + t.dirY * n.speed * .033, t.z = t.z + t.dirZ * n.speed * .033
                }
                t.frame = l + 1
            }
            let r = n.createBulletID;
            if (r && n.createBulletFrame == l) {
                let e = this.getPetView(t.roleIndex), i = this.getPetView(t.tarRoleIndex);
                this.bullte_creates.push({
                    view_pet: e,
                    bulletId: r,
                    view_tar_pet: i,
                    info: {startX: t.x, startY: t.y, startZ: 100}
                })
            }
            t = null
        }
        return end - start
    }

    /**
     *
     * @param viewPet {PetView}
     * @returns {boolean}
     */
    public void update_frame(viewPet) {
        let frame = viewPet.frame;
        let loopFrame = SheepConfig.loopFrame;
        let i = frame % loopFrame == loopFrame - 1;
        let posBefX = viewPet.posBefX;
        let posBefY = viewPet.posBefY;
        let posX = viewPet.posX;
        let posY = viewPet.posY;
        if (!viewPet.isDie) {
            viewPet.animX = posBefX + (posX - posBefX) * (frame % loopFrame) / loopFrame;
            viewPet.animY = posBefY + (posY - posBefY) * (frame % loopFrame) / loopFrame;
        }
        frame += 1;
        viewPet.frame = frame;
        if (!viewPet.isDie && i) {
            viewPet.logicMove(posX, posY);
        }
        return i
    }

    public void update_boss_state(e) {
        switch (e.state) {
            case SheepBossState.NomalRun:
            case SheepBossState.AwakeRun:
            case SheepBossState.BackRun:
                let t = e.conf, i = e.curAckFrame;
                if (0 == i) {
                    let {xn: i, yn: o} = Util.getXnYn(e.posX, e.posY);
                    let l = false;
                    UtilFind.findNearBlocksByAckView(e, i, o, Math.floor(t.findR * SheepConfig.loongExaminationRangeBet), (t => {
                        return !!l || !!Util.isCanAckByRole(e, t) && (l = !0, !0);
                    }));
                    if (!l) {
                        break
                    }
                }
                i += 1;
                e.curAckFrame = i
                if (i == Math.floor(t.readyAtks[0] / 3)) {
                    let {xn: i, yn: s} = Util.getXnYn(e.posX, e.posY);
                    UtilFind.forfeachBlocksByAckView(e.camp, i, s, t.findR, (t => {
                        Util.isCanAckByRole(e, t) && UtilAck.hurtByRole(e, t, e.conf.atk)
                    }))
                }
                i >= Math.floor(1e3 * t.atkCd / 100) && (e.curAckFrame = 0)
        }
    }

    public void update_role_state_in(petSkin){
        if (petSkin.conf.skillIn) {
            let t = SheepSkill.getById(petSkin.conf.skillIn);
            if (t.skillType == SheepSkillType.Boom) {
                let i = SheepSkillSubBoom.getById(t.id);
                if (1 == petSkin.animFrame) {
                    let t = petSkin.camp == SheepCamp.Red ? -1200 : 1200;
                    let {xn: o, yn: l} = Util.getXnYn(t, 0);
                    let n = null;
                    UtilFind.findNearBlocksByAckView(petSkin, o, l, 100, e => {
                        n = e
                        return true;
                    });
                    if (n) {
                        petSkin.posBefX = petSkin.posX;
                        petSkin.posBefY = petSkin.posY;
                        petSkin.posX = n.posX;
                        petSkin.posY = n.posY;
                        petSkin.animX = petSkin.posX;
                        petSkin.animY = petSkin.posY;
                    } else {
                        petSkin.posBefX = t;
                        petSkin.posBefY = 0;
                        petSkin.posX = t;
                        petSkin.posY = 0;
                        petSkin.animX = t;
                        petSkin.animY = 0;
                    }
                    petSkin.readySkillId = i.id;
                    petSkin.isLock = 1
                }
            }
        }
    }

    public void update_role_state_move(petSkin, t, i){
        if (petSkin.isLock) {
            return
        }
        let {atkTar: s, moveTar: o, moveBoss: l} = UtilFind.findTar(petSkin);
        if (s) {
            petSkin.state = SheepRoleState.Attack;
            petSkin.subState = SheepRoleSubState.AttackAwait;
            return
        }
        if (o) {
            petSkin.subState = SheepRoleSubState.MoveTar;
            Util.moveTar(petSkin, o, i, t);
            return
        }
        if (l) {
            petSkin.subState = SheepRoleSubState.MoveBoss;
            Util.moveTar(petSkin, l, i, t);
            return
        }
        console.error("移动状态没有目标??")
    }

    public void update_role_state_attack(petSkin, t, i){
        let o = petSkin.conf.atkMoveType;
        if (petSkin.conf.isLoongStopDistance) {
            const t = sheepMode;
            let i = petSkin.conf.loongStopDistanceR;
            Util.dis(petSkin.posX, petSkin.posY, petSkin.camp === SheepCamp.Red ? t.loongX : -t.loongX, 0) <= i && (o = SheepRoleAtkMoveType.None)
        }
        if (petSkin.subState == SheepRoleSubState.AttackAwait) {
            if (!Util.isAtkCd(petSkin)) {
                petSkin.subState = SheepRoleSubState.AttackAnim;
                petSkin.animType = SheepRoleAnimType.Attack;
            }
        } else if (petSkin.subState == SheepRoleSubState.AttackAnim) {
            let t = petSkin.conf;
            let i = t.finishAtk;
            let atkCd = t.atkCd;
            let l = petSkin.animFrame;
            let n = t.readyAtks;
            for (const i of n) {
                if (l == i) {
                    let i = null;
                    if (petSkin.conf.atkType == SheepRoleAtkType.Nearest) {
                        i = UtilFind.findNearAck(petSkin);
                    } else if (petSkin.conf.atkType == SheepRoleAtkType.Throw) {
                        i = UtilFind.findSortAck(petSkin, petSkin.conf.findR)
                        if (petSkin.conf.roleType == SheepRoleType.pao_che) {
                            let t = Util.getBackBoss(petSkin.camp);
                            Util.isCanAckByRole(petSkin, t) && (i = t)
                        }
                    } else {
                        i = UtilFind.findNearAck(petSkin);
                    }
                    if (t.bullet && 0 != t.bullet.length) {
                        if (i) {
                            this.bullte_creates.push({
                                view_pet: petSkin,
                                bulletId: t.bullet[petSkin.camp == SheepCamp.Red ? 0 : 1],
                                view_tar_pet: i
                            });
                        } else {
                            this.bullte_creates.push({
                                view_pet: petSkin,
                                bulletId: t.bullet[petSkin.camp == SheepCamp.Red ? 0 : 1]
                            });
                        }
                    } else {
                        i && UtilAck.ackTar(petSkin, i);
                    }
                    break
                }
            }
            if (l >= i) {
                Util.resetAtkCd(petSkin, atkCd);
                let {atkTar: t, moveTar: i, moveBoss: s} = UtilFind.findTar(petSkin);
                if (t) {
                    petSkin.subState = SheepRoleSubState.AttackAwait;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return
                }
                if (i) {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveTar;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return
                }
                if (s) {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return
                }
            }
        }
        if (t && (o == SheepRoleAtkMoveType.Move || o == SheepRoleAtkMoveType.CdMove && petSkin.subState == SheepRoleSubState.AttackAwait)) {
            let s = UtilFind.findNearAck(petSkin);
            s && Util.disByRole(petSkin, s) > petSkin.conf.atkMinMoveR + s.conf.collideR && Util.moveTar(petSkin, s, i, t)
        }
    }

    public void update_role_state_killer(petSkin){
        let t = SheepSkillSubKiller.getById(petSkin.readySkillId);
        let i = petSkin.animFrame;
        if (i == t.findMoveFrame) {
            let i = !1;
            const s = petSkin.conf;
            if (petSkin.conf.roleType == SheepRoleType.ci_ke && UtilFind.foreachFront(petSkin, (e => {
                e.conf.roleType != SheepRoleType.dun_bing || (i = !0)
            }), s.findR, 60), i) {
                console.log("刺客被中断，直接回到移动状态"), petSkin.state = SheepRoleState.Move, petSkin.subState = SheepRoleSubState.MoveBoss, petSkin.animType = SheepRoleAnimType.Idle;
                return
            }
            let o = UtilFind.findFarAck(petSkin, t.findR);
            o ? petSkin.logicMove(o.posX, o.posY) : (petSkin.state = SheepRoleState.Move, petSkin.subState = SheepRoleSubState.MoveBoss, petSkin.animType = SheepRoleAnimType.Idle)
        }
        i == t.atkFrame && UtilAck.ackMe(petSkin, t.spiltRadiusBet, t.atkBet, t.atkFindR)
        if (i >= t.endFrame) {
            let i = petSkin.subState;
            if (i == SheepRoleSubState.KillerEnd || i - SheepRoleSubState.KillerStart >= t.cnt) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
                return;
            }
            petSkin.subState = i + 1;
            petSkin.animType = SheepRoleAnimType.Killer
        }
    }

    public void update_role_state_boom(petSkin){
        let t = SheepSkill.getById(petSkin.readySkillId);
        let i = SheepSkillSubBoom.getById(t.id);
        let s = petSkin.animFrame;
        if (s == i.atkFrame) {
            let t = [];
            petSkin.conf.roleType != SheepRoleType.chong_feng_bing && petSkin.conf.roleType != SheepRoleType.qi_lin || t.push(SheepRoleType.qi_lin), UtilAck.ackMe(petSkin, i.spiltRadiusBet, i.atkBet, i.atkFindR, i.hitBackDistance, t)
        }
        s >= i.endFrame && (petSkin.isLock = 0, i.endState == SheepRoleState.Move ? (petSkin.state = SheepRoleState.Move, petSkin.subState = SheepRoleSubState.MoveBoss, petSkin.animType = SheepRoleAnimType.Idle) : i.endState == SheepRoleState.Rigidity ? (petSkin.state = SheepRoleState.Rigidity, petSkin.animType = SheepRoleAnimType.Idle, petSkin.readySkillId = i.endSkill) : i.endState == SheepRoleState.Dead ? (petSkin.isDie = !0, petSkin.state = SheepRoleState.Dead) : i.endState == SheepRoleState.Palm ? (petSkin.state = SheepRoleState.Palm, petSkin.subState = SheepRoleSubState.Palm, petSkin.animType = SheepRoleAnimType.Palm, petSkin.readySkillId = i.endSkill) : console.error("endState错误"))
    }

    public void update_role_state_invincible(petSkin){
        let t = petSkin.animFrame, i = SheepSkill.getById(petSkin.readySkillId),
            s = SheepSkillSubInvincible.getById(i.id), o = s.healFrames;
        for (const i of o) if (t == i) {
            let t = Math.floor((petSkin.conf.hp - petSkin.curHp) * (s.healHealthPercent / 100));
            UtilAck.hurtByRole(petSkin, petSkin, -t);
            break
        }
        let l = s.atkFrames;
        for (const i of l) if (t == i) {
            UtilAck.ackMe(petSkin, s.spiltRadiusBet, s.atkBet, s.atkFindR);
            break
        }
        t >= s.endFrame && (petSkin.state = SheepRoleState.Move, petSkin.subState = SheepRoleSubState.MoveBoss, petSkin.animType = SheepRoleAnimType.Idle)
    }
    public void update_role_state_bladestorm(petSkin, t, i){
        let s = petSkin.animFrame;
        let o = SheepSkill.getById(petSkin.readySkillId);
        let l = SheepSkillSubBladestorm.getById(o.id);
        if (t) {
            petSkin.posX;
            let {atkTar: t, moveTar: s, moveBoss: o} = UtilFind.findTar(petSkin, l.findR), n = t || s || o;
            Util.dirTar(petSkin, n);
            let r = l.speed;
            let x = petSkin.posX + petSkin.dirX * r * i * 3;
            let y = petSkin.posY + petSkin.dirY * r * i * 3;
            petSkin.logicMove(x, y);
        }
        let n = l.atkFrames;
        for (const t of n) {
            if (s == t) {
                UtilAck.ackMe(petSkin, l.spiltRadiusBet, l.atkBet, l.atkFindR);
                break
            }
        }
        s >= l.endFrame && (petSkin.state = SheepRoleState.Move, petSkin.subState = SheepRoleSubState.MoveBoss, petSkin.animType = SheepRoleAnimType.Idle)
    }
    public void update_role_state_palm(petSkin){
        let t = petSkin.animFrame, i = SheepSkill.getById(petSkin.readySkillId),
            s = SheepSkillSubPalm.getById(i.id), o = s.healFrames;
        for (const i of o) if (t == i) {
            let t = Math.floor((petSkin.conf.hp - petSkin.curHp) * (s.healHealthPercent / 100));
            UtilAck.hurtByRole(petSkin, petSkin, -t);
            break
        }
        let l = s.atkFrames;
        for (const i of l) if (t == i) {
            UtilAck.ackMe(petSkin, s.spiltRadiusBet, s.atkBet, s.atkFindR);
            break
        }
        let n = s.hitBackFrames;
        for (let i = 0; i < n.length; i++) {
            const o = n[i], l = s.hitBackDistances[i];
            if (t == o) {
                UtilAck.hitBackMe(petSkin, s.spiltRadiusBet, s.atkFindR, l);
                break
            }
        }
        t >= s.endFrame && (petSkin.state = SheepRoleState.Move, petSkin.subState = SheepRoleSubState.MoveBoss, petSkin.animType = SheepRoleAnimType.Idle)
    }
    public void update_role_state_callbullets(petSkin){
        let t = petSkin.animFrame;
        let i = SheepSkill.getById(petSkin.readySkillId);
        let s = SheepSkillSubCallBullets.getById(i.id);
        let o = 0;
        if (s.frameStep) {
            t % s.frameStep == 0 && (o = s.frameCnt);
        } else {
            let e = s.callFrames;
            for (let i = 0; i < e.length; i++) {
                if (t == e[i]) {
                    o = s.callCnts[i];
                    break
                }
            }
        }
        if (o) {
            for (let t = 0; t < o; t++) {
                if (1 == s.type) {
                    let t = petSkin.posX + s.startOffsetPos[0];
                    let i = petSkin.posY + s.startOffsetPos[1];
                    let o = s.startOffsetPos[2];
                    let l = 360 * Math.random();
                    let n = petSkin.posX + petSkin.dirX * s.len + s.endRadius * Math.cos(l);
                    let r = petSkin.posY + petSkin.dirY * s.len + s.endRadius * Math.sin(l);
                    let a = 0;
                    this.bullte_creates.push({
                        view_pet: petSkin,
                        bulletId: s.bullet,
                        info: {startX: t, startY: i, startZ: o, endX: n, endY: r, endZ: a}
                    })
                } else if (2 == s.type) {
                    let t = s.startOffsetPos[2];
                    let i = 360 * Math.random();
                    let o = petSkin.posX + petSkin.dirX * s.len + s.endRadius * Math.cos(i);
                    let l = petSkin.posY + petSkin.dirY * s.len + s.endRadius * Math.sin(i);
                    let n = 0;
                    this.bullte_creates.push({
                        view_pet: petSkin,
                        bulletId: s.bullet,
                        info: {
                            startX: o,
                            startY: l,
                            startZ: t,
                            endX: o,
                            endY: l,
                            endZ: n,
                            dirX: 0,
                            dirY: 0,
                            dirZ: -1
                        }
                    })
                } else if (3 == s.type) {
                    this.bullte_creates.push({
                        view_pet: petSkin,
                        bulletId: s.bullet,
                        info: {dirX: 0, dirY: 0, dirZ: -1, angle: 360 / o * t}
                    });
                } else if (4 == s.type) {
                    this.bullte_creates.push({
                        view_pet: petSkin,
                        bulletId: s.bullet,
                        info: {dirX: 1, dirY: 0, dirZ: 0}
                    });
                } else {
                    this.bullte_creates.push({view_pet: petSkin, bulletId: s.bullet});
                }
            }
        }
        t >= s.endFrame && (petSkin.state = SheepRoleState.Move, petSkin.subState = SheepRoleSubState.MoveBoss, petSkin.animType = SheepRoleAnimType.Idle)
    }
    public void update_role_state_buff(petSkin){
        let t = petSkin.animFrame;
        let i = SheepSkill.getById(petSkin.readySkillId);
        if(t >= SheepSkillSubBuff.getById(i.id).endFrame) {
            petSkin.state = SheepRoleState.Move;
            petSkin.subState = SheepRoleSubState.MoveBoss;
            petSkin.animType = SheepRoleAnimType.Idle
        }
    }
    public void update_role_state_rigidity(petSkin){
        let t = SheepSkill.getById(petSkin.readySkillId), i = SheepSkillSubRigidity.getById(t.id);
        petSkin.animFrame >= i.endFrame && (petSkin.state = SheepRoleState.SpinAtk, petSkin.animType = SheepRoleAnimType.Attack, petSkin.readySkillId = i.endSkill)
    }
    public void update_role_state_spinatk(petSkin, t, i){
        let s = petSkin.posX;
        let o = petSkin.posY;
        let {xn: l, yn: n} = Util.getXnYn(s, o);
        let r = petSkin.animFrame;
        let a = SheepSkill.getById(petSkin.readySkillId);
        let c = SheepSkillSubSpinAtk.getById(a.id);
        if (1 == r) {
            let t = UtilFind.findSortAck1(petSkin, petSkin.conf.findR);
            t && Util.dirTar(petSkin, t)
        }
        if (t) {
            let s = !0;
            UtilFind.forNearBlocksByAckView(petSkin, l, n, petSkin.conf.findR, (t => !(t.isDie || t.camp == petSkin.camp || 0 == t.roleId || (s && t.conf.roleType == SheepRoleType.dun_bing && Util.isCanAckByRole(petSkin, t) && (s = !1), !Util.isCanAckByRole(petSkin, t)) || (UtilAck.ackTar(petSkin, t), 1)))), s && Util.moveTar(petSkin, null, i, t)
        }
        r >= c.endFrame && (petSkin.state = c.endState, petSkin.animType = SheepRoleAnimType.Boom, petSkin.readySkillId = c.endSkill)
    }

    /**
     *
     * @param petSkin {PetView}
     * @param t
     * @param i
     * @returns {Promise<void>}
     */
    public void update_role_state(petSkin, t, i = .033) {
        Util.subAtkCd(petSkin, i)
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
                this.update_role_state_move(petSkin,t,i)
                break;
            case SheepRoleState.Attack:
                this.update_role_state_attack(petSkin,t,i);
                break;
            case SheepRoleState.Killer:
                this.update_role_state_killer(petSkin,t,i);
                break;
            case SheepRoleState.Boom:
                this.update_role_state_boom(petSkin,t,i);
                break;
            case SheepRoleState.Invincible:
                this.update_role_state_invincible(petSkin,t,i);
                break;
            case SheepRoleState.Bladestorm:
                this.update_role_state_bladestorm(petSkin,t,i);
                break;
            case SheepRoleState.Palm:
                this.update_role_state_palm(petSkin,t,i);
                break;
            case SheepRoleState.CallBullets:
                this.update_role_state_callbullets(petSkin,t,i);
                break;
            case SheepRoleState.Buff:
                this.update_role_state_buff(petSkin,t,i);
                break;
            case SheepRoleState.Rigidity:
                this.update_role_state_rigidity(petSkin,t,i);
                break;
            case SheepRoleState.SpinAtk:
                this.update_role_state_spinatk(petSkin,t,i);
        }

        if (petSkin.impulseX || petSkin.impulseY) {
            if (!petSkin.isDie && petSkin.curHp > 0) {
                let t = petSkin.impulseX;
                let i = petSkin.impulseY;
                petSkin.logicMove(petSkin.animX + t, petSkin.posY + i);
            }
            petSkin.impulseX = 0;
            petSkin.impulseY = 0;
        }
    }

    /**
     *
     * @param petSkin {PetView}
     * @param t
     * @param s
     */
    public void update_role_state_start(petSkin, t, s) {
        if (this.state == SheepRoomState.Start) {
            if (t) {
                let t = petSkin.posX;
                let i = petSkin.posY;
                let o = petSkin.tarPosX;
                let l = petSkin.tarPosY;
                let n = Util.dis(t, i, o, l);
                let r = 3 * petSkin.conf.runSpeed;
                if (n > r * s) {
                    let [t, i] = Util.dirTarByPos(petSkin, petSkin.tarPosX, petSkin.tarPosY);
                    let o = {x: petSkin.posX, y: petSkin.posY};
                        let l = {x: t * r * s, y: i * r * s};
                        let n = {x: o.x + l.x, y: o.y + l.y};
                    petSkin.logicMove(n.x, n.y)
                } else {
                    petSkin.logicMove(o, l)
                }
            }
        } else if (petSkin.conf.skillSpurt) {
            let t = SheepSkill.getById(petSkin.conf.skillSpurt);
            if (t.skillType == SheepSkillType.Charge) {
                petSkin.state = SheepRoleState.Charge;
                petSkin.subState = SheepRoleSubState.Spurt;
                petSkin.animType = SheepRoleAnimType.Spurt
            } else if (t.skillType == SheepSkillType.SpinSpurt) {
                petSkin.state = SheepRoleState.SpinSpurt;
                petSkin.animType = SheepRoleAnimType.Attack
            } else {
                petSkin.state = SheepRoleState.Spurt;
                petSkin.subState = SheepRoleSubState.Spurt;
                if (petSkin.conf.isSpurtAnim) {
                    petSkin.animType = SheepRoleAnimType.Spurt
                } else {
                    petSkin.animType = SheepRoleAnimType.Idle
                }
            }
        } else {
            petSkin.state = SheepRoleState.Spurt;
            petSkin.subState = SheepRoleSubState.Spurt;
            if (petSkin.conf.isSpurtAnim) {
                petSkin.animType = SheepRoleAnimType.Spurt
            } else {
                petSkin.animType = SheepRoleAnimType.Idle
            }
        }
    }

    /**
     *
     * @param e {PetView}
     * @param t
     * @param i
     * @return {*}
     */
    public void update_role_state_charge(e, t, i) {
        let o = e.posX, l = e.posY, {xn: n, yn: r} = Util.getXnYn(o, l);
        if (e.camp == SheepCamp.Red && e.posX > e.conf.runEndX || e.camp == SheepCamp.Blue && e.posX < -e.conf.runEndX) {
            let t = !1;
            if (UtilFind.findNearBlocksByAckView(e, n, r, 5, (i => (i.isDie || i.camp == e.camp || 0 == i.roleId || (t = !0), t))), t) {
                e.state = SheepRoleState.Boom;
                e.subState = SheepRoleSubState.Boom;
                let t = SheepSkillSubCharge.getById(e.conf.skillSpurt);
                let i = SheepSkillSubBoom.getById(t.endSkill);
                i.isAnim ? e.animType = SheepRoleAnimType.Boom : e.animType = SheepRoleAnimType.Idle;
                e.readySkillId = i.id
            } else {
                e.state = SheepRoleState.Move;
                e.subState = SheepRoleSubState.MoveBoss;
                e.animType = SheepRoleAnimType.Idle
            }
        } else {
            let s = false;
            UtilFind.findNearBlocksByAckView(e, n, r, 5, t => {
                if (!t.isDie && t.camp != e.camp && 0 != t.roleId && Util.isCanAckByRole(e, t)) {
                    if (t.conf.roleType == SheepRoleType.xiao_bing) {
                        let i = t;
                        UtilAck.ackTar(e, i)
                    } else {
                        s = true;
                    }
                }
                return false
            })
            if (s) {
                e.state = SheepRoleState.Boom;
                e.subState = SheepRoleSubState.Boom;
                let t = SheepSkillSubCharge.getById(e.conf.skillSpurt);
                let i = SheepSkillSubBoom.getById(t.endSkill);
                return i.isAnim ? e.animType = SheepRoleAnimType.Boom : e.animType = SheepRoleAnimType.Idle, void (e.readySkillId = i.id)
            }
            let o = null;
            UtilFind.findNearBlocksByAckView(e, n, r, e.conf.findR, t => {
                // 跳过：死亡的、同阵营的、没有 roleId 的
                if (t.isDie || t.camp == e.camp || t.roleId === 0) {
                    return false;
                }

                // 只允许 roleType = role3
                if (t.conf.roleType !== SheepRoleType.gong_jian_shou) {
                    return false;
                }

                // 必须可攻击
                if (!Util.isCanAckByRole(e, t)) {
                    return false;
                }

                // 如果满足条件，克隆并返回 true
                o = t;
                return true;
            });
            Util.moveTar(e, o, i, t)
        }
    }

    public void update_role_state_charge_plus(e, t, i) {
        let o = e.posX, l = e.posY, {xn: n, yn: r} = Util.getXnYn(o, l);
        if (e.camp == SheepCamp.Red && e.posX > e.conf.runEndX || e.camp == SheepCamp.Blue && e.posX < -e.conf.runEndX) {
            e.state = SheepRoleState.Boom, e.subState = SheepRoleSubState.Boom;
            let t = SheepSkillSubChargePlus.getById(e.conf.skillSpurt),
                i = SheepSkillSubBoom.getById(t.endSkill);
            e.animType = SheepRoleAnimType.Boom, e.readySkillId = i.id
        } else UtilFind.findNearBlocksByAckView(e, n, r, 5, (t => {
            if (!t.isDie && t.camp != e.camp && 0 != t.roleId && Util.isCanAckByRole(e, t)) {
                const i = SheepConfig.beheadLine;
                if (t.curHp < i) t.isDie = !0, t.state = SheepRoleState.Dead; else {
                    let t = e.conf;
                    UtilAck.ackMe(e, t.collideR, 0, t.findR, t.hitBackDistance)
                }
            }
            return !1
        })), Util.moveTar(e, null, i, t)
    }

    public void update_role_state_spinspurt(e, t, i) {
        let o = e.posX, l = e.posY, {xn: n, yn: r} = Util.getXnYn(o, l);
        if (e.camp == SheepCamp.Red && e.posX > e.conf.runEndX || e.camp == SheepCamp.Blue && e.posX < -e.conf.runEndX) {
            e.state = SheepRoleState.Boom;
            e.subState = SheepRoleSubState.Boom;
            let t = SheepSkillSubSpinSpurt.getById(e.conf.skillSpurt);
            let i = SheepSkillSubBoom.getById(t.endSkill);
            i.isAnim ? e.animType = SheepRoleAnimType.Boom : e.animType = SheepRoleAnimType.Idle;
            e.readySkillId = i.id;
        } else {
            Util.moveTar(e, null, i, t);
            UtilFind.forNearBlocksByAckView(e, n, r, e.conf.findR, (t => !(t.isDie || t.camp == e.camp || 0 == t.roleId || !Util.isCanAckByRole(e, t) || (UtilAck.ackTar(e, t), 1))))
        }
    }

    public void update_role_state_spurt(e, t, i) {
        if (e.conf.skillSpurt) {
            let s = SheepSkill.getById(e.conf.skillSpurt);
            if (s.skillType == SheepSkillType.Boom) {
                let o = SheepSkillSubBoom.getById(s.id), {
                    atkTar: l,
                    moveTar: n,
                    moveBoss: r
                } = UtilFind.findTar(e);
                if (l || r) return e.state = SheepRoleState.Boom, e.subState = SheepRoleSubState.Boom, o.isAnim ? e.animType = SheepRoleAnimType.Boom : e.animType = SheepRoleAnimType.Idle, void (e.readySkillId = o.id);
                Util.moveTar(e, null, i, t)
            } else if (s.skillType == SheepSkillType.Killer) {
                let o = SheepSkillSubKiller.getById(s.id), {
                    atkTar: l,
                    moveTar: n,
                    moveBoss: r
                } = UtilFind.findTar(e);
                if (l) return e.state = SheepRoleState.Killer, e.subState = SheepRoleSubState.KillerStart, e.animType = SheepRoleAnimType.Killer, void (e.readySkillId = o.id);
                if (r) return e.state = SheepRoleState.Move, e.subState = SheepRoleSubState.MoveBoss, e.animType = SheepRoleAnimType.Idle, void Util.moveTar(e, r, i, t);
                Util.moveTar(e, null, i, t)
            } else if (s.skillType == SheepSkillType.Bullet) {
                let o = SheepSkillSubBullet.getById(s.id), {
                    atkTar: l,
                    moveTar: n,
                    moveBoss: r
                } = UtilFind.findTar(e);
                if (l || n || r) {
                    this.bullte_creates.push({
                        view_pet: e,
                        bulletId: o.bullet
                    })
                }
                if (l) {
                    return e.state = SheepRoleState.Attack, void (e.subState = SheepRoleSubState.AttackAwait);
                }
                if (n) {
                    return e.state = SheepRoleState.Move, e.subState = SheepRoleSubState.MoveTar, void Util.moveTar(e, n, i, t);
                }
                if (r) {
                    return e.state = SheepRoleState.Move, e.subState = SheepRoleSubState.MoveBoss, e.animType = SheepRoleAnimType.Idle, void Util.moveTar(e, r, i, t);
                }
                Util.moveTar(e, null, i, t)
            } else if (s.skillType == SheepSkillType.CallBullets) {
                let o = SheepSkillSubCallBullets.getById(s.id), {
                    atkTar: l,
                    moveTar: n,
                    moveBoss: r
                } = UtilFind.findTar(e);
                if (l || r) return e.state = SheepRoleState.CallBullets, e.subState = SheepRoleSubState.CallBullets, o.isAnim ? e.animType = SheepRoleAnimType.CallBullets : e.animType = SheepRoleAnimType.Idle, void (e.readySkillId = o.id);
                Util.moveTar(e, null, i, t)
            }
        } else {
            let {atkTar: s, moveTar: o, moveBoss: l} = UtilFind.findTar(e);
            if (s) return e.state = SheepRoleState.Attack, void (e.subState = SheepRoleSubState.AttackAwait);
            if (o) return e.state = SheepRoleState.Move, e.subState = SheepRoleSubState.MoveTar, void Util.moveTar(e, o, i, t);
            if (l) return e.state = SheepRoleState.Move, e.subState = SheepRoleSubState.MoveBoss, void Util.moveTar(e, l, i, t);
            Util.moveTar(e, null, i, t)
        }
    }

    /**
     *
     * @param e {PetView}
     */
    public void update_role_anim(e) {
        e.animFrame = e.animFrame + 1
    }

    /**
     * 向池中 添加 要生产的 单位
     * @param typeID 角色类型 ID
     * @param count 数量
     * @param camp 阵营
     */
    public void produce_pets(typeID, count, camp) {
        // 根据阵营 获取 map
        let callInfos = camp == SheepCamp.Red ? this.redCallInfos : this.blueCallInfos;

        let sheepCallInfo = callInfos.get(typeID);

        // 没有就创建一个 然后加进去
        if (!sheepCallInfo) {
            sheepCallInfo = new SheepCallInfo();
            sheepCallInfo.camp = camp;
            sheepCallInfo.type = typeID;
            sheepCallInfo.count = 0;
            sheepCallInfo.frame = 0;
            sheepCallInfo.count_line = 0;

            sheepCallInfo.items = [];
            sheepCallInfo.pets = [];
            callInfos.set(typeID, sheepCallInfo)
        }

        // todo 爆炸是什么意思? 用什么用?
        sheepCallInfo.count += count;
        sheepCallInfo.pets.push({camp: camp, count: count})
    }

    /**
     * 消耗 待 召唤的池 中的 单位
     * @param sheepCtl {SheepCtl}
     * @param t
     */
    public void consume(sheepCtl, t) {
        let o = this;

        let sheepConfig = SheepConfig;

        this.autoTime += t;

        if (sheepMgr.isAutoCall && this.autoTime > sheepConfig.systemAutomaticTroopsIntervalTime) {
            this.autoTime = 0;
            if (sheepMgr.pets[0].size + sheepMgr.pets[1].size < sheepConfig.systemLongerAutomaticallyDispatch) {
                [SheepCamp.Red, SheepCamp.Blue].forEach(e => {
                    if (sheepMgr.pets[e].size < sheepConfig.systemAutomaticallyMaxTroops) {
                        o.produce_pets(sheepConfig.WarmUpID, sheepConfig.systemAutomaticallyTroopsOneNumber, e)
                    }
                });
            }
        }

        [this.redCallInfos, this.blueCallInfos].forEach((function (t) {
            let n = t == o.redCallInfos ? SheepCamp.Red : SheepCamp.Blue;

            t.forEach((o, a) => {

                if (o.count <= 0) {
                    return;
                }

                let c = SheepRoleTypeInfo.getById(a);
                let formation = SheepRoleFormation.getById(c.formationId);

                let u = n == SheepCamp.Red ? sheepMgr.perfStat.redNums[c.roleType] : sheepMgr.perfStat.blueNums[c.roleType];

                if (c.roleType == SheepRoleType.xiao_bing) {
                    if (u > 14500) {
                        return
                    }
                } else if (c.roleType == SheepRoleType.ci_ke && u > 9500) {
                    return;
                }

                o.frame += 1

                // 限制每帧生成的单位
                if (o.frame <= formation.frameItemX) {
                    return;
                }

                o.frame = 0

                if (formation.formationType == SheepRoleFormationType.RectangleTidy) {
                    let h = formation.itemNumY;
                    let m = formation.itemY;
                    let d = formation.itemYGapNum;
                    let g = formation.itemYGap;

                    let y = formation.startX + sheepMode.startAddX;
                    let S = n == SheepCamp.Red ? -y : y;

                    let pets = o.pets;
                    let I = 0;

                    for (; pets.length > 0 && I < h;) {

                        let T = Math.floor(I / 2);
                        let M = 0;

                        if (h % 2 == 0) {
                            if (I % 2 == 0) {
                                M = m * T + m / 2 + Math.floor(T / d + 1) * g;
                            } else {
                                M = -m * T - m / 2 - Math.floor(T / d + 1) * g;
                            }
                        } else {
                            if (I % 2 == 0) {
                                M = m * Math.floor(T) + Math.floor(T / d) * g;
                            } else {
                                M = -m * Math.floor(T + 1) - Math.floor(T / d) * g;
                            }
                        }
                        let C = pets[0];
                        C.count -= 1;
                        o.count -= 1;
                        I += 1;

                        let R = new Vec3(S, M);
                        if (C.booms) {
                            createPetView(sheepCtl, C.camp, a, h, 1, !0, R, C.booms.pop());
                        } else {
                            createPetView(sheepCtl, C.camp, a, h, 1, !0, R);
                        }
                        C.count <= 0 && pets.shift()
                    }
                    o.count_line += 1;
                    if (o.count_line >= formation.itemNumX) {
                        o.frame -= formation.itemYGapFrame;
                        o.count_line = 0;
                    }
                    o.count <= 0 && t.delete(a)
                } else if (formation.formationType == SheepRoleFormationType.AngleTidy) {

                    let k = 2 * formation.maxAngle - formation.minAngle;
                    let G = Math.floor(k / formation.startStepAngle);
                    let A = Math.floor(1 * G);

                    let pets = o.pets;
                    let b = 0;

                    for (; pets.length > 0 && b < A;) {

                        let P = pets[0];
                        let w = P.count;

                        for (var D = 0; D < w && b < A; D++) {
                            P.count -= 1;
                            o.count -= 1;
                            b += 1;

                            let _ = Math.floor(b / G);
                            let N = formation.startR + sheepMode.startAddR + formation.startStepR * _;

                            let E = (b % 2 == 0 ? 1 : -1) * (Math.floor(b % G / 2) * formation.startStepAngle + formation.minAngle);

                            let x = Math.cos(E * Math.PI / 180) * N;
                            let F = Math.sin(E * Math.PI / 180) * N;
                            let X = null;
                            let L = sheepMode.loongX;
                            X = n == SheepCamp.Red ? new Vec3(L - x, F, 0) : new Vec3(x - L, F, 0);
                            if (P.booms) {
                                createPetView(sheepCtl, P.player, a, G, 1, !0, X, P.booms.pop())
                            } else {
                                createPetView(sheepCtl, P.camp, a, G, 1, !0, X)
                            }
                        }

                        P.count <= 0 && pets.shift()
                    }
                }
            })
        }))
    }

    /**
     * 貌似只负责开场 布阵阶段的 小兵位置 计算
     * @param petId
     * @param camp
     * @returns {*}
     */
    public Vector3 getPetStartEndPos(int petId,SheepCamp camp) {
        let petStartCount = this.petStartCounts[(int)camp];
        let a = petStartCount.get(petId) || 0;
        petStartCount.set(petId, a + 1);
        let sheepRoleTypeInfo = SheepRoleTypeInfo.getById(petId);
        if (!sheepRoleTypeInfo) {
            console.error("SheepMgr.getPetStartEndPos roleId=" + petId + " not found");
        }
        let sheepRoleFormation = SheepRoleFormation.getById(sheepRoleTypeInfo.formationId);
        if (!sheepRoleFormation) {
            console.error("SheepMgr.getPetStartEndPos formationId=" + sheepRoleTypeInfo.formationId + " not found");
        }
        let c = sheepRoleFormation.preItemNumY;
        let l = sheepRoleFormation.preItemX;
        let u = sheepRoleFormation.preItemY;
        let h = sheepRoleFormation.preStartX + Math.floor(a / c) * l;
        let m = Math.floor(a % c);
        let d = 0;
        if (c % 2 == 0) {
            if (m % 2 == 0) {
                d = u * Math.floor(m / 2) + u / 2
            } else {
                d = -u * Math.floor(m / 2) - u / 2
            }
        } else {
            if (m % 2 == 0) {
                d = u * Math.floor(m / 2)
            } else {
                d = -u * Math.floor(m / 2 + 1)
            }
        }
        camp == SheepCamp.Red && (h *= -1)
        return v3(h, d + Utils.random.range(-1, 1), 0)
    }

    public void clearCallPets() {
        this.redCallInfos.clear();
        this.blueCallInfos.clear()
    }



    public void clearBlocks() {
        this.attackViews[SheepCamp.Red].fill(null);
        this.attackViews[SheepCamp.Blue].fill(null);
        this.attackView1s[SheepCamp.Red].fill(null);
        this.attackView1s[SheepCamp.Blue].fill(null);

        for (let e = 0; e < SheepConfig.MaxGroupCount; e++) {
            this.collisionViews[SheepCamp.Red][e].fill(null);
            this.collisionViews[SheepCamp.Blue][e].fill(null);
            this.collisionView1s[SheepCamp.Red][e].fill(null);
            this.collisionView1s[SheepCamp.Blue][e].fill(null);
        }

        this.pre_blocks.clear()
    }

    public IndexLen getBlockByIndex(IndexLen[] e,int blockIndex){
        if (!e[blockIndex]){
            e[blockIndex]={
                Len:0,
                Index:0,
            };
        }
        return e[blockIndex];
    }

    public void mainClearBlocks() {
        null == this.isChangeAckFlags && (this.isChangeAckFlags = [!0, !0])
        if (null == this.isChangeCollsionFlags) {
            this.isChangeCollsionFlags = [[], []];
            for (let e = 0; e < SheepConfig.MaxGroupCount; e++) {
                this.isChangeCollsionFlags[SheepCamp.Red].push(true);
                this.isChangeCollsionFlags[SheepCamp.Blue].push(true)
            }
        }
        Date.now();
        this.isChangeAckFlags[SheepCamp.Red] && this.attackViews[SheepCamp.Red].fill(null);
        this.isChangeAckFlags[SheepCamp.Red] = !1;
        this.isChangeAckFlags[SheepCamp.Blue] && this.attackViews[SheepCamp.Blue].fill(null);
        this.isChangeAckFlags[SheepCamp.Blue] = !1;
        for (let e = 0; e < SheepConfig.MaxGroupCount; e++) {
            this.isChangeCollsionFlags[SheepCamp.Red][e] && this.collisionViews[SheepCamp.Red][e].fill(null);
            this.isChangeCollsionFlags[SheepCamp.Red][e] = false;
            this.isChangeCollsionFlags[SheepCamp.Blue][e] && this.collisionViews[SheepCamp.Blue][e].fill(null);
            this.isChangeCollsionFlags[SheepCamp.Blue][e] = false;
        }
        Date.now();
        this.pre_blocks.clear();
        Date.now()
    }

    public void mainPreAddBlock(int blockIndex,int buffIndex,SheepCamp camp,int collideId) {
        let o = this.pre_blocks.get(blockIndex);
        if (!o) {
            o = [];
            this.pre_blocks.set(blockIndex, o);
        }

        let l = o[camp];
        if (!l) {
            l = [];
            o[camp] = l;
        }
        let n = l[collideId];
        if (!n) {
            n = new Array;
            l[collideId] = n;
        }
        n.push(buffIndex);
        if (this.isChangeAckFlags && 0 == this.isChangeAckFlags[camp]) {
            this.isChangeAckFlags[camp] = true;
        }
        if (this.isChangeCollsionFlags && 0 == this.isChangeCollsionFlags[camp][collideId]) {
            this.isChangeCollsionFlags[camp][collideId] = true
        }
    }

    public void mainSyncBlocksToWokers() {
        let e = [0, 0];
        let t = [[], []];
        for (let e = 0; e < SheepConfig.MaxGroupCount; e++) {
            t[SheepCamp.Red].push(0);
            t[SheepCamp.Blue].push(0);
        }
        this.pre_blocks.forEach((i, s) => {
            if (i && i.length) {
                i.forEach((i, o) => {
                    if (i && i.length) {
                        let l = 0;
                        let n = t[o];
                        i.forEach((e, t) => {
                            if (e && e.length) {
                                let i = this.collisionViews[o][t];
                                let a = this.collisionView1s[o][t];
                                let c = n[t];
                                let f = e.length;
                                l += f;
                                let blockByIndex = this.getBlockByIndex(i,s);
                                blockByIndex.Index=  c;
                                blockByIndex.Len=  f;
                                e.forEach(e => {
                                    a[c] = e;
                                    c++
                                });
                                n[t] = c
                            }
                        });
                        if (0 == l) {
                            return;
                        }
                        let a = this.attackViews[o];
                        let c = this.attackView1s[o];
                        let f = e[o];
                        let blockByIndex = this.getBlockByIndex(a,s);
                        blockByIndex.Index=  f;
                        blockByIndex.Len=  l;
                        i.forEach((e, t) => {
                            if(e && e.length) {
                                e.forEach(e => {
                                    c[f] = e;
                                    f++
                                })
                            }
                        });
                        e[o] = f
                    }
                })
            }
        })
    }

    public void forEachBlock(IndexLen[] ee,int[] t,int blockIndex,Action<int> callback) {
        var blockByIndex = this.getBlockByIndex(ee, blockIndex);
        var o = blockByIndex.Index;
        var l = blockByIndex.Len;
        if (l!=0) {
            for (var e = 0; e < l; e++) {
                callback(t[o + e]);
            }
        }
    }

    public bool findBlock(IndexLen[] e,int[] t,int blockIndex,Func<int, bool> callback) {
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


        public static SheepMgr sheepMgr = new SheepMgr();
    }
}