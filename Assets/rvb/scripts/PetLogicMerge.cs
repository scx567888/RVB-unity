using UnityEngine;

namespace rvb.scripts {
    public class PetLogicMerge : PetLogic{
        public static readonly PetLogicMerge  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            Debug.Log(pet.state.ToString());
        }
    }
}