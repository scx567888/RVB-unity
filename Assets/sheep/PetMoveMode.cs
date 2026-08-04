namespace sheep {
    public enum PetMoveMode {
        // 不移动
        NONE = 0,

        // 向量移动
        DIRECTION = 1,

        // 向目标移动
        TARGET = 2,

        // 瞬移
        TELEPORT = 3,
    }
}