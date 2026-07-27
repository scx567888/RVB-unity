using UnityEngine;

namespace rvb.scripts {
    public class PetLogicChargePlus : PetLogic{
        public static readonly PetLogicChargePlus  Instance = new ();
        public void tick(PetView petView, SheepMgr sheepMgr, bool isLogicFrame) {
            Debug.Log(petView.state.ToString());
        }
    }
}