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
        public boss = [null, null];

        // 地块比例
        public float plotRatio = 0.5f;

        // 核心状态机
        public SheepRoomState state = SheepRoomState.Ready;

        // 尝试角色 (todo 但是是哪一种? 当前在场上的? )
        public pets = [new Set, new Set];

        public int gameIndex = 0;
        public int gameStartTimerForBuff = 0;
        public int cameraEulerAngles = v3();
        public int endTime = 0;
        public int preBuffs = [[], []];
        public Buff[] buffs = [[], []];
        public int countNewBuffs = [0, 0];
        public int countBuffs = [0, 0];
        public int countShowBuffs = [0, 0];

        // 反击时刻标识符 (防止多次触发反击时刻)
        public flagLongBuffs = [false, false];

        public petStartCounts = [new Map, new Map];
        public god_view_pets = [];
        public perfStat = {
            redNums: [0, 0, 0, 0, 0, 0, 0, 0],
            blueNums: [0, 0, 0, 0, 0, 0, 0, 0],
        };

    public view_pets = [];
    public view_bullets = [];
    public pre_view_bullets = [];
    public updateTime = undefined;
    public petsAdd = [];
    public petsDel = [];
    public petCount = 0;
    public bulletsDel = [];
    public bulletCount = 0;
    public bulletId = 0;

    public logic_counts = [1, 1];

    public bullte_creates = [];

    public pre_blocks = new Map;
    public isChangeCollsionFlags = null;
    public isChangeAckFlags = null;

    public MaxCount = SheepConfig.line_w * SheepConfig.line_w

    public attackViews = [
            new Array(this.MaxCount),
            new Array(this.MaxCount),
        ];

    public attackView1s = [
            new Array(SheepConfig.MaxPetCount),
            new Array(SheepConfig.MaxPetCount)
        ];

    public collisionViews = [[], []];
        

    public collisionView1s = [[], []];
        

        /**
         * 红方召唤池
         * key 是 类型 id
         * @type {Map<Number,SheepCallInfo>}
         */
        public redCallInfos = new Map();

        /**
         * 蓝方召唤池
         * key 是 类型 id
         * @type {Map<Number,SheepCallInfo>}
         */
        public blueCallInfos = new Map();

        /**
         * 是否自动出兵
         * @type {boolean}
         */


        // ************************ 以下待整理 **************************

        /**
         * @type ComSheepImages
         */
        public comImages=null;

        public cur_rob_role_index;
        public cur_rob_bullet_index;
        public cur_rob_role_mesh_index;
        public cur_rob_bullet_mesh_index;
        public cur_rob_star_mesh_index;
        public roleMaxIndex;
        public bulletMaxIndex;
        public preBulletIndex;
        public curIndexImages;
        public int redBuffCount;
        public int blueBuffCount;

        // 角色 id 分配器
        public int petId ;



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
        this.boss = [null, null];

        // 地块比例
        this.plotRatio = 0.5;

        // 核心状态机
        this.state = SheepRoomState.Ready;

        // 尝试角色 (todo 但是是哪一种? 当前在场上的? )
        this.pets = [new Set, new Set];

        this.gameIndex = 0;
        this.gameStartTimerForBuff = 0;
        this.cameraEulerAngles = v3();
        this.endTime = 0;
        this.preBuffs = [[], []];
        this.buffs = [[], []];
        this.countNewBuffs = [0, 0];
        this.countBuffs = [0, 0];
        this.countShowBuffs = [0, 0];

        // 反击时刻标识符 (防止多次触发反击时刻)
        this.flagLongBuffs = [false, false];

        this.petStartCounts = [new Map, new Map];
        this.god_view_pets = [];
        this.perfStat = {
            redNums: [0, 0, 0, 0, 0, 0, 0, 0],
            blueNums: [0, 0, 0, 0, 0, 0, 0, 0],
        };

        this.view_pets = [];
        this.view_bullets = [];
        this.pre_view_bullets = [];
        this.updateTime = undefined;
        this.petsAdd = [];
        this.petsDel = [];
        this.petCount = 0;
        this.bulletsDel = [];
        this.bulletCount = 0;
        this.bulletId = 0;

        this.logic_counts = [1, 1];

        this.bullte_creates = [];

        this.pre_blocks = new Map;
        this.isChangeCollsionFlags = null;
        this.isChangeAckFlags = null;

        this.MaxCount = SheepConfig.line_w * SheepConfig.line_w

        this.attackViews = [
            new Array(this.MaxCount),
            new Array(this.MaxCount),
        ];

        this.attackView1s = [
            new Array(SheepConfig.MaxPetCount),
            new Array(SheepConfig.MaxPetCount)
        ];

        this.collisionViews = [[], []];
        for (let e = 0; e < SheepConfig.MaxGroupCount; e++) {
            this.collisionViews[SheepCamp.Red].push(new Array( this.MaxCount));
            this.collisionViews[SheepCamp.Blue].push(new Array(this.MaxCount));
        }

        this.collisionView1s = [[], []];
        for (let e = 0; e < SheepConfig.MaxGroupCount; e++) {
            this.collisionView1s[SheepCamp.Red].push(new Array(SheepConfig.MaxPetCount));
            this.collisionView1s[SheepCamp.Blue].push(new Array(SheepConfig.MaxPetCount))
        }

        /**
         * 红方召唤池
         * key 是 类型 id
         * @type {Map<Number,SheepCallInfo>}
         */
        this.redCallInfos = new Map();

        /**
         * 蓝方召唤池
         * key 是 类型 id
         * @type {Map<Number,SheepCallInfo>}
         */
        this.blueCallInfos = new Map();

        /**
         * 是否自动出兵
         * @type {boolean}
         */


        // ************************ 以下待整理 **************************

        /**
         * @type ComSheepImages
         */
        this.comImages=null;

        this.cur_rob_role_index;
        this.cur_rob_bullet_index;
        this.cur_rob_role_mesh_index;
        this.cur_rob_bullet_mesh_index;
        this.cur_rob_star_mesh_index;
        this.roleMaxIndex;
        this.bulletMaxIndex;
        this.preBulletIndex;
        this.curIndexImages;
        this.redBuffCount;
        this.blueBuffCount;

        // 角色 id 分配器
        this.petId = 0;


        // 绑定 system
        Util.system = this;
        UtilFind.system = this;
        UtilAck.system = this;

        window.system = this;

        }
        
        
        public static SheepMgr sheepMgr = new SheepMgr();
    }
}