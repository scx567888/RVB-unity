using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubCharge {
        public int id;
        public string name = string.Empty;
        public int endState;
        public int endSkill;

        public static IReadOnlyList<SheepSkillSubCharge> List => SheepSkillSubCharges.All;

        public static SheepSkillSubCharge getById(int id) {
            return SheepSkillSubCharges.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubCharge config) {
            return SheepSkillSubCharges.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubCharges {
        // 骑兵冲刺技能
        public static readonly SheepSkillSubCharge config_110001 = new() {
            id = 110001,
            name = "骑兵冲刺技能",
            endState = 8,
            endSkill = 10008
        };

        public static readonly SheepSkillSubCharge[] All = {
            config_110001,
        };

        private static readonly Dictionary<int, SheepSkillSubCharge> Map = BuildMap();

        public static SheepSkillSubCharge GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubCharge config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubCharge 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubCharge config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubCharge> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubCharge>(All.Length);

            foreach (SheepSkillSubCharge config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubCharge 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}