public class SpriteRenderUnit : RenderUnit {
    
    public SpriteRenderUnit(SpriteDynamicBatchRenderer dynamicBatchRenderer, BatchRenderer batchRenderer, int chunkID, int index) :
        base(dynamicBatchRenderer, batchRenderer, chunkID, index) {
    }

    // UV
    public void setFrame(string name) {
        var uvs = this.spriteDynamicBatchRenderer().getUVsByFrameName(name);
        this.batchRenderer.setUnitUVs(this.index, uvs);
    }
    
    // UV
    public void setFrame(int index) {
        var uvs = this.spriteDynamicBatchRenderer().getUVsByFrameIndex(index);
        this.batchRenderer.setUnitUVs(this.index, uvs);
    }

    public string getFrame() {
        return null;
    }

    private SpriteDynamicBatchRenderer spriteDynamicBatchRenderer() {
        return this.dynamicBatchRenderer as SpriteDynamicBatchRenderer;
    }
    
}