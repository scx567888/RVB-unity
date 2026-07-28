using UnityEngine;

namespace rvb.scripts {
    public class PetLogicSpurt : PetLogic{
        public static readonly PetLogicSpurt  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr, bool t) {
            if (!t) {
                return;
            }

            if (pet.conf.skillSpurt != 0) {
                var s = SheepSkill.getById(pet.conf.skillSpurt);
                if (s.skillType == SheepSkillType.Boom) {
                    var o = SheepSkillSubBoom.getById(s.id);
                    o.tick(sheepMgr, pet, t);
                }
                else if (s.skillType == SheepSkillType.Killer) {
                    var o = SheepSkillSubKiller.getById(s.id);
                    o.tick(sheepMgr, pet, t);
                }
                else if (s.skillType == SheepSkillType.Bullet) {
                    var o = SheepSkillSubBullet.getById(s.id);
                    o.tick(sheepMgr, pet, t);
                }
                else if (s.skillType == SheepSkillType.CallBullets) {
                    var o = SheepSkillSubCallBullets.getById(s.id);
                    o.tick(sheepMgr, pet, t);
                }
            }
            else {
                var fff = sheepMgr.findTar(pet);
                var s = fff.atkTar;
                var o = fff.moveTar;
                var l = fff.moveBoss;

                if (s != null) {
                    pet.state = SheepRoleState.Attack;
                    pet.subState = SheepRoleSubState.AttackAwait;
                    return;
                }

                if (o != null) {
                    pet.state = SheepRoleState.Move;
                    pet.subState = SheepRoleSubState.MoveTar;
                    sheepMgr.moveTar(pet, o, t);
                    return;
                }

                if (l != null) {
                    pet.state = SheepRoleState.Move;
                    pet.subState = SheepRoleSubState.MoveBoss;
                    sheepMgr.moveTar(pet, l, t);
                    return;
                }

                sheepMgr.moveTar(pet, null, t);
            }
        }
    }
}