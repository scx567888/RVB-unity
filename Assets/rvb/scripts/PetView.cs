using System;
using System.Collections.Generic;
using UnityEngine;
using static rvb.scripts.SheepMgr;
using static rvb.scripts.SheepModes;

namespace rvb.scripts {
    /// <summary>
    /// 与原 JS 文件中的 BuffID 常量对象对应。
    /// </summary>
    public enum BuffID {
        GeneralOrder = 0,
        CardBuff = 1
    }

    /// <summary>
    /// GeneralOrder Buff 写入 buff.arg 的数据。
    /// </summary>
    public sealed class GeneralOrderBuffArg {
        public int addHp;
        public float addAtk;
    }

    /// <summary>
    /// 单位数据定义。
    /// 字段名、方法名和主要执行顺序均保持原 JS 写法。
    /// </summary>
    public class PetView {

        public int id = 0;
        public bool isActive = false;
        public bool isDie;
        public SheepCamp camp = (SheepCamp)0;
        public int roleId = 0;
        public int skinId = 0;
        public SheepRoleState state = (SheepRoleState)0;
        public SheepRoleSubState subState = (SheepRoleSubState)0;
        public int isLock = 0;
        public int frame = 0;
        public float posBefX = 0f;
        public float posBefY = 0f;
        public float animX = 0f;
        public float animY = 0f;
        public float animZ = 0f;
        public float posX = 0f;
        public float posY = 0f;
        public int befBlockIndex = 0;
        public int blockIndex = 0;
        public float dirX = 0f;
        public float dirY = 0f;
        public int tarIndex = 0;
        public int tarId = 0;
        public float curHp = 0f;
        public float curAtkBuff = 0f;
        public int curAckFrame = 0;
        public float curAckCd = 0f;
        public bool isHeavyAtk = false;
        public bool isNotConn = false;
        public bool isBoom = false;
        public SheepRoleAnimType _animType = (SheepRoleAnimType)0;
        public int animFrame = 0;
        public float tarPosX = 0f;
        public float tarPosY = 0f;
        public float impulseX = 0f;
        public float impulseY = 0f;
        public int readySkillId = 0;
        public float energy = 0f;

        public SheepRoleTypeInfo conf;

        // constructor 中出现、但没有在类字段区显式声明的 JS 成员。
        public int index;
        public List<object> uids;
        public int buff_index;
        public PetView view_pet;
        public float scale;

        // 请在接入项目后将 dynamic 替换为现有 Buff Attacher 的实际类型。
        // 需要具有 addIndependBuff、updateTimer、clear 方法。
        public BuffTimeAttacher attacher;

        public int petId;
        public Vector3? pos;
        public Vector3? position;

        public PetView(int t) {
            index = t;

            // 原 JS 先取一次配置，随后又把 conf 设为 undefined。
            // 此处保留其执行顺序和最终结果。
            conf = SheepRoleTypeInfo.getById(roleId);

            uids = new List<object>();
            skinId = 0;
            buff_index = -1;
            view_pet = null;
            scale = 1f;
            attacher = null;
            camp = (SheepCamp)0;
            state = (SheepRoleState)0;
            petId = 0;
            conf = null;
            pos = null;
            isDie = false;
            isBoom = false;
            position = null;
        }

        private static T arrOn<T>(IReadOnlyList<T> values) {
            int index = UnityEngine.Random.Range(0, values.Count);
            return values[index];
        }

