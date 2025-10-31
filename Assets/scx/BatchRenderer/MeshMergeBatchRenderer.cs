using System;
using UnityEngine;
using Object = UnityEngine.Object;

/**
 * 网格合并 批量渲染器
 */
// 移植完成
public class MeshMergeBatchRenderer : BatchRenderer {

    // 容量
    protected readonly int _capacity;

    // 原始网格
    protected readonly Mesh _rawMesh;

    // 原始网格的 顶点 数据
    protected readonly Vector3[] _rawPositions;
    // 原始网格的 法线 数据
    protected readonly Vector3[] _rawNormals;
    // 原始网格的 UV 数据
    protected readonly Vector2[] _rawUVs;
    // 原始网格的 索引 数据
    protected readonly int[] _rawIndices;

    // 整个网格的 顶点 数据
    protected readonly Vector3[] _positions;
    // 整个网格的 法线 数据
    protected readonly Vector3[] _normals;
    // 整个网格的 UV 数据
    protected readonly Vector2[] _uvs;
    // 整个网格的 索引 数据
    protected readonly int[] _indices;

    // 整个网格
    protected readonly Mesh _mesh;
    // 持有节点
    protected readonly GameObject _node;
    // 网格渲染器
    protected readonly MeshRenderer _meshRenderer;
    // 网格渲染器 (Filter)
    protected readonly MeshFilter _meshFilter;

    // Unit 的变换数据
    protected readonly UnitTransform[] _unitTransforms;

