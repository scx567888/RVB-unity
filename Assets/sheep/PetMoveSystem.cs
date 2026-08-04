using UnityEngine;

namespace sheep {
    public class PetMoveSystem {
        // 计算自主移动位移 
        // x 当前位置
        // y 当前位置
        public Vector2 calculateSelfMove(Pet pet) {
            var moveIntent = pet.moveIntent;
            var x = pet.x;
            var y = pet.y;


            // NONE 不移动
            if (moveIntent.moveMode == PetMoveMode.NONE) {
                return Vector2.zero;
            }

            // 固定逻辑帧下, moveSpeed 就是每帧移动距离
            var moveDistance = moveIntent.moveSpeed;

            // 方向模式
            if (moveIntent.moveMode == PetMoveMode.DIRECTION) {
                // 这里 directionX, directionY 已经归一化
                var direction = new Vector2(moveIntent.directionX, moveIntent.directionY);
                return direction * moveDistance;
            }

            // 目标模式
            if (moveIntent.moveMode == PetMoveMode.TARGET) {
                var offset = new Vector2(moveIntent.targetX - x, moveIntent.targetY - y);

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