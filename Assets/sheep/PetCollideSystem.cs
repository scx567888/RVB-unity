using UnityEngine;

namespace sheep {
    // todo
    public class PetCollideSystem {
        // 根据自主位移计算碰撞修正后的最终位移
        public Vector2 calculateCollisionMove(Pet pet, Vector2 selfMove, SheepWorld sheepWorld) {
            return selfMove;
            var intent = pet.collideIntent;

            // 当前单位不参与碰撞
            if (!intent.enabled) {
                return selfMove;
            }

            Vector2 collisionPush = Vector2.zero;

            int collideCount = 0;
            int notSpurtCount = 0;

            // 这个方法由你的格子系统实现：
            // 查询指定格子范围内可能与pet碰撞的单位
            foreach (var other in sheepWorld.pets) {
                if (collideCount >= intent.maxCollideCount) {
                    continue;
                }

                if (other.id == pet.id) {
                    continue;
                }

                var otherIntent = other.collideIntent;

                if (!otherIntent.enabled) {
                    continue;
                }

                // 旧版只碰撞相同碰撞组
                if (otherIntent.group != intent.group) {
                    continue;
                }

                float dx = pet.x - other.x;
                float dy = pet.y - other.y;

                float radiusSum =
                    intent.radius +
                    otherIntent.radius;

                float distanceSquared =
                    dx * dx +
                    dy * dy;

                if (distanceSquared >= radiusSum * radiusSum) {
                    continue;
                }

                float distance =
                    Mathf.Sqrt(distanceSquared);

                if (distance > 0.000001f) {
                    float overlap =
                        radiusSum - distance;

                    // 保留旧版的柔性排斥公式
                    collisionPush.x +=
                        dx * overlap / radiusSum;

                    collisionPush.y +=
                        dy * overlap / radiusSum;
                }
                else {
                    // 复刻旧版完全重合处理
                    collisionPush.x +=
                        0.1f * sheepWorld.random01();

                    collisionPush.y +=
                        0.1f * sheepWorld.random01();
                }

                collideCount++;

                if (
                    otherIntent.moveMode !=
                    PetCollideMoveMode.SPURT
                ) {
                    notSpurtCount++;
                }
            }

            // 没发生碰撞，正常全速移动
            if (collideCount == 0) {
                return selfMove;
            }

            Vector2 retainedSelfMove;

            // 冲刺单位
            if (
                intent.moveMode ==
                PetCollideMoveMode.SPURT
            ) {
                // 超过3个普通单位阻挡才减速
                if (notSpurtCount > 3) {
                    retainedSelfMove =
                        clampMoveByRadius(
                            selfMove,
                            intent.radius
                        ) * intent.moveScale;
                }
                else {
                    retainedSelfMove = selfMove;
                }
            }
            // 普通单位
            else {
                if (collideCount < intent.notMoveNum) {
                    retainedSelfMove =
                        clampMoveByRadius(
                            selfMove,
                            intent.radius
                        ) * intent.moveScale;
                }
                else {
                    // 太拥挤，取消主动移动
                    retainedSelfMove = Vector2.zero;
                }
            }

            // 限制累积排斥
            collisionPush.x = Mathf.Clamp(
                collisionPush.x,
                -intent.radius,
                intent.radius
            );

            collisionPush.y = Mathf.Clamp(
                collisionPush.y,
                -intent.radius,
                intent.radius
            );

            Vector2 collisionCorrection =
                collisionPush *
                intent.elasticityScale;

            return retainedSelfMove +
                   collisionCorrection;
        }

        private static Vector2 clampMoveByRadius(
            Vector2 move,
            float radius
        ) {
            move.x = Mathf.Clamp(
                move.x,
                -radius,
                radius
            );

            move.y = Mathf.Clamp(
                move.y,
                -radius,
                radius
            );

            return move;
        }
    }
}