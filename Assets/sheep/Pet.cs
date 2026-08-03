namespace sheep {
    public class Pet {
        // 唯一 ID
        public int id;

        // 唯一真实位置 X
        public float x;

        // 唯一真实位置 Y
        public float y;

        // 核心逻辑帧
        public int frame;

        // 移动意图
        public PetMoveIntent moveIntent;

        // 碰撞意图
        public PetCollideIntent collideIntent;

        // ********************* 渲染器挂载相关 **********************

        // 渲染器句柄 (逻辑层不应使用此字段)
        public PetRenderHandle renderHandle;

        public void action(SheepWorld sheepWorld) {
            // 1. 执行逻辑, 更新自主移动意图
            PetLogic.INSTANCE.tick(this, sheepWorld);

            // 2, 更新逻辑帧
            frame++;
        }
    }
}