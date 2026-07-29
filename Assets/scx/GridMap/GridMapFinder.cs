using System;

namespace scx.GridMap {
    public static class GridMapFinder {
        /// 查找 矩形区域内格子 (相交包含)
        /// centerX - 中心 X (世界坐标)
        /// centerY - 中心 Y (世界坐标)
        /// width - 矩形宽度 (世界坐标)
        /// height - 矩形高度 (世界坐标)
        /// callback 回调 (返回 true 表示中途退出)
        /// return 返回 true 表示提前停止, false 表示完整遍历.
        public static bool findCellsInRect<T>(
            this GridMap<T> gridMap,
            float centerX, float centerY, float width, float height,
            Func<T, bool> callback
        ) where T : GridCell {
            // 1. 计算所覆盖的格子
            var startGridX = Math.Max(gridMap.worldToGridX(centerX - width / 2), 0);
            var endGridX = Math.Min(gridMap.worldToGridX(centerX + width / 2), gridMap.gridWidth - 1);
            var startGridY = Math.Max(gridMap.worldToGridY(centerY - height / 2), 0);
            var endGridY = Math.Min(gridMap.worldToGridY(centerY + height / 2), gridMap.gridHeight - 1);

            // 2. 遍历格子
            for (var gridY = startGridY; gridY <= endGridY; gridY++) {
                var row = gridMap.cells[gridY];
                for (var gridX = startGridX; gridX <= endGridX; gridX++) {
                    var cell = row[gridX];
                    // 调用回调函数
                    var stop = callback(cell);
                    if (stop) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// 查找 圆形区域内格子 (朴素算法) (相交包含)
        /// centerX - 圆心 X (世界坐标)
        /// centerY - 圆心 Y (世界坐标)
        /// radius - 半径 (世界坐标)
        /// callback 回调 (返回 true 表示中途退出)
        /// return 返回 true 表示提前停止, false 表示完整遍历.
        public static bool findCellsInCircleNaive<T>(
            this GridMap<T> gridMap,
            float centerX, float centerY, float radius,
            Func<T, bool> callback
        ) where T : GridCell {
            // 1. 计算所覆盖的格子
            var startGridX = Math.Max(gridMap.worldToGridX(centerX - radius), 0);
            var endGridX = Math.Min(gridMap.worldToGridX(centerX + radius), gridMap.gridWidth - 1);
            var startGridY = Math.Max(gridMap.worldToGridY(centerY - radius), 0);
            var endGridY = Math.Min(gridMap.worldToGridY(centerY + radius), gridMap.gridHeight - 1);

            // 1.1. 计算常量值
            var radius2 = radius * radius;

            // 2. 遍历格子
            for (var gridY = startGridY; gridY <= endGridY; gridY++) {
                var row = gridMap.cells[gridY];
                for (var gridX = startGridX; gridX <= endGridX; gridX++) {
                    var cell = row[gridX];

                    // 2.1. 跳过不在圆的范围内的

                    // 计算格子水平方向上到圆心的最短距离
                    var dx = 0f;
                    if (centerX < cell.worldStartX) {
                        dx = cell.worldStartX - centerX; // 圆心在格子左边
                    }
                    else if (centerX > cell.worldEndX) {
                        dx = centerX - cell.worldEndX; // 圆心在格子右边
                    }

                    // 计算格子垂直方向上到圆心的最短距离
                    var dy = 0f;
                    if (centerY < cell.worldStartY) {
                        dy = cell.worldStartY - centerY; // 圆心在格子上边
                    }
                    else if (centerY > cell.worldEndY) {
                        dy = centerY - cell.worldEndY; // 圆心在格子下边
                    }

                    // 勾股定理 判断是否在圆的范围内
                    if (dx * dx + dy * dy > radius2) {
                        continue;
                    }

                    // 调用回调函数
                    var stop = callback(cell);
                    if (stop) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// 查找 圆形区域内格子 (扫描线算法) (相交包含)
        /// centerX - 圆心 X (世界坐标)
        /// centerY - 圆心 Y (世界坐标)
        /// radius - 半径 (世界坐标)
        /// callback 回调 (返回 true 表示中途退出)
        /// return 返回 true 表示提前停止, false 表示完整遍历.
        public static bool findCellsInCircleScanLine<T>(
            this GridMap<T> gridMap,
            float centerX, float centerY, float radius,
            Func<T, bool> callback
        ) where T : GridCell {
            // 1. 计算覆盖的行范围
            var startGridY = Math.Max(gridMap.worldToGridY(centerY - radius), 0);
            var endGridY = Math.Min(gridMap.worldToGridY(centerY + radius), gridMap.gridHeight - 1);

            // 1.1. 计算常量值
            var radius2 = radius * radius;

            // 2. 循环每一行
            for (var gridY = startGridY; gridY <= endGridY; gridY++) {
                // 计算当前行的 上下 Y (世界坐标距离)
                var worldStartY = gridMap.gridToWorldStartY(gridY);
                var worldEndY = gridMap.gridToWorldEndY(gridY);

                // 计算当前行垂直方向上到圆心的最短距离
                var dy = 0f;
                if (centerY < worldStartY) {
                    dy = worldStartY - centerY; // 圆心在格子上边
                }
                else if (centerY > worldEndY) {
                    dy = centerY - worldEndY; // 圆心在格子下边
                }

                // 计算当前行覆盖的列范围
                var dxMax = (float)Math.Sqrt(radius2 - dy * dy);
                var startGridX = Math.Max(gridMap.worldToGridX(centerX - dxMax), 0);
                var endGridX = Math.Min(gridMap.worldToGridX(centerX + dxMax), gridMap.gridWidth - 1);

                var row = gridMap.cells[gridY];
                // 遍历当前行
                for (var gridX = startGridX; gridX <= endGridX; gridX++) {
                    var cell = row[gridX];
                    // 调用回调函数
                    var stop = callback(cell);
                    if (stop) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}