using System.Collections.Generic;
using UnityEngine;

public sealed class ScxSpriteRenderBatch {
    
    private readonly int capacity; // 容量

    private readonly Vector3[] positions; // 整个网格的 顶点 数据
    private readonly Vector3[] normals; // 整个网格的 法线 数据
    private readonly Vector2[] uvs; // 整个网格的 UV 数据
    private readonly int[] indices; // 整个网格的 索引 数据

    private readonly GameObject node; // 持有节点
    private readonly Mesh mesh; // 整个网格
    private readonly MeshRenderer meshRenderer; // 网格渲染器
    private readonly MeshFilter meshFilter; // 网格渲染器 (Filter)

    private readonly Stack<int> freeIndex; // 空闲的 索引

    public ScxSpriteRenderBatch(int capacity, GameObject parentNode, Material material) {
        this.capacity = capacity;

        // 我们都是 四边形 小图 
        this.positions = new Vector3[capacity * 4];
        this.normals = new Vector3[capacity * 4];
        this.uvs = new Vector2[capacity * 4];
        this.indices = new int[capacity * 6];


        // 创建容器节点, 同时绑定到 父节点上.
        this.node = new GameObject("ScxSpriteRenderBatch");
        this.node.transform.SetParent(parentNode.transform, false);

        // 创建网格
        this.mesh = new Mesh {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        this.mesh.MarkDynamic();
        this.mesh.vertices = positions;
        this.mesh.normals = normals;
        this.mesh.uv = uvs;
        this.mesh.triangles = indices;

        // 创建 MeshRenderer 和 MeshFilter
        this.meshRenderer = this.node.AddComponent<MeshRenderer>();
        this.meshFilter = this.node.AddComponent<MeshFilter>();
        this.meshFilter.mesh = this.mesh;
        this.meshRenderer.sharedMaterial = material;

        this.freeIndex = new Stack<int>(capacity);
        for (var i = 0; i < capacity; i++) {
            this.freeIndex.Push(i);
        }
    }

    // ********************* 索引相关 ***********************

    public int allocate() {
        return this.freeIndex.Pop();
    }

    public void release(int index) {
        this.freeIndex.Push(index);
    }

    public bool hasFree() {
        return this.freeIndex.Count > 0;
    }

    public bool allFree() {
        return this.freeIndex.Count == this.capacity;
    }


    /// 更新 网格
    public void update() {
        // 更新网格
        meshFilter.mesh.vertices = positions;
        meshFilter.mesh.normals = normals;
        meshFilter.mesh.uv = uvs;
        meshFilter.mesh.triangles = indices; // todo 顶点用更新吗? 

        // 更新包围盒
        meshFilter.mesh.RecalculateBounds();
    }
    
}