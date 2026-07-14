using System;
using UnityEngine;

namespace rvb.scripts {
    public static class Util {
        public static SheepMgr system;

        public static bool isInitViewBoss = false;
        public static PetView view_boss_red = null;
        public static PetView view_boss_blue = null;

        public static (int xn, int yn) getXnYnByIndex(int e) {
            return (
                e % SheepConfig.line_w,
                Mathf.FloorToInt((float)e / SheepConfig.line_w)
            );
        }

        // 根据 空间坐标 获取 格子坐标
        public static (int xn, int yn) getXnYn(float x, float y) {
            return (
                Mathf.FloorToInt(x / SheepConfig.d + SheepConfig.h / SheepConfig.d / 2f),
                Mathf.FloorToInt(y / SheepConfig.d + SheepConfig.w / SheepConfig.d / 2f)
            );
        }

        // 根据格子坐标 获取 index
        // 具有边界保护
        public static int getIndexByXnYn(int xn, int yn) {
            if (xn < 0) {
                xn = 0;
            }
            else if (xn >= SheepConfig.line_w) {
                xn = SheepConfig.line_w - 1;
            }

            if (yn < 0) {
                yn = 0;
            }
            else if (yn >= SheepConfig.line_h) {
                yn = SheepConfig.line_h - 1;
            }

            return xn * SheepConfig.line_w + yn;
        }

        // 根据 空间坐标 获取 索引 (只是组合方法)
        public static int getIndexByXY(float x, float y) {
            (int xn, int yn) i = getXnYn(x, y);
            return getIndexByXnYn(i.xn, i.yn);
        }

        public static bool isCanAckByRole(PetView e, PetView t, float i = 1f) {
            //判断单位是否死亡
            bool o = !t.isDie;
            if (!o) {
                return o;
            }

            SheepRoleState l = t.state;
            if (
                t.roleId != 0 &&
                (
                    l == SheepRoleState.In ||
                    l == SheepRoleState.Dead ||
                    l == SheepRoleState.Merge ||
                    l == SheepRoleState.Res ||
                    l == SheepRoleState.Killer
                )
            ) {
                return false;
            }

            // 阵营判断
            SheepCamp r = e.camp;
            SheepCamp a = t.camp;
            if (a == r) {
                return false;
            }

            // 越界判断
            if (a == SheepCamp.Red && t.posX < -SheepConfig.limitSearchBorderX) {
                return false;
            }

            if (a == SheepCamp.Blue && t.posX > SheepConfig.limitSearchBorderX) {
                return false;
            }

            //距离判断
            float f = e.posX;
            float h = e.posY;
            float p = t.posX - f;
            float u = t.posY - h;
            float d = p * p + u * u;
            float g = Mathf.Sqrt(d);

            //攻击范围判断
            return g < e.conf.atkR * i + e.conf.collideR + t.conf.collideR;
        }

        // 是否可以移动?
        public static bool isCanMove(PetView petSkin, PetView targetPetSkin) {
            SheepCamp o = targetPetSkin.camp;
            return !(
                o == SheepCamp.Red && targetPetSkin.posX < -SheepConfig.limitSearchBorderX ||
                o == SheepCamp.Blue && targetPetSkin.posX > SheepConfig.limitSearchBorderX ||
                targetPetSkin.isDie ||
                targetPetSkin.camp == petSkin.camp
            );
        }

        // 设置 e 到 t 的方向向量
        public static void dirTar(PetView e, PetView t) {
            float i = e.posX;
            float s = e.posY;
            float o = t.posX - i;
            float l = t.posY - s;
            float r = Mathf.Sqrt(o * o + l * l);
            if (r == 0f) {
                r = 1f;
            }

            float a = o / r;
            float c = l / r;
            e.dirX = a;
            e.dirY = c;
        }

        // 设置 e 到指定 x,y 的方向向量
        public static float[] dirTarByPos(PetView e, float x, float y) {
            float s = x - e.posX;
            float o = y - e.posY;
            float l = Mathf.Sqrt(s * s + o * o);
            if (l == 0f) {
                l = 1f;
            }

            return new[] { s / l, o / l };
        }

