using System.Collections.Generic;
using System.Threading.Tasks;
using rvb.utils;
using scx.SpriteRenderer;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class RVB : MonoBehaviour {
    // 贴图
    public Texture2D texture;

    // json
    public TextAsset json;
    
    // 主材质
    public Material mainMaterial;
    
    // 高亮材质
    public Material highlightMaterial;

    // 渲染器
    private ScxSpriteRenderer scxSpriteRenderer;

    // Pet 列表
    private List<Pet> pets;

    private string[] spriteNames;
    
    // 动画播放帧率
    [SerializeField]
    private float animationFPS = 60f;

    // 动画计时器
    private float animationTimer = 0f;
    
    [SerializeField]
    private bool enableRotate = true;

    [SerializeField]
    private float rotateSpeed = 10f;

    void Start() {
        // 创建渲染器
        var scxSpriteAtlas = SheepSpriteAtlasLoader.load(texture,json.text);
        this.scxSpriteRenderer = new ScxSpriteRenderer(scxSpriteAtlas, 200, mainMaterial, 5000);
        this.spriteNames = this.scxSpriteRenderer.getSpriteNames();

        this.scxSpriteRenderer.setParent(this.gameObject);

        // 创建
        this.pets = new List<Pet>();
        for (var j = 0; j < 10000 * 5; j++) {
            var spriteRenderUnit = this.scxSpriteRenderer.createUnit();
            spriteRenderUnit.setVisible(true);
            spriteRenderUnit.setPosition(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50));

            spriteRenderUnit.setFrame(this.spriteNames[0]);
            // 给每个单元一个随机起始帧索引
            var obj = new Pet(spriteRenderUnit, Random.Range(0, this.spriteNames.Length));
            this.pets.Add(obj);
        }
    }

    // 计数器
    private int time = 0;

    void Update() {
        UpdateRotate();

        // 测试更换材质
        if (time == 500) {
            this.scxSpriteRenderer.setMaterialTemplate(highlightMaterial);
        }

        if (time == 1000) {
            this.scxSpriteRenderer.setMaterialTemplate(mainMaterial);
        }

        time++;

        // 按指定 FPS 播放动画
        UpdateAnimationByFps(animationFPS);
       

        this.scxSpriteRenderer.update();
    }
    
    private void UpdateAnimationByFps(float fps) {
        if (fps <= 0f) {
            return;
        }

        animationTimer += Time.deltaTime;

        float frameInterval = 1f / fps;

        if (animationTimer < frameInterval) {
            return;
        }

        int step = Mathf.FloorToInt(animationTimer / frameInterval);
        animationTimer -= step * frameInterval;

        // 多核并行执行方式
        Parallel.For(0, pets.Count, i => {
            var pet = pets[i];

            pet.frameIndex += step;

            int index = pet.frameIndex % spriteNames.Length;
            pet.renderUnit.setFrame(spriteNames[index]);
        });

        // // 传统方式
        // foreach (var pet in this.pets) {
        //
        //     pet.frameIndex += step;
        //
        //     int index = pet.frameIndex % spriteNames.Length;
        //     pet.renderUnit.setFrame(spriteNames[index]);
        // }
        
    }
    
    private void UpdateRotate() {
        if (!enableRotate) {
            return;
        }

        var euler = transform.eulerAngles;
        euler.y += rotateSpeed * Time.deltaTime;
        transform.eulerAngles = euler;
    }
    
}