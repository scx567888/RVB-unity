using System;

namespace scx.GridMap {
    /// 一个网格容器, 可用于 寻敌, 空间划分 等
    /// 每一个格子都是正方形
    public class GridMap<T> where T : GridCell {
        /// 世界起始 X (世界坐标系)
        public readonly float worldStartX;

        /// 世界起始 Y (世界坐标系)
        public readonly float worldStartY;

        /// 世界宽度 (世界坐标系)
        public readonly float worldWidth;

        /// 世界高度 (世界坐标系)
        public readonly float worldHeight;

        /// 格子大小 (正方形) (世界坐标系)
        public readonly float cellSize;

        /// 横向的格子数量 (Grid 坐标系)
        public readonly int gridWidth;

        /// 纵向的格子数量 (Grid 坐标系)
        public readonly int gridHeight;

        /// 格子 (二维数组)
        public readonly T[][] cells;

        /// 创建一个 GridMap
        /// worldStartX  世界起始 X (世界坐标系)
        /// worldStartY  世界起始 Y (世界坐标系)
        /// worldWidth  世界宽度
        /// worldHeight  世界高度
        /// cellSize  格子大小 (正方形宽高)
        /// cellFactory 格子工厂
        public GridMap(
            float worldStartX, float worldStartY,
            float worldWidth, float worldHeight,
            float cellSize,
            Func<int, int, float, float, float, float, T> cellFactory
        ) {
            if (worldWidth <= 0 || worldHeight <= 0 || cellSize <= 0) {
                throw new ArgumentException("worldWidth, worldHeight, cellSize 必须为正数");
            }

            this.worldStartX = worldStartX;
            this.worldStartY = worldStartY;
            this.worldWidth = worldWidth;
            this.worldHeight = worldHeight;
            this.cellSize = cellSize;

            // 计算有多少个格子
            gridWidth = (int)Math.Ceiling(worldWidth / cellSize);
            gridHeight = (int)Math.Ceiling(worldHeight / cellSize);

            // 创建二维数组
            cells = new T[gridHeight][];
            for (var gridY = 0; gridY < gridHeight; gridY++) {
                var row = cells[gridY] = new T[gridWidth];
                for (var gridX = 0; gridX < gridWidth; gridX++) {
                    var cell = cellFactory(
                        gridX, gridY,
                        gridToWorldStartX(gridX), gridToWorldStartY(gridY),
                        gridToWorldEndX(gridX), gridToWorldEndY(gridY)
                    );
                    row[gridX] = cell;
                }
            }
        }

        /// 世界坐标 X 转 Grid 坐标 X
        /// 坐标正好在格子边界时, 归入索引较大的格子.
        /// x X 坐标 (世界坐标系)
        /// return 格子 X (Grid 坐标系)
        public int worldToGridX(float x) {
            return (int)Math.Floor((x - worldStartX) / cellSize);
        }

        /// 世界坐标 Y 转 Grid 坐标 Y
        /// 坐标正好在格子边界时, 归入索引较大的格子.
        /// y Y 坐标 (世界坐标系)
        /// return 格子 Y (Grid 坐标系)
        public int worldToGridY(float y) {
            return (int)Math.Floor((y - worldStartY) / cellSize);
        }

        /// Grid 坐标 X 转 格子起始 X 坐标 (世界坐标系)
        /// gridX 格子 X (Grid 坐标系)
        /// return 格子起始 X 坐标 (世界坐标系)
        public float gridToWorldStartX(int gridX) {
            return worldStartX + gridX * cellSize;
        }

        /// Grid 坐标 Y 转 格子起始 Y 坐标 (世界坐标系)
        /// gridY 格子 Y (Grid 坐标系)
        /// return 格子起始 Y 坐标 (世界坐标系)
        public float gridToWorldStartY(int gridY) {
            return worldStartY + gridY * cellSize;
        }

        /// Grid 坐标 X 转 格子结束 X 坐标 (世界坐标系)
        /// gridX 格子 X (Grid 坐标系)
        /// return 格子结束 X 坐标 (世界坐标系)
        public float gridToWorldEndX(int gridX) {
            return gridToWorldStartX(gridX) + cellSize;
        }

        /// Grid 坐标 Y 转 格子结束 Y 坐标 (世界坐标系)
        /// gridY 格子 Y (Grid 坐标系)
        /// return 格子结束 Y 坐标 (世界坐标系)
        public float gridToWorldEndY(int gridY) {
            return gridToWorldStartY(gridY) + cellSize;
        }

        /// 获取格子 (越界会返回 null)
        /// gridX (Grid 坐标系)
        /// gridY (Grid 坐标系)
        /// return 格子
        public T getCell(int gridX, int gridY) {
            // 越界判断
            if (gridX < 0 || gridX >= gridWidth || gridY < 0 || gridY >= gridHeight) {
                return null;
            }

            return cells[gridY][gridX];
        }

        /// 获取格子 (越界会返回 边界)
        /// gridX (Grid 坐标系)
        /// gridY (Grid 坐标系)
        /// return 格子
        public T getCellSafe(int gridX, int gridY) {
            if (gridX < 0) {
                gridX = 0;
            }
            else if (gridX >= gridWidth) {
                gridX = gridWidth - 1;
            }

            if (gridY < 0) {
                gridY = 0;
            }
            else if (gridY >= gridHeight) {
                gridY = gridHeight - 1;
            }

            return cells[gridY][gridX];
        }

        /// 根据世界坐标获取格子 (越界返回 null)
        /// x (世界坐标系)
        /// y (世界坐标系)
        /// return 格子
        public T getCellByWorldPosition(float x, float y) {
            var gridX = worldToGridX(x);
            var gridY = worldToGridY(y);
            return getCell(gridX, gridY);
        }

        /// 根据世界坐标获取格子 (越界返回 边界)
        /// x (世界坐标系)
        /// y (世界坐标系)
        /// return 格子
        public T getCellByWorldPositionSafe(float x, float y) {
            var gridX = worldToGridX(x);
            var gridY = worldToGridY(y);
            return getCellSafe(gridX, gridY);
        }

        /// 遍历所有格子 (gridY 从小到大, gridX 从小到大)
        /// callback 回调
        public void forEachCell(Action<T> callback) {
            for (var gridY = 0; gridY < gridHeight; gridY++) {
                var row = cells[gridY];
                for (var gridX = 0; gridX < gridWidth; gridX++) {
                    var cell = row[gridX];
                    callback(cell);
                }
            }
        }
    }
}