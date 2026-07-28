using UnityEngine;

namespace rvb.scripts {
    public class PetLogicBladestorm : PetLogic{
        public static readonly PetLogicBladestorm  Instance = new ();
        public void tick(PetView petSkin, SheepMgr sheepMgr, bool isLogicFrame) {
            var i = SheepMgr.FixedDeltaTime;
            var s = petSkin.animFrame;
            var o = SheepSkill.getById(petSkin.readySkillId);
            var l = SheepSkillSubBladestorm.getById(o.id);
            if (isLogicFrame) {
                var fff = sheepMgr.findTar(petSkin, l.findR);
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

                SheepMgr.dirTar(petSkin, n);
                var r = l.speed;
                var x = petSkin.posX + petSkin.dirX * r * i * 3f;
                var y = petSkin.posY + petSkin.dirY * r * i * 3f;
                petSkin.logicMove(x, y);
            }

            var n1 = l.atkFrames;
            foreach (var t3 in n1) {
                if (s == t3) {
                    sheepMgr.ackMe(petSkin, l.spiltRadiusBet, l.atkBet, l.atkFindR);
                    break;
                }
            }

            if (s >= l.endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
        }
    }
}