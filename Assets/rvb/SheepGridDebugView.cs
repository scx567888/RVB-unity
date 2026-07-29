using scx.GridMap;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace rvb.scripts
{
    /// <summary>
    /// Sheep 格子系统调试显示。
    ///
    /// 逻辑坐标：
    ///     logic X -> Unity X
    ///     logic Y -> Unity Z
    ///
    /// 使用方法：
    /// 1. 挂到场景中的任意 GameObject。
    /// 2. 确保 SheepMgr.inc 已初始化。
    /// 3. 确保 SheepMgr.gridMap 可以被访问。
    /// 4. 在 Scene 视图中打开 Gizmos。
    /// </summary>
    public class SheepGridDebugView : MonoBehaviour
    {
        [Header("坐标转换")]

        [Tooltip("逻辑坐标转换成 Unity 世界坐标的缩放比例")]
        [SerializeField]
        private float logicToWorldScale = 0.01f;

        [Tooltip("格子绘制在 Unity 世界中的高度")]
        [SerializeField]
        private float drawHeight = 0.05f;

        [Tooltip("整个逻辑地图在 Unity 世界中的位置偏移")]
        [SerializeField]
        private Vector3 worldOffset = Vector3.zero;

        [Header("格子显示")]

        [SerializeField]
        private bool drawGrid = true;

        [Tooltip("只显示有单位的格子")]
        [SerializeField]
        private bool drawOnlyNonEmptyCells;

        [Tooltip("显示格子坐标和单位数量")]
        [SerializeField]
        private bool drawCellLabels = true;

        [Tooltip("显示单位和所属格子中心之间的连线")]
        [SerializeField]
        private bool drawPetToCellLines;

        [Tooltip("显示单位位置")]
        [SerializeField]
        private bool drawPetPositions = true;

        [Header("查询范围显示")]

        [Tooltip("需要显示索敌范围的单位 ID。小于等于 0 时不显示")]
        [SerializeField]
        private int debugPetId;

        [Tooltip("使用单位配置中的 findR")]
        [SerializeField]
        private bool usePetFindRadius = true;

        [Tooltip("不使用单位 findR 时采用这个查询半径")]
        [SerializeField]
        private float customQueryRadius = 300f;

        [Tooltip("显示索敌圆")]
        [SerializeField]
        private bool drawQueryCircle = true;

        [Tooltip("显示索敌范围粗筛所覆盖的格子")]
        [SerializeField]
        private bool drawQueryCells = true;

        [Range(8, 128)]
        [SerializeField]
        private int circleSegments = 48;

        [Header("颜色")]

        [SerializeField]
        private Color emptyCellColor =
            new Color(1f, 1f, 1f, 0.15f);

        [SerializeField]
        private Color occupiedCellColor =
            new Color(1f, 0.3f, 0.3f, 0.85f);

        [SerializeField]
        private Color petPositionColor =
            new Color(1f, 0.9f, 0.1f, 1f);

        [SerializeField]
        private Color petToCellLineColor =
            new Color(1f, 0.8f, 0.1f, 0.5f);

        [SerializeField]
        private Color queryCircleColor =
            new Color(0f, 1f, 1f, 1f);

        [SerializeField]
        private Color queryCellColor =
            new Color(0f, 1f, 1f, 0.7f);

        private void OnDrawGizmos()
        {
            var mgr = SheepMgr.inc;

            if (mgr == null || mgr.gridMap == null)
            {
                return;
            }

            if (drawGrid)
            {
                DrawGrid(mgr);
            }

            if (debugPetId > 0)
            {
                DrawPetQuery(mgr, debugPetId);
            }
        }

        /// <summary>
        /// 绘制全部格子。
        /// </summary>
        private void DrawGrid(SheepMgr mgr)
        {
            mgr.gridMap.forEachCell(cell =>
            {
                if (cell == null)
                {
                    return;
                }

                int petCount = cell.petCounts[0]+cell.petCounts[1];

                if (drawOnlyNonEmptyCells && petCount == 0)
                {
                    return ;
                }

                Vector3 center = GetCellWorldCenter(cell);
                Vector3 size = GetCellWorldSize(cell);

                Gizmos.color = petCount > 0
                    ? occupiedCellColor
                    : emptyCellColor;

                Gizmos.DrawWireCube(center, size);

                if (petCount > 0)
                {
                    DrawCellPets(cell, center);
                }

#if UNITY_EDITOR
                if (drawCellLabels && petCount > 0)
                {
                    DrawCellLabel(cell, center);
                }
#endif

                return ;
            });
        }

        /// <summary>
        /// 绘制格子中的单位位置和连线。
        /// </summary>
        private void DrawCellPets(SheepCell cell, Vector3 cellCenter)
        {
            cell.forEachPet((pet) => {
                
                Vector3 petWorldPosition =
                    LogicToWorld(pet.posX, pet.posY);

                if (drawPetPositions)
                {
                    Gizmos.color = petPositionColor;

                    float radius = Mathf.Max(
                        0.025f,
                        logicToWorldScale * 3f
                    );

                    Gizmos.DrawSphere(
                        petWorldPosition + Vector3.up * 0.015f,
                        radius
                    );
                }

                if (drawPetToCellLines)
                {
                    Gizmos.color = petToCellLineColor;
                    Gizmos.DrawLine(cellCenter, petWorldPosition);
                }
                return false;
            });
          
        }

#if UNITY_EDITOR
        /// <summary>
        /// 显示格子坐标、红蓝单位数量和总数。
        /// </summary>
        private void DrawCellLabel(
            SheepCell cell,
            Vector3 center
        )
        {
            int redCount = 0;
            int blueCount = 0;

            cell.forEachPet((pet) => {
                
                switch (pet.camp) {
                    case SheepCamp.Red:
                        redCount++;
                        break;

                    case SheepCamp.Blue:
                        blueCount++;
                        break;
                }

                return false;
            });

            string text =
                $"[{cell.gridX}, {cell.gridY}]\n" +
                $"总:{redCount + blueCount} " +
                $"红:{redCount} 蓝:{blueCount}";

            Handles.Label(
                center + Vector3.up * 0.04f,
                text
            );
        }
#endif

        /// <summary>
        /// 绘制指定单位的索敌范围和查询格子。
        /// </summary>
        private void DrawPetQuery(
            SheepMgr mgr,
            int petId
        )
        {
            PetView pet = FindPetById(mgr, petId);

            if (pet == null)
            {
                return;
            }

            float queryRadius = customQueryRadius;

            if (usePetFindRadius && pet.conf != null)
            {
                queryRadius = pet.conf.findR;
            }

            if (queryRadius <= 0f)
            {
                return;
            }

            if (drawQueryCircle)
            {
                Gizmos.color = queryCircleColor;

                DrawLogicCircle(
                    pet.posX,
                    pet.posY,
                    queryRadius,
                    circleSegments
                );
            }

            if (!drawQueryCells)
            {
                return;
            }

            mgr.gridMap.findCellsInCircleScanLine(
                pet.posX,
                pet.posY,
                queryRadius,
                cell =>
                {
                    if (cell == null)
                    {
                        return false;
                    }

                    Gizmos.color = queryCellColor;

                    Gizmos.DrawWireCube(
                        GetCellWorldCenter(cell) +
                        Vector3.up * 0.01f,
                        GetCellWorldSize(cell)
                    );

                    return false;
                }
            );
        }

        /// <summary>
        /// 根据单位 ID 查找单位。
        /// </summary>
        private static PetView FindPetById(
            SheepMgr mgr,
            int petId
        )
        {
            if (mgr.pets == null)
            {
                return null;
            }

            foreach (var pet in mgr.pets)
            {
                
                    if (pet != null && pet.id == petId)
                    {
                        return pet;
                    }
                
            }

            return null;
        }

        /// <summary>
        /// 逻辑二维坐标转换为 Unity XZ 世界坐标。
        /// </summary>
        private Vector3 LogicToWorld(
            float logicX,
            float logicY
        )
        {
            return worldOffset + new Vector3(
                logicX * logicToWorldScale,
                drawHeight,
                logicY * logicToWorldScale
            );
        }

        /// <summary>
        /// 获取格子在 Unity 世界中的中心。
        /// </summary>
        private Vector3 GetCellWorldCenter(SheepCell cell)
        {
            float logicCenterX =
                (cell.worldStartX + cell.worldEndX) * 0.5f;

            float logicCenterY =
                (cell.worldStartY + cell.worldEndY) * 0.5f;

            return LogicToWorld(
                logicCenterX,
                logicCenterY
            );
        }

        /// <summary>
        /// 获取格子在 Unity 世界中的尺寸。
        /// </summary>
        private Vector3 GetCellWorldSize(SheepCell cell)
        {
            float logicWidth =
                cell.worldEndX - cell.worldStartX;

            float logicHeight =
                cell.worldEndY - cell.worldStartY;

            return new Vector3(
                logicWidth * logicToWorldScale,
                0.005f,
                logicHeight * logicToWorldScale
            );
        }

        /// <summary>
        /// 在 Unity XZ 平面绘制逻辑坐标圆。
        /// </summary>
        private void DrawLogicCircle(
            float logicCenterX,
            float logicCenterY,
            float logicRadius,
            int segments
        )
        {
            segments = Mathf.Max(8, segments);

            Vector3 center = LogicToWorld(
                logicCenterX,
                logicCenterY
            );

            float worldRadius =
                logicRadius * logicToWorldScale;

            Vector3 previous = center + new Vector3(
                worldRadius,
                0f,
                0f
            );

            for (int i = 1; i <= segments; i++)
            {
                float angle =
                    i * Mathf.PI * 2f / segments;

                Vector3 current = center + new Vector3(
                    Mathf.Cos(angle) * worldRadius,
                    0f,
                    Mathf.Sin(angle) * worldRadius
                );

                Gizmos.DrawLine(previous, current);
                previous = current;
            }
        }
    }
}