using UnityEngine;

namespace rvb.scripts {
    public class PetLogicSpurt : PetLogic{
        public static readonly PetLogicSpurt  Instance = new ();
        public void tick(PetView e, SheepMgr sheepMgr, bool t) {
            if (!t) {
                return;
            }

            if (e.conf.skillSpurt != 0) {
                var s = SheepSkill.getById(e.conf.skillSpurt);
                if (s.skillType == SheepSkillType.Boom) {
                    var o = SheepSkillSubBoom.getById(s.id);
                    o.tick(sheepMgr, e, t);
                }
                else if (s.skillType == SheepSkillType.Killer) {
                    var o = SheepSkillSubKiller.getById(s.id);
                    o.tick(sheepMgr, e, t);
                }
                else if (s.skillType == SheepSkillType.Bullet) {
                    var o = SheepSkillSubBullet.getById(s.id);
                    o.tick(sheepMgr, e, t);
                }
                else if (s.skillType == SheepSkillType.CallBullets) {
                    var o = SheepSkillSubCallBullets.getById(s.id);
                    o.tick(sheepMgr, e, t);
                }
            }
            else {
                var fff = sheepMgr.findTar(e);
                var s = fff.atkTar;
                var o = fff.moveTar;
                var l = fff.moveBoss;

                if (s != null) {
                    e.state = SheepRoleState.Attack;
                    e.subState = SheepRoleSubState.AttackAwait;
                    return;
                }

                if (o != null) {
                    e.state = SheepRoleState.Move;
                    e.subState = SheepRoleSubState.MoveTar;
                    sheepMgr.moveTar(e, o, t);
                    return;
                }

                if (l != null) {
                    e.state = SheepRoleState.Move;
                    e.subState = SheepRoleSubState.MoveBoss;
                    sheepMgr.moveTar(e, l, t);
                    return;
                }

                sheepMgr.moveTar(e, null, t);
            }
        }
    }
}