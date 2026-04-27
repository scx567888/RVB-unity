using UnityEngine;

namespace scx.SpriteRenderer {
    public class ScxSpriteRenderUnit {

        private ScxSpriteRenderer spriteRenderer;
        public ScxSpriteInstanceData instance;

        public ScxSpriteRenderUnit(ScxSpriteRenderer spriteRenderer, ScxSpriteInstanceData instance) {
            this.spriteRenderer = spriteRenderer;
            this.instance = instance;

            // 默认初始化第一个 
            instance.geometry = this.spriteRenderer.getSpriteGeometryByIndex(0);
            instance.setVisible(false);
        }

        // UV
        public void setFrame(string name) {
            instance.geometry = this.spriteRenderer.getSpriteGeometryByName(name);
        }

        // UV
        public void setFrame(int index) {
            instance.geometry = this.spriteRenderer.getSpriteGeometryByIndex(index);
        }

        // 位置
        public void setPosition(float x, float y, float z) {
            instance.position.Set(x, y, z);
        }

        public Vector3 getPosition() {
            return this.instance.position;
        }

        public void translate(float dx, float dy, float dz) {
            instance.position.x += dx;
            instance.position.y += dy;
            instance.position.z += dz;
        }

        // 旋转
        public void setRotation(float x, float y, float z, float w) {
            instance.rotation.Set(x, y, z, w);
        }

        public void setRotationFromEuler(float x, float y, float z) {
            instance.rotation = Quaternion.Euler(x, y, z);
        }

        public Quaternion getRotation() {
            return instance.rotation;
        }

        public void rotate(float dx, float dy, float dz, float dw) {
            instance.rotation *= new Quaternion(dx, dy, dz, dw);
        }

        public void rotateFromEuler(float dx, float dy, float dz) {
            instance.rotation *= Quaternion.Euler(dx, dy, dz);
        }

        // 缩放
        public void setScale(float x, float y, float z) {
            instance.scale.Set(x, y, z);
        }

        public Vector3 getScale() {
            return instance.scale;
        }

        // 可见性
        public void setVisible(bool visible) {
            instance.setVisible(visible);
        }

        public bool getVisible() {
            return instance.visible;
        }

        // 销毁
        public void destroy() {
            this.spriteRenderer.destroyUnit(this);
            // 置空 防止后续外部调用
            this.spriteRenderer = null;
            this.instance = null;
        }
        
    }
}