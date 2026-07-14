using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubForceState {
        public int id;
        public string name = string.Empty;
        public int state;
        public int subState;
        public int animType;

        public static IReadOnlyList<SheepSkillSubForceState> List => SheepSkillSubForceStates.All;

        public static SheepSkillSubForceState getById(int id) {
            return SheepSkillSubForceStates.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubForceState config) {
            return SheepSkillSubForceStates.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubForceStates {
        // 重新强制进入in状态
        public static readonly SheepSkillSubForceState config_60001 = new() {
            id = 60001,
            name = "重新强制进入in状态",
            state = 15,
            subState = 27,
            animType = 14
        };

        public static readonly SheepSkillSubForceState[] All = {
            config_60001,
        };

        private static readonly Dictionary<int, SheepSkillSubForceState> Map = BuildMap();

        public static SheepSkillSubForceState GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubForceState config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubForceState 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubForceState config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubForceState> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubForceState>(All.Length);

            foreach (SheepSkillSubForceState config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubForceState 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}