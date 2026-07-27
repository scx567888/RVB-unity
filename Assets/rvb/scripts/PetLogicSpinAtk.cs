using UnityEngine;

namespace rvb.scripts {
    public class PetLogicSpinAtk : PetLogic{
        public static readonly PetLogicSpinAtk  Instance = new ();
        public void tick(PetView petSkin, SheepMgr sheepMgr, bool t) {
            var s = petSkin.posX;
            var o = petSkin.posY;
            var xnyn = sheepMgr.getXnYn(s, o);
            var l = xnyn.xn;
            var n = xnyn.yn;
            var r = petSkin.animFrame;
            var a = SheepSkill.getById(petSkin.readySkillId);
            var c = SheepSkillSubSpinAtk.getById(a.id);
            if (1 == r) {
                var t1 = sheepMgr.findSortAck1(petSkin, petSkin.conf.findR);

                if (t1 != null) {
                    SheepMgr.dirTar(petSkin, t1);
                }
            }

            if (t) {
                var s1 = true;
                sheepMgr.forNearBlocksByAckView(petSkin, l, n, petSkin.conf.findR,
                    t1 => {
                        if (t1.isDie || t1.camp == petSkin.camp) {
                            return false;
                        }

                        if (s1 && t1.conf.roleType == SheepRoleType.DUN_BING && sheepMgr.isCanAckByRole(petSkin, t1)) {
                            s1 = false;
                        }

                        if (!sheepMgr.isCanAckByRole(petSkin, t1)) {
                            return false;
                        }

                        sheepMgr.ackTar(petSkin, t1);
                        return false;
                    });
                if (s1) {
                    sheepMgr.moveTar(petSkin, null, t);
                }
            }

            if (r >= c.endFrame) {
                petSkin.state = (SheepRoleState)c.endState;
                petSkin.animType = SheepRoleAnimType.Boom;
                petSkin.readySkillId = c.endSkill;
            }
        }
    }
}