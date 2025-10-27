/**
 * 表示 GridMap 中的一个单元, 可以继承以实现更多功能
 */
public class GridCell {

    /**
     *  自己所在的 列 (Grid 坐标系)
     *  @type {number}
     */
    public readonly int gridX;

    /**
     * 自己所在的 行 (Grid 坐标系)
     *  @type {number}
     */
    public readonly int gridY;

    /**
     * 格子起始 X 坐标 (世界坐标系)
     *  @type {number}
     */
    public readonly float worldStartX;

    /**
     * 格子起始 Y 坐标 (世界坐标系)
     *  @type {number}
     */
    public readonly float worldStartY;

    /**
     * 格子结束 X 坐标 (世界坐标系)
     *  @type {number}
     */
    public readonly float worldEndX;

    /**
     * 格子结束 Y 坐标 (世界坐标系)
     *  @type {number}
     */
    public readonly float worldEndY;

    /**
     * @param {number} gridX
     * @param {number} gridY
     * @param {number} worldStartX
     * @param {number} worldStartY
     * @param {number} worldEndX
     * @param {number} worldEndY
     */
    public GridCell(int gridX, int gridY, float worldStartX, float worldStartY, float worldEndX, float worldEndY) {
        this.gridX = gridX;
        this.gridY = gridY;
        this.worldStartX = worldStartX;
        this.worldStartY = worldStartY;
        this.worldEndX = worldEndX;
        this.worldEndY = worldEndY;
    }

}