        // 返回两点之间的距离
        public static float dis(float x, float y, float x1, float y1) {
            float o = x1 - x;
            float l = y1 - y;
            return Mathf.Sqrt(o * o + l * l);
        }

        // 返回两个单位之间的距离
        public static float disByRole(PetView e, PetView t) {
            float i = e.posX;
            float s = e.posY;
            float o = t.posX - i;
            float l = t.posY - s;
            return Mathf.Sqrt(o * o + l * l);
        }

        // 以 e / t 的概率返回 true
        public static bool numToBool(float e, float t = 1000f) {
            return UnityEngine.Random.value * t < e;
        }

        // 是否处于攻击 cd
        public static bool isAtkCd(PetView e) {
            return e.curAckCd > 0f;
        }

        public static float subAtkCd(PetView viewPet, float t) {
            float i = viewPet.curAckCd;
            if (i != 0f) {
                i -= t;
                if (i < 0f) {
                    i = 0f;
                }

                viewPet.curAckCd = i;
            }

            return i;
        }

        // 重置 攻击 cd
        public static void resetAtkCd(PetView e, float t) {
            e.curAckCd = t;
        }

        // 获取 BOSS
        public static PetView getBackBoss(SheepCamp camp) {
            if (!isInitViewBoss) {
                isInitViewBoss = true;
                view_boss_red = system.getPetView((int)SheepCamp.Red);
                view_boss_blue = system.getPetView((int)SheepCamp.Blue);
            }

            if (camp == SheepCamp.Red) {
                return view_boss_blue;
            }
            else {
                return view_boss_red;
            }
        }

