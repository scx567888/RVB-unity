using UnityEngine;

namespace rvb.scripts {
    public class PetLogicKiller : PetLogic{
        public static readonly PetLogicKiller  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            var t = SheepSkillSubKiller.getById(pet.readySkillId);
            var i = pet.animFrame;
            if (i == t.findMoveFrame) {
                var i3 = false;
                var s = pet.conf;
                if (pet.conf.roleType == SheepRoleType.CI_KE) {
                    sheepMgr.foreachFront(pet, (e => {
                        if (e.conf.roleType != SheepRoleType.DUN_BING) {
                        }
                        else {
                            i3 = true;
                        }
                    }), s.findR, 60);
                }

                if (i3) {
                    Debug.LogWarning("刺客被中断，直接回到移动状态");
                    pet.state = SheepRoleState.Move;
                    pet.subState = SheepRoleSubState.MoveBoss;
                    pet.animType = SheepRoleAnimType.Idle;
                    return;
                }

                var o = sheepMgr.findFarAck(pet, t.findR);
                if (o != null) {
                    pet.logicMove(o.posX, o.posY);
                }
                else {
                    pet.state = SheepRoleState.Move;
                    pet.subState = SheepRoleSubState.MoveBoss;
                    pet.animType = SheepRoleAnimType.Idle;
                }
            }

            if (i == t.atkFrame) {
                sheepMgr.ackMe(pet, t.spiltRadiusBet, t.atkBet, t.atkFindR);
            }

            if (i >= t.endFrame) {
                var i1 = (int)pet.subState;
                if (i1 == (int)SheepRoleSubState.KillerEnd || i1 - (int)SheepRoleSubState.KillerStart >= t.cnt) {
                    pet.state = SheepRoleState.Move;
                    pet.subState = SheepRoleSubState.MoveBoss;
                    pet.animType = SheepRoleAnimType.Idle;
                    return;
                }

                pet.subState = (SheepRoleSubState)((int)i1 + 1);
                pet.animType = SheepRoleAnimType.Killer;
            }
        }
    }
}