using System;
using System.Collections.Generic;

namespace rvb.scripts
{
    /// <summary>
    /// 兵种静态配置。这里只保存原始配置数据，不保存单个单位的运行时状态。
    /// </summary>
    public class SheepRoleTypeInfo
    {
        public int id;
        public string[] name = Array.Empty<string>();
        public string pla = string.Empty;
        public SheepRoleType roleType;
        public int collideId;
        public int animId;
        public int skinId;
        public float scale;
        public int detectCollideR;
        public float collideR;
        public int startState;
        public int formationId;
        public int runEndX;
        public int runSpeed;
        public int walkSpeed;
        public int isSpurtAnim;
        public int skillSpurt;
        public int skillIn;
        public int[] bullet = Array.Empty<int>();

        // 血量
        public float hp;
        // 攻击力
        public int atk;

        public int findR;
        public SheepRoleAtkType atkType;
        public int atkMoveType;
        public int atkMinMoveR;
        public int atkR;
        public float atkCd;
        public int[] readyAtks = Array.Empty<int>();
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
        public int[] deadAnimType = Array.Empty<int>();
        public int isSteering;
        public int[] findAtkSort = Array.Empty<int>();
        public int hitBackDistance;
        public int isLoongStopDistance;
        public int loongStopDistanceR;
        public static SheepRoleTypeInfo GetById(int id)
        {
            return SheepRoleTypeInfos.GetById(id);
        }

        public static bool TryGetById(int id, out SheepRoleTypeInfo info)
        {
            return SheepRoleTypeInfos.TryGetById(id, out info);
        }
    }

    /// <summary>
    /// 全部兵种配置。字段名称与逆向得到的原始 JavaScript 配置保持一致。
    /// </summary>
    public static class SheepRoleTypeInfos
    {
        // 炮车 / 炮车
        public static readonly SheepRoleTypeInfo pao_che = new()
        {
            id = 23,
            pla = "dy",
            roleType = SheepRoleType.pao_che,
            collideId = 3,
            animId = 106,
            scale = 9f,
            detectCollideR = 4,
            collideR = 225f,
            startState = 1,
            formationId = 5,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 0,
            skillIn = 0,
            hp = 28000f,
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
            },
        };

