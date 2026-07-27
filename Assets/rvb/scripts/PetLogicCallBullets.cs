using UnityEngine;

namespace rvb.scripts {
    public class PetLogicCallBullets : PetLogic{
        public static readonly PetLogicCallBullets  Instance = new ();
        public void tick(PetView petView, SheepMgr sheepMgr, bool isLogicFrame) {
            Debug.Log(petView.state.ToString());
        }
    }
}