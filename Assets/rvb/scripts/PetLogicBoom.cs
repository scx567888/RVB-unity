using System.Collections.Generic;
using UnityEngine;

namespace rvb.scripts {
    public class PetLogicBoom : PetLogic{
        public static readonly PetLogicBoom  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr, bool isLogicFrame) {
            var t = SheepSkill.getById(pet.readySkillId);
            var i = SheepSkillSubBoom.getById(t.id);
            var s = pet.animFrame;
            if (s == i.atkFrame) {
                var t1 = new List<SheepRoleType>();
                if (pet.conf.roleType != SheepRoleType.CHONG_FENG_BING &&
                    pet.conf.roleType != SheepRoleType.QI_LIN) {
                }
                else {
                    t1.Add(SheepRoleType.QI_LIN);
                }

                sheepMgr.ackMe(pet, i.spiltRadiusBet, i.atkBet, i.atkFindR, i.hitBackDistance, t1);
            }

            if (s >= i.endFrame) {
                pet.isLock = false;
                if (i.endState == (int)SheepRoleState.Move) {
                    pet.state = SheepRoleState.Move;
                    pet.subState = SheepRoleSubState.MoveBoss;
                    pet.animType = SheepRoleAnimType.Idle;
                }
                else if (i.endState == (int)SheepRoleState.Rigidity) {
                    pet.state = SheepRoleState.Rigidity;
                    pet.animType = SheepRoleAnimType.Idle;
                    pet.readySkillId = i.endSkill;
                }
                else {
                    if (i.endState == (int)SheepRoleState.Dead) {
                        pet.isDie = true;
                        pet.state = SheepRoleState.Dead;
                    }
                    else if (i.endState == (int)SheepRoleState.Palm) {
                        pet.state = SheepRoleState.Palm;
                        pet.subState = SheepRoleSubState.Palm;
                        pet.animType = SheepRoleAnimType.Palm;
                        pet.readySkillId = i.endSkill;
                    }
                    else {
                        Debug.LogError("endState错误");
                    }
                }
            }
        }
    }
}