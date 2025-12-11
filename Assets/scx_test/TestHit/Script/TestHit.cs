using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public class TestHit : MonoBehaviour
{
    public SpriteAtlas atlas;
    public Material hitMaterial;

    private SpriteDynamicBatchRenderer spriteDynamicBatchRenderer;
    private List<ObjHit> spriteRenderUnits;

    private string[] list;
    
    void Awake()
    {
        Application.targetFrameRate = 60; // 目标帧率 60 FPS
    }

    // Start is called before the first frame update
    void Start() {
        this.spriteRenderUnits = new List<ObjHit>();
        this.spriteDynamicBatchRenderer = new SpriteDynamicBatchRenderer(5000, this.atlas, 30, null);
        this.list = this.spriteDynamicBatchRenderer.getFrameNames();

        this.spriteDynamicBatchRenderer.setParent(this.gameObject);

        
            var spriteRenderUnit = this.spriteDynamicBatchRenderer.createUnit();
            spriteRenderUnit.setVisible(true);
            spriteRenderUnit.setPosition(0,0,0);

            spriteRenderUnit.setFrame(this.list[0]);
            
            // 给每个单元一个随机起始帧索引
            var obj = new ObjHit(spriteRenderUnit, RandomFloat(0, this.list.Length));
            
            this.spriteRenderUnits.Add(obj);
        
    }

    // Update is called once per frame
    void Update() {
        int frame = Time.frameCount; // 当前帧数
        int blinkPhase = (frame / 2) % 2; // 0 = 亮  1 = 暗

        // 测试更换材质
        if (blinkPhase == 0) {
            this.spriteDynamicBatchRenderer.setMaterial(hitMaterial);
        }
        else {
            this.spriteDynamicBatchRenderer.resetMaterial();
        }

        this.spriteDynamicBatchRenderer.update();
    }

    public static int RandomFloat(int min, int max) {
        return Random.Range(min, max);
    }
}
