using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubRigidity {
        public int id;
        public string name = string.Empty;
        public int endFrame;
        public int endState;
        public int endSkill;

        public static IReadOnlyList<SheepSkillSubRigidity> List => SheepSkillSubRigidities.All;

        public static SheepSkillSubRigidity getById(int id) {
            return SheepSkillSubRigidities.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubRigidity config) {
            return SheepSkillSubRigidities.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubRigidities {
        // 僵直
        public static readonly SheepSkillSubRigidity config_150001 = new() {
            id = 150001,
            name = "僵直",
            endFrame = 60,
            endState = 19,
            endSkill = 130001
        };

        public static readonly SheepSkillSubRigidity[] All = {
            config_150001,
        };

        private static readonly Dictionary<int, SheepSkillSubRigidity> Map = BuildMap();

        public static SheepSkillSubRigidity GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubRigidity config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubRigidity 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubRigidity config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubRigidity> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubRigidity>(All.Length);

            foreach (SheepSkillSubRigidity config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubRigidity 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}