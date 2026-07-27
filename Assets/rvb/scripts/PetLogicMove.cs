using UnityEngine;

namespace rvb.scripts {
    public class PetLogicMove : PetLogic{
        public static readonly PetLogicMove  Instance = new ();
        public void tick(PetView petSkin, SheepMgr sheepMgr, bool t) {
            if (!t) {
                return;
            }

            if (petSkin.isLock) {
                return;
            }

            var fff = sheepMgr.findTar(petSkin);
            var s = fff.atkTar;
            var o = fff.moveTar;
            var l = fff.moveBoss;

            if (s != null) {
                petSkin.state = SheepRoleState.Attack;
                petSkin.subState = SheepRoleSubState.AttackAwait;
                return;
            }

            if (o != null) {
                petSkin.subState = SheepRoleSubState.MoveTar;
                sheepMgr.moveTar(petSkin, o, t);
                return;
            }

            if (l != null) {
                petSkin.subState = SheepRoleSubState.MoveBoss;
                sheepMgr.moveTar(petSkin, l, t);
                return;
            }

            Debug.LogError("移动状态没有目标??");
        }
    }
}