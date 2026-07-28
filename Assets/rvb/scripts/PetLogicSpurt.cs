using UnityEngine;

namespace rvb.scripts {
    public class PetLogicSpurt : PetLogic{
        private static readonly int LOOP_FRAME = 4;
        public static readonly PetLogicSpurt  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            // 每四个逻辑帧 (action) 执行一次
            var shouldExecute = pet.frame % LOOP_FRAME == LOOP_FRAME - 1;
            if (!shouldExecute) {
                return;
            }

            if (pet.conf.skillSpurt != 0) {
                var s = SheepSkill.getById(pet.conf.skillSpurt);
                if (s.skillType == SheepSkillType.Boom) {
                    var o = SheepSkillSubBoom.getById(s.id);
                    o.tick(sheepMgr, pet);
                }
                else if (s.skillType == SheepSkillType.Killer) {
                    var o = SheepSkillSubKiller.getById(s.id);
                    o.tick(sheepMgr, pet);
                }
                else if (s.skillType == SheepSkillType.Bullet) {
                    var o = SheepSkillSubBullet.getById(s.id);
                    o.tick(sheepMgr, pet);
                }
                else if (s.skillType == SheepSkillType.CallBullets) {
                    var o = SheepSkillSubCallBullets.getById(s.id);
                    o.tick(sheepMgr, pet);
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
                    sheepMgr.moveTar(pet, o);
                    return;
                }

                if (l != null) {
                    pet.state = SheepRoleState.Move;
                    pet.subState = SheepRoleSubState.MoveBoss;
                    sheepMgr.moveTar(pet, l);
                    return;
                }

                sheepMgr.moveTar(pet, null);
            }
        }
    }
}