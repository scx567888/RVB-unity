using System;
using System.Numerics;
using scx.SpriteRenderer;

namespace sheep {
    public class Pet {
        // 唯一真实位置 X
        public float x;

        // 唯一真实位置 Y
        public float y;

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

        // ********************* 碰撞移动相关 **************************
        
        // 碰撞半径
        public float collideR;

        // 渲染器挂载相关 逻辑层应无视
        public ScxSpriteRenderUnit scxSpriteRenderUnit;

        public void action(SheepWorld sheepWorld) {
            // 1, 执行逻辑
            PetLogic.INSTANCE.tick(this, sheepWorld);

            // 2, 计算自主位移
            var selfMove = calculateSelfMove();

            // 3, 计算碰撞位移
            var collisionMove = calculateCollisionMove(selfMove);

            // 3, 应用位移
            this.x += collisionMove.X;
            this.y += collisionMove.Y;
        }

        // 计算自主移动位移
        public Vector2 calculateSelfMove() {
            // NONE 不移动
            if (this.moveMode == PetMoveMode.NONE) {
                return Vector2.Zero;
            }

            // 计算移动距离 这里因为固定帧 所以 我们 的速度 实际上 就是 位移距离
            float moveDistance = moveSpeed;

            // 向量模式
            if (this.moveMode == PetMoveMode.DIRECTION) {
                // 这里 directionX, directionY 一定是归一化后的
                var direction = new Vector2(this.directionX, this.directionY);
                return direction * moveDistance;
            }

            // 目标模式
            if (this.moveMode == PetMoveMode.TARGET) {
                var offset = new Vector2(
                    targetX - x,
                    targetY - y
                );

                float distance = offset.Length();

                // 已到达, 或者这一帧可以直接到达
                if (distance <= moveDistance) {
                    return offset;
                }

                var direction = offset / distance;
                return direction * moveDistance;
            }

            // 默认兜底
            return Vector2.Zero;
        }

        // 根据自主位移计算碰撞位移
        private Vector2 calculateCollisionMove(Vector2 selfMove) {
            return selfMove;
        }
    }
}