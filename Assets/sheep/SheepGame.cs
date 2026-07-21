using UnityEngine;

namespace sheep {
    public class SheepGame : MonoBehaviour {
        private int tick;
        private SheepWorld sheepWorld;

        void Start() {
            sheepWorld = new SheepWorld();
        }

        void Update() {
            sheepWorld.tick();
        }
    }
}