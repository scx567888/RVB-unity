namespace rvb.scripts {
    public class UtilFind {
        
    /**
     * @type {SheepMgr}
     */
    public static SheepMgr system;

    /**
     *
     * @param petSkin {PetView}
     * @param findR
     */
    public static void  findTar(petSkin, findR = 0) {
        let i = petSkin.posX;
        let o = petSkin.posY;
        let {xn: xn, yn: yn} = Util.getXnYn(i, o);
        let r = null;
        let a = null;
        let c = 0;
        findR = findR || petSkin.conf.findR;
        this.forNearBlocksByAckView(petSkin, xn, yn, findR, targetPetView => {
            if (!targetPetView.isDie && targetPetView.camp != petSkin.camp && 0 != targetPetView.roleId) {
                if (Util.isCanAckByRole(petSkin, targetPetView)) {
                    r = targetPetView;
                    return !0;
                }
                if (petSkin.conf.isFindMoveTar && !a && Util.isCanMove(petSkin, targetPetView)) {
                    let i = targetPetView.posX - petSkin.posX;
                    let s = targetPetView.posY - petSkin.posY;
                    c = i * i + s * s;
                    a = targetPetView
                } else if (petSkin.conf.isFindMoveTar && a && Util.isCanMove(petSkin, targetPetView)) {
                    let i = targetPetView.posX - petSkin.posX, s = targetPetView.posY - petSkin.posY, o = i * i + s * s;
                    if (o < c) {
                        c = o;
                        a = targetPetView
                    }
                }
                return !1
            }
            return !1
        })
        if (r) {
            petSkin.tarPosX = r.posX;
            petSkin.tarPosY = r.posY
            return {atkTar: r};
        }
        let backBoss = Util.getBackBoss(petSkin.camp);
        if (Util.isCanAckByRole(petSkin, backBoss)) {
            petSkin.tarPosX = backBoss.posX;
            petSkin.tarPosY = backBoss.posY;
            return {atkTar: backBoss};
        }
        if (a) {
            return {moveTar: a};
        }
        if (petSkin.state == SheepRoleState.Spurt && !petSkin.conf.skillSpurt) {
            let t = null;
            this.findNearBlocksByCollisionView(petSkin, xn, yn, petSkin.conf.findR, i => {
                if (i.state == SheepRoleState.Move) {
                    let s = i.posX - petSkin.posX;
                    let o = i.posY - petSkin.posY;
                    let l = s * s + o * o;
                    let n = i.conf.collideR + petSkin.conf.collideR;
                    if (l < n * n * .25) {
                        t = i
                        return !0
                    }
                }
                return !1
            })
            if (t) {
                return {moveTar: t}
            }
        }
        return petSkin.state != SheepRoleState.Spurt || petSkin.camp == SheepCamp.Red && petSkin.posX > petSkin.conf.runEndX || petSkin.camp == SheepCamp.Blue && petSkin.posX < petSkin.conf.runEndX ? {moveBoss: backBoss} : {}
    }

    /**
     *
     * @param petSkin {PetView}
     * @return {*}
     */
    public  static void  findNearAck(PetView petSkin) {
        let t = petSkin.posX;
        let i = petSkin.posY;
        let {xn: xn, yn: yn} = Util.getXnYn(t, i);
        let l = null;
        this.findNearBlocksByAckView(petSkin, xn, yn, petSkin.conf.findR, t => {
            if (!t.isDie && t.camp != petSkin.camp && 0 != t.roleId && Util.isCanAckByRole(petSkin, t)) {
                l = t;
                return true
            }
            return false;
        })
        if (l) {
            return l;
        }
        if (null == l) {
            let t = Util.getBackBoss(petSkin.camp);
            if (Util.isCanAckByRole(petSkin, t)) {
                l = t
            }
        }
        return l
    }

    public  static void  findFarAck(e, findR) {
        let posX = e.posX;
        let posY = e.posY;
        let {xn: xn, yn: yn} = Util.getXnYn(posX, posY);
        let n = null;
        this.findFarBlocksByAckView(e, xn, yn, findR, e => {
            n = e
            return true;
        })
        if (null == n) {
            let t = Util.getBackBoss(e.camp);
            Util.isCanAckByRole(e, t) && (n = t)
        }
        return n
    }

    public static void  findRandomAck(e, findR) {
        let i = e.posX;
        let s = e.posY;
        let {xn: o, yn: l} = Util.getXnYn(i, s);
        let n = null;
        this.findRandomBlocksByAckView(e, o, l, findR, e => {
            n = e
            return true;
        })
        if (null == n) {
            let t = Util.getBackBoss(e.camp);
            Util.isCanAckByRole(e, t) && (n = t)
        }
        return n
    }

    /**
     *
     * @param petView {PetView}
     * @param targetPetView {PetView}
     * @return {number}
     */
    public  static void  getAtkRank(petView, targetPetView) {
        if (petView.conf.findAtkSort) {
            for (let i = 0; i < petView.conf.findAtkSort.length; i++) {
                if (petView.conf.findAtkSort[i] == targetPetView.conf.roleType) {
                    return i;
                }
            }
        }
        return 100
    }

    /**
     *
     * @param petView {PetView}
     * @param targetPetView {PetView}
     * @returns {null}
     */
    public  static void  findSortAck(petView, targetPetView) {
        let posX = petView.posX;
        let posY = petView.posY;
        let {xn: o, yn: l} = Util.getXnYn(posX, posY);
        let n = null;
        let r = 100;
        let a = 0;
        petView.conf.findAtkSort && (a = petView.conf.findAtkSort[0]);
        this.findNearBlocksByAckView(petView, o, l, targetPetView, (t => {
            if (!Util.isCanAckByRole(petView, t)) {
                return !1;
            }
            if (null == n) {
                n = t;
                r = this.getAtkRank(petView, t)
                return !1;
            }
            if (t.roleId == a) {
                n = t
                return !0;
            }
            {
                let i = t;
                let s = this.getAtkRank(petView, t);
                s < r && (n = i, r = s)
                return !1
            }
        }))
        if (null == n) {
            let t = Util.getBackBoss(petView.camp);
            Util.isCanAckByRole(petView, t) && (n = t)
        }
        return n
    }

    /**
     *
     * @param petSkin {PetView}
     * @param findR
     * @return {null}
     */
    public  static void  findSortAck1(petSkin, findR) {
        let i = petSkin.posX;
        let s = petSkin.posY;
        let {xn: xn, yn: yn} = Util.getXnYn(i, s);
        let n = null;
        let r = 100;
        let a = 0;
        if (petSkin.conf.findAtkSort) {
            a = petSkin.conf.findAtkSort[0];
        }
        this.findNearBlocksByAckView(petSkin, xn, yn, findR, t => {
            if (null == n) {
                n = t;
                r = this.getAtkRank(petSkin, t)
                return false;
            }

            if (t.roleId == a) {
                n = t
                return true;
            }


            let i = t;
            let s = this.getAtkRank(petSkin, t);
            if (s < r) {
                n = i;
                r = s
            }
            return false

        })
        if (null == n) {
            let backBoss = Util.getBackBoss(petSkin.camp);
            Util.isCanAckByRole(petSkin, backBoss) && (n = backBoss)
        }
        return n
    }

    public  static void  foreachFront(e, t, i = 0, o = 30) {
        let l = e.posX;
        let n = e.posY;
        let {xn: r, yn: a} = Util.getXnYn(l, n);
        let c = e.tarPosX - l;
        let f = e.tarPosY - n;
        const h = Math.sqrt(c * c + f * f);

        if (h > 0) {
            c /= h;
            f /= h;
            i = i || e.conf.findR;
        } else {
            c = e.camp === SheepCamp.Red ? 1 : -1;
            f = 0;
            i = i || e.conf.findR;
        }

        const p = Math.cos(o * Math.PI / 180);
        let u = null;
        let d = 1 / 0;
        this.forNearBlocksByAckView(e, r, a, i, i => {
            if (!i.isDie && i.camp != e.camp && 0 != i.roleId && Util.isCanAckByRole(e, i)) {
                const e = i.posX - l;
                const s = i.posY - n;
                const o = e * e + s * s;
                const r = Math.sqrt(e * e + s * s);
                if (0 != r) {
                    if ((e * c + s * f) / r > p && o < d) {
                        d = o;
                        u = i;
                        t(u)
                    }
                }
                return false
            }
            return false
        })
    }

    public static  void forfeachBlocksByAckView(camp, xn, yn, splitN, callback) {
        camp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;
        let r = this.system.attackViews[camp];
        let a = this.system.attackView1s[camp];
        this.forfeachBlocks(r, a, xn, yn, splitN, callback)
    }

    /**
     *
     * @param petSkin {PetView}
     * @param xn
     * @param yn
     * @param splitN
     * @param callback {(PetView)=>{}}
     */
    public static void  forfeachBlocksByCollView(petSkin, xn, yn, splitN, callback) {
        let camp = petSkin.camp;
        let n = this.system.collisionViews[camp][petSkin.conf.collideId];
        let r = this.system.collisionView1s[camp][petSkin.conf.collideId];
        this.forfeachBlocks(n, r, xn, yn, splitN, callback)
    }

    /**
     *
     * @param e
     * @param t
     * @param xn
     * @param yn
     * @param splitN
     * @param callback {(PetView)=>{}}
     */
    public  static void  forfeachBlocks(e, t, xn, yn, splitN, callback) {
        for (let n = -splitN; n <= splitN; n++) {
            for (let r = -splitN; r <= splitN; r++) {
                if (xn + n < 0 || xn + n >= SheepConfig.line_w) {
                    continue;
                }
                if (yn + r < 0 || yn + r >= SheepConfig.line_w) {
                    continue;
                }
                let blockIndex = Util.getIndexByXnYn(xn + n, yn + r);
                this.system.forEachBlock(e, t, blockIndex, (petIndex => {
                    let t = this.system.getPetView(petIndex);
                    if (t) {
                        callback(t);
                        t = null;
                    }
                }))
            }
        }
    }

    public  static  void forNearBlocksByCollView(e, t, i, s, callback) {
        let l = e.camp;
        let n = this.system.collisionViews[l][e.conf.collideId];
        let r = this.system.collisionView1s[l][e.conf.collideId];
        return this.forNearBlocks(n, r, t, i, s, callback);
    }

    /**
     *
     * @param e
     * @param t
     * @param i
     * @param s
     * @param findR
     * @param callback {(PetView)=>{}}
     * @return {boolean}
     */
    public static void  forNearBlocks(e, t, i, s, findR, callback) {
        let n = 0;
        let r = (i, s) => {
            let o = Util.getIndexByXnYn(i, s);
            return o < 0 || o >= SheepConfig.line_w * SheepConfig.line_w || this.system.findBlock(e, t, o, (petIndex => {
                let petView = this.system.getPetView(petIndex);
                if (petView) {
                    let e = callback(petView);
                    petView = null;
                    return e;
                }
                return !1
            })), !1
        };
        for (let e = 0; e <= findR; e++) {
            if (e) {
                let e = {x: i - n, y: s + n}, t = {x: i + n, y: s + n}, o = {x: i + n, y: s - n},
                    l = {x: i - n, y: s - n};
                if (Math.random() < .5) {
                    for (let i = e.x; i < t.x; i++) if (r(i, e.y)) return !0;
                    for (let e = t.y; e > o.y; e--) if (r(t.x, e)) return !0;
                    for (let e = o.x; e > l.x; e--) if (r(e, o.y)) return !0;
                    for (let t = l.y; t < e.y; t++) if (r(l.x, t)) return !0
                } else {
                    for (let i = t.x; i > e.x; i--) if (r(i, e.y)) return !0;
                    for (let t = e.y; t > l.y; t--) if (r(l.x, t)) return !0;
                    for (let e = l.x; e < o.x; e++) if (r(e, o.y)) return !0;
                    for (let e = o.y; e < t.y; e++) if (r(t.x, e)) return !0
                }
            } else if (r(i, s)) return !0;
            n += 1
        }
        return !1
    }

    /**
     *
     * @param e
     * @param t
     * @param i
     * @param o
     * @param callback {(PetView)=>{}}
     * @return {boolean}
     */
    public static void  forNearBlocksByAckView(e, t, i, o, callback) {
        let camp = e.camp;
        camp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;
        let r = this.system.attackViews[camp];
        let a = this.system.attackView1s[camp];
        return this.forNearBlocks(r, a, t, i, o, callback)
    }

    public static void  findNearBlocksByAckView(PetView e, xn, yn, o, callback) {
        let camp = e.camp;
        camp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;
        let r = this.system.attackViews[camp];
        let a = this.system.attackView1s[camp];
        return this.findNearBlocks(r, a, xn, yn, o, callback)
    }

    /**
     *
     * @param petSkin {PetView}
     * @param xn
     * @param yn
     * @param findR
     * @param callback
     * @return {*}
     */
    public  static void  findFarBlocksByAckView(petSkin, xn, yn, findR, callback) {
        let camp = petSkin.camp;
        camp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;
        let r = this.system.attackViews[camp];
        let a = this.system.attackView1s[camp];
        return this.findFarBlocks(r, a, xn, yn, findR, callback)
    }

    public static void  findRandomBlocksByAckView(e, t, i, findR, callback) {
        let camp = e.camp;
        camp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;
        let r = this.system.attackViews[camp];
        let a = this.system.attackView1s[camp];
        return this.findRandomBlocks(r, a, t, i, findR, callback)
    }

    public static void  findNearBlocksByCollisionView(e, xn, yn, s, callback) {
        let camp = e.camp;
        let n = this.system.collisionViews[camp][e.conf.collideId];
        let r = this.system.collisionView1s[camp][e.conf.collideId];
        return this.findNearBlocks(n, r, xn, yn, s, callback)
    }

    public  static void  findNearBlocks(e, t, xn, yn, o, callback) {
        let n = 0;
        let r = (xn, yn) => {
            let blockIndex = Util.getIndexByXnYn(xn, yn);
            return !(blockIndex < 0 || blockIndex >= SheepConfig.line_w * SheepConfig.line_w) && this.system.findBlock(e, t, blockIndex, (petIndex => {
                let petView = this.system.getPetView(petIndex);
                if (petView) {
                    let e = callback(petView);
                    petView = null;
                    return e;
                }
                return !1
            }))
        };
        for (let e = 0; e <= o; e++) {
            if (e) {
                let e = {x: xn - n, y: yn + n};
                let t = {x: xn + n, y: yn + n};
                let o = {x: xn + n, y: yn - n};
                let l = {x: xn - n, y: yn - n};
                if (Math.random() < .5) {
                    for (let i = e.x; i < t.x; i++) {
                        if (r(i, e.y)) {
                            return !0;
                        }
                    }
                    for (let e = t.y; e > o.y; e--) {
                        if (r(t.x, e)) {
                            return !0;
                        }
                    }
                    for (let e = o.x; e > l.x; e--) {
                        if (r(e, o.y)) {
                            return !0;
                        }
                    }
                    for (let t = l.y; t < e.y; t++) {
                        if (r(l.x, t)) {
                            return !0
                        }
                    }
                } else {
                    for (let i = t.x; i > e.x; i--) {
                        if (r(i, e.y)) {
                            return !0;
                        }
                    }
                    for (let t = e.y; t > l.y; t--) {
                        if (r(l.x, t)) {
                            return !0;
                        }
                    }
                    for (let e = l.x; e < o.x; e++) {
                        if (r(e, o.y)) {
                            return !0;
                        }
                    }
                    for (let e = o.y; e < t.y; e++) {
                        if (r(t.x, e)) {
                            return !0
                        }
                    }
                }
            } else if (r(xn, yn)) {
                return !0;
            }
            n += 1
        }
        return !1
    }

    public static void  findFarBlocks(e, t, xn, yn, o, callback) {
        let n = (xn, yn) => {
            let s = Util.getIndexByXnYn(xn, yn);
            return !(s < 0 || s >= SheepConfig.line_w * SheepConfig.line_w || !this.system.getBlockByIndex(e, s).Len)
        };
        let a = (xn, yn) => {
            let blockIndex = Util.getIndexByXnYn(xn, yn);
            return this.system.findBlock(e, t, blockIndex, (petIndex => {
                let t = this.system.getPetView(petIndex);
                if (t) {
                    let e = callback(t);
                    t = null;
                    return e;
                }
                return !1
            }))
        };
        for (let e = o; e > 0; e--) {
            let t = {x: xn - e, y: yn + e};
            let o = {x: xn + e, y: yn + e};
            let l = {x: xn + e, y: yn - e};
            let r = {x: xn - e, y: yn - e};
            let c = new Set;
            for (let e = t.x; e < o.x; e++) {
                n(e, t.y) && c.add({x: e, y: t.y});
            }
            for (let e = o.y; e > l.y; e--) {
                n(o.x, e) && c.add({x: o.x, y: e});
            }
            for (let e = l.x; e > r.x; e--) {
                n(e, l.y) && c.add({x: e, y: l.y});
            }
            for (let e = r.y; e < t.y; e++) {
                n(r.x, e) && c.add({x: r.x, y: e});
            }
            for (; c.size;) {
                let e = [];
                c.forEach(t => {
                    e.push(t)
                });
                let t = Math.floor(Math.random() * c.size);
                let i = e[t];
                if (a(i.x, i.y)) {
                    return !0;
                }
                c.delete(i)
            }
        }
        return !(!n(xn, yn) || 1 != a(xn, yn))
    }

    public  static void  findRandomBlocks(e, t, i, s, findR, callback) {
        let n = (xn, yn) => {
            let blockIndex = Util.getIndexByXnYn(xn, yn);
            return !(blockIndex < 0 || blockIndex >= SheepConfig.line_w * SheepConfig.line_w || !this.system.getBlockByIndex(e, blockIndex).Len)
        };
        let a = (xn, yn) => {
            let blockIndex = Util.getIndexByXnYn(xn, yn);
            return this.system.findBlock(e, t, blockIndex, (petIndex => {
                let petSkin = this.system.getPetView(petIndex);
                if (petSkin) {
                    let e = callback(petSkin);
                    petSkin = null;
                    return e;
                }
                return !1
            }))
        };
        let c = [];
        for (let e = 0; e <= findR; e++) {
            c.push(e);
        }
        c.sort((e, t) => Math.random() - .5);
        for (let e = 0; e <= findR; e++) {
            let t = c[e];
            let o = {x: i - t, y: s + t};
            let l = {x: i + t, y: s + t};
            let r = {x: i + t, y: s - t};
            let f = {x: i - t, y: s - t};
            let h = [];
            for (let e = o.x; e < l.x; e++) {
                n(e, o.y) && h.push({x: e, y: o.y});
            }
            for (let e = l.y; e > r.y; e--) {
                n(l.x, e) && h.push({x: l.x, y: e});
            }
            for (let e = r.x; e > f.x; e--) {
                n(e, r.y) && h.push({x: e, y: r.y});
            }
            for (let e = f.y; e < o.y; e++) {
                n(f.x, e) && h.push({x: f.x, y: e});
            }
            for (h.sort(((e, t) => Math.random() - .5)); h.length;) {
                let e = h.pop();
                if (a(e.x, e.y)) {
                    return !0
                }
            }
        }
        return !1
    }
    }
}