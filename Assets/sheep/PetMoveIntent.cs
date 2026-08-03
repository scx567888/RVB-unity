using UnityEngine;

namespace sheep {
    /// 自主移动意图
    public class PetMoveIntent {
        // ********************** 自主移动相关 ***********************

        // 自主移动模式
        public PetMoveMode moveMode;

        // 自主移动速度 (每帧/像素)
        public float moveSpeed;

        // 自主移动向量, 归一化方向 两个分量范围是 [-1, 1], 整体长度为 1.
        public float directionX;
        public float directionY;

        // 目标位置
        public float targetX;
        public float targetY;
        
        // 计算自主移动位移 
        // x 当前位置
        // y 当前位置
        public Vector2 calculateSelfMove(float x,float y) {
            // NONE 不移动
            if (this.moveMode == PetMoveMode.NONE) {
                return Vector2.zero;
            }

            // 固定逻辑帧下, moveSpeed 就是每帧移动距离
            var moveDistance = this.moveSpeed;

            // 方向模式
            if (this.moveMode == PetMoveMode.DIRECTION) {
                // 这里 directionX, directionY 已经归一化
                var direction = new Vector2(this.directionX, this.directionY);
                return direction * moveDistance;
            }

            // 目标模式
            if (this.moveMode == PetMoveMode.TARGET) {
                var offset = new Vector2(targetX - x, targetY - y);

                var distance = offset.magnitude;

                // 已到达, 或者这一帧可以直接到达
                if (distance <= moveDistance) {
                    return offset;
                }

                var direction = offset / distance;
                return direction * moveDistance;
            }

            // 默认兜底
            return Vector2.zero;
        }
        
    }
}