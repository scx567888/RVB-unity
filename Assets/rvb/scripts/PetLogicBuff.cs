using UnityEngine;

namespace rvb.scripts {
    public class PetLogicBuff : PetLogic{
        public static readonly PetLogicBuff  Instance = new ();
        public void tick(PetView petSkin, SheepMgr sheepMgr, bool isLogicFrame) {
            var t = petSkin.animFrame;
            var i = SheepSkill.getById(petSkin.readySkillId);
            if (t >= SheepSkillSubBuff.getById(i.id).endFrame) {
                petSkin.state = SheepRoleState.Move;
                petSkin.subState = SheepRoleSubState.MoveBoss;
                petSkin.animType = SheepRoleAnimType.Idle;
            }
        }
    }
}