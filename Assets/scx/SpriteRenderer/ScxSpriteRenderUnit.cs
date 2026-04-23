using UnityEngine;

public class ScxSpriteRenderUnit {
    private ScxSpriteRenderer spriteRenderer;
    private ScxSpriteRenderBatch renderBatch;
    public readonly int batchID;
    public readonly int index;

    public ScxSprite sprite;
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
        
        // 默认初始化第一个 
        this.sprite = spriteRenderer.getSpriteByIndex(0);
        this.position = new Vector3(0, 0, 0);
        this.rotation = new Quaternion(0, 0, 0, 1);
        this.scale = new Vector3(1, 1, 1);
        this.visible = false;
        this.renderBatch.setUVs(this.index, this.sprite.uv);
    }

    // UV
    public void setFrame(string name) {
        this.sprite = this.spriteRenderer.getSpriteByName(name);
        this.renderBatch.setUVs(this.index, sprite.uv);
        this.updateUnitVertices();
    }

    // UV
    public void setFrame(int index) {
        this.sprite = this.spriteRenderer.getSpriteByIndex(index);
        this.renderBatch.setUVs(this.index, sprite.uv);
        this.updateUnitVertices();
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

        var sprite = this.sprite;
        var pixelsPerUnit = this.spriteRenderer.getPixelsPerUnit();

        // 1. pivot 在原图中的像素位置
        var pivotPixelX = sprite.pivot.x * sprite.sourceSize.x;
        var pivotPixelY = sprite.pivot.y * sprite.sourceSize.y;

        // 2. 裁边后矩形在“以 pivot 为原点”的局部像素空间中的范围
        var left = sprite.sourceRect.x - pivotPixelX;
        var right = sprite.sourceRect.x + sprite.sourceRect.width - pivotPixelX;
        var bottom = sprite.sourceRect.y - pivotPixelY;
        var top = sprite.sourceRect.y + sprite.sourceRect.height - pivotPixelY;

        // 3. 像素转单位
        left /= pixelsPerUnit;
        right /= pixelsPerUnit;
        bottom /= pixelsPerUnit;
        top /= pixelsPerUnit;

        // 4. 生成局部四个顶点
        // 顶点顺序:
        // 0 = 左下
        // 1 = 右下
        // 2 = 左上
        // 3 = 右上
        var p0 = new Vector3(left,  bottom, 0);
        var p1 = new Vector3(right, bottom, 0);
        var p2 = new Vector3(left,  top,    0);
        var p3 = new Vector3(right, top,    0);

        // 5. 应用缩放
        p0 = Vector3.Scale(p0, this.scale);
        p1 = Vector3.Scale(p1, this.scale);
        p2 = Vector3.Scale(p2, this.scale);
        p3 = Vector3.Scale(p3, this.scale);

        // 6. 应用旋转
        p0 = this.rotation * p0;
        p1 = this.rotation * p1;
        p2 = this.rotation * p2;
        p3 = this.rotation * p3;

        // 7. 应用平移
        p0 += this.position;
        p1 += this.position;
        p2 += this.position;
        p3 += this.position;

        // 8. 写回 batch
        this.renderBatch.setPositions(this.index, new[] { p0, p1, p2, p3 });
    }
    
    
}