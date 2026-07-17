using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public class SheepSkillSubBuff {
        public int id;
        public string name = string.Empty;
        public int buffStratFrame;
        public int buffEndFrame;
        public int endFrame;

        public static IReadOnlyList<SheepSkillSubBuff> List => SheepSkillSubBuffs.All;

        public static SheepSkillSubBuff getById(int id) {
            return SheepSkillSubBuffs.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubBuff config) {
            return SheepSkillSubBuffs.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubBuffs {
        // 全体攻速翻倍
        public static readonly SheepSkillSubBuff config_90001 = new() {
            id = 90001,
            name = "全体攻速翻倍",
            buffStratFrame = 20,
            buffEndFrame = 380,
            endFrame = 391
        };

        public static readonly SheepSkillSubBuff[] All = {
            config_90001,
        };

        private static readonly Dictionary<int, SheepSkillSubBuff> Map = BuildMap();

        public static SheepSkillSubBuff GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubBuff config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubBuff 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubBuff config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubBuff> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubBuff>(All.Length);

            foreach (SheepSkillSubBuff config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubBuff 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}