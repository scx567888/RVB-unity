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

    }
}