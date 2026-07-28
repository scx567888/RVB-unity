using UnityEngine;

namespace rvb.scripts {
    public class PetLogicChargePlus : PetLogic{
        private static readonly int LOOP_FRAME = 4;
        public static readonly PetLogicChargePlus  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            // 每四个逻辑帧 (action) 执行一次
            var shouldExecute = pet.frame % LOOP_FRAME == LOOP_FRAME - 1;
            if (!shouldExecute) {
                return;
            }

            var o = pet.posX;
            var l = pet.posY;
            var (n, r) = sheepMgr.getXnYn(o, l);
            if (pet.camp == SheepCamp.Red && pet.posX > pet.conf.runEndX ||
                pet.camp == SheepCamp.Blue && pet.posX < -pet.conf.runEndX) {
                pet.state = SheepRoleState.Boom;
                pet.subState = SheepRoleSubState.Boom;
                var t3 = SheepSkillSubChargePlus.getById(pet.conf.skillSpurt);
                var i3 = SheepSkillSubBoom.getById(t3.endSkill);
                pet.animType = SheepRoleAnimType.Boom;
                pet.readySkillId = i3.id;
            }
            else {
                sheepMgr.findNearBlocksByAckView(pet, n, r, 5, tt2 => {
                    if (!tt2.isDie && tt2.camp != pet.camp && sheepMgr.isCanAckByRole(pet, tt2)) {
                        var i7 = sheepMgr.sheepConfig.beheadLine;
                        if (tt2.curHp < i7) {
                            tt2.isDie = true;
                            tt2.state = SheepRoleState.Dead;
                        }
                        else {
                            var t1 = pet.conf;
                            sheepMgr.ackMe(pet, t1.collideR, 0, t1.findR, t1.hitBackDistance);
                        }
                    }

                    return false;
                });

                sheepMgr.moveTar(pet, null, shouldExecute);
            }
        }
    }
}