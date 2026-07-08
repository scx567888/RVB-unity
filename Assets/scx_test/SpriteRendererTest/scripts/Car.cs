using scx.SpriteRenderer;

public class Car {

    public ScxSpriteRenderUnit renderUnit;

    public int frameIndex;

    public Car(ScxSpriteRenderUnit renderUnit, int frameIndex) {
        this.renderUnit = renderUnit;
        this.frameIndex = frameIndex;
    }
    
}