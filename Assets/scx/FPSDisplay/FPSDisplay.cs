using UnityEngine;

namespace scx.FPSDisplay {
    public class FPSDisplay : MonoBehaviour {
        private float deltaTime = 0.0f;

        void Update() {
            // 平滑计算每帧时间
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI() {
            // 计算区域
            var w = Screen.width;
            var h = Screen.height;

            // 设置显示样式
            var position = new Rect(10, 10, w, h * 2f / 100);

            // 设置样式
            var style = new GUIStyle {
                alignment = TextAnchor.UpperLeft,
                fontSize = h * 2 / 50,
                normal = {
                    textColor = Color.green
                }
            };

            // 计算 FPS
            var fps = 1.0f / deltaTime;
            var text = $"{fps:0.} FPS";

            // 绘制在屏幕左上角
            GUI.Label(position, text, style);
        }
    }
}