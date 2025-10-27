using UnityEngine;

// 移植完成
public class DefaultDynamicBatchRenderer : BaseDynamicBatchRenderer<RenderUnit> {

    public DefaultDynamicBatchRenderer(int chunkCapacity, Mesh rawMesh, Material material) :
        base(chunkCapacity, rawMesh, material) {
    }

    protected override RenderUnit createUnit0(BatchRenderer batchRenderer, int chunkID, int index) {
        // 创建一个 RenderUnit
        var unit = new RenderUnit(this, batchRenderer, chunkID, index);
        unit.setVisible(true);
        return unit;
    }

}