using UnityEngine;

/**
 * 动态的 面向对象的 批量渲染器
 */
// 移植完成
public interface DynamicBatchRenderer<out U> : NodeLike where U : RenderUnit {
    
    // 材质
    void setMaterial(Material material);

    Material getMaterial();

    // Unit
    U createUnit();

    void destroyUnit(RenderUnit unit);

    // 更新
    void update();

}