        // 黄金炮车 / 黄金炮车
        public static readonly SheepRoleTypeInfo huang_jin_pao_che = new()
        {
            id = 29,
            pla = "dy",
            roleType = SheepRoleType.pao_che,
            collideId = 3,
            animId = 116,
            scale = 9f,
            detectCollideR = 4,
            collideR = 225f,
            startState = 1,
            formationId = 5,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 0,
            skillIn = 0,
            hp = 32666.6666666667f,
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
                "黄金炮车",
                "黄金炮车"
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
            },
        };

        // 旋风斩 / 旋风斩
        public static readonly SheepRoleTypeInfo xuan_feng_zhan = new()
        {
            id = 19,
            pla = "dy",
            roleType = SheepRoleType.xuan_feng_zhan,
            collideId = 5,
            animId = 105,
            scale = 7f,
            detectCollideR = 2,
            collideR = 112.5f,
            startState = 18,
            formationId = 8,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 1,
            skillSpurt = 140001,
            skillIn = 0,
            hp = 24000f,
            atk = 400,
            findR = 24,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 16,
            splitN = 3,
            spiltR = 200,
            fontSize = 88,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "旋风斩",
                "旋风斩"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                4,
                3,
                7,
                5,
                2,
                6,
                8,
                1
            },
        };

        // 大旋风斩 / 大旋风斩
        public static readonly SheepRoleTypeInfo da_xuan_feng_zhan = new()
        {
            id = 26,
            pla = "dy",
            roleType = SheepRoleType.xuan_feng_zhan,
            collideId = 5,
            animId = 115,
            scale = 7f,
            detectCollideR = 2,
            collideR = 112.5f,
            startState = 18,
            formationId = 8,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 1,
            skillSpurt = 140001,
            skillIn = 0,
            hp = 28000f,
            atk = 400,
            findR = 24,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 16,
            splitN = 3,
            spiltR = 200,
            fontSize = 88,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "大旋风斩",
                "大旋风斩"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                4,
                3,
                7,
                5,
                2,
                6,
                8,
                1
            },
        };

        // 先锋兵 / 先锋兵
        public static readonly SheepRoleTypeInfo xian_feng_bing = new()
        {
            id = 7,
            pla = "dy",
            roleType = SheepRoleType.xuan_feng_zhan,
            collideId = 7,
            animId = 8,
            scale = 7.5f,
            detectCollideR = 3,
            collideR = 180f,
            startState = 0,
            formationId = 7,
            runEndX = 0,
            runSpeed = 3000,
            walkSpeed = 600,
            isSpurtAnim = 0,
            skillSpurt = 0,
            skillIn = 0,
            hp = 10000f,
            atk = 150,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 100,
            atkR = 150,
            atkCd = 0.2f,
            finishAtk = 36,
            splitN = 3,
            spiltR = 300,
            fontSize = 96,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 1,
            colliderMoveScale = 0.1f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "先锋兵",
                "先锋兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                18
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 小兵 / 小兵
        public static readonly SheepRoleTypeInfo xiao_bing = new()
        {
            id = 22,
            pla = "dy",
            roleType = SheepRoleType.xiao_bing,
            collideId = 0,
            animId = 100,
            scale = 3f,
            detectCollideR = 2,
            collideR = 110f,
            startState = 1,
            formationId = 1,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 500,
            isSpurtAnim = 0,
            skillSpurt = 0,
            skillIn = 0,
            hp = 100f,
            atk = 10,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 0,
            atkR = 55,
            atkCd = 0.45f,
            finishAtk = 16,
            splitN = 1,
            spiltR = 55,
            fontSize = 48,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.3f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.8f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "小兵",
                "小兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new[] {
                4,
                15,
                16
            },
            findAtkSort = new int[] { },
        };

        // 刺客 / 刺客
        public static readonly SheepRoleTypeInfo ci_ke = new()
        {
            id = 16,
            pla = "dy",
            roleType = SheepRoleType.ci_ke,
            collideId = 1,
            animId = 101,
            scale = 6f,
            detectCollideR = 2,
            collideR = 112.5f,
            startState = 1,
            formationId = 2,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 20006,
            skillIn = 0,
            hp = 1000f,
            atk = 240,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 275,
            atkCd = 0f,
            finishAtk = 16,
            splitN = 1,
            spiltR = 90,
            fontSize = 88,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "刺客",
                "刺客"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 大刺客 / 大刺客
        public static readonly SheepRoleTypeInfo da_ci_ke = new()
        {
            id = 17,
            pla = "dy",
            roleType = SheepRoleType.ci_ke,
            collideId = 1,
            animId = 111,
            scale = 5f,
            detectCollideR = 2,
            collideR = 112.5f,
            startState = 1,
            formationId = 2,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 20006,
            skillIn = 0,
            hp = 1600f,
            atk = 384,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 16,
            splitN = 2,
            spiltR = 130,
            fontSize = 88,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "大刺客",
                "大刺客"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 刺客 / 刺客
        public static readonly SheepRoleTypeInfo ci_ke_1 = new()
        {
            id = 102,
            pla = "dy",
            roleType = SheepRoleType.ci_ke,
            collideId = 1,
            animId = 101,
            scale = 6f,
            detectCollideR = 2,
            collideR = 112.5f,
            startState = 1,
            formationId = 10,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 20006,
            skillIn = 0,
            hp = 1000f,
            atk = 240,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 275,
            atkCd = 0f,
            finishAtk = 16,
            splitN = 1,
            spiltR = 90,
            fontSize = 88,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "刺客",
                "刺客"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 盾兵 / 盾兵
        public static readonly SheepRoleTypeInfo dun_bing = new()
        {
            id = 18,
            pla = "dy",
            roleType = SheepRoleType.dun_bing,
            collideId = 5,
            animId = 103,
            scale = 5f,
            detectCollideR = 2,
            collideR = 180f,
            startState = 1,
            formationId = 4,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 0,
            skillIn = 0,
            hp = 24000f,
            atk = 650,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 65,
            splitN = 3,
            spiltR = 200,
            fontSize = 88,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 0,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "盾兵",
                "盾兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                30
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                6,
                5,
                4,
                2,
                1,
                3,
                7
            },
        };

        // 重甲盾兵 / 重甲盾兵
        public static readonly SheepRoleTypeInfo zhong_jia_dun_bing = new()
        {
            id = 25,
            pla = "dy",
            roleType = SheepRoleType.dun_bing,
            collideId = 5,
            animId = 113,
            scale = 5f,
            detectCollideR = 2,
            collideR = 180f,
            startState = 1,
            formationId = 4,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 0,
            skillIn = 0,
            hp = 28000f,
            atk = 650,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 65,
            splitN = 3,
            spiltR = 200,
            fontSize = 88,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 0,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "重甲盾兵",
                "重甲盾兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                30
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                6,
                5,
                4,
                2,
                1,
                3,
                7
            },
        };

        // 重甲盾兵 / 重甲盾兵
        public static readonly SheepRoleTypeInfo zhong_jia_dun_bing_1 = new()
        {
            id = 104,
            pla = "dy",
            roleType = SheepRoleType.dun_bing,
            collideId = 5,
            animId = 113,
            scale = 5f,
            detectCollideR = 2,
            collideR = 180f,
            startState = 1,
            formationId = 12,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 0,
            skillIn = 0,
            hp = 31544.4444444444f,
            atk = 650,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 65,
            splitN = 3,
            spiltR = 200,
            fontSize = 88,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 0,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "重甲盾兵",
                "重甲盾兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                30
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                6,
                5,
                4,
                2,
                1,
                3,
                7
            },
        };

        // 弓箭手 / 弓箭手
        public static readonly SheepRoleTypeInfo gong_jian_shou = new()
        {
            id = 21,
            pla = "dy",
            roleType = SheepRoleType.gong_jian_shou,
            collideId = 2,
            animId = 102,
            scale = 5f,
            detectCollideR = 3,
            collideR = 180f,
            startState = 1,
            formationId = 3,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 30001,
            skillIn = 0,
            hp = 3569f,
            atk = 170,
            findR = 7,
            atkType = SheepRoleAtkType.Throw,
            atkMoveType = 1,
            atkMinMoveR = 1500,
            atkR = 3000,
            atkCd = 0.27f,
            finishAtk = 15,
            splitN = 2,
            spiltR = 100,
            fontSize = 56,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 1,
            loongStopDistanceR = 3000,
            name = new[] {
                "弓箭手",
                "弓箭手"
            },
            bullet = new[] {
                1,
                1
            },
            readyAtks = new[] {
                6
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                4,
                5,
                3,
                7,
                6,
                2,
                1
            },
        };

        // 游侠 / 游侠
        public static readonly SheepRoleTypeInfo you_xia = new()
        {
            id = 28,
            pla = "dy",
            roleType = SheepRoleType.gong_jian_shou,
            collideId = 2,
            animId = 112,
            scale = 5f,
            detectCollideR = 3,
            collideR = 180f,
            startState = 1,
            formationId = 3,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 30001,
            skillIn = 0,
            hp = 4163.83333333333f,
            atk = 170,
            findR = 7,
            atkType = SheepRoleAtkType.Throw,
            atkMoveType = 1,
            atkMinMoveR = 1500,
            atkR = 3000,
            atkCd = 0.27f,
            finishAtk = 15,
            splitN = 2,
            spiltR = 100,
            fontSize = 56,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 1,
            loongStopDistanceR = 3000,
            name = new[] {
                "游侠",
                "游侠"
            },
            bullet = new[] {
                1,
                1
            },
            readyAtks = new[] {
                6
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                4,
                5,
                3,
                7,
                6,
                2,
                1
            },
        };

        // 游侠 / 游侠
        public static readonly SheepRoleTypeInfo you_xia_1 = new()
        {
            id = 103,
            pla = "dy",
            roleType = SheepRoleType.gong_jian_shou,
            collideId = 2,
            animId = 112,
            scale = 5f,
            detectCollideR = 3,
            collideR = 180f,
            startState = 1,
            formationId = 11,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = 0,
            skillSpurt = 30001,
            skillIn = 0,
            hp = 8235.66666666667f,
            atk = 170,
            findR = 7,
            atkType = SheepRoleAtkType.Throw,
            atkMoveType = 1,
            atkMinMoveR = 1500,
            atkR = 3000,
            atkCd = 0.27f,
            finishAtk = 15,
            splitN = 2,
            spiltR = 100,
            fontSize = 56,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 1,
            loongStopDistanceR = 3000,
            name = new[] {
                "游侠",
                "游侠"
            },
            bullet = new[] {
                1,
                1
            },
            readyAtks = new[] {
                6
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                4,
                5,
                3,
                7,
                6,
                2,
                1
            },
        };

        // 冲锋兵 / 冲锋兵
        public static readonly SheepRoleTypeInfo chong_feng_bing = new()
        {
            id = 20,
            pla = "dy",
            roleType = SheepRoleType.chong_feng_bing,
            collideId = 4,
            animId = 104,
            scale = 7f,
            detectCollideR = 5,
            collideR = 240f,
            startState = 17,
            formationId = 4,
            runEndX = 200,
            runSpeed = 4500,
            walkSpeed = 700,
            isSpurtAnim = 1,
            skillSpurt = 110001,
            skillIn = 0,
            hp = 38800f,
            atk = 800,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 150,
            atkR = 200,
            atkCd = 0f,
            finishAtk = 70,
            splitN = 3,
            spiltR = 200,
            fontSize = 80,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "冲锋兵",
                "冲锋兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                20
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 大冲锋兵 / 大冲锋兵
        public static readonly SheepRoleTypeInfo da_chong_feng_bing = new()
        {
            id = 27,
            pla = "dy",
            roleType = SheepRoleType.chong_feng_bing,
            collideId = 4,
            animId = 114,
            scale = 7f,
            detectCollideR = 5,
            collideR = 240f,
            startState = 17,
            formationId = 4,
            runEndX = 200,
            runSpeed = 4500,
            walkSpeed = 700,
            isSpurtAnim = 1,
            skillSpurt = 110001,
            skillIn = 0,
            hp = 45266.6666666667f,
            atk = 800,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 150,
            atkR = 200,
            atkCd = 0f,
            finishAtk = 70,
            splitN = 3,
            spiltR = 200,
            fontSize = 80,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "大冲锋兵",
                "大冲锋兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                20
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 大冲锋兵 / 大冲锋兵
        public static readonly SheepRoleTypeInfo da_chong_feng_bing_1 = new()
        {
            id = 106,
            pla = "dy",
            roleType = SheepRoleType.chong_feng_bing,
            collideId = 4,
            animId = 114,
            scale = 7f,
            detectCollideR = 5,
            collideR = 240f,
            startState = 17,
            formationId = 12,
            runEndX = 200,
            runSpeed = 4500,
            walkSpeed = 700,
            isSpurtAnim = 1,
            skillSpurt = 110001,
            skillIn = 0,
            hp = 44244.4444444444f,
            atk = 800,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 150,
            atkR = 200,
            atkCd = 0f,
            finishAtk = 70,
            splitN = 3,
            spiltR = 200,
            fontSize = 80,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "大冲锋兵",
                "大冲锋兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                20
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 羊神 / 狼神
        public static readonly SheepRoleTypeInfo yang_shen = new()
        {
            id = 24,
            pla = "dy",
            roleType = SheepRoleType.yang_shen,
            collideId = 8,
            animId = 107,
            scale = 20f,
            detectCollideR = 6,
            collideR = 300f,
            startState = 0,
            formationId = 7,
            runEndX = 0,
            runSpeed = 3000,
            walkSpeed = 600,
            isSpurtAnim = 0,
            skillSpurt = 0,
            skillIn = 10004,
            hp = 125000f,
            atk = 3600,
            findR = 2,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 500,
            atkR = 300,
            atkCd = 0.2f,
            finishAtk = 60,
            splitN = 8,
            spiltR = 800,
            fontSize = 96,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 1,
            colliderMoveScale = 0.1f,
            isSteering = 0,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "羊神",
                "狼神"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                21
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                8,
                7,
                6,
                5,
                4,
                3,
                2,
                1
            },
        };

        // 麒麟 / 麒麟
        public static readonly SheepRoleTypeInfo qi_lin = new()
        {
            id = 30,
            pla = "dy",
            roleType = SheepRoleType.qi_lin,
            collideId = 6,
            animId = 108,
            scale = 12f,
            detectCollideR = 10,
            collideR = 480f,
            startState = 21,
            formationId = 9,
            runEndX = 3200,
            runSpeed = 4500,
            walkSpeed = 700,
            isSpurtAnim = 1,
            skillSpurt = 160001,
            skillIn = 0,
            hp = 3884666.66666667f,
            atk = 3000,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 150,
            atkR = 1200,
            atkCd = 0f,
            finishAtk = 70,
            splitN = 3,
            spiltR = 400,
            fontSize = 80,
            isFindMoveTar = 1,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 1000,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "麒麟",
                "麒麟"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                20
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // Boss
        public static readonly SheepRoleTypeInfo boss = new()
        {
            id = 0,
            pla = "all",
            roleType = SheepRoleType.boss,
            collideId = 0,
            animId = 0,
            scale = 1f,
            detectCollideR = 5,
            collideR = 600f,
            startState = 1,
            formationId = 0,
            runEndX = 0,
            runSpeed = 400,
            walkSpeed = 70,
            isSpurtAnim = 0,
            skillSpurt = 0,
            skillIn = 0,
            hp = 1000f,
            atk = 400,
            findR = 3,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 0,
            atkR = 600,
            atkCd = 4f,
            finishAtk = 12,
            splitN = 5,
            spiltR = 500,
            fontSize = 0,
            isFindMoveTar = 0,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new string[] { },
            bullet = new int[] { },
            readyAtks = new[] {
                18
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        private static readonly SheepRoleTypeInfo[] _data =
        {
            boss,
            xian_feng_bing,
            ci_ke,
            da_ci_ke,
            dun_bing,
            xuan_feng_zhan,
            chong_feng_bing,
            gong_jian_shou,
            xiao_bing,
            pao_che,
            yang_shen,
            zhong_jia_dun_bing,
            da_xuan_feng_zhan,
            da_chong_feng_bing,
            you_xia,
            huang_jin_pao_che,
            qi_lin,
            ci_ke_1,
            you_xia_1,
            zhong_jia_dun_bing_1,
            da_chong_feng_bing_1,
        };

        private static readonly Dictionary<int, SheepRoleTypeInfo> _map = CreateMap();

        public static IReadOnlyList<SheepRoleTypeInfo> All => _data;

        public static SheepRoleTypeInfo GetById(int id)
        {
            if (_map.TryGetValue(id, out SheepRoleTypeInfo info))
            {
                return info;
            }

            throw new KeyNotFoundException($"没有找到 SheepRoleTypeInfo，id = {id}");
        }

        public static bool TryGetById(int id, out SheepRoleTypeInfo info)
        {
            return _map.TryGetValue(id, out info);
        }

        private static Dictionary<int, SheepRoleTypeInfo> CreateMap()
        {
            var map = new Dictionary<int, SheepRoleTypeInfo>(_data.Length);

            foreach (SheepRoleTypeInfo info in _data)
            {
                if (map.ContainsKey(info.id))
                {
                    throw new InvalidOperationException($"重复的兵种配置 ID：{info.id}");
                }

                map.Add(info.id, info);
            }

            return map;
        }
    }
}