        public void init(int buffIndex, PetView viewPet) {
            viewPet.clear();

            if (!position.HasValue) {
                // JS 在 position 为 null 时访问 position.x 会直接报错。
                throw new InvalidOperationException("PetView.position 尚未设置，无法执行 init。");
            }

            Vector3 a = position.Value;
            int x = Mathf.FloorToInt(a.x);
            int y = Mathf.FloorToInt(a.y);

            int blockIndex = sheepMgr.getBlockIndex(new Vector3(x, y, 0f));

            viewPet.id = sheepMgr.getNextPetId();

            viewPet.isActive = true;
            viewPet.isDie = false;
            viewPet.camp = camp;
            viewPet.roleId = petId;
            viewPet.skinId = skinId != 0 ? skinId : 0;
            viewPet.conf = conf;

            if (petId != 0) {
                if (sheepMgr.state == SheepRoomState.Start) {
                    viewPet.state = SheepRoleState.Start;
                    viewPet.subState = SheepRoleSubState.Start;
                    viewPet.animType = SheepRoleAnimType.Idle;
                    viewPet.animFrame = UnityEngine.Random.Range(0, 10);
                }
                else if (conf.skillIn != 0) {
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

                    if (conf.isSpurtAnim != 0) {
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

            var formation = SheepRoleFormation.getById(conf.formationId);
            float d = camp == SheepCamp.Red ? 1f : -1f;

            if (
                formation.formationType == SheepRoleFormationType.RectangleTidy ||
                formation.formationType == SheepRoleFormationType.RectangleRandom
            ) {
                viewPet.dirX = d;
                viewPet.dirY = 0f;
            }
            else if (
                formation.formationType == SheepRoleFormationType.AngleTidy ||
                formation.formationType == SheepRoleFormationType.AngleRandom
            ) {
                Vector3 g = new Vector3(
                    d * (float)sheepMode.loongX - x,
                    -y,
                    0f
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
            viewPet.curAtkBuff = 0f;

            if (isBoom) {
                viewPet.isConnNot = true;
                viewPet.isBoom = true;
            }
            else {
                viewPet.isBoom = false;
            }

            buff_index = buffIndex;
            view_pet = viewPet;

            // 对应 JS：sheepMgr.buffs.forEach(group => group.forEach(buff => ...))
            foreach (var buffGroup in sheepMgr.buffs) {
                foreach (var buff in buffGroup) {
                    float time = (buff.time - sheepMgr.gameStartTimerForBuff) / 1000f;
                    int count = buff.count;
                    addGeneralOrderBuff(viewPet, time, count);
                }
            }

            if (
                sheepMgr.state == SheepRoomState.Start &&
                viewPet.conf.roleType == SheepRoleType.yang_shen
            ) {
                sheepMgr.god_view_pets.Add(viewPet);
            }
        }

        /// <summary>
        /// e、t、n 在原 JS 中没有提供类型信息。
        /// dynamic 只用于保持原成员访问方式；接入时可替换为项目中的实际类型。
        /// </summary>
        public void updateSkin(object e, SheepMgr t, SheepMgr n, float o) {
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

            if (curHp <= 0f) {
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

                if (i.conf.deadAnimType != null && i.conf.deadAnimType.Length > 0) {
                    i.animType = (SheepRoleAnimType)arrOn(i.conf.deadAnimType);
                }

                if (i.conf.roleType == SheepRoleType.xiao_bing) {
                    i.animFrame = UnityEngine.Random.Range(0, 10);
                }

                onDead();
            }

            if (!isDie) {
                t.mainPreAddBlock(blockIndex, buffIndex, camp, a.conf.collideId);

                int S = i.conf.detectCollideR;

                for (int y = -S; y <= S; ++y) {
                    for (int v = -S; v <= S; ++v) {
                        // 原 JS 循环中没有使用 y/v，重复添加同一个 blockIndex。
                        e.comImages.mesh_block.addFrameBlockCamp(blockIndex, camp);
                    }
                }

                Vector3 B = new Vector3(i.animX, i.animY, 0f);
                a.position = B;
            }

            if (!isDie) {
                int countNewBuff = (int)n.countNewBuffs[(int)camp];
                if (countNewBuff != 0) {
                    addGeneralOrderBuff(i, SheepConfig.buffLastTime, countNewBuff);
                }
            }

            attacher.updateTimer(o / 1000f);
        }

        public void onDead() {
            view_pet.isDie = true;
            view_pet.id = 0;
            attacher.clear();
        }

        public void onRes(object e, SheepMgr t) {
            view_pet.isActive = false;
            t.buff_del_pet(buff_index);
            sheepMgr.delPet(this);
            view_pet = null;
        }

        public void addGeneralOrderBuff(PetView e, float t, int n) {
            PetView o = this;

            attacher.addIndependBuff(
                (int)BuffID.GeneralOrder,
                t,
                (Action<i>)(buff => {
                    var arg = new GeneralOrderBuffArg {
                        addHp = Mathf.FloorToInt(
                            o.conf.hp * SheepConfig.buffHpIncreaseRate * n
                        ),
                        addAtk = n * SheepConfig.buffAtkIncreaseRate * 100f
                    };

                    buff.arg = arg;
                    e.curHp += arg.addHp;
                    e.curAtkBuff += arg.addAtk;
                }),
                (Action<i>)(buff => {
                    GeneralOrderBuffArg arg = (GeneralOrderBuffArg)buff.arg;
                    e.curHp -= arg.addHp;
                    e.curAtkBuff -= arg.addAtk;
                })
            );
        }

        public bool isConnNot {
            set => isNotConn = value;
        }

        public SheepRoleAnimType animType {
            get => _animType;
            set {
                _animType = value;
                animFrame = 0;
            }
        }

        public float subCurHp(float t) {
            float old = curHp;
            curHp -= t;
            return old;
        }

        public void logicMove(float x, float y) {
            posBefX = posX;
            posBefY = posY;

            int befIndex = Util.getIndexByXY(posBefX, posBefY);

            posX = x;
            posY = y;

            int newBlockIndex = Util.getIndexByXY(posX, posY);
            befBlockIndex = befIndex;
            blockIndex = newBlockIndex;
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
            isLock = 0;
            frame = 0;
            posBefX = 0f;
            posBefY = 0f;
            animX = 0f;
            animY = 0f;
            animZ = 0f;
            posX = 0f;
            posY = 0f;
            befBlockIndex = 0;
            blockIndex = 0;
            dirX = 0f;
            dirY = 0f;
            tarIndex = 0;
            tarId = 0;
            curHp = 0f;
            curAtkBuff = 0f;
            curAckFrame = 0;
            curAckCd = 0f;
            isHeavyAtk = false;
            isNotConn = false;
            isBoom = false;
            _animType = (SheepRoleAnimType)0;
            animFrame = 0;
            tarPosX = 0f;
            tarPosY = 0f;
            impulseX = 0f;
            impulseY = 0f;
            readySkillId = 0;
            energy = 0f;
        }
    }
}
