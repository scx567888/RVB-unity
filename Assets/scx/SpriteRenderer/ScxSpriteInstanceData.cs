using scx.SpriteRenderer;
using UnityEngine;

public class ScxSpriteInstanceData {

    public ScxSpriteRenderBatch batch;
    public int index;
    
    public ScxSpriteGeometry geometry;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    private bool visible;
    
    // 以下为性能优化字段
    private Vector3 p0;
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    public ScxSpriteInstanceData(ScxSpriteRenderBatch batch, int index) {
        this.batch = batch;
        this.index = index;
    }

    public void update() {
        
        
    }

    public void onAllocate(int i) {
        
    }

    public void onMove(int removeIndex) {
        
    }

    public void markAllDirty() {
        
        
    }

    public void onRelease() {
        
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

            // 更新 positions
            p0.x = m00 * sprite.p0x + m04 * sprite.p0y + m08 * sprite.p0z + m12;
            p0.y = m01 * sprite.p0x + m05 * sprite.p0y + m09 * sprite.p0z + m13;
            p0.z = m02 * sprite.p0x + m06 * sprite.p0y + m10 * sprite.p0z + m14;

            p1.x = m00 * sprite.p1x + m04 * sprite.p1y + m08 * sprite.p1z + m12;
            p1.y = m01 * sprite.p1x + m05 * sprite.p1y + m09 * sprite.p1z + m13;
            p1.z = m02 * sprite.p1x + m06 * sprite.p1y + m10 * sprite.p1z + m14;

            p2.x = m00 * sprite.p2x + m04 * sprite.p2y + m08 * sprite.p2z + m12;
            p2.y = m01 * sprite.p2x + m05 * sprite.p2y + m09 * sprite.p2z + m13;
            p2.z = m02 * sprite.p2x + m06 * sprite.p2y + m10 * sprite.p2z + m14;

            p3.x = m00 * sprite.p3x + m04 * sprite.p3y + m08 * sprite.p3z + m12;
            p3.y = m01 * sprite.p3x + m05 * sprite.p3y + m09 * sprite.p3z + m13;
            p3.z = m02 * sprite.p3x + m06 * sprite.p3y + m10 * sprite.p3z + m14;

            this.renderBatch.setPositions(index, p0, p1, p2, p3);
        }
    
}
