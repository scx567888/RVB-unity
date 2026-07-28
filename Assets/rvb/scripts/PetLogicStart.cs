using UnityEngine;

namespace rvb.scripts {
    public class PetLogicStart : PetLogic {
        public static readonly PetLogicStart Instance = new PetLogicStart();

        public void tick(PetView pet, SheepMgr sheepMgr, bool t) {
            if (!t) {
                return;
            }

            if (sheepMgr.state == SheepRoomState.Start) {
                var t2 = pet.posX;
                var i = pet.posY;
                var o = pet.tarPosX;
                var l = pet.tarPosY;
                var n = SheepMgr.dis(t2, i, o, l);
                var r = 3 * pet.conf.runSpeed;
                if (n > r * SheepMgr.FixedDeltaTime) {
                    var ddd = SheepMgr.dirTarByPos(pet, pet.tarPosX, pet.tarPosY);
                    var t3 = ddd[0];
                    var i3 = ddd[1];
                    var o3 = new Vector3() { x = pet.posX, y = pet.posY };
                    var l3 = new Vector3()
                        { x = t3 * r * SheepMgr.FixedDeltaTime, y = i3 * r * SheepMgr.FixedDeltaTime };
                    var n3 = new Vector3() { x = o3.x + l3.x, y = o3.y + l3.y };
                    pet.logicMove(n3.x, n3.y);
                }
                else {
                    pet.logicMove(o, l);
                }
            }
            else if (pet.conf.skillSpurt != 0) {
                var t1 = SheepSkill.getById(pet.conf.skillSpurt);
                if (t1.skillType == SheepSkillType.Charge) {
                    pet.state = SheepRoleState.Charge;
                    pet.subState = SheepRoleSubState.Spurt;
                    pet.animType = SheepRoleAnimType.Spurt;
                }
                else if (t1.skillType == SheepSkillType.SpinSpurt) {
                    pet.state = SheepRoleState.SpinSpurt;
                    pet.animType = SheepRoleAnimType.Attack;
                }
                else {
                    pet.state = SheepRoleState.Spurt;
                    pet.subState = SheepRoleSubState.Spurt;
                    if (pet.conf.isSpurtAnim) {
                        pet.animType = SheepRoleAnimType.Spurt;
                    }
                    else {
                        pet.animType = SheepRoleAnimType.Idle;
                    }
                }
            }
            else {
                pet.state = SheepRoleState.Spurt;
                pet.subState = SheepRoleSubState.Spurt;
                if (pet.conf.isSpurtAnim) {
                    pet.animType = SheepRoleAnimType.Spurt;
                }
                else {
                    pet.animType = SheepRoleAnimType.Idle;
                }
            }
        }
    }
}