using UnityEngine;

namespace rvb.scripts {
    public class PetLogicMove : PetLogic {
        public static readonly PetLogicMove Instance = new();

        public void tick(PetView pet, SheepMgr sheepMgr, bool t) {
            if (!t) {
                return;
            }

            if (pet.isLock) {
                return;
            }

            var fff = sheepMgr.findTar(pet);
            var s = fff.atkTar;
            var o = fff.moveTar;
            var l = fff.moveBoss;

            if (s != null) {
                pet.state = SheepRoleState.Attack;
                pet.subState = SheepRoleSubState.AttackAwait;
                return;
            }

            if (o != null) {
                pet.subState = SheepRoleSubState.MoveTar;
                sheepMgr.moveTar(pet, o, t);
                return;
            }

            if (l != null) {
                pet.subState = SheepRoleSubState.MoveBoss;
                sheepMgr.moveTar(pet, l, t);
                return;
            }

            Debug.LogError("移动状态没有目标??");
        }
    }
}