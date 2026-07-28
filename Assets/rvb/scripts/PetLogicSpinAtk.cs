using UnityEngine;

namespace rvb.scripts {
    public class PetLogicSpinAtk : PetLogic{
        public static readonly PetLogicSpinAtk  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr, bool t) {
            var s = pet.posX;
            var o = pet.posY;
            var xnyn = sheepMgr.getXnYn(s, o);
            var l = xnyn.xn;
            var n = xnyn.yn;
            var r = pet.animFrame;
            var a = SheepSkill.getById(pet.readySkillId);
            var c = SheepSkillSubSpinAtk.getById(a.id);
            if (1 == r) {
                var t1 = sheepMgr.findSortAck1(pet, pet.conf.findR);

                if (t1 != null) {
                    SheepMgr.dirTar(pet, t1);
                }
            }

            if (t) {
                var s1 = true;
                sheepMgr.forNearBlocksByAckView(pet, l, n, pet.conf.findR,
                    t1 => {
                        if (t1.isDie || t1.camp == pet.camp) {
                            return false;
                        }

                        if (s1 && t1.conf.roleType == SheepRoleType.DUN_BING && sheepMgr.isCanAckByRole(pet, t1)) {
                            s1 = false;
                        }

                        if (!sheepMgr.isCanAckByRole(pet, t1)) {
                            return false;
                        }

                        sheepMgr.ackTar(pet, t1);
                        return false;
                    });
                if (s1) {
                    sheepMgr.moveTar(pet, null, t);
                }
            }

            if (r >= c.endFrame) {
                pet.state = (SheepRoleState)c.endState;
                pet.animType = SheepRoleAnimType.Boom;
                pet.readySkillId = c.endSkill;
            }
        }
    }
}