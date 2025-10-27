using System;

/**
 * 一个网格容器, 可用于 寻敌, 空间划分 等
 * 每一个格子都是正方形
 * @template {GridCell} T
 */
public class GridMap<T>  where T : GridCell{
    
    
      /**
     * 世界 X (世界坐标系)
     * @type {number}
     */
    private readonly float worldX;

    /**
     * 世界 Y (世界坐标系)
     * @type {number}
     */
    private readonly float worldY;

    /**
     * 世界宽度 (世界坐标系)
     * @type {number}
     */
    private readonly float worldWidth;

    /**
     * 世界高度 (世界坐标系)
     * @type {number}
     */
    private readonly float worldHeight;

    /**
     * 格子大小 (正方形) (世界坐标系)
     * @type {number}
     */
    private readonly float cellSize;

    /**
     * 横向的格子数量 (Grid 坐标系)
     * @type {number}
     */
    private readonly int gridWidth;

    /**
     * 纵向的格子数量 (Grid 坐标系)
     * @type {number}
     */
    private readonly int gridHeight;

    /**
     * 格子 (二维数组)
     * @type {T[][]}
     */
    private readonly T[,] cells;

    /**
     * 创建一个 GridMap
     * @param {number} worldX   世界 X
     * @param {number} worldY  世界 Y
     * @param {number}  worldWidth  世界宽度
     * @param {number} worldHeight  世界高度
     * @param {number} cellSize  格子大小 (正方形宽高)
     * @param CellClass 格子构造函数
     */
    public GridMap(float worldX,float worldY,float worldWidth,float worldHeight,float cellSize, Func<int,int,float,float,float,float,T>  CellClass) {
        if (worldWidth <= 0 || worldHeight <= 0 || cellSize <= 0) {
            throw new ArgumentException("worldWidth, worldHeight, cellSize 必须为正数");
        }
        this.worldX = worldX;
        this.worldY = worldY;
        this.worldWidth = worldWidth;
        this.worldHeight = worldHeight;
        this.cellSize = cellSize;

        // 计算有多少个格子
        this.gridWidth = (int)Math.Ceiling(worldWidth / cellSize);
        this.gridHeight = (int)Math.Ceiling(worldHeight / cellSize);

        // 创建二维数组
        this.cells = new T[gridHeight,gridWidth];
        for (var gridY = 0; gridY < this.gridHeight; gridY++) {
            for (var gridX = 0; gridX < this.gridWidth; gridX++) {
                var cell = CellClass(
                    gridX, gridY,
                    this.gridToWorldStartX(gridX), this.gridToWorldStartY(gridY),
                    this.gridToWorldEndX(gridX), this.gridToWorldEndY(gridY)
                );
                cells[gridY, gridX] = cell;
            }
        }

    }

    /**
     * 世界坐标 X 转 Grid 坐标 X
     * 坐标正好在格子边界时, 归入索引较大的格子.
     * @param {number} x X 坐标 (世界坐标系)
     * @returns {number} 格子 X (Grid 坐标系)
     */
    public int worldToGridX(float x) {
        return (int) Math.Floor((x - this.worldX) / this.cellSize);
    }

    /**
     * 世界坐标 Y 转 Grid 坐标 Y
     * 坐标正好在格子边界时, 归入索引较大的格子.
     * @param {number} y Y 坐标 (世界坐标系)
     * @returns {number} 格子 Y (Grid 坐标系)
     */
    public int worldToGridY(float y) {
        return (int)Math.Floor((y - this.worldY) / this.cellSize);
    }

    /**
     * Grid 坐标 X 转 格子起始 X 坐标 (世界坐标系)
     * @param {number} gridX
     * @return {number} 格子起始 X 坐标
     */
    public float gridToWorldStartX(int gridX) {
        return this.worldX + gridX * this.cellSize;
    }

    /**
     * Grid 坐标 Y 转 格子起始 Y 坐标 (世界坐标系)
     * @param {number} gridY
     * @returns {number} 格子起始 Y 坐标
     */
    public float gridToWorldStartY(int gridY) {
        return this.worldY + gridY * this.cellSize;
    }

