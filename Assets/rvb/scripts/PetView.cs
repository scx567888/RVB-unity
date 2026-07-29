using System;
using scx.SpriteRenderer;
using UnityEngine;

namespace rvb.scripts {
    
    /// 单位数据定义
    public class PetView {
        // 唯一 id
        public int id = 0;
        // 是否活跃 (用于 SheepMgr 使用)
        public bool isActive = false;
        // 是否死亡
        public bool isDie = false;
        // 阵营
        public SheepCamp camp = SheepCamp.Red;
        // 静态配置
        public SheepRoleTypeInfo conf;
        // 
        public int roleId = 0;
        // 主状态
        public SheepRoleState state = SheepRoleState.In;
        // 子状态
        public SheepRoleSubState subState = SheepRoleSubState.None;
        
        // 逻辑位置相关
        public float posBefX = 0;
        public float posBefY = 0;
        public float posX = 0;
        public float posY = 0;
        public float tarPosX = 0;
        public float tarPosY = 0;
        public float dirX = 0;
        public float dirY = 0;
        public float impulseX = 0;
        public float impulseY = 0;
        public int frame = 0;
        
        // 动画位置相关
        public float animX = 0;
        public float animY = 0;
        public float animZ = 0;
        private SheepRoleAnimType _animType = 0;
        public int animFrame = 0;

        // 当前 血量
        public float curHp = 0;
        public float curAtkBuff = 0;
        public int curAckFrame = 0;
        public float curAckCd = 0;
        public bool isHeavyAtk = false;
        public bool isNotConn = false;
        public bool isBoom = false;
        
        public bool isLock = false;
        
        public int readySkillId = 0;
        public int energy = 0;

        public BuffTimeAttacher attacher;

        // 渲染器句柄
        public ScxSpriteRenderUnit renderUnit;

        public PetView() {
            
        }

        public SheepRoleAnimType animType {
            get { return _animType; }

            set {
                _animType = value;
                animFrame = 0;
            }
        }

        public float subCurHp(int t) {
            var old = curHp;
            curHp -= t;
            return old;
        }
        
        public float subAtkCd(float deltaTime) {
            float i = curAckCd;
            if (i != 0f) {
                i -= deltaTime;
                if (i < 0f) {
                    i = 0f;
                }
                curAckCd = i;
            }

            return i;
        }
        
        // 是否处于攻击 cd
        public bool isAtkCd() {
            return curAckCd > 0f;
        }

        // 重置 攻击 cd
        public void resetAtkCd( float t) {
            curAckCd = t;
        }

        public void logicMove(float x, float y) {
            posBefX = posX;
            posBefY = posY;

            posX = x;
            posY = y;
        }

        public virtual void action(SheepMgr sheepMgr, float fixedDeltaTime) {
           
            var bbb = this.update_frame(sheepMgr);
            var petIsDie = this.isDie;
            if (!petIsDie) {
                this.update_role_state(bbb,sheepMgr,fixedDeltaTime);
            }

            this.updateAnimFrame();
            
            // 增加逻辑帧
            frame += 1;
        }
        
        private bool update_frame(SheepMgr sheepMgr) {
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

            
            if (!this.isDie && i) {
                this.logicMove(posX, posY);
            }

            return i;
        }
        
        private void update_role_state(bool isLogicFrame, SheepMgr sheepMgr, float fixedDeltaTime) {
            this.subAtkCd(fixedDeltaTime);

            PetLogic petLogic;
            switch (state) {
                case SheepRoleState.Start:
                    petLogic = PetLogicStart.Instance;
                    break;
                case SheepRoleState.In:
                    petLogic = PetLogicIn.Instance;
                    break;
                case SheepRoleState.Spurt:
                    petLogic = PetLogicSpurt.Instance;
                    break;
                case SheepRoleState.Charge:
                    petLogic = PetLogicCharge.Instance;
                    break;
                case SheepRoleState.ChargePlus:
                    petLogic = PetLogicChargePlus.Instance;
                    break;
                case SheepRoleState.SpinSpurt:
                    petLogic = PetLogicSpinSpurt.Instance;
                    break;
                case SheepRoleState.Move:
                    petLogic = PetLogicMove.Instance;
                    break;
                case SheepRoleState.Attack:
                    petLogic = PetLogicAttack.Instance;
                    break;
                case SheepRoleState.Killer:
                    petLogic = PetLogicKiller.Instance;
                    break;
                case SheepRoleState.Boom:
                    petLogic = PetLogicBoom.Instance;
                    break;
                case SheepRoleState.Invincible:
                    petLogic = PetLogicInvincible.Instance;
                    break;
                case SheepRoleState.Bladestorm:
                    petLogic = PetLogicBladestorm.Instance;
                    break;
                case SheepRoleState.Palm:
                    petLogic = PetLogicPalm.Instance;
                    break;
                case SheepRoleState.CallBullets:
                    petLogic = PetLogicCallBullets.Instance;
                    break;
                case SheepRoleState.Buff:
                    petLogic = PetLogicBuff.Instance;
                    break;
                case SheepRoleState.Rigidity:
                    petLogic = PetLogicRigidity.Instance;
                    break;
                case SheepRoleState.SpinAtk:
                    petLogic = PetLogicSpinAtk.Instance;
                    break;
                case SheepRoleState.Dead:
                    petLogic = PetLogicDead.Instance;
                    break;
                case SheepRoleState.Merge:
                    petLogic = PetLogicMerge.Instance;
                    break;
                case SheepRoleState.Res:
                    petLogic = PetLogicRes.Instance;
                    break;
                case SheepRoleState.SkillBullet:
                    petLogic = PetLogicSkillBullet.Instance;
                    break;
                case SheepRoleState.Up:
                    petLogic = PetLogicUp.Instance;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            petLogic.tick(this, sheepMgr);
            
            if (impulseX != 0 || impulseY != 0) {
                if (!isDie && curHp > 0) {
                    var t1 = impulseX;
                    var i1 = impulseY;
                    logicMove(animX + t1, posY + i1);
                }

                impulseX = 0;
                impulseY = 0;
            }
        }    
        
        
        // 每逻辑帧调用一次
        public void updateAnimFrame() {
            animFrame += 1;
        }
        
    }
}