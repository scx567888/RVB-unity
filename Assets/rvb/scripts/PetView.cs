using System;
using System.Collections.Generic;
using UnityEngine;
using static rvb.scripts.SheepModes;

namespace rvb.scripts {
    public enum BuffID {
        GeneralOrder = 0,
        CardBuff = 1
    }

    /// <summary>
    /// 单位数据定义
    /// </summary>
    public class PetView {
        public int id = 0;
        public bool isActive = false;
        public bool isDie;
        public SheepCamp camp = (SheepCamp)0;
        public int roleId = 0;
        public int? skinId = 0;
        public SheepRoleState state = (SheepRoleState)0;
        public SheepRoleSubState subState = (SheepRoleSubState)0;
        public bool isLock = false;
        public int frame = 0;
        public float posBefX = 0;
        public float posBefY = 0;
        public float animX = 0;
        public float animY = 0;
        public float animZ = 0;
        public float posX = 0;
        public float posY = 0;
        public int befBlockIndex = 0;
        public int blockIndex = 0;
        public float dirX = 0;
        public float dirY = 0;
        public int tarIndex = 0;
        public int tarId = 0;
        public float curHp = 0;
        public float curAtkBuff = 0;
        public int curAckFrame = 0;
        public float curAckCd = 0;
        public bool isHeavyAtk = false;
        public bool isNotConn = false;
        public bool isBoom = false;
        public SheepRoleAnimType _animType = (SheepRoleAnimType)0;
        public int animFrame = 0;
        public float tarPosX = 0;
        public float tarPosY = 0;
        public float impulseX = 0;
        public float impulseY = 0;
        public int readySkillId = 0;
        public int energy = 0;

        public SheepRoleTypeInfo conf;

        public int index;
        public List<int> uids;
        public PetView view_pet;
        public float scale;
        public BuffTimeAttacher attacher;
        public int petId;
        public Vector3? pos;
        public Vector3? position;


        public SheepMgr sheepMgr;


        public PetView(int t) {
            index = t;
            conf = SheepRoleTypeInfo.getById(roleId);
            uids = new List<int>();
            skinId = null;
            view_pet = null;
            scale = 1;
            attacher = null;
            camp = default;
            state = default;
            petId = default;
            conf = null;
            pos = null;
            isDie = false;
            isBoom = false;
            position = null;
        }

        private T arrOn<T>(T[] r) {
            return r[sheepMgr.RandomInt(0, r.Length)];
        }

        public void init(PetView buffIndex, PetView viewPet) {
            // this.clear();

            Vector3 a = position.Value;
            int x = Mathf.FloorToInt(a.x);
            int y = Mathf.FloorToInt(a.y);

            int blockIndex = sheepMgr.getBlockIndex(new Vector3(x, y, 0));

            this.id = sheepMgr.getNextPetId();

            this.isActive = true;
            this.isDie = false;
            this.camp = camp;
            this.roleId = petId;
            this.skinId = skinId ?? 0;
            this.conf = conf;

            if (petId != 0) {
                if (sheepMgr.state == SheepRoomState.Start) {
                    this.state = SheepRoleState.Start;
                    this.subState = SheepRoleSubState.Start;
                    this.animType = SheepRoleAnimType.Idle;
                    this.animFrame = sheepMgr.RandomInt(0, 10);
                }
                else if (conf.skillIn!=0) {
                    this.state = SheepRoleState.In;
                    this.subState = SheepRoleSubState.In;
                    this.animType = SheepRoleAnimType.In;
                    this.animFrame = 0;
                }
                else if (conf.startState == SheepRoleState.In) {
                   this.state = conf.startState;
                   this.subState = SheepRoleSubState.In;
                   this.animType = SheepRoleAnimType.In;
                   this.animFrame = 0;
                }
                else if (conf.startState == SheepRoleState.SpinSpurt) {
                    this.state = conf.startState;
                    this.animType = SheepRoleAnimType.Attack;
                    this.animFrame = 0;
                }
                else {
                    this.state = conf.startState;
                    this.subState = SheepRoleSubState.Spurt;

                    if (conf.isSpurtAnim) {
                        this.animType = SheepRoleAnimType.Spurt;
                        this.animFrame = sheepMgr.RandomInt(0, 10);
                    }
                    else {
                        this.animType = SheepRoleAnimType.Idle;
                        this.animFrame = sheepMgr.RandomInt(0, 10);
                    }
                }
            }

            this.frame = 0;
            this.posBefX = x;
            this.posBefY = y;
            this.animX = x;
            this.animY = y;
            this.posX = x;
            this.posY = y;
            this.befBlockIndex = blockIndex;
            this.blockIndex = blockIndex;

            if (petId != 0 && sheepMgr.state == SheepRoomState.Start) {
                var m = sheepMgr.getPetStartEndPos(petId, camp);

                this.tarPosX = m.x;
                this.tarPosY = m.y;
                this.animY = m.y;
                this.posBefY = m.y;
                this.posY = m.y;
            }

            var l = SheepRoleFormation.getById(conf.formationId);
            float d = camp == SheepCamp.Red ? 1 : -1;

            if (l.formationType == SheepRoleFormationType.RectangleTidy ||
                l.formationType == SheepRoleFormationType.RectangleRandom) {
                this.dirX = d;
                this.dirY = 0;
            }
            else if (l.formationType == SheepRoleFormationType.AngleTidy ||
                     l.formationType == SheepRoleFormationType.AngleRandom) {
                Vector3 g = new Vector3(
                    d * sheepMode.loongX - x,
                    0 - y,
                    0
                ).normalized;

                this.dirX = g.x;
                this.dirY = g.y;
            }

            if (petId != 0 && sheepMgr.state == SheepRoomState.Start) {
                this.isConnNot = true;
            }
            else {
                this.isConnNot = false;
            }

            this.tarIndex = -1;
            this.tarId = -1;
            this.curHp = conf.hp;
            this.curAtkBuff = 0;

            if (isBoom) {
                this.isConnNot = true;
                this.isBoom = true;
            }
            else {
                this.isBoom = false;
            }
            
            // view_pet = viewPet;

            foreach (var e in sheepMgr.buffs) {
                foreach (var item in e) {
                    double i = (item.time - sheepMgr.gameStartTimerForBuff) / 1e3;

                    int r = item.count;

                    addGeneralOrderBuff(this, i, r);
                }
            }

            if (sheepMgr.state == SheepRoomState.Start && conf.roleType == SheepRoleType.yang_shen) {
                sheepMgr.god_view_pets.Add(this);
            }
        }

