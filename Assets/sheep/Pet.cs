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
        
        // 同时碰撞到多少人以后，停止自主移动
        public float collideNotMoveNum;

        // 发生碰撞时，自主位移保留比例
        public float collideMoveScale;
        
        // 碰撞推开位移的比例
        public float collideElasticityScale;

        // *********************  渲染器挂载相关 逻辑层应无视 **********************
        
        // 挂载的渲染器
        public ScxSpriteRenderUnit scxSpriteRenderUnit;

        public void action(SheepWorld sheepWorld) {
            // 1, 执行逻辑
            PetLogic.INSTANCE.tick(this, sheepWorld);

            // 2, 计算自主位移
            var selfMove = calculateSelfMove();

            // 3, 计算碰撞位移
            var collisionMove = calculateCollisionMove(selfMove, sheepWorld);

            // 3, 应用位移
            this.x += collisionMove.x;
            this.y += collisionMove.y;
        }

        // 计算自主移动位移
        public Vector2 calculateSelfMove() {
            // NONE 不移动
            if (this.moveMode == PetMoveMode.NONE) {
                return Vector2.zero;
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

                float distance = offset.magnitude;

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

        // 根据自主位移计算碰撞位移
        private Vector2 calculateCollisionMove(Vector2 selfMove, SheepWorld sheepWorld) {
            // 碰撞移动
            Vector2 collideMove = Vector2.zero;

            // 碰撞了多少单位的 计数器
            int collideCount = 0;

            foreach (var otherPet in sheepWorld.pets) {
                // 最多只处理 20 个碰撞单位.
                if (collideCount >= 20) {
                    break;
                }

                // 排除自己
                if (otherPet == this) {
                    continue;
                }

                // otherPet 指向当前 Pet 的偏移向量
                float offsetX = x - otherPet.x;
                float offsetY = y - otherPet.y;

                // 计算和另一个对象的距离
                float distance = (float)Math.Sqrt(offsetX * offsetX + offsetY * offsetY);

                // 最小距离
                float minDistance =
                    this.collideR +
                    otherPet.collideR;

                // 没有发生重叠 
                if (distance >= minDistance) {
                    continue;
                }
                
                // 重叠距离
                float overlap = minDistance - distance;

                // 不是完全重合, 可以计算推开的距离
                if (distance > 0f) {
                    // 从 otherPet 指向当前 Pet 的归一化方向
                    float directionX = offsetX / distance;
                    float directionY = offsetY / distance;

                    // 完全消除当前重叠需要产生的位移
                    collideMove.x += directionX * overlap;
                    collideMove.y += directionY * overlap;
                }
                else {
                    // 完全重合时给予微小随机扰动, 让下一帧可以正常计算分离方向
                    collideMove.x += sheepWorld.randomFloat(-0.1f, 0.1f);
                    collideMove.y += sheepWorld.randomFloat(-0.1f, 0.1f);
                }

                // 增加计数器
                collideCount++;
            }


            // 存在碰撞
            if (collideCount >= 1) {
                
                // 普通单位 , 如果小于 colliderNotMoveNum 的人阻挡 采用和上边相同的位移逻辑
                if (collideCount < collideNotMoveNum) {
                    // 这里是不是需要速度? 
                }
                else {
                    // 否则 太多人挡着, 不做位移 (卡在原地不动)
                    // 没有碰撞直接返回
                    return Vector2.zero;
                }
                
                
                if (collideMove.x > collideR) {
                    collideMove.x = collideR;
                }
                else if (collideMove.x < -collideR) {
                    collideMove.x = -collideR;
                }

                if (collideMove.y > collideR) {
                    collideMove.y = collideR;
                }
                else if (collideMove.y < -collideR) {
                    collideMove.y = -collideR;
                }

                collideMove = collideElasticityScale * collideMove;

                return selfMove + collideMove;
            
            }
            else {
                // 没有碰撞直接返回
                return selfMove;
            }
        }
    }
}