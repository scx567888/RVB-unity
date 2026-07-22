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
    }
}