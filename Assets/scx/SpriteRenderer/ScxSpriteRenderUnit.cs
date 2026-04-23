using UnityEngine;

public class ScxSpriteRenderUnit {
    private ScxSpriteRenderer spriteRenderer;
    private ScxSpriteRenderBatch renderBatch;
    public readonly int batchID;
    public readonly int index;

    public int frame;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool visible;

    public ScxSpriteRenderUnit(ScxSpriteRenderer spriteRenderer, ScxSpriteRenderBatch renderBatch, int batchID,
        int index) {
        this.spriteRenderer = spriteRenderer;
        this.renderBatch = renderBatch;
        this.batchID = batchID;
        this.index = index;

        this.frame = 0;
        this.position = new Vector3(0, 0, 0);
        this.rotation = new Quaternion(0, 0, 0, 1);
        this.scale = new Vector3(1, 1, 1);
        this.visible = false;
    }

    // UV
    public void setFrame(string name) {
        // todo 这里先不管
        // var uvs = this.spriteRenderer.getUVsByFrameName(name);
        // this.renderBatch.setUnitUVs(this.index, uvs);
    }

    // UV
    public void setFrame(int index) {
        // todo 这里先不管
        // var uvs = this.spriteRenderer.getUVsByFrameIndex(index);
        // this.renderBatch.setUnitUVs(this.index, uvs);
    }

    // 位置
    public void setPosition(float x, float y, float z) {
        this.position.Set(x, y, z);
        this.updateUnitVertices();
    }

    public Vector3 getPosition() {
        return this.position;
    }

    public void translate(float dx, float dy, float dz) {
        this.position.x += dx;
        this.position.y += dy;
        this.position.z += dz;
        this.updateUnitVertices();
    }

    // 旋转
    public void setRotation(float x, float y, float z, float w) {
        this.rotation.Set(x, y, z, w);
        this.updateUnitVertices();
    }

    public void setRotationFromEuler(float x, float y, float z) {
        this.rotation = Quaternion.Euler(x, y, z);
        this.updateUnitVertices();
    }

    public Quaternion getRotation() {
        return rotation;
    }

    public void rotate(float dx, float dy, float dz, float dw) {
        this.rotation *= new Quaternion(dx, dy, dz, dw);
        this.updateUnitVertices();
    }

    public void rotateFromEuler(float dx, float dy, float dz) {
        this.rotation *= Quaternion.Euler(dx, dy, dz);
        this.updateUnitVertices();
    }

    // 缩放
    public void setScale(float x, float y, float z) {
        this.scale.Set(x, y, z);
        this.updateUnitVertices();
    }

    public Vector3 getScale() {
        return this.scale;
    }

    // 可见性
    public void setVisible(bool visible) {
        // 如果和之前一样 跳过
        if (this.visible == visible) {
            return;
        }

        this.visible = visible;

        if (this.visible) {
            this.updateUnitVertices();
        }
        else {
            // 通过将单元的所有顶点塌缩到 0 点(0, 0, 0), 使其在视觉上隐藏/移除
            var vector3 = new[]
                { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            this.renderBatch.setPositions(index, vector3);
        }
    }

    public bool getVisible() {
        return visible;
    }

    // 销毁
    public void destroy() {
        this.spriteRenderer.destroyUnit(this);
        // 置空 防止后续外部调用
        this.spriteRenderer = null;
        this.renderBatch = null;
    }

    private void updateUnitVertices() {
        if (!this.visible) {
            return;
        }

        // todo 这里需要处理 复杂的计算 是不是 甚至需要 计算 原图的一些东西? 
        this.renderBatch.setPositions(this.index,  ?);
    }
}