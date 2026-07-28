using static rvb.scripts.SheepModes;

namespace rvb.scripts {
    public class PetLogicAttack : PetLogic {
        private static readonly int LOOP_FRAME = 4;

        public static readonly PetLogicAttack Instance = new();

        public void tick(PetView pet, SheepMgr sheepMgr) {
            var atkMoveType = pet.conf.atkMoveType;
            if (pet.conf.isLoongStopDistance != 0) {
                var t3 = sheepMode;
                var i1 = pet.conf.loongStopDistanceR;
                if (SheepMgr.dis(pet.posX, pet.posY, pet.camp == SheepCamp.Red ? t3.loongX : -t3.loongX, 0) <= i1) {
                    atkMoveType = (int)SheepRoleAtkMoveType.None;
                }
            }

            if (pet.subState == SheepRoleSubState.AttackAwait) {
                if (!pet.isAtkCd()) {
                    pet.subState = SheepRoleSubState.AttackAnim;
                    pet.animType = SheepRoleAnimType.Attack;
                }
            }
            else if (pet.subState == SheepRoleSubState.AttackAnim) {
                var t3 = pet.conf;
                var i7 = t3.finishAtk;
                var atkCd = t3.atkCd;
                var l = pet.animFrame;
                var n = t3.readyAtks;
                foreach (var i9 in n) {
                    if (l == i9) {
                        PetView i5 = null;
                        if (pet.conf.atkType == SheepRoleAtkType.Nearest) {
                            i5 = sheepMgr.findNearAck(pet);
                        }
                        else if (pet.conf.atkType == SheepRoleAtkType.Throw) {
                            i5 = sheepMgr.findSortAck(pet, pet.conf.findR);
                            if (pet.conf.roleType == SheepRoleType.PAO_CHE) {
                                var t6 = sheepMgr.getBackBoss(pet.camp);
                                if (sheepMgr.isCanAckByRole(pet, t6)) {
                                    i5 = t6;
                                }
                            }
                        }
                        else {
                            i5 = sheepMgr.findNearAck(pet);
                        }

                        if (t3.bullet != null && 0 != t3.bullet.Length) {
                            if (i5 != null) {
                                sheepMgr.createBullet(new BullteCreate() {
                                    view_pet = pet,
                                    bulletId = t3.bullet[pet.camp == SheepCamp.Red ? 0 : 1],
                                    view_tar_pet = i5
                                });
                            }
                            else {
                                sheepMgr.createBullet(new BullteCreate() {
                                    view_pet = pet,
                                    bulletId = t3.bullet[pet.camp == SheepCamp.Red ? 0 : 1]
                                });
                            }
                        }
                        else {
                            if (i5 != null) {
                                sheepMgr.ackTar(pet, i5);
                            }
                        }

                        break;
                    }
                }

                if (l >= i7) {
                    pet.resetAtkCd(atkCd);
                    var fff = sheepMgr.findTar(pet);
                    var t5 = fff.atkTar;
                    var i5 = fff.moveTar;
                    var s = fff.moveBoss;
                    if (t5 != null) {
                        pet.subState = SheepRoleSubState.AttackAwait;
                        pet.animType = SheepRoleAnimType.Idle;
                        return;
                    }

                    if (i5 != null) {
                        pet.state = SheepRoleState.Move;
                        pet.subState = SheepRoleSubState.MoveTar;
                        pet.animType = SheepRoleAnimType.Idle;
                        return;
                    }

                    if (s != null) {
                        pet.state = SheepRoleState.Move;
                        pet.subState = SheepRoleSubState.MoveBoss;
                        pet.animType = SheepRoleAnimType.Idle;
                        return;
                    }
                }
            }
            
            var isMoveFrame = pet.frame % LOOP_FRAME == LOOP_FRAME - 1;

            if (!isMoveFrame) {
                return;
            }

            if (atkMoveType == (int)SheepRoleAtkMoveType.Move || atkMoveType == (int)SheepRoleAtkMoveType.CdMove &&
                pet.subState == SheepRoleSubState.AttackAwait) {
                var s = sheepMgr.findNearAck(pet);
                if (s != null && SheepMgr.disByRole(pet, s) > pet.conf.atkMinMoveR + s.conf.collideR) {
                    sheepMgr.moveTar(pet, s, isMoveFrame);
                }
            }
        }
    }
}