        public static void moveTar(PetView e, PetView t, float i, bool o) {
            // todo 这个是什么意思 某种跳过开关吗?
            if (!o) {
                return;
            }

            // todo 当这两个任意不为 0 的时候 跳过 ? 什么作用?
            if (e.impulseX != 0f || e.impulseY != 0f) {
                return;
            }

            // 时间系数
            i *= 3f;

            // 当前位置
            Vector2 l = new Vector2(e.posX, e.posY);

            // 有目标
            if (t != null) {
                // 如果目标是 boss
                if (t.conf.roleType == SheepRoleType.boss) {
                    // 当红方在中线左侧的时候 逼着往中线推进 (不贴边)
                    if (e.camp == SheepCamp.Red && e.posX < 0f) {
                        e.dirX = 1f;
                        e.dirY = 0.02f * UnityEngine.Random.value - 0.01f;
                    }
                    // 同理当 蓝方在中线右侧的时候  (不贴边)
                    else if (e.camp == SheepCamp.Blue && e.posX > 0f) {
                        e.dirX = -1f;
                        e.dirY = 0.02f * UnityEngine.Random.value - 0.01f;
                    }
                    else {
                        // 否则正常 移动
                        dirTar(e, t);
                    }
                }
                else {
                    // 否则正常移动
                    dirTar(e, t);
                }
            }
            else {
                // 冲锋类
                if (
                    e.state == SheepRoleState.Charge ||
                    e.state == SheepRoleState.SpinSpurt ||
                    e.state == SheepRoleState.ChargePlus
                ) {
                    // 根据阵营 左冲右冲
                    if (e.camp == SheepCamp.Red) {
                        e.dirX = 1f;
                    }
                    else {
                        e.dirX = -1f;
                    }

                    // 纵向没有力
                    e.dirY = 0f;
                }
            }

            // 速度
            float n;

            // 根据状态不同 采取不用的速度
            if (
                e.state == SheepRoleState.Spurt ||
                e.state == SheepRoleState.Charge ||
                e.state == SheepRoleState.SpinSpurt ||
                e.state == SheepRoleState.SpinAtk ||
                e.state == SheepRoleState.ChargePlus
            ) {
                n = e.conf.runSpeed;
            }
            else {
                n = e.conf.walkSpeed;
            }

            // 计算不考虑碰撞的情况下 应该移动的向量
            Vector2 r = new Vector2((float)(e.dirX * n * i), (float)(e.dirY * n * i));

            // 获取原始对应的格子
            (int xn, int yn) block = getXnYn(l.x, l.y);
            int xn = block.xn;
            int yn = block.yn;

            // 处于 ChargePlus 状态
            if (e.state == SheepRoleState.ChargePlus) {
                // 强制移动
                Vector2 target = new Vector2(l.x + r.x, l.y + r.y);
                e.logicMove(target.x, target.y);
            }
            else if (e.state == SheepRoleState.Charge || e.state == SheepRoleState.SpinSpurt) {
                // 一样强制移动
                Vector2 target = new Vector2(l.x + r.x, l.y + r.y);
                e.logicMove(target.x, target.y);
            }
            else if (e.isBoom) {
                // 一样强制移动
                Vector2 target = new Vector2(l.x + r.x, l.y + r.y);
                e.logicMove(target.x, target.y);
            }
            else if (e.state == SheepRoleState.SpinAtk) {
                Vector2 target = new Vector2(l.x + r.x, l.y + r.y);

                // 限制在某个区域内?
                if (
                    target.x < SheepConfig.w / 2f &&
                    target.x > -SheepConfig.w / 2f &&
                    target.y < SheepConfig.h / 2f &&
                    target.y > -SheepConfig.h / 2f ||
                    l.x > SheepConfig.w / 2f ||
                    l.x < -SheepConfig.w / 2f ||
                    l.y > SheepConfig.h / 2f ||
                    l.y < -SheepConfig.h / 2f
                ) {
                    e.logicMove(target.x, target.y);
                }
            }
            else {
                Vector2 tCollide = Vector2.zero;

                // 碰撞了多少单位的 计数器
                int collideCount = 0;

                // 碰撞了多少非冲刺单位的 计数器
                int notSpurtCount = 0;

                if (!e.isNotConn) {
                    UtilFind.forfeachBlocksByCollView(e, xn, yn, e.conf.detectCollideR, s => {
                        if (collideCount >= 20) {
                            return;
                        }

                        if (s.isNotConn) {
                            return;
                        }

                        float nX = l.x - s.posX;
                        float rY = l.y - s.posY;

                        // 计算 当前 单位 位置和目标的距离
                        float a = Mathf.Sqrt(nX * nX + rY * rY);

                        // 如果太近了 还不是同一个人 (需要推开? )
                        if (a < e.conf.collideR + s.conf.collideR && e.id != s.id) {
                            // 不是完全重合, 可以计算推开的距离
                            if (a > 0f) {
                                float push = e.conf.collideR + s.conf.collideR - a;
                                tCollide.x += nX * push / (e.conf.collideR + s.conf.collideR);
                                tCollide.y += rY * push / (e.conf.collideR + s.conf.collideR);
                            }
                            else {
                                // 完全重合防止死锁 给予一个随机数
                                tCollide.x += 0.1f * UnityEngine.Random.value;
                                tCollide.y += 0.1f * UnityEngine.Random.value;
                            }

                            collideCount++;

                            // 非 冲刺单位计数器
                            if (s.state != SheepRoleState.Spurt) {
                                notSpurtCount++;
                            }
                        }
                    });
                }

                if (collideCount >= 1) {
                    // 按照 碰撞算法 最终要移动到的位置
                    Vector2 movePos = new Vector2(l.x, l.y);

                    // 大体逻辑
                    // 冲刺：
                    //     人少 → 全速
                    //     人多 → 慢挪
                    //
                    // 普通：
                    //     人少 → 慢挪
                    //     人多 → 不动
                    if (e.state == SheepRoleState.Spurt) {
                        // 前面有大于 3 个 "不在冲刺的普通单位" 堵着
                        if (notSpurtCount > 3) {
                            // 限制最大只能移动 碰撞半径的距离
                            if (r.x > e.conf.collideR) {
                                r.x = e.conf.collideR;
                            }
                            else if (r.x < -e.conf.collideR) {
                                r.x = -e.conf.collideR;
                            }

                            if (r.y > e.conf.collideR) {
                                r.y = e.conf.collideR;
                            }
                            else if (r.y < -e.conf.collideR) {
                                r.y = -e.conf.collideR;
                            }

                            // 乘以 碰撞移动缩放系数
                            movePos.x += e.conf.colliderMoveScale * r.x;
                            movePos.y += e.conf.colliderMoveScale * r.y;
                        }
                        else {
                            // 如果没有那么多人 直接无视阻挡.
                            movePos.x += r.x;
                            movePos.y += r.y;
                        }
                    }
                    else {
                        // 普通单位 , 如果小于 colliderNotMoveNum 的人阻挡 采用和上边相同的位移逻辑
                        if (collideCount < e.conf.colliderNotMoveNum) {
                            if (r.x > e.conf.collideR) {
                                r.x = e.conf.collideR;
                            }
                            else if (r.x < -e.conf.collideR) {
                                r.x = -e.conf.collideR;
                            }

                            if (r.y > e.conf.collideR) {
                                r.y = e.conf.collideR;
                            }
                            else if (r.y < -e.conf.collideR) {
                                r.y = -e.conf.collideR;
                            }

                            movePos.x += e.conf.colliderMoveScale * r.x;
                            movePos.y += e.conf.colliderMoveScale * r.y;
                        }
                        else {
                            // 否则 太多人挡着, 不做位移 (卡在原地不动)
                            r.x = 0f;
                            r.y = 0f;
                        }
                    }

                    // todo  聚拢效果?
                    if (e.camp == SheepCamp.Red && e.posX >= 0f) {
                        float centerY = 0f;
                        float targetX = 1200f - e.posX;
                        float targetY = centerY - e.posY;
                        float distance = Mathf.Sqrt(targetX * targetX + targetY * targetY);
                        float dirY = targetY / distance;
                        float dirX = targetX / distance;

                        if (e.posY > 0f && tCollide.y > 0f) {
                            movePos.x -= dirY;
                            movePos.y += dirX;
                        }
                        else if (e.posY < 0f && tCollide.y < 0f) {
                            movePos.x += dirY;
                            movePos.y -= dirX;
                        }
                    }
                    else if (e.camp == SheepCamp.Blue && e.posX <= 0f) {
                        float centerY = 0f;
                        float targetX = -1200f - e.posX;
                        float targetY = centerY - e.posY;
                        float distance = Mathf.Sqrt(targetX * targetX + targetY * targetY);
                        float dirY = targetY / distance;
                        float dirX = targetX / distance;

                        if (e.posY > 0f && tCollide.y > 0f) {
                            movePos.x += dirY;
                            movePos.y -= dirX;
                        }
                        else if (e.posY < 0f && tCollide.y < 0f) {
                            movePos.x -= dirY;
                            movePos.y += dirX;
                        }
                    }

                    // 修正碰撞
                    if (tCollide.x > e.conf.collideR) {
                        tCollide.x = e.conf.collideR;
                    }
                    else if (tCollide.x < -e.conf.collideR) {
                        tCollide.x = -e.conf.collideR;
                    }

                    if (tCollide.y > e.conf.collideR) {
                        tCollide.y = e.conf.collideR;
                    }
                    else if (tCollide.y < -e.conf.collideR) {
                        tCollide.y = -e.conf.collideR;
                    }

                    movePos.x += e.conf.colliderElasticityScale * tCollide.x;
                    movePos.y += e.conf.colliderElasticityScale * tCollide.y;
                    e.logicMove(movePos.x, movePos.y);
                }
                else {
                    // 没有任何碰撞, 直接移动
                    Vector2 target = new Vector2(l.x + r.x, l.y + r.y);
                    e.logicMove(target.x, target.y);
                }
            }
        }
    }
}