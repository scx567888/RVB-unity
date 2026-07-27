using UnityEngine;

namespace rvb.scripts {
    public class PetLogicSpinSpurt : PetLogic{
        public static readonly PetLogicSpinSpurt  Instance = new ();
        public void tick(PetView e, SheepMgr sheepMgr, bool t) {
            if (!t) {
                return;
            }

            var o = e.posX;
            var l = e.posY;
            (int n, int r) = sheepMgr.getXnYn(o, l);
            if (e.camp == SheepCamp.Red && e.posX > e.conf.runEndX ||
                e.camp == SheepCamp.Blue && e.posX < -e.conf.runEndX) {
                e.state = SheepRoleState.Boom;
                e.subState = SheepRoleSubState.Boom;
                var t1 = SheepSkillSubSpinSpurt.getById(e.conf.skillSpurt);
                var i1 = SheepSkillSubBoom.getById(t1.endSkill);
                if (i1.isAnim != 0) {
                    e.animType = SheepRoleAnimType.Boom;
                }
                else {
                    e.animType = SheepRoleAnimType.Idle;
                }

                e.readySkillId = i1.id;
            }
            else {
                sheepMgr.moveTar(e, null, t);
                sheepMgr.forNearBlocksByAckView(e, n, r, e.conf.findR,
                    t2 => {
                        if (t2.isDie || t2.camp == e.camp || !sheepMgr.isCanAckByRole(e, t2)) {
                            return false;
                        }

                        sheepMgr.ackTar(e, t2);
                        return false;
                    });
            }
        }
    }
}