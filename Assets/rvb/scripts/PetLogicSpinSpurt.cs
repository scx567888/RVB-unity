using UnityEngine;

namespace rvb.scripts {
    public class PetLogicSpinSpurt : PetLogic{
        public static readonly PetLogicSpinSpurt  Instance = new ();
        public void tick(PetView petView, SheepMgr sheepMgr, bool isLogicFrame) {
            Debug.Log(petView.state.ToString());
        }
    }
}