namespace sheep {
    // todo
    public enum PetCollideMoveMode {
        // 普通单位
        NORMAL = 0,

        // 冲刺单位
        SPURT = 1
    }

//todo
    public class PetCollideIntent {
        // 是否参与碰撞
        public bool enabled = true;

        // 碰撞组
        public int group;

        // 查询周围多少格
        // 注意：这是格子数量，不是世界距离
        public int detectCellRadius = 1;

        // 圆形碰撞半径
        public float radius = 0.5f;

        // 碰撞数量达到该值后停止自主移动
        public int notMoveNum = 3;

        // 发生碰撞时，自主移动保留比例
        public float moveScale = 0.5f;

        // 排斥位移倍率
        public float elasticityScale = 0.325f;

        // 最多处理多少个碰撞单位
        public int maxCollideCount = 20;

        // 普通或冲刺模式
        public PetCollideMoveMode moveMode =
            PetCollideMoveMode.NORMAL;
    }
}