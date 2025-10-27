using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;

    void Update()
    {
        // 平滑计算每帧时间
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width;
        int h = Screen.height;

        // 设置显示样式
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(10, 10, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = Color.white;

        // 计算 FPS
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} FPS", fps);

        // 绘制在屏幕左上角
        GUI.Label(rect, text, style);
    }
}