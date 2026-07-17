using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public class SheepSkillSubBullet {
        public int id;
        public string name = string.Empty;
        public int bullet;

        public static IReadOnlyList<SheepSkillSubBullet> List => SheepSkillSubBullets.All;

        public static SheepSkillSubBullet getById(int id) {
            return SheepSkillSubBullets.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubBullet config) {
            return SheepSkillSubBullets.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubBullets {
        // 弓箭手冲刺射箭
        public static readonly SheepSkillSubBullet config_30001 = new() {
            id = 30001,
            name = "弓箭手冲刺射箭",
            bullet = 2
        };

        // 金箍棒
        public static readonly SheepSkillSubBullet config_30002 = new() {
            id = 30002,
            name = "金箍棒",
            bullet = 105
        };

        public static readonly SheepSkillSubBullet[] All = {
            config_30001,
            config_30002,
        };

        private static readonly Dictionary<int, SheepSkillSubBullet> Map = BuildMap();

        public static SheepSkillSubBullet GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubBullet config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubBullet 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubBullet config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubBullet> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubBullet>(All.Length);

            foreach (SheepSkillSubBullet config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubBullet 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}