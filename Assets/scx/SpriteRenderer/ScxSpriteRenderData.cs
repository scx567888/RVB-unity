using scx.SpriteRenderer;
using UnityEngine;

/// 运行时渲染缓存数据。
///
/// 所有输入都基于统一规范:
/// - atlasRect: 图集像素坐标，左下原点
/// - sourceRect: 原图像素坐标，左下原点
/// - pivot: 基于 sourceSize 的归一化锚点
///
/// 该类会预计算两类数据:
/// 1. UV（图集采样范围）
/// 2. 以 pivot 为局部原点的四边形顶点坐标
public sealed class ScxSpriteRenderData {
    // UV 对应关系:
    // uv0 -> 左下
    // uv1 -> 右下
    // uv2 -> 左上
    // uv3 -> 右上
    public readonly Vector2 uv0;
    public readonly Vector2 uv1;
    public readonly Vector2 uv2;
    public readonly Vector2 uv3;

    // 局部顶点（单位：世界单位）
    // 顶点顺序:
    // p0 -> 左下
    // p1 -> 右下
    // p2 -> 左上
    // p3 -> 右上
    //
    // 坐标均以 sprite 的 pivot 为局部原点。
    public readonly float p0x;
    public readonly float p0y;
    public readonly float p0z;

    public readonly float p1x;
    public readonly float p1y;
    public readonly float p1z;

    public readonly float p2x;
    public readonly float p2y;
    public readonly float p2z;

    public readonly float p3x;
    public readonly float p3y;
    public readonly float p3z;

    public ScxSpriteRenderData(
        ScxSprite sprite,
        int textureWidth,
        int textureHeight,
        float pixelsPerUnit
    ) {
        // -----------------------------
        // 1) 计算 UV
        // -----------------------------
        // atlasRect 基于图集左下角为原点，因此可以直接映射到 0~1 UV。
        var rect = sprite.atlasRect;

        var uMin = (float)rect.x / textureWidth;
        var vMin = (float)rect.y / textureHeight;
        var uMax = (float)(rect.x + rect.width) / textureWidth;
        var vMax = (float)(rect.y + rect.height) / textureHeight;

        uv0 = new Vector2(uMin, vMin); // 左下
        uv1 = new Vector2(uMax, vMin); // 右下
        uv2 = new Vector2(uMin, vMax); // 左上
        uv3 = new Vector2(uMax, vMax); // 右上

        // -----------------------------
        // 2) 计算 pivot 在原图中的像素位置
        // -----------------------------
        // pivot 是基于原图尺寸 sourceSize 的归一化锚点:
        // (0,0)=原图左下角, (1,1)=原图右上角
        var pivotPixelX = sprite.pivot.x * sprite.sourceSize.x;
        var pivotPixelY = sprite.pivot.y * sprite.sourceSize.y;

        // -----------------------------
        // 3) 计算裁边后有效区域，相对 pivot 的局部边界（像素）
        // -----------------------------
        // sourceRect 表示“有效图像区域在原图中的位置”，其坐标原点也是左下角。
        //
        // left/right/bottom/top 的含义:
        // - left   : 有效区域左边界，相对 pivot 的偏移
        // - right  : 有效区域右边界，相对 pivot 的偏移
        // - bottom : 有效区域下边界，相对 pivot 的偏移
        // - top    : 有效区域上边界，相对 pivot 的偏移
        //
        // 然后再除以 pixelsPerUnit，转换到世界单位。
        var left = (sprite.sourceRect.x - pivotPixelX) / pixelsPerUnit;
        var right = (sprite.sourceRect.x + sprite.sourceRect.width - pivotPixelX) / pixelsPerUnit;
        var bottom = (sprite.sourceRect.y - pivotPixelY) / pixelsPerUnit;
        var top = (sprite.sourceRect.y + sprite.sourceRect.height - pivotPixelY) / pixelsPerUnit;

        // -----------------------------
        // 4) 生成局部四边形顶点
        // -----------------------------
        // 顶点顺序固定为:
        // p0 左下, p1 右下, p2 左上, p3 右上
        p0x = left;
        p0y = bottom;
        p0z = 0f;
        
        p1x = right;
        p1y = bottom;
        p1z = 0f;
        
        p2x = left;
        p2y = top;
        p2z = 0f;
        
        p3x = right;
        p3y = top;
        p3z = 0f;
    }
}