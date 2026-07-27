using UnityEngine;

namespace rvb.scripts {
    public class PetLogicMerge : PetLogic{
        public static readonly PetLogicMerge  Instance = new ();
        public void tick(PetView petView, SheepMgr sheepMgr, bool isLogicFrame) {
            Debug.Log(petView.state.ToString());
        }
    }
}