using System;
using UnityEngine;

namespace rvb.scripts
{
    public class SheepDebugPanel : MonoBehaviour
    {
        private bool isVisible=true;
        private Rect windowRect = new Rect(20, 20, 420, 480);
        private Vector2 scrollPosition;

        private SheepRoleType[] roleTypes;

        private void Awake()
        {
            roleTypes =
                (SheepRoleType[])Enum.GetValues(typeof(SheepRoleType));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                isVisible = !isVisible;
            }
        }

        private void OnGUI()
        {

            if (!isVisible)
            {
                return;
            }

            windowRect = GUI.Window(
                GetInstanceID(),
                windowRect,
                DrawWindow,
                "兵种数量"
            );

        }

        private void DrawWindow(int windowId)
        {
            SheepMgr mgr = SheepMgr.inc;

            if (mgr == null)
            {
                GUILayout.Label("SheepMgr 尚未初始化");
                GUI.DragWindow();
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("兵种", GUILayout.Width(160));
            GUILayout.Label("红方", GUILayout.Width(80));
            GUILayout.Label("蓝方", GUILayout.Width(80));
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (SheepRoleType roleType in roleTypes)
            {
                // Count 只是枚举长度标记，不是真正兵种
                if (roleType == SheepRoleType.Count)
                {
                    continue;
                }

                int redCount = mgr.petCounts[(int)SheepCamp.Red][(int)roleType];
                

                int blueCount = mgr.petCounts[(int)SheepCamp.Blue][(int)roleType];

                GUILayout.BeginHorizontal();

                GUILayout.Label(
                    roleType.ToString(),
                    GUILayout.Width(160)
                );

                GUILayout.Label(
                    redCount.ToString(),
                    GUILayout.Width(80)
                );

                GUILayout.Label(
                    blueCount.ToString(),
                    GUILayout.Width(80)
                );

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.Space(8);
            GUILayout.Label("按 F8 关闭");

            // 允许拖动窗口标题区域
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 24));
        }
    }
}