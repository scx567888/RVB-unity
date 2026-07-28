using UnityEngine;

namespace rvb.scripts {
    public class PetLogicSpinSpurt : PetLogic{
        public static readonly PetLogicSpinSpurt  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr, bool t) {
            if (!t) {
                return;
            }

            var o = pet.posX;
            var l = pet.posY;
            (int n, int r) = sheepMgr.getXnYn(o, l);
            if (pet.camp == SheepCamp.Red && pet.posX > pet.conf.runEndX ||
                pet.camp == SheepCamp.Blue && pet.posX < -pet.conf.runEndX) {
                pet.state = SheepRoleState.Boom;
                pet.subState = SheepRoleSubState.Boom;
                var t1 = SheepSkillSubSpinSpurt.getById(pet.conf.skillSpurt);
                var i1 = SheepSkillSubBoom.getById(t1.endSkill);
                if (i1.isAnim != 0) {
                    pet.animType = SheepRoleAnimType.Boom;
                }
                else {
                    pet.animType = SheepRoleAnimType.Idle;
                }

                pet.readySkillId = i1.id;
            }
            else {
                sheepMgr.moveTar(pet, null, t);
                sheepMgr.forNearBlocksByAckView(pet, n, r, pet.conf.findR,
                    t2 => {
                        if (t2.isDie || t2.camp == pet.camp || !sheepMgr.isCanAckByRole(pet, t2)) {
                            return false;
                        }

                        sheepMgr.ackTar(pet, t2);
                        return false;
                    });
            }
        }
    }
}