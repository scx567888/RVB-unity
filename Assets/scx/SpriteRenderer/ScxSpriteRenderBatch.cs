using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class ScxSpriteRenderBatch {
    // 四边形的基础网格信息
    private static readonly Vector3[] BASE_NORMALS = { new(0, 0, -1), new(0, 0, -1), new(0, 0, -1), new(0, 0, -1) };
    private static readonly Vector2[] BASE_UVS = { new(0, 0), new(1, 0), new(0, 1), new(1, 1) };
    private static readonly int[] BASE_INDICES = { 0, 3, 1, 3, 0, 2 };

    private readonly int capacity; // 容量

    private Vector3[] positions; // 整个网格的 顶点 数据
    private Vector3[] normals; // 整个网格的 法线 数据
    private Vector2[] uvs; // 整个网格的 UV 数据
    private int[] indices; // 整个网格的 索引 数据

    private readonly GameObject node; // 持有节点
    private readonly Mesh mesh; // 整个网格
    private readonly MeshRenderer meshRenderer; // 网格渲染器
    private readonly MeshFilter meshFilter; // 网格渲染器 (Filter)

    private readonly Stack<int> freeIndex; // 空闲的 索引

    public ScxSpriteRenderBatch(int capacity, GameObject parentNode) {
        this.capacity = capacity;

        // 我们都是 四边形 小图 
        this.positions = new Vector3[capacity * 4];
        this.normals = new Vector3[capacity * 4];
        this.uvs = new Vector2[capacity * 4];
        this.indices = new int[capacity * 6];

        // 初始化网格数据
        for (var i = 0; i < this.capacity; i++) {
            // 我们忽略填充 this.positions 以便 在视觉上默认隐藏所有单位

            var vertexOffset = i * 4;
            var indexOffset = i * 6;

            // 填充法线
            Array.Copy(BASE_NORMALS, 0, this.normals, vertexOffset, 4);
            // 填充 UV
            Array.Copy(BASE_UVS, 0, this.uvs, vertexOffset, 4);
            // 填充 索引 (索引需要计算偏移)
            this.indices[indexOffset + 0] = BASE_INDICES[0] + vertexOffset;
            this.indices[indexOffset + 1] = BASE_INDICES[1] + vertexOffset;
            this.indices[indexOffset + 2] = BASE_INDICES[2] + vertexOffset;
            this.indices[indexOffset + 3] = BASE_INDICES[3] + vertexOffset;
            this.indices[indexOffset + 4] = BASE_INDICES[4] + vertexOffset;
            this.indices[indexOffset + 5] = BASE_INDICES[5] + vertexOffset;
        }


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

        this.freeIndex = new Stack<int>(capacity);
        for (var i = 0; i < capacity; i++) {
            this.freeIndex.Push(i);
        }
    }

    // ********************* GameObject 相关 ***********************

    public void setLayer(int layer) {
        this.node.layer = layer;
    }

    public void destroy() {
        // 销毁 GPU buffer (否则会导致内存泄露)
        Object.Destroy(this.mesh);
        // 销毁 Node
        Object.Destroy(this.node);
    }

    // ********************* free 相关 ***********************

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

    /// 更新 UVs
    public void setUVs(int index, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3) {
        // 计算 Unit 在 uvs 数组中的起始位置
        var startIndex = index * 4;
        this.uvs[startIndex + 0] = uv0;
        this.uvs[startIndex + 1] = uv1;
        this.uvs[startIndex + 2] = uv2;
        this.uvs[startIndex + 3] = uv3;
    }

    /// 更新 Positions
    public void setPositions(int index, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        // 计算 Unit 在 positions 数组中的起始位置
        var startIndex = index * 4;
        this.positions[startIndex + 0] = p0;
        this.positions[startIndex + 1] = p1;
        this.positions[startIndex + 2] = p2;
        this.positions[startIndex + 3] = p3;
    }

    /// 更新材质
    public void setMaterial(Material material) {
        this.meshRenderer.sharedMaterial = material;
    }

    /// 更新 网格
    public void update() {
        // 更新网格 (索引和法线无需更新)
        mesh.SetVertices(positions);
        mesh.SetUVs(0, uvs);

        // 更新包围盒
        mesh.RecalculateBounds();
    }
}