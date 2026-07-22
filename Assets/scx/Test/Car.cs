using scx.SpriteRenderer;

namespace scx.Test {
    public class Car {
        public ScxSpriteRenderUnit renderUnit;

        public int frameIndex;

        public Car(ScxSpriteRenderUnit renderUnit, int frameIndex) {
            this.renderUnit = renderUnit;
            this.frameIndex = frameIndex;
        }
    }
}