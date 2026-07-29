using System;
using UnityEngine;

namespace rvb.scripts {
    public class PetLogicPalm : PetLogic{
        public static readonly PetLogicPalm  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            var t1 = pet.animFrame;
            var i1 = SheepSkill.getById(pet.readySkillId);
            var s = SheepSkillSubPalm.getById(i1.id);
            var o1 = s.healFrames;
            foreach (var i in o1) {
                if (t1 == i) {
                    var t = (float)Math.Floor((pet.conf.hp - pet.curHp) * (s.healHealthPercent / 100f));
                    SheepMgr.hurtByRole(pet, pet, -t);
                    break;
                }
            }

            var l1 = s.atkFrames;
            foreach (var i in l1) {
                if (t1 == i) {
                    sheepMgr.ackMe(pet, s.spiltRadiusBet, s.atkBet, s.atkFindR);
                    break;
                }
            }

            var n = s.hitBackFrames;
            for (var i = 0; i < n.Length; i++) {
                var o = n[i];
                var l = s.hitBackDistances[i];
                if (t1 == o) {
                    sheepMgr.hitBackMe(pet, s.spiltRadiusBet, s.atkFindR, l);
                    break;
                }
            }

            if (t1 >= s.endFrame) {
                pet.state = SheepRoleState.Move;
                pet.subState = SheepRoleSubState.MoveBoss;
                pet.animType = SheepRoleAnimType.Idle;
            }
        }
    }
}