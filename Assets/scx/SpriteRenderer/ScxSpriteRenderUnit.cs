using UnityEngine;

public class ScxSpriteRenderUnit {
    private ScxSpriteRenderer spriteRenderer;
    private ScxSpriteRenderBatch renderBatch;
    public readonly int batchID;
    public readonly int index;

    public ScxSpriteRenderData sprite;
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
        this.renderBatch.setUVs(this.index, this.sprite.uv0,this.sprite.uv1,this.sprite.uv2,this.sprite.uv3);
    }

    // UV
    public void setFrame(string name) {
        this.sprite = this.spriteRenderer.getSpriteByName(name);
        this.renderBatch.setUVs(this.index, sprite.uv0,sprite.uv1,sprite.uv2,sprite.uv3);
        this.updateUnitVertices();
    }

    // UV
    public void setFrame(int index) {
        this.sprite = this.spriteRenderer.getSpriteByIndex(index);
        this.renderBatch.setUVs(this.index, sprite.uv0,sprite.uv1,sprite.uv2,sprite.uv3);
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
            this.renderBatch.setPositions(index, Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero);
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
        

        var qx = rotation.x;
        var qy = rotation.y;
        var qz = rotation.z;
        var qw = rotation.w;

        var x2 = qx + qx;
        var y2 = qy + qy;
        var z2 = qz + qz;

        var xx = qx * x2;
        var xy = qx * y2;
        var xz = qx * z2;
        var yy = qy * y2;
        var yz = qy * z2;
        var zz = qz * z2;
        var wx = qw * x2;
        var wy = qw * y2;
        var wz = qw * z2;

        var sx = scale.x;
        var sy = scale.y;
        var sz = scale.z;

        var m00 = (1 - (yy + zz)) * sx;
        var m01 = (xy + wz) * sx;
        var m02 = (xz - wy) * sx;

        var m04 = (xy - wz) * sy;
        var m05 = (1 - (xx + zz)) * sy;
        var m06 = (yz + wx) * sy;

        var m08 = (xz + wy) * sz;
        var m09 = (yz - wx) * sz;
        var m10 = (1 - (xx + yy)) * sz;

        var m12 = position.x;
        var m13 = position.y;
        var m14 = position.z;
            
            
        var sprite = this.sprite;
        var rawP0=sprite.p0;
        var rawP1=sprite.p1;
        var rawP2=sprite.p2;
        var rawP3=sprite.p3;

        
        var vx = rawP0.x;
        var vy = rawP0.y;
        var vz = rawP0.z;

        // 更新 positions
        var p0 = new Vector3(
            m00 * vx + m04 * vy + m08 * vz + m12,
            m01 * vx + m05 * vy + m09 * vz + m13,
            m02 * vx + m06 * vy + m10 * vz + m14
        );
        
         vx = rawP1.x;
        vy = rawP1.y;
         vz = rawP1.z;
        
        var p1= new Vector3(
            m00 * vx + m04 * vy + m08 * vz + m12,
            m01 * vx + m05 * vy + m09 * vz + m13,
            m02 * vx + m06 * vy + m10 * vz + m14
        );
        
         vx = rawP2.x;
         vy = rawP2.y;
         vz = rawP2.z;
        var p2= new Vector3(
            m00 * vx + m04 * vy + m08 * vz + m12,
            m01 * vx + m05 * vy + m09 * vz + m13,
            m02 * vx + m06 * vy + m10 * vz + m14
        );
        
         vx = rawP3.x;
         vy = rawP3.y;
         vz = rawP3.z;
        
        var p3= new Vector3(
            m00 * vx + m04 * vy + m08 * vz + m12,
            m01 * vx + m05 * vy + m09 * vz + m13,
            m02 * vx + m06 * vy + m10 * vz + m14
        );
        
        this.renderBatch.setPositions(index,p0, p1, p2, p3);
    }
    
    
}