        public void updateSkin(SheepCtl e, SheepMgr t, SheepMgr n, double o) {
            PetView a = this;
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
                    i.animType= (SheepRoleAnimType)arrOn(i.conf.deadAnimType);
                }

                if (i.conf.roleType == SheepRoleType.xiao_bing) {
                    i.animFrame = sheepMgr.RandomInt(0, 10);
                }

                onDead();
            }

            if (!isDie) {
                t.mainPreAddBlock( blockIndex, this, camp, a.conf.collideId );

                int S = i.conf.detectCollideR;

                for (int y = -S; y <= S; ++y) {
                    for (int v = -S; v <= S; ++v) {
                        
                        e.comImages.mesh_block.addFrameBlockCamp(blockIndex, camp);
                    }
                }

                Vector3 B = new Vector3(i.animX, i.animY, 0);

                a.position = B;
            }

            if (!isDie) {
                int countNewBuff = n.countNewBuffs[(int)camp];

                if (countNewBuff != 0) {
                    addGeneralOrderBuff(i, SheepConfig.buffLastTime, countNewBuff);
                }
            }

            attacher.updateTimer(o / 1e3);
        }

        public void onDead() {
            this.isDie = true;
            // this.id = 0;
            attacher.clear();
        }

        public void onRes(dynamic e, SheepMgr t) {
            this.isActive = false;
            id = 0;
            t.buff_del_pet(this);
            sheepMgr.delPet(this);
            view_pet = null;
        }

        public void addGeneralOrderBuff(PetView e, double t, int n) {
            PetView o = this;

            attacher.addIndependBuff(
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

        public bool isConnNot {
            set { isNotConn = value; }
        }

        public SheepRoleAnimType animType {
            get { return _animType; }

            set {
                _animType = value;
                animFrame = 0;
            }
        }

        public float subCurHp(int t) {
            float old = curHp;
            curHp -= t;
            return old;
        }

        public void logicMove(float x, float y) {
            posBefX = posX;
            posBefY = posY;

            int befBlockIndex = Util.getIndexByXY(
                posBefX,
                posBefY
            );

            posX = x;
            posY = y;

            int blockIndex = Util.getIndexByXY(
                posX,
                posY
            );

            this.befBlockIndex = befBlockIndex;
            this.blockIndex = blockIndex;
        }

        public void clear() {
            id = 0;
            isActive = false;
            isDie = false;
            camp = (SheepCamp)0;
            roleId = 0;
            skinId = 0;
            state = (SheepRoleState)0;
            subState = (SheepRoleSubState)0;
            isLock = false;
            frame = 0;
            posBefX = 0;
            posBefY = 0;
            animX = 0;
            animY = 0;
            animZ = 0;
            posX = 0;
            posY = 0;
            befBlockIndex = 0;
            blockIndex = 0;
            dirX = 0;
            dirY = 0;
            tarIndex = 0;
            tarId = 0;
            curHp = 0;
            curAtkBuff = 0;
            curAckFrame = 0;
            curAckCd = 0;
            isHeavyAtk = false;
            isNotConn = false;
            isBoom = false;
            _animType = (SheepRoleAnimType)0;
            animFrame = 0;
            tarPosX = 0;
            tarPosY = 0;
            impulseX = 0;
            impulseY = 0;
            readySkillId = 0;
            energy = 0;
        }
    }
}