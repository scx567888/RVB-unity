using UnityEngine;

namespace rvb.scripts {
    public class PetLogicUp : PetLogic{
        public static readonly PetLogicUp  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            Debug.Log(pet.state.ToString());
        }
    }
}