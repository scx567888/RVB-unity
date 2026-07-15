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
        public PetView[] god_view_pets;
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

        public static SheepMgr sheepMgr = new SheepMgr();
    }
}