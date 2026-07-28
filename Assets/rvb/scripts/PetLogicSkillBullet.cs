using UnityEngine;

namespace rvb.scripts {
    public class PetLogicSkillBullet : PetLogic{
        public static readonly PetLogicSkillBullet  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            Debug.Log(pet.state.ToString());
        }
    }
}