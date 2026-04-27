using scx.SpriteRenderer;
using UnityEngine;

// 此类不做脏检查 因为没必要 实际使用中每一帧 uv 和 position 都会变.
public class ScxSpriteInstanceData {

    public ScxSpriteRenderBatch batch;
    public int index;

    public ScxSpriteGeometry geometry;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool visible;

    // 以下为性能优化字段
    private Vector3 p0;
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    public ScxSpriteInstanceData(ScxSpriteRenderBatch batch, int index) {
        this.batch = batch;
        this.index = index;
    }

    public void onAllocate(int i) {
    }

    public void onMove(int removeIndex) {
    }

    public void markAllDirty() {
    }

    public void onRelease() {
        
    }
    
    // 可见性
    public void setVisible(bool visible) {
        // 如果和之前一样 跳过
        if (this.visible == visible) {
            return;
        }

        this.visible = visible;

        if (this.visible) {
            this.update();
        }
        else {
            // 通过将单元的所有顶点塌缩到 0 点(0, 0, 0), 使其在视觉上隐藏/移除
            this.batch.setPositions(index, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero);
        }
    }

    public void update() {
        if (!visible) {
            return;
        }
        
        updateUVs();
        updatePositions();
    }

    public void updateUVs() {
        this.batch.setUVs(this.index, geometry.uv0, geometry.uv1, geometry.uv2, geometry.uv3);
    }

    public void updatePositions() {
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

        var geometry = this.geometry;

        // 更新 positions
        p0.x = m00 * geometry.p0x + m04 * geometry.p0y + m08 * geometry.p0z + m12;
        p0.y = m01 * geometry.p0x + m05 * geometry.p0y + m09 * geometry.p0z + m13;
        p0.z = m02 * geometry.p0x + m06 * geometry.p0y + m10 * geometry.p0z + m14;

        p1.x = m00 * geometry.p1x + m04 * geometry.p1y + m08 * geometry.p1z + m12;
        p1.y = m01 * geometry.p1x + m05 * geometry.p1y + m09 * geometry.p1z + m13;
        p1.z = m02 * geometry.p1x + m06 * geometry.p1y + m10 * geometry.p1z + m14;

        p2.x = m00 * geometry.p2x + m04 * geometry.p2y + m08 * geometry.p2z + m12;
        p2.y = m01 * geometry.p2x + m05 * geometry.p2y + m09 * geometry.p2z + m13;
        p2.z = m02 * geometry.p2x + m06 * geometry.p2y + m10 * geometry.p2z + m14;

        p3.x = m00 * geometry.p3x + m04 * geometry.p3y + m08 * geometry.p3z + m12;
        p3.y = m01 * geometry.p3x + m05 * geometry.p3y + m09 * geometry.p3z + m13;
        p3.z = m02 * geometry.p3x + m06 * geometry.p3y + m10 * geometry.p3z + m14;

        this.batch.setPositions(index, p0, p1, p2, p3);
    }
}