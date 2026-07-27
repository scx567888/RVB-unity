using scx.SpriteRenderer;

namespace rvb.scripts {
    
    public enum BuffID {
        GeneralOrder = 0,
        CardBuff = 1
    }

    /// <summary>
    /// 单位数据定义
    /// </summary>
    public class PetView {
        public int id = 0;
        public bool isActive = false;
        public bool isDie = false;
        public SheepCamp camp = SheepCamp.Red;
        public int roleId = 0;
        public int skinId = 0;
        public SheepRoleState state = SheepRoleState.In;
        public SheepRoleSubState subState = SheepRoleSubState.None;
        public bool isLock = false;
        public int frame = 0;
        public float posBefX = 0;
        public float posBefY = 0;
        public float animX = 0;
        public float animY = 0;
        public float animZ = 0;
        public float posX = 0;
        public float posY = 0;
        public int blockIndex = 0;
        public float dirX = 0;
        public float dirY = 0;
        public float curHp = 0;
        public float curAtkBuff = 0;
        public int curAckFrame = 0;
        public float curAckCd = 0;
        public bool isHeavyAtk = false;
        public bool isNotConn = false;
        public bool isBoom = false;
        public SheepRoleAnimType _animType = 0;
        public int animFrame = 0;
        public float tarPosX = 0;
        public float tarPosY = 0;
        public float impulseX = 0;
        public float impulseY = 0;
        public int readySkillId = 0;
        public int energy = 0;

        public SheepRoleTypeInfo conf;

        public float scale;
        public BuffTimeAttacher attacher;
        public int petId;

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
            float old = curHp;
            curHp -= t;
            return old;
        }

        public void logicMove(float x, float y) {
            posBefX = posX;
            posBefY = posY;

            posX = x;
            posY = y;
        }
    }
}