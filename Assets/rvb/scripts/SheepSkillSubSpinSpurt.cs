using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubSpinSpurt {
        public int id;
        public string name = string.Empty;
        public int endState;
        public int endSkill;

        public static IReadOnlyList<SheepSkillSubSpinSpurt> List => SheepSkillSubSpinSpurts.All;

        public static SheepSkillSubSpinSpurt getById(int id) {
            return SheepSkillSubSpinSpurts.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubSpinSpurt config) {
            return SheepSkillSubSpinSpurts.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubSpinSpurts {
        // 旋转冲刺技能
        public static readonly SheepSkillSubSpinSpurt config_140001 = new() {
            id = 140001,
            name = "旋转冲刺技能",
            endState = 8,
            endSkill = 10009
        };

        public static readonly SheepSkillSubSpinSpurt[] All = {
            config_140001,
        };

        private static readonly Dictionary<int, SheepSkillSubSpinSpurt> Map = BuildMap();

        public static SheepSkillSubSpinSpurt GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubSpinSpurt config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubSpinSpurt 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubSpinSpurt config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubSpinSpurt> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubSpinSpurt>(All.Length);

            foreach (SheepSkillSubSpinSpurt config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubSpinSpurt 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}