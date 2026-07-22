namespace rvb.scripts {
    public class SheepConfig {
        // 地图宽度
        public static int w = 12000;

        // 地图高度
        public static int h = 12000;

        // 格子大小 (格子是正方形)
        public static int d = 500;

        // 横向分成多少行
        public static int line_w = 24;

        // 纵向分成多少行
        public static int line_h = 24;

        public static int loopFrame = 4;

        // 最大单位数
        public static int MaxPetCount = 100000;

        // 最大子弹数
        public static int MaxBulletCount = 100000;

        // 最大组数 (跟 碰撞 id 相关联)
        public static int MaxGroupCount = 15;
        public static int ImageMaxCount = 5;

        public static int damageK = 2;
        public static int damageB = 1;
        public static int costShield = 33;
        public static int maxShield = 30000;
        public static int buffLastTime = 60;
        public static float buffHpIncreaseRate = 0.2f;
        public static float buffAtkIncreaseRate = 0.1f;
        public static float buffScoreIncreaseRate = 0.1f;
        public static float buffDragonDamageIncreseRate = 0.1f;
        public static float buffDragonReduceRate = 0.4f;
        public static float buffDragonMaxReduceRate = 0.999f;
        public static int limitSearchBorderX = 4900;
        public static int loongExaminationRangeBet = 2;
        public static float systemAutomaticTroopsIntervalTime = 0.8f;
        public static int systemAutomaticallyMaxTroops = 700;
        public static int systemAutomaticallyTroopsOneNumber = 6;
        public static int systemLongerAutomaticallyDispatch = 5000;
        public static int DestructionDecreaseNumber = 5;
        public static int DestructionMax = 12;
        public static int DestructionMin = 3;
        public static int DestructionDefault = 8;
        public static int shockBeginNumber = 6;
        public static int shockEndNumber = 6;
        public static int counterBuffNumber = 3;
        public static float counterHpRatio = 0.3f;
        public static int counterTime = 120;
        // public static int WarmUpID = 22;
        public static int WarmUpID = 23;
        public static int beheadLine = 8000;
        public static int startMaxTime = 300;

        public static int[] gameTime = {
            1200,
            1800,
            3600
        };

        public static int[] loongHps = {
            888800,
            1888800,
            2888800
        };

        public static float[] loongStateSwitching = {
            0.35f,
            0.45f,
            0.55f,
            0.65f,
            1
        };

        public static string[] groundColor = new[] {
            "#ecf8fb",
            "#150c0a"
        };
    }
}