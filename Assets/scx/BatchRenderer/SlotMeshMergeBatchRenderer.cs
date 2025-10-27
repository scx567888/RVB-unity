using System.Collections.Generic;
using UnityEngine;

// 移植完成
public class SlotMeshMergeBatchRenderer : MeshMergeBatchRenderer {

    private Stack<int> _free;
    private bool _hasChange;

    public SlotMeshMergeBatchRenderer(int capacity, Mesh rawMesh, Material material) :
        base(capacity, rawMesh, material) {
        this._free = new Stack<int>(capacity);
        for (var i = 0; i < capacity; i++) {
            this._free.Push(i);
        }
        this._hasChange = false;
    }

    public int allocate() {
        return this._free.Pop();
    }

    public void release(int index) {
        this._free.Push(index);
    }

    public bool hasFree() {
        return this._free.Count > 0;
    }

    public bool allFree() {
        return this._free.Count == this._capacity;
    }

    public override void update() {
        if (!this._hasChange) {
            return;
        }

        base.update();
        this._hasChange = false;
    }

    public override void setMaterial(Material material) {
        base.setMaterial(material);
        this._hasChange = true;
    }

    public override void setUnitUVs(int index, Vector2[] uvs) {
        base.setUnitUVs(index, uvs);
        this._hasChange = true;
    }

    protected override void collapseUnitVertices(int index, float x, float y, float z) {
        base.collapseUnitVertices(index, x, y, z);
        this._hasChange = true;
    }

    protected override void updateUnitVertices(int index, UnitTransform unitTransform) {
        base.updateUnitVertices(index, unitTransform);
        this._hasChange = true;
    }

}