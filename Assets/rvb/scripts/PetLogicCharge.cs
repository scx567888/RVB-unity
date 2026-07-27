using UnityEngine;

namespace rvb.scripts {
    public class PetLogicCharge : PetLogic{
        public static readonly PetLogicCharge  Instance = new ();
        public void tick(PetView e, SheepMgr sheepMgr, bool t) {
             if (!t) {
                return;
            }

            var o = e.posX;
            var l = e.posY;
            var (n, r) = sheepMgr.getXnYn(o, l);
            if (e.camp == SheepCamp.Red && e.posX > e.conf.runEndX ||
                e.camp == SheepCamp.Blue && e.posX < -e.conf.runEndX) {
                var t6 = false;
                sheepMgr.findNearBlocksByAckView(e, n, r, 5, i8 => {
                    if (i8.isDie || i8.camp == e.camp) {
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
                sheepMgr.findNearBlocksByAckView(e, n, r, 5, t8 => {
                    if (!t8.isDie && t8.camp != e.camp && sheepMgr.isCanAckByRole(e, t8)) {
                        if (t8.conf.roleType == SheepRoleType.XIAO_BING) {
                            var i = t8;
                            sheepMgr.ackTar(e, i);
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
                sheepMgr.findNearBlocksByAckView(e, n, r, e.conf.findR, t4 => {
                    // 跳过：死亡的、同阵营的、没有 roleId 的
                    if (t4.isDie || t4.camp == e.camp) {
                        return false;
                    }

                    // 只允许 roleType = role3
                    if (t4.conf.roleType != SheepRoleType.GONG_JIAN_SHOU) {
                        return false;
                    }

                    // 必须可攻击
                    if (!sheepMgr.isCanAckByRole(e, t4)) {
                        return false;
                    }

                    // 如果满足条件，克隆并返回 true
                    o3 = t4;
                    return true;
                });
                sheepMgr.moveTar(e, o3, t);
            }
        }
    }
}