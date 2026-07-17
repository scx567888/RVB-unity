using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubSpinAtk {
        public int id;
        public string name = string.Empty;
        public int endFrame;
        public float atkBet;
        public float spiltRadiusBet;
        public int atkFindR;
        public int endState;
        public int endSkill;

        public static IReadOnlyList<SheepSkillSubSpinAtk> List => SheepSkillSubSpinAtks.All;

        public static SheepSkillSubSpinAtk getById(int id) {
            return SheepSkillSubSpinAtks.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubSpinAtk config) {
            return SheepSkillSubSpinAtks.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubSpinAtks {
        // 旋转攻击技能
        public static readonly SheepSkillSubSpinAtk config_130001 = new() {
            id = 130001,
            name = "旋转攻击技能",
            endFrame = 30,
            atkBet = 0.3f,
            spiltRadiusBet = 10f,
            atkFindR = 20,
            endState = 8,
            endSkill = 10009
        };

        public static readonly SheepSkillSubSpinAtk[] All = {
            config_130001,
        };

        private static readonly Dictionary<int, SheepSkillSubSpinAtk> Map = BuildMap();

        public static SheepSkillSubSpinAtk GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubSpinAtk config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubSpinAtk 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubSpinAtk config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubSpinAtk> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubSpinAtk>(All.Length);

            foreach (SheepSkillSubSpinAtk config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubSpinAtk 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}