using UnityEngine;

namespace rvb.scripts {
    public class PetLogicStart : PetLogic{
        
        public static readonly PetLogicStart  Instance = new PetLogicStart();
        
        public void tick(PetView petSkin, SheepMgr sheepMgr, bool t) {
            if (!t) {
                return;
            }

            if (sheepMgr.state == SheepRoomState.Start) {
                if (t) {
                    var t2 = petSkin.posX;
                    var i = petSkin.posY;
                    var o = petSkin.tarPosX;
                    var l = petSkin.tarPosY;
                    var n = SheepMgr.dis(t2, i, o, l);
                    var r = 3 * petSkin.conf.runSpeed;
                    if (n > r * SheepMgr.FixedDeltaTime) {
                        var ddd = SheepMgr.dirTarByPos(petSkin, petSkin.tarPosX, petSkin.tarPosY);
                        var t3 = ddd[0];
                        var i3 = ddd[1];
                        var o3 = new Vector3() { x = petSkin.posX, y = petSkin.posY };
                        var l3 = new Vector3() { x = t3 * r * SheepMgr.FixedDeltaTime, y = i3 * r * SheepMgr.FixedDeltaTime };
                        var n3 = new Vector3() { x = o3.x + l3.x, y = o3.y + l3.y };
                        petSkin.logicMove(n3.x, n3.y);
                    }
                    else {
                        petSkin.logicMove(o, l);
                    }
                }
            }
            else if (petSkin.conf.skillSpurt != 0) {
                var t1 = SheepSkill.getById(petSkin.conf.skillSpurt);
                if (t1.skillType == SheepSkillType.Charge) {
                    petSkin.state = SheepRoleState.Charge;
                    petSkin.subState = SheepRoleSubState.Spurt;
                    petSkin.animType = SheepRoleAnimType.Spurt;
                }
                else if (t1.skillType == SheepSkillType.SpinSpurt) {
                    petSkin.state = SheepRoleState.SpinSpurt;
                    petSkin.animType = SheepRoleAnimType.Attack;
                }
                else {
                    petSkin.state = SheepRoleState.Spurt;
                    petSkin.subState = SheepRoleSubState.Spurt;
                    if (petSkin.conf.isSpurtAnim) {
                        petSkin.animType = SheepRoleAnimType.Spurt;
                    }
                    else {
                        petSkin.animType = SheepRoleAnimType.Idle;
                    }
                }
            }
            else {
                petSkin.state = SheepRoleState.Spurt;
                petSkin.subState = SheepRoleSubState.Spurt;
                if (petSkin.conf.isSpurtAnim) {
                    petSkin.animType = SheepRoleAnimType.Spurt;
                }
                else {
                    petSkin.animType = SheepRoleAnimType.Idle;
                }
            }
        }
    }
}