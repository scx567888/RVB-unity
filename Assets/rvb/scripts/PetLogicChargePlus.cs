using UnityEngine;

namespace rvb.scripts {
    public class PetLogicChargePlus : PetLogic{
        public static readonly PetLogicChargePlus  Instance = new ();
        public void tick(PetView e, SheepMgr sheepMgr, bool t) {
            if (!t) {
                return;
            }

            var o = e.posX;
            var l = e.posY;
            var (n, r) = sheepMgr.getXnYn(o, l);
            if (e.camp == SheepCamp.Red && e.posX > e.conf.runEndX ||
                e.camp == SheepCamp.Blue && e.posX < -e.conf.runEndX) {
                e.state = SheepRoleState.Boom;
                e.subState = SheepRoleSubState.Boom;
                var t3 = SheepSkillSubChargePlus.getById(e.conf.skillSpurt);
                var i3 = SheepSkillSubBoom.getById(t3.endSkill);
                e.animType = SheepRoleAnimType.Boom;
                e.readySkillId = i3.id;
            }
            else {
                sheepMgr.findNearBlocksByAckView(e, n, r, 5, tt2 => {
                    if (!tt2.isDie && tt2.camp != e.camp && sheepMgr.isCanAckByRole(e, tt2)) {
                        var i7 = sheepMgr.sheepConfig.beheadLine;
                        if (tt2.curHp < i7) {
                            tt2.isDie = true;
                            tt2.state = SheepRoleState.Dead;
                        }
                        else {
                            var t1 = e.conf;
                            sheepMgr.ackMe(e, t1.collideR, 0, t1.findR, t1.hitBackDistance);
                        }
                    }

                    return false;
                });

                sheepMgr.moveTar(e, null, t);
            }
        }
    }
}