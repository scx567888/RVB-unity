using UnityEngine;

namespace rvb.scripts {
    public class PetLogicMove : PetLogic {
        private static readonly int LOOP_FRAME = 4;
        public static readonly PetLogicMove Instance = new();

        public void tick(PetView pet, SheepMgr sheepMgr) {
            // 每四个逻辑帧 (action) 执行一次
            var shouldExecute = pet.frame % LOOP_FRAME == LOOP_FRAME - 1;
            if (!shouldExecute) {
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
                sheepMgr.moveTar(pet, o, shouldExecute);
                return;
            }

            if (l != null) {
                pet.subState = SheepRoleSubState.MoveBoss;
                sheepMgr.moveTar(pet, l, shouldExecute);
                return;
            }

            Debug.LogError("移动状态没有目标??");
        }
    }
}