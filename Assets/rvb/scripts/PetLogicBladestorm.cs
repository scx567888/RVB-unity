using UnityEngine;

namespace rvb.scripts {
    public class PetLogicBladestorm : PetLogic{
        public static readonly PetLogicBladestorm  Instance = new ();
        public void tick(PetView petView, SheepMgr sheepMgr, bool isLogicFrame) {
            Debug.Log(petView.state.ToString());
        }
    }
}