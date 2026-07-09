using System.Collections.Generic;
using UnityEngine;

namespace scx.SpriteRenderer {
    /// ScxSpriteRenderer
    public sealed class ScxSpriteRenderer {
        private readonly ScxSpriteAtlas atlas;
        private Material material;
        private readonly int batchCapacity;

        private readonly GameObject node; // 持有节点
        private readonly Dictionary<int, ScxSpriteRenderBatch> batches; // 分块列表
        private int nextBatchID; // 分块 ID

        // 这里同时使用两种方式存储 空间换时间
        private readonly ScxSpriteRenderData[] renderDatas0;
        private readonly Dictionary<string, ScxSpriteRenderData> renderDatas1;
        private readonly string[] spriteNames;

        public ScxSpriteRenderer(
            ScxSpriteAtlas atlas,
            float pixelsPerUnit,
            Material materialTemplate,
            int batchCapacity
        ) {
            this.atlas = atlas;
            this.material = createMaterial(atlas.texture, materialTemplate);
            this.batchCapacity = batchCapacity;
            this.node = new GameObject("ScxSpriteRenderer");
            this.batches = new Dictionary<int, ScxSpriteRenderBatch>();
            this.nextBatchID = 0;

            // 初始化 渲染相关
            this.renderDatas0 = new ScxSpriteRenderData[atlas.sprites.Length];
            this.renderDatas1 = new Dictionary<string, ScxSpriteRenderData>();
            this.spriteNames = new string[atlas.sprites.Length];

            var textureWidth = atlas.texture.width;
            var textureHeight = atlas.texture.height;
            for (var i = 0; i < atlas.sprites.Length; i += 1) {
                var sprite = atlas.sprites[i];
                var renderData = sprite.createSpriteRenderData(textureWidth, textureHeight, pixelsPerUnit);
                renderDatas0[i] = renderData;
                renderDatas1[sprite.name()] = renderData;
                spriteNames[i] = sprite.name();
            }
        }

        // 适用于 URP 管线
        private static Material createMaterial(Texture2D texture, Material materialTemplate) {
            var material = Object.Instantiate(materialTemplate);

            material.SetTexture("_MainTex", texture);

            return material;
        }

        // ================ GameObject 模拟接口 ================

        public void setParent(GameObject parent) {
            this.node.transform.SetParent(parent.transform, false);
        }

        public GameObject getParent() {
            return this.node.transform.parent.gameObject;
        }

        public void setPosition(float x, float y, float z) {
            this.node.transform.position = new Vector3(x, y, z);
        }

        public Vector3 getPosition() {
            return this.node.transform.position;
        }

        public void setRotation(float x, float y, float z, float w) {
            this.node.transform.rotation = new Quaternion(x, y, z, w);
        }

        public void setRotationFromEuler(float x, float y, float z) {
            this.node.transform.rotation = Quaternion.Euler(x, y, z);
        }

        public Quaternion getRotation() {
            return this.node.transform.rotation;
        }

        public void setScale(float x, float y, float z) {
            this.node.transform.localScale = new Vector3(x, y, z);
        }

        public Vector3 getScale() {
            return this.node.transform.localScale;
        }

        public void setActive(bool active) {
            this.node.SetActive(active);
        }

        public bool getActive() {
            return this.node.activeSelf;
        }

        public void setLayer(string name) {
            this.node.layer = LayerMask.NameToLayer(name);
            // 处理子 layer
            foreach (var batch in this.batches) {
                batch.Value.setLayer(this.node.layer);
            }
        }

        public string getLayer() {
            return LayerMask.LayerToName(this.node.layer);
        }

        public void destroy() {
            foreach (var chunk in this.batches) {
                chunk.Value.destroy();
            }

            if (this.material != null) {
                Object.Destroy(this.material);
            }

            // 销毁 Node
            UnityEngine.Object.Destroy(this.node);
        }

        // ================ DynamicBatchRenderer 接口 ================

        // 材质
        public void setMaterialTemplate(Material materialTemplate) {
            var oldMaterial = this.material;
            this.material = createMaterial(atlas.texture, materialTemplate);

            foreach (var batch in this.batches) {
                batch.Value.setMaterial(this.material);
            }

            if (oldMaterial != null) {
                Object.Destroy(oldMaterial);
            }
        }

        // Unit
        public ScxSpriteRenderUnit createUnit() {
            // 寻找一个空位
            ScxSpriteRenderBatch renderBatch = null;
            int batchID = -1;
            int index = -1;

            // 先尝试寻找一个空位
            foreach (var batch in this.batches) {
                if (batch.Value.hasFree()) {
                    renderBatch = batch.Value;
                    batchID = batch.Key;
                    index = renderBatch.allocate();
                    break;
                }
            }

            // 没找到任何符合的 创建 (扩容)
            if (renderBatch == null) {
                renderBatch = new ScxSpriteRenderBatch(this.batchCapacity, this.node);
                renderBatch.setLayer(this.node.layer);
                renderBatch.setMaterial(this.material);
                batchID = this.nextBatchID++;
                index = renderBatch.allocate();
                this.batches.Add(batchID, renderBatch);
            }

            // 创建一个 SpriteRenderUnit
            var unit = new ScxSpriteRenderUnit(this, renderBatch, batchID, index);
            unit.setVisible(true);
            return unit;
        }

        public void destroyUnit(ScxSpriteRenderUnit unit) {
            // 获取分块
            var batch = this.batches[unit.batchID];
            // 回收 id
            batch.release(unit.index);
            // 设为不可见
            unit.setVisible(false);
            // 全部空闲 则回收整个 分块
            if (batch.allFree()) {
                batch.destroy();
                this.batches.Remove(unit.batchID);
            }
        }

        // 更新
        public void update() {
            foreach (var batch in this.batches) {
                batch.Value.update();
            }
        }

        public void sortFrame(string[] name) {
            for (int i = 0; i < name.Length; i++) {
                renderDatas0[i] = getSpriteByName(name[i]);
            }
        }

        public ScxSpriteRenderData getSpriteByIndex(int index) {
            return renderDatas0[index];
        }

        public ScxSpriteRenderData getSpriteByName(string name) {
            return renderDatas1[name];
        }

        public string[] getSpriteNames() {
            return spriteNames;
        }
    }
}