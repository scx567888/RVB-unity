using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubDestruction {
        public int id;
        public string name = string.Empty;
        public int bulletId;

        public static IReadOnlyList<SheepSkillSubDestruction> List => SheepSkillSubDestructions.All;

        public static SheepSkillSubDestruction getById(int id) {
            return SheepSkillSubDestructions.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubDestruction config) {
            return SheepSkillSubDestructions.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubDestructions {
        // 小兵自爆
        public static readonly SheepSkillSubDestruction config_120001 = new() {
            id = 120001,
            name = "小兵自爆",
            bulletId = 14
        };

        public static readonly SheepSkillSubDestruction[] All = {
            config_120001,
        };

        private static readonly Dictionary<int, SheepSkillSubDestruction> Map = BuildMap();

        public static SheepSkillSubDestruction GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubDestruction config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubDestruction 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubDestruction config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubDestruction> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubDestruction>(All.Length);

            foreach (SheepSkillSubDestruction config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubDestruction 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}