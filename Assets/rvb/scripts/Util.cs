using System;

namespace rvb.scripts {
    public class Util {

        
        public static bool isInitViewBoss = false;
        public static Object view_boss_red = null;
        public static Object view_boss_blue = null;

        /**
         * @type {SheepMgr}
         */
        public static SheepMgr system;

public    static void getXnYnByIndex(e) {
        return {
            yn: Math.floor(e / SheepConfig.line_w),
            xn: e % SheepConfig.line_w
        }
    }

    // 根据 空间坐标 获取 格子坐标
    public static XnYn getXnYn(int x,int y) {
        return new  XnYn() {
            xn= Math.Floor(x / SheepConfig.d + SheepConfig.h / SheepConfig.d / 2),
            yn= Math.Floor(y / SheepConfig.d + SheepConfig.w / SheepConfig.d / 2)
        };
    }

    // 根据格子坐标 获取 index
    // 具有边界保护
    public   static int  getIndexByXnYn(int xn,int yn) {
        if (xn < 0) {
            xn = 0;
        } else if (xn >= SheepConfig.line_w) {
            xn = SheepConfig.line_w - 1;
        }
        if (yn < 0) {
            yn = 0;
        } else if (yn >= SheepConfig.line_h) {
            yn = SheepConfig.line_h - 1;
        }

        return xn * SheepConfig.line_w + yn;
    }

    // 根据 空间坐标 获取 索引 (只是组合方法)
    public  static int  getIndexByXY(int x,int y) {
        var i = Util.getXnYn(x, y);
        return Util.getIndexByXnYn(i.xn, i.yn);
    }

