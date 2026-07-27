using scx.SpriteRenderer;

namespace rvb.scripts {
    
    /// 单位数据定义
    public class PetView {
        // 唯一 id
        public int id = 0;
        // 是否活跃
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
        public SheepRoleAnimType _animType = 0;
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
    }
}