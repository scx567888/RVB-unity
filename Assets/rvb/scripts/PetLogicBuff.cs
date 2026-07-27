using UnityEngine;

namespace rvb.scripts {
    public class PetLogicBuff : PetLogic{
        public static readonly PetLogicBuff  Instance = new ();
        public void tick(PetView petView, SheepMgr sheepMgr, bool isLogicFrame) {
            Debug.Log(petView.state.ToString());
        }
    }
}