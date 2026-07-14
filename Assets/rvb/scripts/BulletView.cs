using rvb.scripts;

public class BulletView {
    public int id = 0;
    public int roleUid = 0;
    public bool isDie = false;
    public int _bulletId = 0;
    public int camp = 0;
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
    public int tarRoleIndex = 0;
    public float atkVue = 0;
    public int frame = 0;
    public float angle = 0;
    public int roleIndex = 0;

    public SheepBullet conf;

    public int bulletId {
        get { return _bulletId; }

        set {
            _bulletId = value;
            conf = SheepBullet.getById(_bulletId);
        }
    }

    public void init(int id, BulletView preBulletView) {
        this.id = id;
        this.isDie = false;
        this.roleUid = preBulletView.roleUid;
        this.isDie = preBulletView.isDie;
        this.bulletId = preBulletView.bulletId;
        this.camp = preBulletView.camp;
        this.x = preBulletView.x;
        this.y = preBulletView.y;
        this.z = preBulletView.z;
        this.startX = preBulletView.startX;
        this.startY = preBulletView.startY;
        this.startZ = preBulletView.startZ;
        this.dirX = preBulletView.dirX;
        this.dirY = preBulletView.dirY;
        this.dirZ = preBulletView.dirZ;
        this.endX = preBulletView.endX;
        this.endY = preBulletView.endY;
        this.endZ = preBulletView.endZ;
        this.tarRoleIndex = preBulletView.tarRoleIndex;
        this.atkVue = preBulletView.atkVue;
        this.frame = preBulletView.frame;
        this.angle = preBulletView.angle;
        this.roleIndex = preBulletView.roleIndex;
    }

    public void clear() {
        this.id = 0;
        this.roleUid = 0;
        this.isDie = false;
        this._bulletId = 0;
        this.camp = 0;
        this.x = 0;
        this.y = 0;
        this.z = 0;
        this.startX = 0;
        this.startY = 0;
        this.startZ = 0;
        this.dirX = 0;
        this.dirY = 0;
        this.dirZ = 0;
        this.endX = 0;
        this.endY = 0;
        this.endZ = 0;
        this.tarRoleIndex = 0;
        this.atkVue = 0;
        this.frame = 0;
        this.angle = 0;
        this.roleIndex = 0;
        this.conf = SheepBullet.getById(this._bulletId);
    }
}