using UnityEngine;

namespace sheep {
    public class SheepWorld {
        public Vector3 position = new Vector3();
        public int a = 1;

        public void tick() {
            if (position.x > 10) {
                a = -1;
            }
            else if (position.x < -10) {
                a = 1;
            }

            position += new Vector3(0.2f * a, 0, 0);
        }
    }
}