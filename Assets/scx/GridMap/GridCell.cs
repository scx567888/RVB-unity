namespace scx.GridMap {
    /// 表示 GridMap 中的一个单元, 可以继承以实现更多功能
    public class GridCell {
        ///  自己所在的 列 (Grid 坐标系)
        public readonly int gridX;

        /// 自己所在的 行 (Grid 坐标系)
        public readonly int gridY;

        /// 格子起始 X 坐标 (世界坐标系)
        public readonly float worldStartX;

        /// 格子起始 Y 坐标 (世界坐标系)
        public readonly float worldStartY;

        /// 格子结束 X 坐标 (世界坐标系)
        public readonly float worldEndX;

        /// 格子结束 Y 坐标 (世界坐标系)
        public readonly float worldEndY;

        /// gridX
        /// gridY
        /// worldStartX
        /// worldStartY
        /// worldEndX
        /// worldEndY
        public GridCell(
            int gridX, int gridY,
            float worldStartX, float worldStartY,
            float worldEndX, float worldEndY
        ) {
            this.gridX = gridX;
            this.gridY = gridY;
            this.worldStartX = worldStartX;
            this.worldStartY = worldStartY;
            this.worldEndX = worldEndX;
            this.worldEndY = worldEndY;
        }
    }
}