    public MeshMergeBatchRenderer(int capacity, Mesh rawMesh, Material material) {
        this._capacity = capacity;
        this._rawMesh = rawMesh;

        // 提取原始网格的数据
        this._rawPositions = this._rawMesh.vertices;
        this._rawNormals = this._rawMesh.normals;
        this._rawUVs = this._rawMesh.uv;
        this._rawIndices = this._rawMesh.triangles;

        // 创建整个网格的数据
        this._positions = new Vector3[this._rawPositions.Length * this._capacity];
        this._normals = new Vector3[this._rawNormals.Length * this._capacity];
        this._uvs = new Vector2[this._rawUVs.Length * this._capacity];
        this._indices = new int[this._rawIndices.Length * this._capacity];

        // 初始化网格数据
        var rawVertexCount = this._rawPositions.Length;
        for (var i = 0; i < this._capacity; i++) {
            // 我们忽略填充 this._positions 以便 在视觉上默认隐藏所有单位
            // 填充法线
            Array.Copy(this._rawNormals, 0, this._normals, i * this._rawNormals.Length, this._rawNormals.Length);
            // 填充 UV
            Array.Copy(this._rawUVs, 0, this._uvs, i * this._rawUVs.Length, this._rawUVs.Length);
            // 填充 索引 (索引需要计算偏移)
            var indicesOffset = i * this._rawIndices.Length;
            for (var j = 0; j < this._rawIndices.Length; j++) {
                this._indices[indicesOffset + j] = this._rawIndices[j] + i * rawVertexCount;
            }
        }

        // 创建网格
        this._mesh = new Mesh {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        this._mesh.MarkDynamic();
        this._mesh.vertices = this._positions;
        this._mesh.normals = this._normals;
        this._mesh.uv = this._uvs;
        this._mesh.triangles = this._indices;

        // 创建容器节点
        this._node = new GameObject("BatchRenderer");

        // 创建 MeshRenderer
        this._meshRenderer = this._node.AddComponent<MeshRenderer>();
        this._meshFilter = this._node.AddComponent<MeshFilter>();
        this._meshFilter.mesh = this._mesh;
        this._meshRenderer.sharedMaterial = material;

        // 创建 单元 变化状态数组
        this._unitTransforms = new UnitTransform[this._capacity];
        for (var i = 0; i < this._capacity; i++) {
            this._unitTransforms[i] = new UnitTransform();
        }

    }

    // ================ Node 模拟接口 ================

    public void setParent(GameObject parent) {
        this._node.transform.SetParent(parent.transform, false);
    }

    public GameObject getParent() {
        return this._node.transform.parent.gameObject;
    }

    public void setPosition(float x, float y, float z) {
        this._node.transform.position = new Vector3(x, y, z);
    }

    public Vector3 getPosition() {
        return this._node.transform.position;
    }

    public void setRotation(float x, float y, float z, float w) {
        this._node.transform.rotation = new Quaternion(x, y, z, w);
    }

    public void setRotationFromEuler(float x, float y, float z) {
        this._node.transform.rotation = Quaternion.Euler(x, y, z);
    }

    public Quaternion getRotation() {
        return this._node.transform.rotation;
    }

    public void setScale(float x, float y, float z) {
        this._node.transform.localScale = new Vector3(x, y, z);
    }

    public Vector3 getScale() {
        return this._node.transform.localScale;
    }

    public void setActive(bool active) {
        this._node.SetActive(active);
    }

    public bool getActive() {
        return this._node.activeSelf;
    }

    public void setLayer(string name) {
        this._node.layer= LayerMask.NameToLayer(name);
    }

    public string getLayer() {
        return LayerMask.LayerToName(this._node.layer);
    }

    public void destroy() {
        // 销毁 GPU buffer (否则会导致内存泄露)
        Object.Destroy(this._mesh);
        // 销毁 Node
        Object.Destroy(this._node);
    }

    // ================ BatchRenderer 接口 ================

    // 容量
    public int capacity() {
        return this._capacity;
    }

    // 材质
    public virtual void setMaterial(Material material) {
        this._meshRenderer.sharedMaterial = material;
    }

    public Material getMaterial() {
        return this._meshRenderer.sharedMaterial;
    }

    // 位置
    public void setUnitPosition(int index, float x, float y, float z) {
        var unitTransform = this._unitTransforms[index];
        unitTransform.position.Set(x, y, z);
        // 更新顶点
        this.refreshUnit(index);
    }

    public Vector3 getUnitPosition(int index) {
        return this._unitTransforms[index].position;
    }

    public void translateUnit(int index, float dx, float dy, float dz) {
        var unitTransform = this._unitTransforms[index];
        unitTransform.position.x += dx;
        unitTransform.position.y += dy;
        unitTransform.position.z += dz;
        // 更新顶点
        this.refreshUnit(index);
    }

    // 旋转
    public void setUnitRotation(int index, float x, float y, float z, float w) {
        var unitTransform = this._unitTransforms[index];
        unitTransform.rotation.Set(x, y, z, w);
        // 更新顶点
        this.refreshUnit(index);
    }

    public void setUnitRotationFromEuler(int index, float x, float y, float z) {
        var unitTransform = this._unitTransforms[index];
        // 将欧拉角转四元数
        unitTransform.rotation = Quaternion.Euler(x, y, z);
        // 更新顶点
        this.refreshUnit(index);
    }

    public Quaternion getUnitRotation(int index) {
        return this._unitTransforms[index].rotation;
    }

    public void rotateUnit(int index, float dx, float dy, float dz, float dw) {
        var unitTransform = this._unitTransforms[index];
        unitTransform.rotation = unitTransform.rotation * new Quaternion(dx, dy, dz, dw);
        // 更新顶点
        this.refreshUnit(index);
    }

    public void rotateUnitFromEuler(int index, float dx, float dy, float dz) {
        var unitTransform = this._unitTransforms[index];
        unitTransform.rotation = unitTransform.rotation * Quaternion.Euler(dx, dy, dz);
        // 更新顶点
        this.refreshUnit(index);
    }

    // 缩放
    public void setUnitScale(int index, float x, float y, float z) {
        var unitTransform = this._unitTransforms[index];
        unitTransform.scale.Set(x, y, z);
        // 更新顶点
        this.refreshUnit(index);
    }

    public Vector3 getUnitScale(int index) {
        return this._unitTransforms[index].scale;
    }

    public void scaleUnit(int index, float sx, float sy, float sz) {
        var unitTransform = this._unitTransforms[index];
        // 累加增量
        unitTransform.scale.x *= sx;
        unitTransform.scale.y *= sy;
        unitTransform.scale.z *= sz;
        this.refreshUnit(index);
    }

    // 可见性
    public void setUnitVisible(int index, bool visible) {
        var unitTransform = this._unitTransforms[index];
        // 如果和之前一样 跳过
        if (unitTransform.visible == visible) {
            return;
        }

        unitTransform.visible = visible;

        if (unitTransform.visible) {
            this.updateUnitVertices(index, unitTransform);
        }
        else {
            // 通过将单元的所有顶点塌缩到 0 点(0, 0, 0), 使其在视觉上隐藏/移除
            this.collapseUnitVertices(index, 0, 0, 0);
        }

    }

    public bool getUnitVisible(int index) {
        return this._unitTransforms[index].visible;
    }

    // UV
    public virtual void setUnitUVs(int index, Vector2[] uvs) {
        // 计算 Unit 在 UVs 数组中的起始位置
        var startIndex = this._rawUVs.Length * index;
        // 更新 UV
        Array.Copy(uvs, 0, this._uvs, startIndex, this._rawUVs.Length);
    }

    public Vector2[] getUnitUVs(int index) {
        // 计算 Unit 在 UVs 数组中的起始位置
        var startIndex = this._rawUVs.Length * index;
        // 截取 UV
        Vector2[] unitUVs = new Vector2[_rawUVs.Length];
        Array.Copy(_uvs, startIndex, unitUVs, 0, _rawUVs.Length);
        return unitUVs;
    }

    // 更新
    public virtual void update() {
        // 更新网格
        _meshFilter.mesh.vertices = _positions;
        _meshFilter.mesh.normals = _normals;
        _meshFilter.mesh.uv = _uvs;
        // _meshFilter.mesh.triangles = _indices;

        // 更新包围盒
        _meshFilter.mesh.RecalculateBounds();
    }

    protected void refreshUnit(int index) {
        var unitTransform = this._unitTransforms[index];
        // 不可见无需更新
        if (!unitTransform.visible) {
            return;
        }
        this.updateUnitVertices(index, unitTransform);
    }

    // 塌缩顶点 (通常用于隐藏单元)
    protected virtual void collapseUnitVertices(int index, float x, float y, float z) {
        // 计算 Unit 在顶点数组中的起始位置
        var startIndex = this._rawPositions.Length * index;
        // 一次更改一个顶点的三个坐标
        for (var i = 0; i < this._rawPositions.Length; i = i + 1) {
            this._positions[startIndex + i] = new Vector3(x, y, z);
        }
    }

    // 更新顶点
    protected virtual void updateUnitVertices(int index, UnitTransform unitTransform) {
        // 计算 Unit 在顶点数组中的起始位置
        var startIndex = this._rawPositions.Length * index;

        var qx = unitTransform.rotation.x;
        var qy = unitTransform.rotation.y;
        var qz = unitTransform.rotation.z;
        var qw = unitTransform.rotation.w;

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

        var sx = unitTransform.scale.x;
        var sy = unitTransform.scale.y;
        var sz = unitTransform.scale.z;

        var m00 = (1 - (yy + zz)) * sx;
        var m01 = (xy + wz) * sx;
        var m02 = (xz - wy) * sx;

        var m04 = (xy - wz) * sy;
        var m05 = (1 - (xx + zz)) * sy;
        var m06 = (yz + wx) * sy;

        var m08 = (xz + wy) * sz;
        var m09 = (yz - wx) * sz;
        var m10 = (1 - (xx + yy)) * sz;

        var m12 = unitTransform.position.x;
        var m13 = unitTransform.position.y;
        var m14 = unitTransform.position.z;

        for (var i = 0; i < this._rawPositions.Length; i += 1) {

            var rawPosition = this._rawPositions[i];

            var vx = rawPosition.x;
            var vy = rawPosition.y;
            var vz = rawPosition.z;

            // 更新 positions
            _positions[startIndex + i] = new Vector3(
                m00 * vx + m04 * vy + m08 * vz + m12,
                m01 * vx + m05 * vy + m09 * vz + m13,
                m02 * vx + m06 * vy + m10 * vz + m14
            );

        }
    }

}