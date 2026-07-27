using UnityEngine;

namespace rvb.scripts {
    public class PetLogicDead : PetLogic{
        public static readonly PetLogicDead  Instance = new ();
        public void tick(PetView petView, SheepMgr sheepMgr, bool isLogicFrame) {
            Debug.Log(petView.state.ToString());
        }
    }
}