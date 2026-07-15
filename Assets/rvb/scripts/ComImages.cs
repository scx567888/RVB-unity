using UnityEngine;

namespace rvb.scripts {
    public class ComImages {
        public MeshBlock mesh_block;
        public int[][][][] roles_framess;

        public void addBullet(BulletView bullet) {
            Debug.Log("addBullet");
        }

        public void addRole(PetView viewPet) {
            Debug.Log("addRole");
        }

        public void update_bullet(CurIndexImages currentImages) {
            Debug.Log("update_bullet");
        }

        public CurIndexImages startAdd() {
            return null;
        }

        public void update_role(CurIndexImages currentImages) {
            Debug.Log("update_role");
        }

        public bool isHasFreeImage() {
            return true;
        }

        public void endAdd() {
            
            
        }
    }
    public class MeshBlock {
        public void onFrameUpdateStart() {
            
        }

        public void addFrameBlockCamp(int blockIndex, SheepCamp bulletCamp) {
            
        }

        public void onFrameUpdateEnd(SheepMgr manager) {
            
        }
    }
}