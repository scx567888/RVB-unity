using UnityEngine;

namespace rvb.scripts {
    public class PetLogicBladestorm : PetLogic{
        public static readonly PetLogicBladestorm  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr, bool isLogicFrame) {
            var i = SheepMgr.FixedDeltaTime;
            var s = pet.animFrame;
            var o = SheepSkill.getById(pet.readySkillId);
            var l = SheepSkillSubBladestorm.getById(o.id);
            if (isLogicFrame) {
                var fff = sheepMgr.findTar(pet, l.findR);
                var t1 = fff.atkTar;
                var s1 = fff.moveTar;
                var o1 = fff.moveBoss;
                PetView n = null;
                if (t1 != null) {
                    n = t1;
                }
                else if (s1 != null) {
                    n = s1;
                }
                else if (o1 != null) {
                    n = o1;
                }

                SheepMgr.dirTar(pet, n);
                var r = l.speed;
                var x = pet.posX + pet.dirX * r * i * 3f;
                var y = pet.posY + pet.dirY * r * i * 3f;
                pet.logicMove(x, y);
            }

            var n1 = l.atkFrames;
            foreach (var t3 in n1) {
                if (s == t3) {
                    sheepMgr.ackMe(pet, l.spiltRadiusBet, l.atkBet, l.atkFindR);
                    break;
                }
            }

            if (s >= l.endFrame) {
                pet.state = SheepRoleState.Move;
                pet.subState = SheepRoleSubState.MoveBoss;
                pet.animType = SheepRoleAnimType.Idle;
            }
        }
    }
}