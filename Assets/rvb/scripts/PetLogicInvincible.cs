using System;
using UnityEngine;

namespace rvb.scripts {
    public class PetLogicInvincible : PetLogic{
        public static readonly PetLogicInvincible  Instance = new ();
        public void tick(PetView petSkin, SheepMgr sheepMgr, bool isLogicFrame) {
            
            var t = petSkin.animFrame;
            var i = SheepSkill.getById(petSkin.readySkillId);
            var s = SheepSkillSubInvincible.getById(i.id);
            var o = s.healFrames;
            foreach (var i1 in o) {
                if (t == i1) {
                    var t3 = (float)Math.Floor((petSkin.conf.hp - petSkin.curHp) * (s.healHealthPercent / 100f));
                    SheepMgr.hurtByRole(petSkin, petSkin, -t3);
                    break;
                }
            }

            var l = s.atkFrames;
            foreach (var i2 in l) {
                if (t == i2) {
                    sheepMgr.ackMe(petSkin, s.spiltRadiusBet, s.atkBet, s.atkFindR);
                    break;
                }
            }

            if (t >= s.endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
        }
    }
}