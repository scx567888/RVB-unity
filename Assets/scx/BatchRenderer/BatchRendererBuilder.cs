using UnityEngine;

// 移植完成
public class BatchRendererBuilder {
    
    public static MeshMergeBatchRenderer createByPrefab(int capacity, GameObject prefab) {
        // 实例化
        var tempNode = Object.Instantiate(prefab);
        // 从预制体提取网格数据
        var tempMeshRenderer = tempNode.GetComponent<MeshRenderer>();
        var tempMeshFilter = tempNode.GetComponent<MeshFilter>();
        var tempMesh = tempMeshFilter.mesh;
        var tempMaterial = tempMeshRenderer.sharedMaterial;
        // 创建 BatchRenderer
        var batchRenderer  = new MeshMergeBatchRenderer(capacity, tempMesh, tempMaterial);
        // 创建完成 销毁 临时节点
        Object.Destroy(tempNode);
        // 返回
        return batchRenderer;
    }
    
    public static DefaultDynamicBatchRenderer createDynamicByPrefab(int chunkCapacity,GameObject prefab )  {
        // 实例化
        var tempNode = Object.Instantiate(prefab);
        // 从预制体提取网格数据
        var tempMeshRenderer = tempNode.GetComponent<MeshRenderer>();
        var tempMeshFilter = tempNode.GetComponent<MeshFilter>();
        var tempMesh = tempMeshFilter.mesh;
        var tempMaterial = tempMeshRenderer.sharedMaterial;
        // 创建 BatchRenderer
        var batchRenderer = new DefaultDynamicBatchRenderer(chunkCapacity, tempMesh, tempMaterial);
        // 创建完成 销毁 临时节点
        Object.Destroy(tempNode);
        // 返回
        return batchRenderer;
    }

}