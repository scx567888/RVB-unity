using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubBoom {
        public int id;
        public string name = string.Empty;
        public int isAnim;
        public float spiltRadiusBet;
        public int atkFindR;
        public float atkBet;
        public int cnt;
        public int findMoveFrame;
        public int atkFrame;
        public int endFrame;
        public int hitBackDistance;
        public int endState;
        public int endSkill;

        public void tick(SheepMgr sheepMgr, PetView e, bool t, float i) {
            
            var fff = sheepMgr.findTar(e);
            var l = fff.atkTar;
            var n = fff.moveTar;
            var r = fff.moveBoss;

            if (l != null || r != null) {
                e.state = SheepRoleState.Boom;
                e.subState = SheepRoleSubState.Boom;
                if (this.isAnim != 0) {
                    e.animType = SheepRoleAnimType.Boom;
                }
                else {
                    e.animType = SheepRoleAnimType.Idle;
                }

                e.readySkillId = this.id;
                return;
            }

            sheepMgr.moveTar(e, null, i, t);
        }

        public static IReadOnlyList<SheepSkillSubBoom> List => SheepSkillSubBooms.All;

        public static SheepSkillSubBoom getById(int id) {
            return SheepSkillSubBooms.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubBoom config) {
            return SheepSkillSubBooms.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubBooms {
        // 小河马冲锋爆炸
        public static readonly SheepSkillSubBoom skill_10001 = new() {
            id = 10001,
            name = "小河马冲锋爆炸",
            isAnim = 0,
            spiltRadiusBet = 5f,
            atkFindR = 10,
            atkBet = 1f,
            cnt = 1,
            findMoveFrame = 1,
            atkFrame = 1,
            endFrame = 1,
            hitBackDistance = 10,
            endState = 2,
            endSkill = 0
        };

        // 河马冲锋爆炸
        public static readonly SheepSkillSubBoom skill_10002 = new() {
            id = 10002,
            name = "河马冲锋爆炸",
            isAnim = 0,
            spiltRadiusBet = 5f,
            atkFindR = 10,
            atkBet = 1f,
            cnt = 1,
            findMoveFrame = 1,
            atkFrame = 1,
            endFrame = 1,
            hitBackDistance = 50,
            endState = 2,
            endSkill = 0
        };

        // 小老虎落地爆炸
        public static readonly SheepSkillSubBoom skill_10003 = new() {
            id = 10003,
            name = "小老虎落地爆炸",
            isAnim = 0,
            spiltRadiusBet = 3f,
            atkFindR = 1,
            atkBet = 1f,
            cnt = 1,
            findMoveFrame = 1,
            atkFrame = 1,
            endFrame = 1,
            hitBackDistance = 10,
            endState = 2,
            endSkill = 0
        };

        // 老虎落地爆炸
        public static readonly SheepSkillSubBoom skill_10004 = new() {
            id = 10004,
            name = "老虎落地爆炸",
            isAnim = 0,
            spiltRadiusBet = 2f,
            atkFindR = 8,
            atkBet = 0f,
            cnt = 1,
            findMoveFrame = 1,
            atkFrame = 1,
            endFrame = 1,
            hitBackDistance = 500,
            endState = 12,
            endSkill = 70002
        };

        // 大河马冲锋爆炸
        public static readonly SheepSkillSubBoom skill_10005 = new() {
            id = 10005,
            name = "大河马冲锋爆炸",
            isAnim = 1,
            spiltRadiusBet = 1f,
            atkFindR = 30,
            atkBet = 1f,
            cnt = 1,
            findMoveFrame = 1,
            atkFrame = 10,
            endFrame = 30,
            hitBackDistance = 150,
            endState = 2,
            endSkill = 0
        };

        // 大河马攻击爆炸
        public static readonly SheepSkillSubBoom skill_10006 = new() {
            id = 10006,
            name = "大河马攻击爆炸",
            isAnim = 1,
            spiltRadiusBet = 1f,
            atkFindR = 30,
            atkBet = 1f,
            cnt = 1,
            findMoveFrame = 1,
            atkFrame = 10,
            endFrame = 30,
            hitBackDistance = 100,
            endState = 2,
            endSkill = 0
        };

        // 新版河马冲锋爆炸
        public static readonly SheepSkillSubBoom skill_10007 = new() {
            id = 10007,
            name = "新版河马冲锋爆炸",
            isAnim = 0,
            spiltRadiusBet = 2f,
            atkFindR = 4,
            atkBet = 0.3f,
            cnt = 1,
            findMoveFrame = 1,
            atkFrame = 1,
            endFrame = 1,
            hitBackDistance = 50,
            endState = 2,
            endSkill = 0
        };

        // 骑兵冲锋爆炸
        public static readonly SheepSkillSubBoom skill_10008 = new() {
            id = 10008,
            name = "骑兵冲锋爆炸",
            isAnim = 1,
            spiltRadiusBet = 5f,
            atkFindR = 15,
            atkBet = 1f,
            cnt = 2,
            findMoveFrame = 1,
            atkFrame = 1,
            endFrame = 26,
            hitBackDistance = 800,
            endState = 2,
            endSkill = 0
        };

        // 旋转攻击爆炸
        public static readonly SheepSkillSubBoom skill_10009 = new() {
            id = 10009,
            name = "旋转攻击爆炸",
            isAnim = 1,
            spiltRadiusBet = 15f,
            atkFindR = 15,
            atkBet = 0.15f,
            cnt = 2,
            findMoveFrame = 1,
            atkFrame = 23,
            endFrame = 47,
            hitBackDistance = 0,
            endState = 20,
            endSkill = 150001
        };

        // 麒麟冲锋爆炸
        public static readonly SheepSkillSubBoom skill_10010 = new() {
            id = 10010,
            name = "麒麟冲锋爆炸",
            isAnim = 1,
            spiltRadiusBet = 5f,
            atkFindR = 15,
            atkBet = 1f,
            cnt = 1,
            findMoveFrame = 1,
            atkFrame = 1,
            endFrame = 18,
            hitBackDistance = 800,
            endState = 4,
            endSkill = 0
        };

        public static readonly SheepSkillSubBoom[] All = {
            skill_10001,
            skill_10002,
            skill_10003,
            skill_10004,
            skill_10005,
            skill_10006,
            skill_10007,
            skill_10008,
            skill_10009,
            skill_10010,
        };

        private static readonly Dictionary<int, SheepSkillSubBoom> Map = BuildMap();

        public static SheepSkillSubBoom GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubBoom config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubBoom 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubBoom config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubBoom> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubBoom>(All.Length);

            foreach (SheepSkillSubBoom config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubBoom 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}