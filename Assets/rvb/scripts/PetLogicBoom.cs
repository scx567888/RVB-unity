using System.Collections.Generic;
using UnityEngine;

namespace rvb.scripts {
    public class PetLogicBoom : PetLogic{
        public static readonly PetLogicBoom  Instance = new ();
        public void tick(PetView petSkin, SheepMgr sheepMgr, bool isLogicFrame) {
            var t = SheepSkill.getById(petSkin.readySkillId);
            var i = SheepSkillSubBoom.getById(t.id);
            var s = petSkin.animFrame;
            if (s == i.atkFrame) {
                var t1 = new List<SheepRoleType>();
                if (petSkin.conf.roleType != SheepRoleType.CHONG_FENG_BING &&
                    petSkin.conf.roleType != SheepRoleType.QI_LIN) {
                }
                else {
                    t1.Add(SheepRoleType.QI_LIN);
                }

                sheepMgr.ackMe(petSkin, i.spiltRadiusBet, i.atkBet, i.atkFindR, i.hitBackDistance, t1);
            }

            if (s >= i.endFrame) {
                petSkin.isLock = false;
                if (i.endState == (int)SheepRoleState.Move) {
                    petSkin.state = SheepRoleState.Move;
                    petSkin.subState = SheepRoleSubState.MoveBoss;
                    petSkin.animType = SheepRoleAnimType.Idle;
                }
                else if (i.endState == (int)SheepRoleState.Rigidity) {
                    petSkin.state = SheepRoleState.Rigidity;
                    petSkin.animType = SheepRoleAnimType.Idle;
                    petSkin.readySkillId = i.endSkill;
                }
                else {
                    if (i.endState == (int)SheepRoleState.Dead) {
                        petSkin.isDie = true;
                        petSkin.state = SheepRoleState.Dead;
                    }
                    else if (i.endState == (int)SheepRoleState.Palm) {
                        petSkin.state = SheepRoleState.Palm;
                        petSkin.subState = SheepRoleSubState.Palm;
                        petSkin.animType = SheepRoleAnimType.Palm;
                        petSkin.readySkillId = i.endSkill;
                    }
                    else {
                        Debug.LogError("endState错误");
                    }
                }
            }
        }
    }
}