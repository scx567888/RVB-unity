using UnityEngine;

namespace rvb.scripts {
    public class PetLogicRigidity : PetLogic{
        public static readonly PetLogicRigidity  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            var t = SheepSkill.getById(pet.readySkillId);
            var i = SheepSkillSubRigidity.getById(t.id);
            if (pet.animFrame >= i.endFrame) {
                pet.state = SheepRoleState.SpinAtk;
                pet.animType = SheepRoleAnimType.Attack;
                pet.readySkillId = i.endSkill;
            }
        }
    }
}