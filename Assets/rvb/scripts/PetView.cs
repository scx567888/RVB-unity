using System;
using System.Collections.Generic;
using UnityEngine;
using static rvb.scripts.SheepMgr;
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
        public int buff_index;
        public PetView view_pet;
        public float scale;
        public BuffTimeAttacher attacher;
        public int petId;
        public Vector3? pos;
        public Vector3? position;


        public PetView(int t) {
            index = t;
            conf = SheepRoleTypeInfo.getById(roleId);
            uids = new List<int>();
            skinId = null;
            buff_index = -1;
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

        private static T arrOn<T>(T[] r) {
            return r[UnityEngine.Random.Range(0, r.Length)];
        }

        public void init(int buffIndex, PetView viewPet) {
            viewPet.clear();

            Vector3 a = position.Value;
            int x = Mathf.FloorToInt(a.x);
            int y = Mathf.FloorToInt(a.y);

            int blockIndex = sheepMgr.getBlockIndex(new Vector3(x, y, 0));

            viewPet.id = sheepMgr.getNextPetId();

            viewPet.isActive = true;
            viewPet.isDie = false;
            viewPet.camp = camp;
            viewPet.roleId = petId;
            viewPet.skinId = skinId ?? 0;
            viewPet.conf = conf;

            if (petId != 0) {
                if (sheepMgr.state == SheepRoomState.Start) {
                    viewPet.state = SheepRoleState.Start;
                    viewPet.subState = SheepRoleSubState.Start;
                    viewPet.animType = SheepRoleAnimType.Idle;
                    viewPet.animFrame = UnityEngine.Random.Range(0, 10);
                }
                else if (conf.skillIn!=0) {
                    viewPet.state = SheepRoleState.In;
                    viewPet.subState = SheepRoleSubState.In;
                    viewPet.animType = SheepRoleAnimType.In;
                    viewPet.animFrame = 0;
                }
                else if (conf.startState == SheepRoleState.In) {
                    viewPet.state = conf.startState;
                    viewPet.subState = SheepRoleSubState.In;
                    viewPet.animType = SheepRoleAnimType.In;
                    viewPet.animFrame = 0;
                }
                else if (conf.startState == SheepRoleState.SpinSpurt) {
                    viewPet.state = conf.startState;
                    viewPet.animType = SheepRoleAnimType.Attack;
                    viewPet.animFrame = 0;
                }
                else {
                    viewPet.state = conf.startState;
                    viewPet.subState = SheepRoleSubState.Spurt;

                    if (conf.isSpurtAnim) {
                        viewPet.animType = SheepRoleAnimType.Spurt;
                        viewPet.animFrame = UnityEngine.Random.Range(0, 10);
                    }
                    else {
                        viewPet.animType = SheepRoleAnimType.Idle;
                        viewPet.animFrame = UnityEngine.Random.Range(0, 10);
                    }
                }
            }

            viewPet.frame = 0;
            viewPet.posBefX = x;
            viewPet.posBefY = y;
            viewPet.animX = x;
            viewPet.animY = y;
            viewPet.posX = x;
            viewPet.posY = y;
            viewPet.befBlockIndex = blockIndex;
            viewPet.blockIndex = blockIndex;

            if (petId != 0 && sheepMgr.state == SheepRoomState.Start) {
                var m = sheepMgr.getPetStartEndPos(petId, camp);

                viewPet.tarPosX = m.x;
                viewPet.tarPosY = m.y;
                viewPet.animY = m.y;
                viewPet.posBefY = m.y;
                viewPet.posY = m.y;
            }

            var l = SheepRoleFormation.getById(conf.formationId);
            float d = camp == SheepCamp.Red ? 1 : -1;

            if (l.formationType == SheepRoleFormationType.RectangleTidy ||
                l.formationType == SheepRoleFormationType.RectangleRandom) {
                viewPet.dirX = d;
                viewPet.dirY = 0;
            }
            else if (l.formationType == SheepRoleFormationType.AngleTidy ||
                     l.formationType == SheepRoleFormationType.AngleRandom) {
                Vector3 g = new Vector3(
                    d * sheepMode.loongX - x,
                    0 - y,
                    0
                ).normalized;

                viewPet.dirX = g.x;
                viewPet.dirY = g.y;
            }

            if (petId != 0 && sheepMgr.state == SheepRoomState.Start) {
                viewPet.isConnNot = true;
            }
            else {
                viewPet.isConnNot = false;
            }

            viewPet.tarIndex = -1;
            viewPet.tarId = -1;
            viewPet.curHp = conf.hp;
            viewPet.curAtkBuff = 0;

            if (isBoom) {
                viewPet.isConnNot = true;
                viewPet.isBoom = true;
            }
            else {
                viewPet.isBoom = false;
            }

            buff_index = buffIndex;
            view_pet = viewPet;

            foreach (var e in sheepMgr.buffs) {
                foreach (var item in e) {
                    double i = (item.time - sheepMgr.gameStartTimerForBuff) / 1e3;

                    int r = item.count;

                    addGeneralOrderBuff(viewPet, i, r);
                }
            }

            if (sheepMgr.state == SheepRoomState.Start && viewPet.conf.roleType == SheepRoleType.yang_shen) {
                sheepMgr.god_view_pets.Add(viewPet);
            }
        }

        public void updateSkin(dynamic e, SheepMgr t, SheepMgr n, double o) {
            PetView a = this;
            PetView i = a.view_pet;
            int buffIndex = a.buff_index;

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
                    i.animFrame = UnityEngine.Random.Range(0, 10);
                }

                onDead();
            }

            if (!isDie) {
                t.mainPreAddBlock( blockIndex, buffIndex, camp, a.conf.collideId );

                int S = i.conf.detectCollideR;

                for (int y = -S; y <= S; ++y) {
                    for (int v = -S; v <= S; ++v) {
                        // todo 这里需要一些处理
                        // e.comImages.mesh_block.addFrameBlockCamp(blockIndex, camp);
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
            view_pet.isDie = true;
            view_pet.id = 0;
            attacher.clear();
        }

        public void onRes(dynamic e, SheepMgr t) {
            view_pet.isActive = false;
            t.buff_del_pet(buff_index);
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