    /**
     * Grid 坐标 X 转 格子结束 X 坐标 (世界坐标系)
     * @param {number} gridX
     * @returns {number} 格子结束 X 坐标
     */
    public float gridToWorldEndX(int gridX) {
        return this.gridToWorldStartX(gridX) + this.cellSize;
    }

    /**
     * Grid 坐标 Y 转 格子结束 Y 坐标 (世界坐标系)
     * @param {number}  gridY
     * @returns {number} 格子结束 Y 坐标
     */
    public float gridToWorldEndY(int gridY) {
        return this.gridToWorldStartY(gridY) + this.cellSize;
    }

    /**
     * 获取格子 (越界会返回 null)
     * @param {number} gridX (Grid 坐标系)
     * @param {number} gridY (Grid 坐标系)
     * @returns {T} 格子
     */
    public T getCell(int gridX,int gridY) {
        // 越界判断
        if (gridX < 0 || gridX >= this.gridWidth || gridY < 0 || gridY >= this.gridHeight) {
            return null;
        }
        return this.cells[gridY,gridX];
    }

    /**
     * 获取格子 (越界会返回 边界)
     * @param {number} gridX (Grid 坐标系)
     * @param {number} gridY (Grid 坐标系)
     * @returns {T} 格子
     */
    public T getCellSafe(int gridX,int gridY) {
        if (gridX < 0) {
            gridX = 0;
        } else if (gridX >= this.gridWidth) {
            gridX = this.gridWidth - 1;
        }
        if (gridY < 0) {
            gridY = 0;
        } else if (gridY >= this.gridHeight) {
            gridY = this.gridHeight - 1;
        }
        return this.cells[gridY,gridX];
    }

    /**
     * 根据世界坐标获取格子 (越界返回 null)
     * @param {number} x (世界坐标系)
     * @param {number} y (世界坐标系)
     * @returns {T} 格子
     */
    public T getCellByWorldPosition(float x,float y) {
        var gridX = this.worldToGridX(x);
        var gridY = this.worldToGridY(y);
        return this.getCell(gridX, gridY);
    }

    /**
     * 根据世界坐标获取格子 (越界返回 边界)
     * @param {number} x (世界坐标系)
     * @param {number} y (世界坐标系)
     * @returns {T} 格子
     */
    public T getCellByWorldPositionSafe(float x,float y) {
        var gridX = this.worldToGridX(x);
        var gridY = this.worldToGridY(y);
        return this.getCellSafe(gridX, gridY);
    }

    /**
     * 遍历所有格子
     * @param {function(T): boolean} callback - 回调 (允许中途退出)
     */
    public void forEachCell(Func<T, bool> callback) {
        for (var gridY = 0; gridY < this.gridHeight; gridY++) {
            for (var gridX = 0; gridX < this.gridWidth; gridX++) {
                var cell = this.cells[gridY,gridX];
                var stop = callback(cell);
                if (stop == true) {
                    return;
                }
            }
        }
    }


    // **************************************** find 方法 ******************************************

    /**
     * 查找 矩形区域内格子 (相交包含)
     * @param {number} centerX - 中心 X (世界坐标)
     * @param {number} centerY - 中心 Y (世界坐标)
     * @param {number} width - 矩形宽度 (世界坐标)
     * @param {number} height - 矩形高度 (世界坐标)
     * @param {function(T): boolean} callback - 回调 (允许中途退出)
     */
    public void findCellsInRect(float centerX,float centerY,float width,float height,Func<T, bool> callback) {
        // 1. 计算所覆盖的格子
        var startGridX = Math.Max(this.worldToGridX(centerX - width / 2), 0);
        var endGridX = Math.Min(this.worldToGridX(centerX + width / 2), this.gridWidth - 1);
        var startGridY = Math.Max(this.worldToGridY(centerY - height / 2), 0);
        var endGridY = Math.Min(this.worldToGridY(centerY + height / 2), this.gridHeight - 1);

        // 2. 遍历格子
        for (var gridY = startGridY; gridY <= endGridY; gridY++) {
            for (var gridX = startGridX; gridX <= endGridX; gridX++) {
                var cell = this.cells[gridY,gridX];
                // 调用回调函数
                var stop = callback(cell);
                if (stop == true) {
                    return;
                }
            }
        }
    }

