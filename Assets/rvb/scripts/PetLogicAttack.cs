using static rvb.scripts.SheepModes;

namespace rvb.scripts {
    public class PetLogicAttack : PetLogic{
        public static readonly PetLogicAttack  Instance = new ();
        public void tick(PetView petSkin, SheepMgr sheepMgr, bool t) {
              var o = petSkin.conf.atkMoveType;
            if (petSkin.conf.isLoongStopDistance != 0) {
                var t3 = sheepMode;
                var i1 = petSkin.conf.loongStopDistanceR;
                if (SheepMgr.dis(petSkin.posX, petSkin.posY, petSkin.camp == SheepCamp.Red ? t3.loongX : -t3.loongX, 0) <=
                    i1) {
                    o = (int)SheepRoleAtkMoveType.None;
                }
            }

            if (petSkin.subState == SheepRoleSubState.AttackAwait) {
                if (!petSkin.isAtkCd()) {
                    petSkin.subState = SheepRoleSubState.AttackAnim;
                    petSkin.animType = SheepRoleAnimType.Attack;
                }
            }
            else if (petSkin.subState == SheepRoleSubState.AttackAnim) {
                var t3 = petSkin.conf;
                var i7 = t3.finishAtk;
                var atkCd = t3.atkCd;
                var l = petSkin.animFrame;
                var n = t3.readyAtks;
                foreach (var i9 in n) {
                    if (l == i9) {
                        PetView i5 = null;
                        if (petSkin.conf.atkType == SheepRoleAtkType.Nearest) {
                            i5 = sheepMgr.findNearAck(petSkin);
                        }
                        else if (petSkin.conf.atkType == SheepRoleAtkType.Throw) {
                            i5 = sheepMgr.findSortAck(petSkin, petSkin.conf.findR);
                            if (petSkin.conf.roleType == SheepRoleType.PAO_CHE) {
                                var t6 = sheepMgr.getBackBoss(petSkin.camp);
                                if (sheepMgr.isCanAckByRole(petSkin, t6)) {
                                    i5 = t6;
                                }
                            }
                        }
                        else {
                            i5 = sheepMgr.findNearAck(petSkin);
                        }

                        if (t3.bullet != null && 0 != t3.bullet.Length) {
                            if (i5 != null) {
                                sheepMgr.createBullet(new BullteCreate() {
                                    view_pet = petSkin,
                                    bulletId = t3.bullet[petSkin.camp == SheepCamp.Red ? 0 : 1],
                                    view_tar_pet = i5
                                });
                            }
                            else {
                                sheepMgr.createBullet(new BullteCreate() {
                                    view_pet = petSkin,
                                    bulletId = t3.bullet[petSkin.camp == SheepCamp.Red ? 0 : 1]
                                });
                            }
                        }
                        else {
                            if (i5 != null) {
                                sheepMgr.ackTar(petSkin, i5);
                            }
                        }

                        break;
                    }
                }

                if (l >= i7) {
                    petSkin.resetAtkCd(atkCd);
                    var fff = sheepMgr.findTar(petSkin);
                    var t5 = fff.atkTar;
                    var i5 = fff.moveTar;
                    var s = fff.moveBoss;
                    if (t5 != null) {
                        petSkin.subState = SheepRoleSubState.AttackAwait;
                        petSkin.animType = SheepRoleAnimType.Idle;
                        return;
                    }

                    if (i5 != null) {
                        petSkin.state = SheepRoleState.Move;
                        petSkin.subState = SheepRoleSubState.MoveTar;
                        petSkin.animType = SheepRoleAnimType.Idle;
                        return;
                    }

                    if (s != null) {
                        petSkin.state = SheepRoleState.Move;
                        petSkin.subState = SheepRoleSubState.MoveBoss;
                        petSkin.animType = SheepRoleAnimType.Idle;
                        return;
                    }
                }
            }

            if (t && (o == (int)SheepRoleAtkMoveType.Move || o == (int)SheepRoleAtkMoveType.CdMove &&
                    petSkin.subState == SheepRoleSubState.AttackAwait)) {
                var s = sheepMgr.findNearAck(petSkin);
                if (s != null && SheepMgr.disByRole(petSkin, s) > petSkin.conf.atkMinMoveR + s.conf.collideR) {
                    sheepMgr.moveTar(petSkin, s, t);
                }
            }
        }
    }
}