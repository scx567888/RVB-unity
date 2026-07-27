namespace rvb.scripts {
    public class PetLogicIn : PetLogic {
        public static readonly PetLogicIn Instance = new();

        public void tick(PetView petSkin, SheepMgr sheepMgr) {
            if (petSkin.conf.skillIn != 0) {
                var t = SheepSkill.getById(petSkin.conf.skillIn);
                if (t.skillType == SheepSkillType.Boom) {
                    var i = SheepSkillSubBoom.getById(t.id);
                    if (1 == petSkin.animFrame) {
                        var t1 = petSkin.camp == SheepCamp.Red ? -1200 : 1200;
                        var xnyn = sheepMgr.getXnYn(t1, 0);
                        var o = xnyn.xn;
                        var l = xnyn.yn;
                        PetView n = null;
                        sheepMgr.findNearBlocksByAckView(petSkin, o, l, 100, e => {
                            n = e;
                            return true;
                        });
                        if (n != null) {
                            petSkin.posBefX = petSkin.posX;
                            petSkin.posBefY = petSkin.posY;
                            petSkin.posX = n.posX;
                            petSkin.posY = n.posY;
                            petSkin.animX = petSkin.posX;
                            petSkin.animY = petSkin.posY;
                        }
                        else {
                            petSkin.posBefX = t1;
                            petSkin.posBefY = 0;
                            petSkin.posX = t1;
                            petSkin.posY = 0;
                            petSkin.animX = t1;
                            petSkin.animY = 0;
                        }

                        petSkin.readySkillId = i.id;
                        petSkin.isLock = true;
                    }
                }
            }
        }
    }
}