using System;
using scx.SpriteRenderer;

namespace rvb.scripts {
    public class BulletView {
        public int id = 0;
        public int roleUid = 0;
        public bool isDie = false;
        public int _bulletId = 0;
        public SheepCamp camp = 0;
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float startX = 0;
        public float startY = 0;
        public float startZ = 0;
        public float dirX = 0;
        public float dirY = 0;
        public float dirZ = 0;
        public float endX = 0;
        public float endY = 0;
        public float endZ = 0;
        public PetView tarRoleIndex =null;
        public float atkVue = 0;
        public int frame = 0;
        public float angle = 0;
        public PetView roleIndex =null;

        public SheepBullet conf;
        
        // 渲染单位
        public ScxSpriteRenderUnit renderUnit;

        public int bulletId {
            get { return _bulletId; }
            set {
                _bulletId = value;
                conf = value == 0 ? null : SheepBullet.getById(value);
            }
        }

        public BulletView() {
        }

        public void init(int newId, BulletView preview) {
            if (preview == null) return;
            this.id = newId;
            this.roleUid = preview.roleUid;
            this.isDie = false;
            this.bulletId = preview.bulletId;
            this.camp = preview.camp;
            this.x = preview.x;
            this.y = preview.y;
            this.z = preview.z;
            this.startX = preview.startX;
            this.startY = preview.startY;
            this.startZ = preview.startZ;
            this.dirX = preview.dirX;
            this.dirY = preview.dirY;
            this.dirZ = preview.dirZ;
            this.endX = preview.endX;
            this.endY = preview.endY;
            this.endZ = preview.endZ;
            this.tarRoleIndex = preview.tarRoleIndex;
            this.atkVue = preview.atkVue;
            this.frame = preview.frame;
            this.angle = preview.angle;
            this.roleIndex = preview.roleIndex;
        }

        public void clear() {
            this.id = 0;
            this.roleUid = 0;
            this.isDie = false;
            this._bulletId = 0;
            this.conf = null;
            this.camp = 0;
            this.x = y = z = 0f;
            this.startX = startY = startZ = 0f;
            this.dirX = dirY = dirZ = 0f;
            this.endX = endY = endZ = 0f;
            this.tarRoleIndex = null;
            this.atkVue = 0f;
            this.frame = 0;
            this.angle = 0f;
            this.roleIndex = null;
        }

        public  void action(SheepMgr sheepMgr) {
                 if (this.isDie) {
                    return;
                }

                if (this.id != 0 && this.conf.animId != 0) {
                    var e = this;
                }

                var xnyn = sheepMgr.getXnYn(this.x, this.y);
                var s = xnyn.xn;
                var o = xnyn.yn;
                var l = this.frame;
                var n = this.conf;
                {
                    var e = this.conf.atkFrames;
                    if (e != null) {
                        for (var i1 = 0; i1 < e.Length; i1++) {
                            var n1 = e[i1];
                            if (-1 == n1 || n1 == l) {
                                if (sheepMgr.bosses[0] == this.tarRoleIndex || sheepMgr.bosses[1] == this.tarRoleIndex) {
                                    var o1 = this.tarRoleIndex;
                                    if (SheepMgr.isCanAckByBullet(this, o1, i1)) {
                                        SheepMgr.hurtByBullet(this, o1, this.atkVue);
                                    }
                                }
                                else
                                    sheepMgr.forfeachBlocksByAckView(this.camp, s, o, this.conf.findR,
                                        (e => {
                                            if (SheepMgr.isCanAckByBullet(this, e, i1)) {
                                                SheepMgr.hurtByBullet(this, e, this.atkVue);
                                            }
                                        }));

                                break;
                            }
                        }
                    }

                    switch (this.conf.moveType) {
                        case (int)SheepBulletMoveType.Fixed:
                            break;
                        case (int)SheepBulletMoveType.LineDir:
                            this.x = (float)(this.x + this.dirX * n.speed * .033);
                            this.y = (float)(this.y + this.dirY * n.speed * .033);
                            break;
                        case (int)SheepBulletMoveType.LinePosFrame:
                            this.x = this.startX + (this.endX - this.startX) * l / n.moveTimeFrame;
                            this.y = this.startY + (this.endY - this.startY) * l / n.moveTimeFrame;
                            this.z = this.startZ + (this.endZ - this.startZ) * l / n.moveTimeFrame;
                            break;
                        case (int)SheepBulletMoveType.LineTarFrame:
                            break;
                        case (int)SheepBulletMoveType.CurvePosFrame:
                            var e1 = (this.startX + this.endX) / 2;
                            var i1 = (this.startY + this.endY) / 2;
                            var s1 = n.curveHigh;
                            var o1 = this.startX + (e1 - this.startX) * l / n.moveTimeFrame;
                            var r1 = e1 + (this.endX - e1) * l / n.moveTimeFrame;
                            var a = this.startY + (i1 - this.startY) * l / n.moveTimeFrame;
                            var c = i1 + (this.endY - i1) * l / n.moveTimeFrame;
                            var f = this.startZ + (s1 - this.startZ) * l / n.moveTimeFrame;
                            var h = s1 + (this.endZ - s1) * l / n.moveTimeFrame;
                            this.x = o1 + (r1 - o1) * l / n.moveTimeFrame;
                            this.y = a + (c - a) * l / n.moveTimeFrame;
                            this.z = f + (h - f) * l / n.moveTimeFrame;
                            var p = this.endX - this.startX > 0 ? 1 : -1;
                            var u = .2 * p;
                            var d = .8;
                            var g = p;
                            var S = 0;
                            var m = -.2 * p;
                            var y = -.8;
                            var k = u + (g - u) * l / n.endFrame;
                            var B = d + (S - d) * l / n.endFrame;
                            var w = g + (m - g) * l / n.endFrame;
                            var R = S + (y - S) * l / n.endFrame;
                            this.dirX = (float)(k + (w - k) * l / n.endFrame);
                            this.dirY = 0;
                            this.dirZ = (float)(B + (R - B) * l / n.endFrame);
                            break;
                        case (int)SheepBulletMoveType.CurveTarFrame:
                            break;
                        case (int)SheepBulletMoveType.LineDirEndPos:
                            this.x = (float)(this.x + this.dirX * n.speed * .033);
                            this.y = (float)(this.y + this.dirY * n.speed * .033);
                            this.z = (float)(this.z + this.dirZ * n.speed * .033);
                            break;
                        case (int)SheepBulletMoveType.RadiusAngle:
                            this.angle += n.speed;
                            var x = this.roleUid;
                            var _ = this.roleIndex;
                            if (x == _.id) {
                                this.x = (float)(_.animX + n.radius * Math.Cos(this.angle));
                                this.y = (float)(_.animY + n.radius * Math.Sin(this.angle));
                            }
                            else {
                                this.isDie = true;
                            }

                            break;
                        case (int)SheepBulletMoveType.DirAngle: {
                            this.x = (float)(this.x + this.dirX * n.speed * .033);
                            this.y = (float)(this.y + this.dirY * n.speed * .033);
                            this.z = (float)(this.z + this.dirZ * n.speed * .033);
                            break;
                        }
                    }

                    this.frame = l + 1;
                }
                var r = n.createBulletID;
                if (r != 0 && n.createBulletFrame == l) {
                    var e = this.roleIndex;
                    var i1 = this.tarRoleIndex;
                    sheepMgr.createBullet(new BullteCreate() {
                        view_pet = e,
                        bulletId = r,
                        view_tar_pet = i1,
                        info = new BullteCreate.Info() { startX = this.x, startY = this.y, startZ = 100 }
                    });
                }
        }
    }
}