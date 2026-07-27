using UnityEngine;

namespace rvb.scripts {
    public class PetLogicRigidity : PetLogic{
        public static readonly PetLogicRigidity  Instance = new ();
        public void tick(PetView petSkin, SheepMgr sheepMgr, bool isLogicFrame) {
            var t = SheepSkill.getById(petSkin.readySkillId);
            var i = SheepSkillSubRigidity.getById(t.id);
            if (petSkin.animFrame >= i.endFrame) {
                petSkin.state = SheepRoleState.SpinAtk;
                petSkin.animType = SheepRoleAnimType.Attack;
                petSkin.readySkillId = i.endSkill;
            }
        }
    }
}