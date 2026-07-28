using System;
using UnityEngine;

namespace rvb.scripts {
    public class PetLogicInvincible : PetLogic{
        public static readonly PetLogicInvincible  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            
            var t = pet.animFrame;
            var i = SheepSkill.getById(pet.readySkillId);
            var s = SheepSkillSubInvincible.getById(i.id);
            var o = s.healFrames;
            foreach (var i1 in o) {
                if (t == i1) {
                    var t3 = (float)Math.Floor((pet.conf.hp - pet.curHp) * (s.healHealthPercent / 100f));
                    SheepMgr.hurtByRole(pet, pet, -t3);
                    break;
                }
            }

            var l = s.atkFrames;
            foreach (var i2 in l) {
                if (t == i2) {
                    sheepMgr.ackMe(pet, s.spiltRadiusBet, s.atkBet, s.atkFindR);
                    break;
                }
            }

            if (t >= s.endFrame) {
                pet.state = SheepRoleState.Move;
                pet.subState = SheepRoleSubState.MoveBoss;
                pet.animType = SheepRoleAnimType.Idle;
            }
        }
    }
}