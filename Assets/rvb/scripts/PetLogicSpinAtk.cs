using UnityEngine;

namespace rvb.scripts {
    public class PetLogicSpinAtk : PetLogic{
        public static readonly PetLogicSpinAtk  Instance = new ();
        public void tick(PetView petView, SheepMgr sheepMgr, bool isLogicFrame) {
            Debug.Log(petView.state.ToString());
        }
    }
}