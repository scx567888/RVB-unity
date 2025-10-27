using UnityEngine;

/**
 * 批量渲染器 (固定容量)
 */
// 移植完成
public interface BatchRenderer : NodeLike {

    // 容量
    int capacity();

    // 材质
    void setMaterial(Material material);

    Material getMaterial();

    // 位置
    void setUnitPosition(int index, float x, float y, float z);

    Vector3 getUnitPosition(int index);

    void translateUnit(int index, float dx, float dy, float dz);

    // 旋转
    void setUnitRotation(int index, float x, float y, float z, float w);

    void setUnitRotationFromEuler(int index, float x, float y, float z);

    Quaternion getUnitRotation(int index);

    void rotateUnit(int index, float dx, float dy, float dz, float dw);

    void rotateUnitFromEuler(int index, float dx, float dy, float dz);

    // 缩放
    void setUnitScale(int index, float x, float y, float z);

    Vector3 getUnitScale(int index);

    void scaleUnit(int index, float sx, float sy, float sz);

    // 可见性
    void setUnitVisible(int index, bool visible);

    bool getUnitVisible(int index);

    // UV
    void setUnitUVs(int index, Vector2[] uvs);

    Vector2[] getUnitUVs(int index);

    // 更新
    void update();
    
}