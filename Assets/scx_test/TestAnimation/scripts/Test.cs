using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour {
    public SpriteAtlas atlas;

    private SpriteDynamicBatchRenderer spriteDynamicBatchRenderer;
    private List<Obj> spriteRenderUnits;

    private string[] list;

    // Start is called before the first frame update
    void Start() {
        this.spriteRenderUnits = new List<Obj>();
        this.spriteDynamicBatchRenderer = new SpriteDynamicBatchRenderer(5000, this.atlas, 300, null);
        this.list = this.spriteDynamicBatchRenderer.getFrameNames();

        this.spriteDynamicBatchRenderer.setParent(this.gameObject);

        for (var j = 0; j < 10000 * 5; j++) {
            var spriteRenderUnit = this.spriteDynamicBatchRenderer.createUnit();
            spriteRenderUnit.setVisible(true);
            spriteRenderUnit.setPosition(RandomFloat(-50, 50), RandomFloat(-50, 50), RandomFloat(-50, 50));

            spriteRenderUnit.setFrame(this.list[0]);
            // 给每个单元一个随机起始帧索引
            var obj = new Obj(spriteRenderUnit, RandomFloat(0, this.list.Length));
            this.spriteRenderUnits.Add(obj);
        }
    }


    // Update is called once per frame
    void Update() {
        // 绕 Y 轴旋转整个节点
        var euler = transform.eulerAngles;
        euler.y += 10f * Time.deltaTime;
        transform.eulerAngles = euler;

        foreach (var spriteRenderUnit in this.spriteRenderUnits) {
            // 每个单元的帧索引累加
            spriteRenderUnit.frameIndex++;
            var frameName = this.list[spriteRenderUnit.frameIndex % this.list.Length];
            spriteRenderUnit.spriteRenderUnit.setFrame(frameName);
        }

        this.spriteDynamicBatchRenderer.update();
    }

    public static int RandomFloat(int min, int max) {
        return Random.Range(min, max);
    }

}