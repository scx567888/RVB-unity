using scx.SpriteRenderer;

namespace sheep {
    /// Pet 对应的渲染层句柄。
    /// 只能由渲染层读取和修改，逻辑层不得使用。
    public class PetRenderHandle {
        // 渲染器句柄 (逻辑层不应使用此字段)
        public ScxSpriteRenderUnit scxSpriteRenderUnit;

        // 渲染器 X 用于插值 (逻辑层不应使用此字段)
        public float lastX;

        // 渲染器 Y 用于插值 (逻辑层不应使用此字段)
        public float lastY;
    }
}