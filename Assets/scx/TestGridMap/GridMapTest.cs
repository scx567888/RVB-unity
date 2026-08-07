using scx.GridMap;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

public class GridMapSimpleTest : MonoBehaviour
{
    private const int MapWidth = 900;
    private const int MapHeight = 700;

    [SerializeField, Range(5, 200)]
    private int cellSize = 10;

    [SerializeField, Range(1, 800)]
    private int radius = 100;

    // 左边留出控制区域
    private readonly Rect mapRect = new Rect(260, 20, MapWidth, MapHeight);

    private Texture2D mapTexture;
    private Color32[] pixels;

    // 非泛型版本改为：private GridMap gridMap;
    private GridMap<GridCell> gridMap;

    private Vector2 circleCenter = new Vector2(450, 350);

    private int previousCellSize;
    private int previousRadius;

    private int hitCellCount;
    private double elapsedMilliseconds;

    private static readonly Color32 White = new Color32(255, 255, 255, 255);
    private static readonly Color32 GridColor = new Color32(220, 220, 220, 255);
    private static readonly Color32 HitColor = new Color32(255, 180, 180, 255);
    private static readonly Color32 CircleColor = new Color32(255, 0, 0, 255);

    private void Awake()
    {
        pixels = new Color32[MapWidth * MapHeight];

        mapTexture = new Texture2D(
            MapWidth,
            MapHeight,
            TextureFormat.RGBA32,
            false
        );

        mapTexture.filterMode = FilterMode.Point;
        mapTexture.wrapMode = TextureWrapMode.Clamp;

        RebuildGridMap();
    }

    private void Update()
    {
        // Unity 鼠标坐标 Y 向上，转换成 GUI 的 Y 向下
        Vector2 guiMouse = new Vector2(
            Input.mousePosition.x,
            Screen.height - Input.mousePosition.y
        );

        if (mapRect.Contains(guiMouse))
        {
            Vector2 newCenter = new Vector2(
                guiMouse.x - mapRect.x,
                guiMouse.y - mapRect.y
            );

            if (newCenter != circleCenter)
            {
                circleCenter = newCenter;
                Redraw();
            }
        }

        if (cellSize != previousCellSize)
        {
            RebuildGridMap();
        }
        else if (radius != previousRadius)
        {
            previousRadius = radius;
            Redraw();
        }
    }

    private void RebuildGridMap()
    {
        previousCellSize = cellSize;
        previousRadius = radius;

        gridMap = new GridMap<GridCell>(
            0,
            0,
            MapWidth,
            MapHeight,
            cellSize,
            (gridX, gridY, worldStartX, worldStartY, worldEndX, worldEndY) =>
                new GridCell(gridX, gridY, worldStartX, worldStartY, worldEndX, worldEndY)
        );

        Redraw();
    }

    private void Redraw()
    {
        ClearPixels();

        hitCellCount = 0;

        Stopwatch stopwatch = Stopwatch.StartNew();

        /*
         * 这里是唯一需要和你的 C# GridMap 对接的地方。
         */
        gridMap.findCellsInCircleScanLine(
            circleCenter.x,
            circleCenter.y,
            radius,
            cell =>
            {
                hitCellCount++;

                FillRect(
                    Mathf.FloorToInt(cell.worldStartX),
                    Mathf.FloorToInt(cell.worldStartY),
                    cellSize,
                    cellSize,
                    HitColor
                );

                // 回调是 Action<GridCell> 时，删除这一行。
                return false;
            }
        );

        stopwatch.Stop();
        elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

        // 后画网格，避免红色格子把网格线完全盖住
        DrawGrid();

        DrawCircle(
            Mathf.RoundToInt(circleCenter.x),
            Mathf.RoundToInt(circleCenter.y),
            radius
        );

        mapTexture.SetPixels32(pixels);
        mapTexture.Apply(false);
    }