    /**
     * 查找 圆形区域内格子 (朴素算法) (相交包含)
     * @param {number} centerX - 圆心 X (世界坐标)
     * @param {number} centerY - 圆心 Y (世界坐标)
     * @param {number} radius - 半径 (世界坐标)
     * @param {function(T): boolean} callback - 回调 (允许中途退出)
     */
    public void findCellsInCircleNaive(float centerX,float centerY,float radius,Func<T, bool> callback) {
        // 1. 计算所覆盖的格子
        var startGridX = Math.Max(this.worldToGridX(centerX - radius), 0);
        var endGridX = Math.Min(this.worldToGridX(centerX + radius), this.gridWidth - 1);
        var startGridY = Math.Max(this.worldToGridY(centerY - radius), 0);
        var endGridY = Math.Min(this.worldToGridY(centerY + radius), this.gridHeight - 1);

        // 1.1. 计算常量值
        var radius2 = radius * radius;

        // 2. 遍历格子
        for (var gridY = startGridY; gridY <= endGridY; gridY++) {
            for (var gridX = startGridX; gridX <= endGridX; gridX++) {
                var cell = this.cells[gridY,gridX];

                // 2.1. 跳过不在圆的范围内的

                // 计算格子水平方向上到圆心的最短距离
                var dx = 0f;
                if (centerX < cell.worldStartX) {
                    dx = cell.worldStartX - centerX; // 圆心在格子左边
                } else if (centerX > cell.worldEndX) {
                    dx = centerX - cell.worldEndX; // 圆心在格子右边
                }

                // 计算格子垂直方向上到圆心的最短距离
                var dy = 0f;
                if (centerY < cell.worldStartY) {
                    dy = cell.worldStartY - centerY; // 圆心在格子上边
                } else if (centerY > cell.worldEndY) {
                    dy = centerY - cell.worldEndY; // 圆心在格子下边
                }

                // 勾股定理 判断是否在圆的范围内
                if (dx * dx + dy * dy > radius2) {
                    continue;
                }

                // 调用回调函数
                var stop = callback(cell);
                if (stop == true) {
                    return;
                }

            }
        }
    }

    /**
     * 查找 圆形区域内格子 (扫描线算法)  (相交包含)
     * @param {number} centerX - 圆心 X (世界坐标)
     * @param {number} centerY - 圆心 Y (世界坐标)
     * @param {number} radius - 半径 (世界坐标)
     * @param {function(T): boolean} callback - 回调 (允许中途退出)
     */
    public void findCellsInCircleScanLine(float centerX,float centerY,float radius,Func<T, bool> callback) {
        // 1. 计算覆盖的行范围
        var startGridY = Math.Max(this.worldToGridY(centerY - radius), 0);
        var endGridY = Math.Min(this.worldToGridY(centerY + radius), this.gridHeight - 1);

        // 1.1. 计算常量值
        var radius2 = radius * radius;

        // 2. 循环每一行
        for (var gridY = startGridY; gridY <= endGridY; gridY++) {
            // 计算当前行的 上下 Y (世界坐标距离)
            var worldStartY = this.gridToWorldStartY(gridY);
            var worldEndY = this.gridToWorldEndY(gridY);

            // 计算当前行垂直方向上到圆心的最短距离
            var dy = 0f;
            if (centerY < worldStartY) {
                dy = worldStartY - centerY; // 圆心在格子上边
            } else if (centerY > worldEndY) {
                dy = centerY - worldEndY; // 圆心在格子下边
            }

            // 计算当前行覆盖的列范围
            var dxMax =(float) Math.Sqrt(radius2 - dy * dy);
            var startGridX = Math.Max(this.worldToGridX(centerX - dxMax), 0);
            var endGridX = Math.Min(this.worldToGridX(centerX + dxMax), this.gridWidth - 1);

            // 遍历当前行
            for (var gridX = startGridX; gridX <= endGridX; gridX++) {
                var cell = this.cells[gridY,gridX];
                // 调用回调函数
                var stop = callback(cell);
                if (stop == true) {
                    return;
                }
            }

        }
    }
    
}