using UnityEngine;

namespace rvb.scripts {
    public class PetLogicRes : PetLogic{
        public static readonly PetLogicRes  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            Debug.Log(pet.state.ToString());
        }
    }
}