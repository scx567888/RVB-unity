using System;

namespace rvb.scripts {
    public class Boss : PetView{
        public long backStateTime;
        public ComProgress comProgress=new ComProgress();
        private int shield;

        public Boss(int t) : base() {
            
        }

        public bool subShield(SheepMgr sheepMgr) {
            if (this.shield != 0) {
                this.shield -= sheepMgr.sheepConfig.costShield;
                if (this.shield <= 0) {
                    this.shield = 0;
                }

                return this.shield > 0;
            }

            return false;
        }

        public void updateState(SheepCtl sheepCtl, SheepMgr manager, int visualState) {
            
        }

        public void updateStateJJL(SheepCtl sheepCtl, SheepMgr manager, int visualState) {
            
        }

        public void hitAnim() {
            
            
        }
        
        public override void action(SheepMgr sheepMgr, float fixedDeltaTime) {
            var bossIsDie = this.isDie;

            var bbb = this.update_boss_frame(sheepMgr);
            if (!bossIsDie && bbb) {
                this.update_boss_state(sheepMgr);
            }

            this.update_boss_anim();
        }
        
        

        public bool update_boss_frame(SheepMgr sheepMgr) {
            var frame = this.frame;
            var loopFrame = sheepMgr.sheepConfig.loopFrame;
            var i = frame % loopFrame == loopFrame - 1;
            var posBefX = this.posBefX;
            var posBefY = this.posBefY;
            var posX = this.posX;
            var posY = this.posY;
            if (!this.isDie) {
                this.animX = posBefX + (posX - posBefX) * (frame % loopFrame) / loopFrame;
                this.animY = posBefY + (posY - posBefY) * (frame % loopFrame) / loopFrame;
            }

            frame += 1;
            this.frame = frame;
            if (!this.isDie && i) {
                this.logicMove(posX, posY);
            }

            return i;
        }

        public void update_boss_state(SheepMgr sheepMgr) {
            switch ((SheepBossState)(int)state) {
                case SheepBossState.NomalRun:
                case SheepBossState.AwakeRun:
                case SheepBossState.BackRun:
                    var t = this.conf;
                    var i = this.curAckFrame;
                    if (0 == i) {
                        var (i9, o) = sheepMgr.getXnYn(this.posX, this.posY);
                        var l = false;
                        sheepMgr.findNearBlocksByAckView(this, i9, o,
                            (int)Math.Floor((double)(t.findR * sheepMgr.sheepConfig.loongExaminationRangeBet)), (t8 => {
                                if (!!l) {
                                    return true;
                                }
                                else {
                                    if (!!sheepMgr.isCanAckByRole(this, t8)) {
                                        l = true;
                                        return true;
                                    }
                                    else {
                                        return false;
                                    }
                                }
                            }));
                        if (!l) {
                            break;
                        }
                    }

                    i += 1;
                    this.curAckFrame = i;
                    if (i == (int)Math.Floor(t.readyAtks[0] / 3f)) {
                        var (i3, s) = sheepMgr.getXnYn(this.posX, this.posY);
                        sheepMgr.forfeachBlocksByAckView(this.camp, i3, s, t.findR, t5 => {
                            if (sheepMgr.isCanAckByRole(this, t5)) {
                                SheepMgr.hurtByRole(this, t5, this.conf.atk);
                            }
                        });
                    }

                    if (i >= Math.Floor(1e3 * t.atkCd / 100)) {
                        this.curAckFrame = 0;
                    }

                    break;
            }
        }
        
        
        public void update_boss_anim() {
            this.animFrame = this.animFrame + 1;
        }
        
       
    }

    public class ComProgress {
        public float _vue;

        public void setVue(float f) {
            _vue = f;
        }
    }
}