namespace rvb.scripts {
    public class PetLogicIn : PetLogic {
        public static readonly PetLogicIn Instance = new();

        public void tick(PetView pet, SheepMgr sheepMgr) {
            if (pet.conf.skillIn != 0) {
                var t = SheepSkill.getById(pet.conf.skillIn);
                if (t.skillType == SheepSkillType.Boom) {
                    var i = SheepSkillSubBoom.getById(t.id);
                    if (1 == pet.animFrame) {
                        var t1 = pet.camp == SheepCamp.Red ? -1200 : 1200;
                        var xnyn = sheepMgr.getXnYn(t1, 0);
                        var o = xnyn.xn;
                        var l = xnyn.yn;
                        PetView n = null;
                        sheepMgr.findNearBlocksByAckView(pet, o, l, 100, e => {
                            n = e;
                            return true;
                        });
                        if (n != null) {
                            pet.posBefX = pet.posX;
                            pet.posBefY = pet.posY;
                            pet.posX = n.posX;
                            pet.posY = n.posY;
                            pet.animX = pet.posX;
                            pet.animY = pet.posY;
                        }
                        else {
                            pet.posBefX = t1;
                            pet.posBefY = 0;
                            pet.posX = t1;
                            pet.posY = 0;
                            pet.animX = t1;
                            pet.animY = 0;
                        }

                        pet.readySkillId = i.id;
                        pet.isLock = true;
                    }
                }
            }
        }
    }
}