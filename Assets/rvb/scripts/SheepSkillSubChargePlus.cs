using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubChargePlus {
        public int id;
        public string name = string.Empty;
        public int endState;
        public int endSkill;

        public static IReadOnlyList<SheepSkillSubChargePlus> List => SheepSkillSubChargePluses.All;

        public static SheepSkillSubChargePlus getById(int id) {
            return SheepSkillSubChargePluses.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubChargePlus config) {
            return SheepSkillSubChargePluses.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubChargePluses {
        // 麒麟冲刺技能
        public static readonly SheepSkillSubChargePlus config_160001 = new() {
            id = 160001,
            name = "麒麟冲刺技能",
            endState = 8,
            endSkill = 10010
        };

        public static readonly SheepSkillSubChargePlus[] All = {
            config_160001,
        };

        private static readonly Dictionary<int, SheepSkillSubChargePlus> Map = BuildMap();

        public static SheepSkillSubChargePlus GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubChargePlus config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubChargePlus 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubChargePlus config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubChargePlus> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubChargePlus>(All.Length);

            foreach (SheepSkillSubChargePlus config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubChargePlus 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}