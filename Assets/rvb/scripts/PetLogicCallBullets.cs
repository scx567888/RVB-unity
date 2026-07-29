using System;
using UnityEngine;

namespace rvb.scripts {
    public class PetLogicCallBullets : PetLogic{
        public static readonly PetLogicCallBullets  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
              var t = pet.animFrame;
            var i = SheepSkill.getById(pet.readySkillId);
            var s = SheepSkillSubCallBullets.getById(i.id);
            var o = 0;
            if (s.frameStep != 0) {
                if (t % s.frameStep == 0) {
                    o = s.frameCnt;
                }
            }
            else {
                var e = s.callFrames;
                for (var i1 = 0; i1 < e.Length; i1++) {
                    if (t == e[i1]) {
                        o = s.callCnts[i1];
                        break;
                    }
                }
            }

            if (o != 0) {
                for (var t1 = 0; t1 < o; t1++) {
                    if (1 == s.type) {
                        var t3 = pet.posX + s.startOffsetPos[0];
                        var i3 = pet.posY + s.startOffsetPos[1];
                        var o3 = s.startOffsetPos[2];
                        var l = 360 * sheepMgr.RandomFloat(0f, 1f);
                        var n = pet.posX + pet.dirX * s.len + s.endRadius * Math.Cos(l);
                        var r = pet.posY + pet.dirY * s.len + s.endRadius * Math.Sin(l);
                        var a = 0;
                        sheepMgr.createBullet(new BullteCreate() {
                            view_pet = pet,
                            bulletId = s.bullet,
                            info = new BullteCreate.Info()
                                { startX = t3, startY = i3, startZ = o3, endX = (float)n, endY = (float)r, endZ = a }
                        });
                    }
                    else if (2 == s.type) {
                        var t4 = s.startOffsetPos[2];
                        var i5 = 360 * sheepMgr.RandomFloat(0f, 1f);
                        var o5 = pet.posX + pet.dirX * s.len + s.endRadius * Math.Cos(i5);
                        var l = pet.posY + pet.dirY * s.len + s.endRadius * Math.Sin(i5);
                        var n = 0;
                        sheepMgr.createBullet(new BullteCreate() {
                            view_pet = pet,
                            bulletId = s.bullet,
                            info = new BullteCreate.Info() {
                                startX = (float)o5,
                                startY = (float)l,
                                startZ = t4,
                                endX = (float)o5,
                                endY = (float)l,
                                endZ = n,
                                dirX = 0,
                                dirY = 0,
                                dirZ = -1
                            }
                        });
                    }
                    else if (3 == s.type) {
                        sheepMgr.createBullet(new BullteCreate() {
                            view_pet = pet,
                            bulletId = s.bullet,
                            info = new BullteCreate.Info() { dirX = 0, dirY = 0, dirZ = -1, angle = 360f / o * t1 }
                        });
                    }
                    else if (4 == s.type) {
                        sheepMgr.createBullet(new BullteCreate() {
                            view_pet = pet,
                            bulletId = s.bullet,
                            info = new BullteCreate.Info() { dirX = 1, dirY = 0, dirZ = 0 }
                        });
                    }
                    else {
                        sheepMgr.createBullet(new BullteCreate() { view_pet = pet, bulletId = s.bullet });
                    }
                }
            }

            if (t >= s.endFrame) {
                pet.state = SheepRoleState.Move;
                pet.subState = SheepRoleSubState.MoveBoss;
                pet.animType = SheepRoleAnimType.Idle;
            }
        }
    }
}