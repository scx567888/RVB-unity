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

        public object view_pets;
        public object view_bullets;
        public object pre_view_bullets;
        public object updateTime;
        public object petsAdd;
        public object petsDel;
        public object petCount;
        public object bulletsDel;
        public object bulletCount;
        public object bulletId;

        public int[] logic_counts;

        public int[] bullte_creates;

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
            this.preBuffs =new int[][] {
              new int[]{},  
              new int[]{},  
            };
            this.buffs = new Buff[][] {
                new Buff[]{},  
                new Buff[]{},  
            };;
            this.countNewBuffs = new int[]{0, 0};
            this.countBuffs = new int[]{0, 0};
            this.countShowBuffs = new int[]{0, 0};
            this.flagLongBuffs = new bool[]{false, false};
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
            this.god_view_pets = new PetView[]{};
        }
        
        public void setState(SheepRoomState e) {
            this.state = e;
            Debug.Log("房间状态改变"+ e);
            // todo 待处理
            // eventBus.emit(EventType.RoomState, {state: e})
        }
        
        public void addPet(PetView e,SheepCamp camp) {
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
            this.perfStat.redNums=new int[]{0,0,0,0,0,0,0,0};
            this.perfStat.blueNums=new int[]{0,0,0,0,0,0,0,0};
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

        public static SheepMgr sheepMgr = new SheepMgr();
    }
}