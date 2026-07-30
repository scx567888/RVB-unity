using System;
using scx.SpriteRenderer;
using UnityEngine;

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

        // 同时碰撞到多少个单位以后, 停止自主移动
        public int collideNotMoveNum;

        // 发生碰撞时, 自主位移保留比例
        public float collideMoveScale;

        // 碰撞推开位移的比例
        public float collideElasticityScale;

        // ********************* 渲染器挂载相关 **********************

        // 逻辑层不应使用此字段
        public ScxSpriteRenderUnit scxSpriteRenderUnit;

        public void action(SheepWorld sheepWorld) {
            // 1. 执行逻辑, 更新自主移动意图
            PetLogic.INSTANCE.tick(this, sheepWorld);

            // 2. 计算自主位移
            var selfMove = calculateSelfMove();

            // 3. 根据碰撞修正最终位移
            var collisionMove = calculateCollisionMove(
                selfMove,
                sheepWorld
            );

            // 4. 应用最终位移
            this.x += collisionMove.x;
            this.y += collisionMove.y;
        }

        // 计算自主移动位移
        public Vector2 calculateSelfMove() {
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

        // 根据自主位移计算碰撞修正后的最终位移
        private Vector2 calculateCollisionMove(Vector2 selfMove, SheepWorld sheepWorld) {
            return selfMove;
        }
    }
}