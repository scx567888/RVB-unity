using UnityEngine;

namespace rvb.scripts {
    public class PetLogicKiller : PetLogic{
        public static readonly PetLogicKiller  Instance = new ();
        public void tick(PetView petSkin, SheepMgr sheepMgr, bool isLogicFrame) {
            var t = SheepSkillSubKiller.getById(petSkin.readySkillId);
            var i = petSkin.animFrame;
            if (i == t.findMoveFrame) {
                var i3 = false;
                var s = petSkin.conf;
                if (petSkin.conf.roleType == SheepRoleType.CI_KE) {
                    sheepMgr.foreachFront(petSkin, (e => {
                        if (e.conf.roleType != SheepRoleType.DUN_BING) {
                        }
                        else {
                            i3 = true;
                        }
                    }), s.findR, 60);
                }

                if (i3) {
                    Debug.LogWarning("刺客被中断，直接回到移动状态");
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return;
                }

                var o = sheepMgr.findFarAck(petSkin, t.findR);
                if (o != null) {
                    petSkin.logicMove(o.posX, o.posY);
                }
                else {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                }
            }

            if (i == t.atkFrame) {
                sheepMgr.ackMe(petSkin, t.spiltRadiusBet, t.atkBet, t.atkFindR);
            }

            if (i >= t.endFrame) {
                var i1 = (int)petSkin.subState;
                if (i1 == (int)SheepRoleSubState.KillerEnd || i1 - (int)SheepRoleSubState.KillerStart >= t.cnt) {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    return;
                }

                petSkin.subState = (SheepRoleSubState)((int)i1 + 1);
                petSkin.animType = SheepRoleAnimType.Killer;
            }
        }
    }
}