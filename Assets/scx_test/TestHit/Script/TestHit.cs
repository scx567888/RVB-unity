using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TestHit : MonoBehaviour
{
    public SpriteAtlas atlas;
    public Material hitMaterial;

    private SpriteDynamicBatchRenderer spriteDynamicBatchRenderer;
    private List<ObjHit> spriteRenderUnits;
    private string[] list;

    // 闪烁控制
    private Queue<int> blinkQueue = new Queue<int>(); // 队列记录待闪烁次数
    private bool isBlinking = false;
    private int blinkStep = 0;
    private int totalBlinkSteps = 6; // 一次完整闪烁阶段数
    private int framesPerStep = 2;   // 每阶段持续帧数
    private int stepFrameCounter = 0;

    void Awake()
    {
        Application.targetFrameRate = 60; // 目标帧率 60 FPS
    }

    void Start()
    {
        this.spriteRenderUnits = new List<ObjHit>();
        this.spriteDynamicBatchRenderer = new SpriteDynamicBatchRenderer(5000, this.atlas, 30, null);
        this.list = this.spriteDynamicBatchRenderer.getFrameNames();

        this.spriteDynamicBatchRenderer.setParent(this.gameObject);

        var spriteRenderUnit = this.spriteDynamicBatchRenderer.createUnit();
        spriteRenderUnit.setVisible(true);
        spriteRenderUnit.setPosition(0,0,0);
        spriteRenderUnit.setFrame(this.list[0]);

        var obj = new ObjHit(spriteRenderUnit, Random.Range(0, this.list.Length));
        this.spriteRenderUnits.Add(obj);
    }

    void Update()
    {
        // 按空格触发闪烁，加入队列
        if (Input.GetKeyDown(KeyCode.Space))
        {
            blinkQueue.Enqueue(1); // 每次空格触发一次闪烁
        }

        if (!isBlinking && blinkQueue.Count > 0)
        {
            blinkQueue.Dequeue(); // 开始下一次闪烁
            isBlinking = true;
            blinkStep = 0;
            stepFrameCounter = 0;
        }

        if (isBlinking)
        {
            stepFrameCounter++;

            if (blinkStep % 2 == 0) // 偶数阶段亮
            {
                this.spriteDynamicBatchRenderer.setMaterial(hitMaterial);
            }
            else // 奇数阶段暗
            {
                this.spriteDynamicBatchRenderer.resetMaterial();
            }

            if (stepFrameCounter >= framesPerStep)
            {
                stepFrameCounter = 0;
                blinkStep++;

                if (blinkStep >= totalBlinkSteps)
                {
                    // 一次闪烁完成
                    isBlinking = false;
                    this.spriteDynamicBatchRenderer.resetMaterial();
                }
            }
        }

        this.spriteDynamicBatchRenderer.update();
    }
}
