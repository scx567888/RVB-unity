using UnityEngine;

public sealed class ScxSpriteRenderer {
    
    private readonly ScxSpriteAtlas atlas;
    private readonly float pixelsPerUnit;
    private readonly Material materialTemplate;
    private readonly int batchCapacity;

    public ScxSpriteRenderer(ScxSpriteAtlas atlas, float pixelsPerUnit, Material materialTemplate, int batchCapacity) {
        this.atlas = atlas;
        this.pixelsPerUnit = pixelsPerUnit;
        this.materialTemplate = materialTemplate;
        this.batchCapacity = batchCapacity;
    }
    
}