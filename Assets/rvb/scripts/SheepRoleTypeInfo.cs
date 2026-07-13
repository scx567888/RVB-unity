namespace rvb.scripts {
    public class SheepRoleTypeInfo {
        public int id;
        public string[] name;
        public string pla;
        public SheepRoleType roleType;
        public int collideId;
        public int animId;
        public int skinId;
        public int scale;
        public int detectCollideR;
        public int collideR;
        public int startState;
        public int formationId;
        public int runEndX;
        public int runSpeed;
        public int walkSpeed;
        public int isSpurtAnim;
        public int skillSpurt;
        public int skillIn;
        public int[] bullet;

        // 血量
        public int hp;

        // 攻击力
        public int atk;

        public int findR;
        public SheepRoleAtkType atkType;
        public int atkMoveType;
        public int atkMinMoveR;
        public int atkR;
        public float atkCd;
        public int[] readyAtks;

        public int finishAtk;

        // 溅射格子数
        public int splitN;

        // 溅射半径
        public int spiltR;
        public int fontSize;
        public int isFindMoveTar;
        public float colliderElasticityScale;
        public int colliderNotMoveNum;
        public float colliderMoveScale;
        public int[] deadAnimType;
        public int isSteering;
        public int[] findAtkSort;
        public int hitBackDistance;
        public int isLoongStopDistance;
        public int loongStopDistanceR;
    }


    public class SheepRoleTypeInfos {
        public static SheepRoleTypeInfo pao_che = new() {
            id = 23,
            pla = "dy",
            roleType = SheepRoleType.pao_che,
            collideId = 3,
            animId = 106,
            scale = 9,
            detectCollideR = 4,
            collideR = 225,
            startState = 1,
            formationId = 5,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 0,
            skillIn = 0,
            hp = 28000,
            atk = 6244,
            findR = 7,
            atkType = SheepRoleAtkType.Throw,
            atkMoveType = 1,
            atkMinMoveR = 2000,
            atkR = 5000,
            atkCd = 3.7f,
            finishAtk = 40,
            splitN = 4,
            spiltR = 540,
            fontSize = 72,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 1,
            loongStopDistanceR = 5000,
            name = new[] {
                "炮车",
                "炮车"
            },
            bullet = new[] {
                15,
                3
            },
            readyAtks = new[] {
                14
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                7,
                5,
                6,
                8,
                3,
                4,
                2,
                1
            }
        };
    }
}