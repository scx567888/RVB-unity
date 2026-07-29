using UnityEngine;

namespace rvb.scripts {
    public class PetLogicBuff : PetLogic{
        public static readonly PetLogicBuff  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            var t = pet.animFrame;
            var i = SheepSkill.getById(pet.readySkillId);
            if (t >= SheepSkillSubBuff.getById(i.id).endFrame) {
                pet.state = SheepRoleState.Move;
                pet.subState = SheepRoleSubState.MoveBoss;
                pet.animType = SheepRoleAnimType.Idle;
            }
        }
    }
}