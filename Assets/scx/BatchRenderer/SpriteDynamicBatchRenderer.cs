using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteDynamicBatchRenderer : BaseDynamicBatchRenderer<SpriteRenderUnit> {
    
    private static readonly Material BASE_MATERIAL = Resources.Load<Material>("scx/Unlit-Alpha");

    private SpriteAtlas _rawSpriteAtlas;
    private Dictionary<string, Vector2[]> _uvs;
    private string[] _frameNames;

    public SpriteDynamicBatchRenderer(int batchCapacity, SpriteAtlas rawSpriteAtlas, int pixelsToUnit, Material material) : base(
        batchCapacity,
        createMash(rawSpriteAtlas, pixelsToUnit),
        // 没有则使用默认贴图
        createMaterial(rawSpriteAtlas)) {
        this._rawSpriteAtlas = rawSpriteAtlas;

        //创建 UVs
        this._uvs = new Dictionary<string, Vector2[]>();

        // 获取所有帧
        var sprites = new Sprite[rawSpriteAtlas.spriteCount];
        rawSpriteAtlas.GetSprites(sprites);

        // 转换为 Float32Array 存储
        foreach (var sprite in sprites) {
            Vector2[] uvs = sprite.uv;
            this._uvs[sprite.name.Replace("(Clone)", "")] = new[] {
                uvs[2], // 左下
                uvs[3], // 右下
                uvs[0], // 左上
                uvs[1], // 右上
            };
        }

        this._frameNames = this._uvs.Keys.ToArray();
    }


    // 创建一个可以承载 精灵图的 4 变形网格
    static Mesh createMash(SpriteAtlas rawSpriteAtlas, int pixelsToUnit) {
        
        var frameNames = new Sprite[rawSpriteAtlas.spriteCount];
        rawSpriteAtlas.GetSprites(frameNames);
        var firstFrame = frameNames[0];

        // 获取精灵原始尺寸
        float width = firstFrame.rect.width;
        float height = firstFrame.rect.height;

        // 根据 pixelsToUnit 计算场景单位尺寸
        float halfWidth = width / pixelsToUnit / 2f;
        float halfHeight = height / pixelsToUnit / 2f;

        // 顶点 positions (x, y, z)
        Vector3[] positions = new Vector3[] 
        {
            new(-halfWidth, -halfHeight, 0), // 左下
            new(halfWidth, -halfHeight, 0), // 右下
            new(-halfWidth, halfHeight, 0), // 左上
            new(halfWidth, halfHeight, 0), // 右上
        };

        // pivot 是像素点
        // 计算偏移量
        var pivot = firstFrame.pivot;

        Vector2 pivotOffset = new Vector2(
            (pivot.x - width / 2f) / pixelsToUnit,
            (pivot.y - height / 2f) / pixelsToUnit
        );

        for (int i = 0; i < positions.Length; i++) {
            positions[i].x -= pivotOffset.x;
            positions[i].y -= pivotOffset.y;
        }


        // 创建法线
        var normals = new Vector3[] { new(0, 0, -1), new(0, 0, -1), new(0, 0, -1), new(0, 0, -1) };

        // 创建 UV
        var uvs = new Vector2[] { new(0, 0), new(1, 0), new(0, 1), new(1, 1), };

        // 创建索引
        var indices = new[] { 0, 3, 1, 3, 0, 2, };

        // 创建网格
        var mesh = new Mesh();
        mesh.vertices = positions;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = indices;
        return mesh;
    }

    // 适用于 URP 管线
    static Material createMaterial(SpriteAtlas spriteAtlas) {
        var frameNames = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(frameNames);

        var texture = frameNames[0].texture;

        var material = Object.Instantiate(BASE_MATERIAL);

        material.SetTexture("_MainTex", texture);

        return material;
    }

    public Vector2[] getUVsByFrameName(string name) {
        return this._uvs[name];
    }

    public string[] getFrameNames() {
        return this._frameNames;
    }

    protected override SpriteRenderUnit createUnit0(BatchRenderer batchRenderer, int chunkID, int index) {
        // 创建一个 SpriteRenderUnit
        var unit = new SpriteRenderUnit(this, batchRenderer, chunkID, index);
        unit.setVisible(true);
        return unit;
    }

}