    /**
     *
     * @param e {PetView}
     * @param t {PetView}
     * @param i
     * @returns {boolean}
     */
    public  static bool  isCanAckByRole(PetView e,PetView t,int i = 1) {
        //判断单位是否死亡
        bool o = !t.isDie;
        if (0 == o) {
            return o;
        }

        int l = t.state;
        if (
            0 != t.roleId &&
            (
                l == SheepRoleState.In ||
                l == SheepRoleState.Dead ||
                l == SheepRoleState.Merge ||
                l == SheepRoleState.Res ||
                l == SheepRoleState.Killer
            )
        ) {
            return !1;
        }

        // 阵营判断
        int r = e.camp;
        int a = t.camp;
        if (a != r == 0) {
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
        int f = e.posX;
        int h = e.posY;
        int p = t.posX - f;
        int u = t.posY - h;
        int d = p * p + u * u;
        double g = Math.Sqrt(d);

        //攻击范围判断
        var S = e.conf;
        var m = t.conf;
        return g < S.atkR * i + S.collideR + m.collideR;
    }

    /**
     * 是否可以移动?
     * @param petSkin {PetView}
     * @param targetPetSkin {PetView}
     * @return {boolean}
     */
    public  static int  isCanMove(PetView petSkin,PetView targetPetSkin) {
        int o = targetPetSkin.camp;
        return !(o == SheepCamp.Red && targetPetSkin.posX < -SheepConfig.limitSearchBorderX ||
                 o == SheepCamp.Blue && targetPetSkin.posX > SheepConfig.limitSearchBorderX ||
                 targetPetSkin.isDie ||
                 targetPetSkin.camp == petSkin.camp);
    }

    /**
     * 设置 e 到 t 的方向向量
     * @param e {PetView}
     * @param t {PetView}
     */
    public  static void  dirTar(PetView e,PetView t) {
        int i = e.posX;
        int s = e.posY;
        int o = t.posX - i;
        int l = t.posY - s;
        double r = Math.Sqrt(o * o + l * l);
        if (r == 0) {
            r = 1;
        }
        double a = o / r;
        double c = l / r;
        e.dirX = a;
        e.dirY = c;
    }

    /**
     * 设置 e 到指定 x,y 的方向向量
     * @param e {PetView}
     * @param x
     * @param y
     * @returns {*}
     */
    public static double[]  dirTarByPos(PetView e,int x,int y) {
        int s = x - e.posX;
        int o = y - e.posY;
        double l = Math.Sqrt(s * s + o * o);
        if (0 == l) {
            l = 1;
        }

        return new []{s / l, o / l};
    }

    // 返回两点之间的距离
    public static double  dis(int x,int y,int x1,int y1) {
        int o = x1 - x;
        int l = y1 - y;
        return Math.Sqrt(o * o + l * l);
    }

    // 返回两个单位之间的距离
    public  static void  disByRole(PetView e,PetView t) {
        int i = e.posX;
        int s = e.posY;
        int o = t.posX - i;
        int l = t.posY - s;
        return Math.Sqrt(o * o + l * l);
    }

    // 以 e / t 的概率返回 true
    public  static void  numToBool(e, t = 1000) {
        return Math.random() * t < e
    }

    // 是否处于攻击 cd
    public static void  isAtkCd(PetView e) {
        return e.curAckCd > 0
    }

    /**
     *
     * @param viewPet {PetView}
     * @param t
     * @returns {*}
     */
    public  static void  subAtkCd(PetView viewPet, t) {
        let i = viewPet.curAckCd;
        if (0 != i) {
            i -= t;
            if (i < 0) {
                i = 0;
            }

            viewPet.curAckCd = i;
        }

        return i;
    }

    // 重置 攻击 cd
    public static void  resetAtkCd(PetView e, t) {
        e.curAckCd = t;
    }

    /**
     * 获取 BOSS
     * @param camp
     * @return {PetView}
     */
    public static void  getBackBoss(int camp) {
        if (!this.isInitViewBoss) {
            this.isInitViewBoss = true;
            this.view_boss_red = this.system.getPetView(SheepCamp.Red);
            this.view_boss_blue = this.system.getPetView(SheepCamp.Blue);
        }
        if (camp == SheepCamp.Red) {
            return this.view_boss_blue
        } else {
            return this.view_boss_red
        }
    }

    /**
     *
     * @param e {PetView}
     * @param t
     * @param i
     * @param o
     */
    public  static void  moveTar(PetView e, t, i, o) {
        // todo 这个是什么意思 某种跳过开关吗?
        if (!o) {
            return;
        }
        // todo 当这两个任意不为 0 的时候 跳过 ? 什么作用?
        if (e.impulseX || e.impulseY) {
            return;
        }
        // 时间系数
        i *= 3;
        // 当前位置
        let l = {x: e.posX, y: e.posY};

        // 有目标
        if (t) {
            // 如果目标是 boss
            if (t.conf.roleType == SheepRoleType.boss) {
                // 当红方在中线左侧的时候 逼着往中线推进 (不贴边)
                if (e.camp == SheepCamp.Red && e.posX < 0) {
                    e.dirX = 1;
                    e.dirY = .02 * Math.random() - .01;
                }
                // 同理当 蓝方在中线右侧的时候  (不贴边)
                else if (e.camp == SheepCamp.Blue && e.posX > 0) {
                    e.dirX = -1;
                    e.dirY = .02 * Math.random() - .01;
                } else {
                    // 否则正常 移动
                    Util.dirTar(e, t);
                }
            } else {
                // 否则正常移动
                Util.dirTar(e, t);
            }
        } else {
            // 冲锋类
            if (e.state == SheepRoleState.Charge ||
                e.state == SheepRoleState.SpinSpurt ||
                e.state == SheepRoleState.ChargePlus) {
                // 根据阵营 左冲右冲
                if (e.camp == SheepCamp.Red) {
                    e.dirX = 1;
                } else {
                    e.dirX = -1;
                }
                // 纵向没有力
                e.dirY = 0;
            }
        }

        // 速度
        let n;
        // 根据状态不同 采取不用的速度
        if (e.state == SheepRoleState.Spurt ||
            e.state == SheepRoleState.Charge ||
            e.state == SheepRoleState.SpinSpurt ||
            e.state == SheepRoleState.SpinAtk ||
            e.state == SheepRoleState.ChargePlus) {
            n = e.conf.runSpeed;
        } else {
            n = e.conf.walkSpeed;
        }

        // 计算不考虑碰撞的情况下 应该移动的向量
        let r = {x: e.dirX * n * i, y: e.dirY * n * i};
        // 获取原始对应的格子
        let {xn: xn, yn: yn} = Util.getXnYn(l.x, l.y);

        // 处于 ChargePlus 状态
        if (e.state == SheepRoleState.ChargePlus) {
            // 强制移动
            let t = {x: l.x + r.x, y: l.y + r.y};
            e.logicMove(t.x, t.y)

        } else if (e.state == SheepRoleState.Charge || e.state == SheepRoleState.SpinSpurt) {

            // 一样强制移动
            let t = {x: l.x + r.x, y: l.y + r.y};
            e.logicMove(t.x, t.y)

        } else if (e.isBoom) {

            // 一样强制移动
            let t = {x: l.x + r.x, y: l.y + r.y};
            e.logicMove(t.x, t.y)

        } else if (e.state == SheepRoleState.SpinAtk) {
            let t = SheepConfig;
            let i = {x: l.x + r.x, y: l.y + r.y};

            // 限制在某个区域内?
            if (i.x < t.w / 2 && i.x > -t.w / 2 && i.y < t.h / 2 && i.y > -t.h / 2 || l.x > t.w / 2 || l.x < -t.w / 2 || l.y > t.h / 2 || l.y < -t.h / 2) {
                e.logicMove(i.x, i.y)
            }

        } else {
            let t = {x: 0, y: 0};

            // 碰撞了多少单位的 计数器
            let i = 0;
            // 碰撞了多少非冲刺单位的 计数器
            let o = 0;

            e.isConnNot || UtilFind.forfeachBlocksByCollView(e, xn, yn, e.conf.detectCollideR, s => {
                if (i >= 20) {
                    return;
                }
                if (s.isConnNot) {
                    return;
                }
                let n = l.x - s.posX;
                let r = l.y - s.posY;
                // 计算 当前 单位 位置和目标的距离
                let a = Math.sqrt(n * n + r * r);

                // 如果太近了 还不是同一个人 (需要推开? )
                if (a < e.conf.collideR + s.conf.collideR && e.id != s.id) {
                    // 不是完全重合, 可以计算推开的距离
                    if (a > 0) {
                        let i = e.conf.collideR + s.conf.collideR - a;
                        t.x += n * i / (e.conf.collideR + s.conf.collideR);
                        t.y += r * i / (e.conf.collideR + s.conf.collideR)
                    } else {
                        // 完全重合防止死锁 给予一个随机数
                        t.x += .1 * Math.random();
                        t.y += .1 * Math.random();
                    }
                    i++;
                    // 非 冲刺单位计数器
                    if (s.state != SheepRoleState.Spurt) {
                        o++
                    }
                }
            })

            if (i >= 1) {
                // 按照 碰撞算法 最终要移动到的位置
                let n = {x: l.x, y: l.y};

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
                    if (o > 3) {
                        // 限制最大只能移动 碰撞半径的距离
                        if (r.x > e.conf.collideR) {
                            r.x = e.conf.collideR;
                        } else if (r.x < -e.conf.collideR) {
                            r.x = -e.conf.collideR;
                        }

                        if (r.y > e.conf.collideR) {
                            r.y = e.conf.collideR;
                        } else if (r.y < -e.conf.collideR) {
                            r.y = -e.conf.collideR;
                        }

                        // 乘以 碰撞移动缩放系数
                        n.x += e.conf.colliderMoveScale * r.x;
                        n.y += e.conf.colliderMoveScale * r.y
                    } else {
                        // 如果没有那么多人 直接无视阻挡.
                        n.x += r.x;
                        n.y += r.y
                    }
                } else {
                    // 普通单位 , 如果小于 colliderNotMoveNum 的人阻挡 采用和上边相同的位移逻辑
                    if (i < e.conf.colliderNotMoveNum) {
                        if (r.x > e.conf.collideR) {
                            r.x = e.conf.collideR;
                        } else if (r.x < -e.conf.collideR) {
                            r.x = -e.conf.collideR;
                        }

                        if (r.y > e.conf.collideR) {
                            r.y = e.conf.collideR;
                        } else if (r.y < -e.conf.collideR) {
                            r.y = -e.conf.collideR;
                        }
                        n.x += e.conf.colliderMoveScale * r.x;
                        n.y += e.conf.colliderMoveScale * r.y
                    } else {
                        // 否则 太多人挡着, 不做位移 (卡在原地不动)
                        r.x = 0;
                        r.y = 0
                    }
                }

                // todo  聚拢效果?
                if (e.camp == SheepCamp.Red && e.posX >= 0) {
                    let i = 0;
                    let s = 1200 - e.posX;
                    let o = i - e.posY;
                    let l = Math.sqrt(s * s + o * o);
                    let r = o / l;
                    let a = s / l;
                    if (e.posY > 0 && t.y > 0) {
                        n.x -= r;
                        n.y += a
                    } else if (e.posY < 0 && t.y < 0) {
                        n.x += r;
                        n.y -= a
                    }
                } else if (e.camp == SheepCamp.Blue && e.posX <= 0) {
                    let i = 0;
                    let s = -1200 - e.posX;
                    let o = i - e.posY;
                    let l = Math.sqrt(s * s + o * o);
                    let r = o / l;
                    let a = s / l;
                    if (e.posY > 0 && t.y > 0) {
                        n.x += r;
                        n.y -= a
                    } else if (e.posY < 0 && t.y < 0) {
                        n.x -= r;
                        n.y += a
                    }
                }

                // 修正碰撞
                if (t.x > e.conf.collideR) {
                    t.x = e.conf.collideR;
                } else if (t.x < -e.conf.collideR) {
                    t.x = -e.conf.collideR;
                }
                if (t.y > e.conf.collideR) {
                    t.y = e.conf.collideR;
                } else if (t.y < -e.conf.collideR) {
                    t.y = -e.conf.collideR;
                }
                n.x += e.conf.colliderElasticityScale * t.x;
                n.y += e.conf.colliderElasticityScale * t.y;
                e.logicMove(n.x, n.y)
            } else {
                // 没有任何碰撞, 直接移动
                let t = {x: l.x + r.x, y: l.y + r.y};
                e.logicMove(t.x, t.y)
            }
        }
    }



    }
}