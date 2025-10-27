using UnityEngine;

/**
 * 渲染单元
 */
// 移植完成
public class RenderUnit {

    protected DynamicBatchRenderer<RenderUnit> dynamicBatchRenderer;
    protected BatchRenderer batchRenderer;
    public readonly int chunkID;
    public readonly int index;

    public RenderUnit(DynamicBatchRenderer<RenderUnit> dynamicBatchRenderer, BatchRenderer batchRenderer, int chunkID, int index) {
        this.dynamicBatchRenderer = dynamicBatchRenderer;
        this.batchRenderer = batchRenderer;
        this.chunkID = chunkID;
        this.index = index;
    }

    // 位置
    public void setPosition(float x, float y, float z) {
        this.batchRenderer.setUnitPosition(this.index, x, y, z);
    }

    public Vector3 getPosition() {
        return this.batchRenderer.getUnitPosition(this.index);
    }

    public void translate(float dx, float dy, float dz) {
        this.batchRenderer.translateUnit(this.index, dx, dy, dz);
    }

    // 旋转
    public void setRotation(float x, float y, float z, float w) {
        this.batchRenderer.setUnitRotation(this.index, x, y, z, w);
    }

    public void setRotationFromEuler(float x, float y, float z) {
        this.batchRenderer.setUnitRotationFromEuler(this.index, x, y, z);
    }

    public Quaternion getRotation() {
        return this.batchRenderer.getUnitRotation(this.index);
    }

    public void rotate(float dx, float dy, float dz, float dw) {
        this.batchRenderer.rotateUnit(this.index, dx, dy, dz, dw);
    }

    public void rotateFromEuler(float dx, float dy, float dz) {
        this.batchRenderer.rotateUnitFromEuler(this.index, dx, dy, dz);
    }

    // 缩放
    public void setScale(float x, float y, float z) {
        this.batchRenderer.setUnitScale(this.index, x, y, z);
    }

    public Vector3 getScale() {
        return this.batchRenderer.getUnitScale(this.index);
    }

    // 可见性
    public void setVisible(bool visible) {
        this.batchRenderer.setUnitVisible(this.index, visible);
    }

    public bool getVisible() {
        return this.batchRenderer.getUnitVisible(this.index);
    }

    // UV
    public void setUVs(Vector2[] uvs) {
        this.batchRenderer.setUnitUVs(this.index, uvs);
    }

    public Vector2[] getUVs() {
        return this.batchRenderer.getUnitUVs(this.index);
    }

    // 销毁
    public void destroy() {
        this.dynamicBatchRenderer.destroyUnit(this);
        // 置空 防止后续外部调用
        this.dynamicBatchRenderer = null;
        this.batchRenderer = null;
    }
    
}