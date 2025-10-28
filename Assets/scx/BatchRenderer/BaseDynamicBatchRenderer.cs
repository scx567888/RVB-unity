using System.Collections.Generic;
using UnityEngine;

// 移植完成
public abstract class BaseDynamicBatchRenderer<U> : DynamicBatchRenderer<U> where U : RenderUnit {

    // 单个分块的容量
    protected readonly int _chunkCapacity;

    // 原始网格
    protected readonly Mesh _rawMesh;

    // 默认材质
    protected readonly Material _material;

    // 分块列表
    protected readonly Dictionary<int, SlotMeshMergeBatchRenderer> _chunks;

    // 持有节点
    protected readonly GameObject _node;

    // 分块 ID
    private int _nextChunkID;

    protected BaseDynamicBatchRenderer(int chunkCapacity, Mesh rawMesh, Material material) {
        this._chunkCapacity = chunkCapacity;
        this._rawMesh = rawMesh;
        this._material = material;
        this._chunks = new Dictionary<int, SlotMeshMergeBatchRenderer>();
        this._node = new GameObject();
        this._nextChunkID = 0;
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
        // 递归处理子 layer
        foreach (var chunk in this._chunks) {
            chunk.Value.setLayer(name);
        }
    }

    public string getLayer() {
        return LayerMask.LayerToName(this._node.layer);
    }

    public void destroy() {
        foreach (var chunk in this._chunks) {
            chunk.Value.destroy();
        }

        // 销毁 Node
        Object.Destroy(this._node);
    }

    // ================ DynamicBatchRenderer 接口 ================

    // 材质
    public void setMaterial(Material material){
        foreach (var chunk in this._chunks) {
            chunk.Value.setMaterial(material);
        }
    }

    public Material getMaterial()  {
        return null;
    }

    // Unit
    public U createUnit()  {
        // 寻找一个空位
        SlotMeshMergeBatchRenderer  batchRenderer= null;
        int chunkID=-1;
        int index = -1;

        // 先尝试寻找一个空位
        foreach (var chunk  in this._chunks) {
            if (chunk.Value.hasFree()) {
                batchRenderer = chunk.Value;
                chunkID = chunk.Key;
                index = chunk.Value.allocate();
                break;
            }
        }

        // 没找到任何符合的 创建 (扩容)
        if (batchRenderer == null) {
            batchRenderer = new SlotMeshMergeBatchRenderer(this._chunkCapacity, this._rawMesh, this._material);
            batchRenderer.setParent(this._node);
            batchRenderer.setLayer(this.getLayer());
            chunkID = this._nextChunkID++;
            index = batchRenderer.allocate();
            this._chunks.Add(chunkID, batchRenderer);
        }

        return this.createUnit0(batchRenderer, chunkID, index);
    }

    public void destroyUnit(RenderUnit unit)  {
        // 获取分块
        var chunk  = this._chunks[unit.chunkID];
        // 回收 id
        chunk.release(unit.index);
        // 设为不可见
        unit.setVisible(false);
        // 全部空闲 则回收整个 分块
        if (chunk.allFree()) {
            chunk.destroy();
            this._chunks.Remove(unit.chunkID);
        }
    }

    // 更新
    public void update() {
        foreach (var chunk in this._chunks) {
            chunk.Value.update();
        }
    }

    protected abstract  U createUnit0(BatchRenderer batchRenderer,int chunkID,int index);
    
}