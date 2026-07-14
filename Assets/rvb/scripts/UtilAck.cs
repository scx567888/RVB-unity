namespace rvb.scripts {
    public class UtilAck {
        
    /**
     * @type {SheepMgr}
     */
    static system;

    static ackTar(e, t) {
        let i;
        let s = e.conf;

        i = s.atk;

        if (e.curAtkBuff) {
            i = Math.floor(i * (1 + e.curAtkBuff / 100));
        }

        if (Util.isCanAckByRole(e, t)) {
            this.hurtByRole(e, t, i)
        }

        if (0 != e.roleId && 0 != t.roleId) {

            let {xn: xn, yn: yn} = Util.getXnYn(t.posX, t.posY);

            UtilFind.forfeachBlocksByAckView(e.camp, xn, yn, e.conf.splitN, s => {
                if (!s.isDie && 0 != s.roleId && s.camp == t.camp && s.id != t.id && s.curHp > 0) {
                    let o = t.posX - s.posX;
                    let l = t.posY - s.posY;
                    if (Math.sqrt(o * o + l * l) <= t.conf.collideR + s.conf.collideR + e.conf.spiltR) {
                        this.hurtByRole(e, s, i)
                    }
                }
            })

        }
    }

    static ackMe(e, t = 1, i = 1, s = 10, o = 0, l = []) {
        let n = i, r = e.conf;
        SheepConfig;
        n *= r.atk;
        e.curAtkBuff && (n = Math.floor(n * (1 + e.curAtkBuff / 100)));
        let {xn: a, yn: c} = Util.getXnYn(e.posX, e.posY);
        UtilFind.forfeachBlocksByAckView(e.camp, a, c, s, (i => {
            if (-1 == l.indexOf(i.conf.roleType) && i.curHp > 0) {
                let s = e.posX - i.posX, l = e.posY - i.posY, r = Math.sqrt(s * s + l * l);
                r <= e.conf.collideR + i.conf.collideR + e.conf.spiltR * t && (this.hurtByRole(e, i, n), o && (s /= r, l /= r, i.impulseX = -s * o, i.impulseY = -l * o))
            }
        }))
    }

    static hitBackMe(e, t = 1, i = 10, s) {
        let {xn: o, yn: l} = Util.getXnYn(e.posX, e.posY);
        UtilFind.forfeachBlocksByAckView(e.camp, o, l, i, (i => {
            if (i.curHp > 0) {
                let o = e.posX - i.posX, l = e.posY - i.posY, n = Math.sqrt(o * o + l * l);
                n <= e.conf.collideR + i.conf.collideR + e.conf.spiltR * t && s && (o /= n, l /= n, i.impulseX = -o * s, i.impulseY = -l * s)
            }
        }))
    }

    /**
     *
     * @param e
     * @param t {PetView}
     * @param i
     */
    static hurtByRole(e, t, i) {
        let s = SheepRoleRestraint.getById(t.conf.roleType).hitRate[e.conf.roleType];
        i = Math.max(1, Math.floor(i * s));
        let o = t.subCurHp(i);
        if (o > 0 && o <= i) {

        }
    }

    /**
     *
     * @param e
     * @param t {PetView}
     * @param i
     */
    static hurtByBullet(e, t, i) {
        let s = SheepRoleRestraint.getById(t.conf.roleType).hitRate[e.conf.roleType];
        i = Math.max(1, Math.floor(i * s));
        let o = t.subCurHp(i);
        if (o > 0 && o <= i) {

        }
    }

    /**
     *
     * @param e
     * @param petSkin {PetView}
     * @param i
     * @returns {boolean}
     */
    static isCanAckByBullet(e, petSkin, i) {
        let s = !petSkin.isDie;
        if (0 == s) return s;
        let o = petSkin.state;
        if (0 != petSkin.roleId && (o == SheepRoleState.In || o == SheepRoleState.Dead || o == SheepRoleState.Merge || o == SheepRoleState.Res || o == SheepRoleState.Killer)) return !1;
        let l = petSkin.camp != e.camp;
        if (0 == l) return l;
        if (e.conf.atkShapeType == SheepBulletAtkShapeType.Ring) {
            let s = e.x, o = e.y, l = petSkin.posX - s, n = petSkin.posY - o, r = l * l + n * n, a = Math.sqrt(r), c = e.conf;
            return a < c.maxRadiuses[i] && a > c.minRadiuses[i]
        }
        {
            let i = e.x, s = e.y, o = petSkin.posX - i, l = petSkin.posY - s, n = o * o + l * l;
            return Math.sqrt(n) < e.conf.atkR
        }
    }


    }
}