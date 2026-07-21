using UnityEngine;

namespace sheep {
    public class SheepGame : MonoBehaviour {
        // 逻辑帧率
        public int tickRate = 30;

        // 逻辑帧率 耗时
        private double tickInterval => 1.0 / tickRate;

        // 逻辑帧率累加器
        private double tickAccumulator = 0;

        private SheepWorld sheepWorld;

        void Start() {
            sheepWorld = new SheepWorld();
        }

        void Update() {
            tickAccumulator += Time.deltaTime;

            while (tickAccumulator >= tickInterval) {
                sheepWorld.tick();
                tickAccumulator -= tickInterval;
            }
        }
    }
}