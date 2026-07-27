namespace rvb.scripts {
    public class SheepConfig {
        // 地图宽度
        public int w = 12000;

        // 地图高度
        public int h = 12000;

        // 格子大小 (格子是正方形)
        public int d = 500;

        // 横向分成多少行
        public int line_w = 24;

        // 纵向分成多少行
        public int line_h = 24;

        public int loopFrame = 4;

        // 最大单位数
        public int MaxPetCount = 100000;

        // 最大子弹数
        public int MaxBulletCount = 100000;

        // 最大组数 (跟 碰撞 id 相关联)
        public int MaxGroupCount = 15;
        public int ImageMaxCount = 5;

        public int damageK = 2;
        public int damageB = 1;
        public int costShield = 33;
        public int maxShield = 30000;
        public int buffLastTime = 60;
        public float buffHpIncreaseRate = 0.2f;
        public float buffAtkIncreaseRate = 0.1f;
        public float buffScoreIncreaseRate = 0.1f;
        public float buffDragonDamageIncreseRate = 0.1f;
        public float buffDragonReduceRate = 0.4f;
        public float buffDragonMaxReduceRate = 0.999f;
        public int limitSearchBorderX = 4900;
        public int loongExaminationRangeBet = 2;
        public float systemAutomaticTroopsIntervalTime = 0.8f;
        public int systemAutomaticallyMaxTroops = 3000;
        public int systemAutomaticallyTroopsOneNumber = 6;
        public int systemLongerAutomaticallyDispatch = 5000;
        public int DestructionDecreaseNumber = 5;
        public int DestructionMax = 12;
        public int DestructionMin = 3;
        public int DestructionDefault = 8;
        public int shockBeginNumber = 6;
        public int shockEndNumber = 6;
        public int counterBuffNumber = 3;
        public float counterHpRatio = 0.3f;
        public int counterTime = 120;

        public int WarmUpID = 22;

        // public static int WarmUpID = 23;
        public int beheadLine = 8000;
        public int startMaxTime = 300;

        public int[] gameTime = {
            1200,
            1800,
            3600
        };

        public int[] loongHps = {
            888800,
            1888800,
            2888800
        };

        public float[] loongStateSwitching = {
            0.35f,
            0.45f,
            0.55f,
            0.65f,
            1
        };

        public string[] groundColor = {
            "#ecf8fb",
            "#150c0a"
        };
    }


    public class SheepConfigs {
        public static SheepConfig sheepConfig = new() {
            // 地图宽度
            w = 12000,

            // 地图高度
            h = 12000,

            // 格子大小 (格子是正方形)
            d = 500,

            // 横向分成多少行
            line_w = 24,

            // 纵向分成多少行
            line_h = 24,

            loopFrame = 4,

            // 最大单位数
            MaxPetCount = 100000,

            // 最大子弹数
            MaxBulletCount = 100000,

            // 最大组数 (跟 碰撞 id 相关联)
            MaxGroupCount = 15,
            ImageMaxCount = 5,

            damageK = 2,
            damageB = 1,
            costShield = 33,
            maxShield = 30000,
            buffLastTime = 60,
            buffHpIncreaseRate = 0.2f,
            buffAtkIncreaseRate = 0.1f,
            buffScoreIncreaseRate = 0.1f,
            buffDragonDamageIncreseRate = 0.1f,
            buffDragonReduceRate = 0.4f,
            buffDragonMaxReduceRate = 0.999f,
            limitSearchBorderX = 4900,
            loongExaminationRangeBet = 2,
            systemAutomaticTroopsIntervalTime = 0.8f,
            systemAutomaticallyMaxTroops = 700,
            systemAutomaticallyTroopsOneNumber = 6,
            systemLongerAutomaticallyDispatch = 5000,
            DestructionDecreaseNumber = 5,
            DestructionMax = 12,
            DestructionMin = 3,
            DestructionDefault = 8,
            shockBeginNumber = 6,
            shockEndNumber = 6,
            counterBuffNumber = 3,
            counterHpRatio = 0.3f,
            counterTime = 120,
            // WarmUpID = 22,
            WarmUpID = 23,
            beheadLine = 8000,
            startMaxTime = 300,

            gameTime = new[] {
                1200,
                1800,
                3600
            },

            loongHps = new[] {
                888800,
                1888800,
                2888800
            },

            loongStateSwitching = new[] {
                0.35f,
                0.45f,
                0.55f,
                0.65f,
                1
            },

            groundColor = new[] {
                "#ecf8fb",
                "#150c0a"
            },
        };
    }
}