    private void ClearPixels()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = White;
        }
    }

    private void DrawGrid()
    {
        for (int x = 0; x < MapWidth; x += cellSize)
        {
            DrawLine(x, 0, x, MapHeight - 1, GridColor);
        }

        for (int y = 0; y < MapHeight; y += cellSize)
        {
            DrawLine(0, y, MapWidth - 1, y, GridColor);
        }

        DrawLine(MapWidth - 1, 0, MapWidth - 1, MapHeight - 1, GridColor);
        DrawLine(0, MapHeight - 1, MapWidth - 1, MapHeight - 1, GridColor);
    }

    private void DrawCircle(int centerX, int centerY, int circleRadius)
    {
        int segments = Mathf.Clamp(circleRadius * 2, 64, 2048);

        int previousX = centerX + circleRadius;
        int previousY = centerY;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            int currentX = Mathf.RoundToInt(
                centerX + Mathf.Cos(angle) * circleRadius
            );

            int currentY = Mathf.RoundToInt(
                centerY + Mathf.Sin(angle) * circleRadius
            );

            DrawLine(
                previousX,
                previousY,
                currentX,
                currentY,
                CircleColor
            );

            previousX = currentX;
            previousY = currentY;
        }
    }

    private void FillRect(
        int startX,
        int startY,
        int width,
        int height,
        Color32 color
    )
    {
        int endX = Mathf.Min(startX + width, MapWidth);
        int endY = Mathf.Min(startY + height, MapHeight);

        startX = Mathf.Max(startX, 0);
        startY = Mathf.Max(startY, 0);

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                SetPixelTopLeft(x, y, color);
            }
        }
    }

    private void DrawLine(
        int x0,
        int y0,
        int x1,
        int y1,
        Color32 color
    )
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int stepX = x0 < x1 ? 1 : -1;
        int stepY = y0 < y1 ? 1 : -1;

        int error = dx - dy;

        while (true)
        {
            SetPixelTopLeft(x0, y0, color);

            if (x0 == x1 && y0 == y1)
            {
                break;
            }

            int error2 = error * 2;

            if (error2 > -dy)
            {
                error -= dy;
                x0 += stepX;
            }

            if (error2 < dx)
            {
                error += dx;
                y0 += stepY;
            }
        }
    }

    /// <summary>
    /// 使用和 HTML Canvas 一样的左上角坐标。
    /// Texture2D 内部是左下角坐标，因此这里翻转 Y。
    /// </summary>
    private void SetPixelTopLeft(int x, int y, Color32 color)
    {
        if (x < 0 || x >= MapWidth || y < 0 || y >= MapHeight)
        {
            return;
        }

        int textureY = MapHeight - 1 - y;
        int index = textureY * MapWidth + x;

        pixels[index] = color;
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(20, 20, 220, 210), "GridMap 测试");

        GUI.Label(new Rect(35, 55, 190, 25), $"网格大小：{cellSize}");

        cellSize = Mathf.RoundToInt(
            GUI.HorizontalSlider(
                new Rect(35, 82, 180, 20),
                cellSize,
                5,
                200
            )
        );

        GUI.Label(new Rect(35, 110, 190, 25), $"圆形半径：{radius}");

        radius = Mathf.RoundToInt(
            GUI.HorizontalSlider(
                new Rect(35, 137, 180, 20),
                radius,
                1,
                800
            )
        );

        GUI.Label(
            new Rect(35, 165, 190, 25),
            $"覆盖格子数：{hitCellCount}"
        );

        GUI.Label(
            new Rect(35, 190, 190, 25),
            $"耗时：{elapsedMilliseconds:F3} ms"
        );

        GUI.DrawTexture(
            mapRect,
            mapTexture,
            ScaleMode.StretchToFill,
            false
        );
    }

    private void OnDestroy()
    {
        if (mapTexture != null)
        {
            Destroy(mapTexture);
        }
    }
}