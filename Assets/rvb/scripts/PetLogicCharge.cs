using UnityEngine;

namespace rvb.scripts {
    public class PetLogicCharge : PetLogic{
        private static readonly int LOOP_FRAME = 4;
        public static readonly PetLogicCharge  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            // 每四个逻辑帧 (action) 执行一次
            var shouldExecute = pet.frame % LOOP_FRAME == LOOP_FRAME - 1;
            if (!shouldExecute) {
                return;
            }

            var o = pet.posX;
            var l = pet.posY;
            var (n, r) = sheepMgr.getXnYn(o, l);
            if (pet.camp == SheepCamp.Red && pet.posX > pet.conf.runEndX ||
                pet.camp == SheepCamp.Blue && pet.posX < -pet.conf.runEndX) {
                var t6 = false;
                sheepMgr.findNearBlocksByAckView(pet, n, r, 5, i8 => {
                    if (i8.isDie || i8.camp == pet.camp) {
                    }
                    else {
                        t6 = true;
                    }

                    return t6;
                });
                if (t6) {
                    pet.state = SheepRoleState.Boom;
                    pet.subState = SheepRoleSubState.Boom;
                    var t3 = SheepSkillSubCharge.getById(pet.conf.skillSpurt);
                    var i3 = SheepSkillSubBoom.getById(t3.endSkill);
                    if (i3.isAnim != 0) {
                        pet.animType = SheepRoleAnimType.Boom;
                    }
                    else {
                        pet.animType = SheepRoleAnimType.Idle;
                    }

                    pet.readySkillId = i3.id;
                }
                else {
                    pet.state = SheepRoleState.Move;
                    pet.subState = SheepRoleSubState.MoveBoss;
                    pet.animType = SheepRoleAnimType.Idle;
                }
            }
            else {
                var s = false;
                sheepMgr.findNearBlocksByAckView(pet, n, r, 5, t8 => {
                    if (!t8.isDie && t8.camp != pet.camp && sheepMgr.isCanAckByRole(pet, t8)) {
                        if (t8.conf.roleType == SheepRoleType.XIAO_BING) {
                            var i = t8;
                            sheepMgr.ackTar(pet, i);
                        }
                        else {
                            s = true;
                        }
                    }

                    return false;
                });
                if (s) {
                    pet.state = SheepRoleState.Boom;
                    pet.subState = SheepRoleSubState.Boom;
                    var t8 = SheepSkillSubCharge.getById(pet.conf.skillSpurt);
                    var i8 = SheepSkillSubBoom.getById(t8.endSkill);
                    if (i8.isAnim != 0) {
                        pet.animType = SheepRoleAnimType.Boom;
                    }
                    else {
                        pet.animType = SheepRoleAnimType.Idle;
                    }

                    pet.readySkillId = i8.id;
                    return;
                }

                PetView o3 = null;
                sheepMgr.findNearBlocksByAckView(pet, n, r, pet.conf.findR, t4 => {
                    // 跳过：死亡的、同阵营的、没有 roleId 的
                    if (t4.isDie || t4.camp == pet.camp) {
                        return false;
                    }

                    // 只允许 roleType = role3
                    if (t4.conf.roleType != SheepRoleType.GONG_JIAN_SHOU) {
                        return false;
                    }

                    // 必须可攻击
                    if (!sheepMgr.isCanAckByRole(pet, t4)) {
                        return false;
                    }

                    // 如果满足条件，克隆并返回 true
                    o3 = t4;
                    return true;
                });
                sheepMgr.moveTar(pet, o3);
            }
        }
    }
}