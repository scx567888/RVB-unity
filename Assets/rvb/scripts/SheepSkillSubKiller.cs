using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubKiller {
        public int id;
        public string name = string.Empty;
        public float spiltRadiusBet;
        public int atkFindR;
        public float atkBet;
        public int cnt;
        public int findMoveFrame;
        public int atkFrame;
        public int endFrame;
        public int findR;


        public void tick(SheepMgr sheepMgr, PetView e, bool t) {
            var fff = sheepMgr.findTar(e);

            var l = fff.atkTar;
            var n = fff.moveTar;
            var r = fff.moveBoss;

            if (l != null) {
                e.state = SheepRoleState.Killer;
                e.subState = SheepRoleSubState.KillerStart;
                e.animType = SheepRoleAnimType.Killer;
                e.readySkillId = this.id;
                return;
            }


            if (r != null) {
                e.state = SheepRoleState.Move;
                e.subState = SheepRoleSubState.MoveBoss;
                e.animType = SheepRoleAnimType.Idle;
                sheepMgr.moveTar(e, r,  t);
                return;
            }

            sheepMgr.moveTar(e, null,  t);
        }

        public static IReadOnlyList<SheepSkillSubKiller> List => SheepSkillSubKillers.All;

        public static SheepSkillSubKiller getById(int id) {
            return SheepSkillSubKillers.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubKiller config) {
            return SheepSkillSubKillers.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubKillers {
        // 小刺客冲刺刺杀
        public static readonly SheepSkillSubKiller config_20001 = new() {
            id = 20001,
            name = "小刺客冲刺刺杀",
            spiltRadiusBet = 2f,
            atkFindR = 3,
            atkBet = 0.5f,
            cnt = 2,
            findMoveFrame = 1,
            atkFrame = 6,
            endFrame = 12,
            findR = 3
        };

        // 刺客冲刺刺杀
        public static readonly SheepSkillSubKiller config_20002 = new() {
            id = 20002,
            name = "刺客冲刺刺杀",
            spiltRadiusBet = 2f,
            atkFindR = 3,
            atkBet = 0.1f,
            cnt = 5,
            findMoveFrame = 1,
            atkFrame = 6,
            endFrame = 12,
            findR = 6
        };

        // 大刺客冲刺刺杀
        public static readonly SheepSkillSubKiller config_20003 = new() {
            id = 20003,
            name = "大刺客冲刺刺杀",
            spiltRadiusBet = 1f,
            atkFindR = 6,
            atkBet = 0.3f,
            cnt = 8,
            findMoveFrame = 1,
            atkFrame = 3,
            endFrame = 6,
            findR = 12
        };

        // 大刺客攻击刺杀
        public static readonly SheepSkillSubKiller config_20004 = new() {
            id = 20004,
            name = "大刺客攻击刺杀",
            spiltRadiusBet = 1f,
            atkFindR = 6,
            atkBet = 0.3f,
            cnt = 0,
            findMoveFrame = 1,
            atkFrame = 3,
            endFrame = 6,
            findR = 12
        };

        // 普通刺客攻击刺杀
        public static readonly SheepSkillSubKiller config_20005 = new() {
            id = 20005,
            name = "普通刺客攻击刺杀",
            spiltRadiusBet = 1f,
            atkFindR = 6,
            atkBet = 0.3f,
            cnt = 0,
            findMoveFrame = 1,
            atkFrame = 4,
            endFrame = 8,
            findR = 5
        };

        // 新模式冲刺刺杀
        public static readonly SheepSkillSubKiller config_20006 = new() {
            id = 20006,
            name = "新模式冲刺刺杀",
            spiltRadiusBet = 2f,
            atkFindR = 6,
            atkBet = 0.2f,
            cnt = 3,
            findMoveFrame = 1,
            atkFrame = 4,
            endFrame = 8,
            findR = 3
        };

        public static readonly SheepSkillSubKiller[] All = {
            config_20001,
            config_20002,
            config_20003,
            config_20004,
            config_20005,
            config_20006,
        };

        private static readonly Dictionary<int, SheepSkillSubKiller> Map = BuildMap();

        public static SheepSkillSubKiller GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubKiller config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubKiller 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubKiller config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubKiller> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubKiller>(All.Length);

            foreach (SheepSkillSubKiller config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubKiller 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}