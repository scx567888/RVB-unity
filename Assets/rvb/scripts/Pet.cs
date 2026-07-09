using scx.SpriteRenderer;

public class Pet {
    public ScxSpriteRenderUnit renderUnit;

    public int frameIndex;

    public Pet(ScxSpriteRenderUnit renderUnit, int frameIndex) {
        this.renderUnit = renderUnit;
        this.frameIndex = frameIndex;
    }
}